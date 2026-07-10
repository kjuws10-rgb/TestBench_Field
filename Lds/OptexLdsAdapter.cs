using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StageWin.Driver.LDS
{
    public interface ILdsSampler
    {
        bool IsConnected { get; }
        (double ch1, double ch2, DateTime ts)? TryGetLatest();
        /// ESC 실측 시점: 내부 폴링을 안전하게 "일시정지"한 뒤 즉시 1샷을 취득
        Task<(double ch1, double ch2)> AcquireOnceWhilePausedAsync(CancellationToken ct);
        /// 수동 일시정지/재개가 필요할 때 사용(Dispose 시 재개)
        IDisposable PausePolling();
        event Action<double, double, DateTime> OnSample;
        void Start();
        void Stop();
    }

    /// <summary>
    /// 평상시엔 백그라운드 폴링을 계속 돌리되, 실측 시점엔 폴링을 잠시 멈추고 1샷을 취득하는 허브.
    /// </summary>
    public sealed class LdsPollingHub : ILdsSampler, IDisposable
    {
        private readonly IOptexLds _lds;
        private readonly int _periodMs;
        private readonly Action<string> _log;

        private CancellationTokenSource _cts;
        private Task _loopTask;

        private volatile bool _paused;
        private readonly object _gate = new object();
        private (double ch1, double ch2, DateTime ts)? _latest;

        public event Action<double, double, DateTime> OnSample;

        public LdsPollingHub(IOptexLds lds, int samplePeriodMs = 50, Action<string> logger = null)
        {
            _lds = lds ?? throw new ArgumentNullException(nameof(lds));
            _periodMs = Math.Max(10, samplePeriodMs);
            _log = logger ?? (_ => { });
        }

        public bool IsConnected => _lds?.IsConnected ?? false;

        public void Start()
        {
            if (_loopTask != null) return;
            _cts = new CancellationTokenSource();
            _loopTask = Task.Run(() => LoopAsync(_cts.Token));
            _log?.Invoke($"[LDS] polling started (period={_periodMs}ms)");
        }

        public void Stop()
        {
            try { _cts?.Cancel(); } catch { }
            try { _loopTask?.Wait(1000); } catch { }
            _cts?.Dispose(); _cts = null; _loopTask = null;
            _log?.Invoke("[LDS] polling stopped");
        }

        private async Task LoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_paused || !IsConnected)
                {
                    await Task.Delay(_periodMs, ct);
                    continue;
                }

                try
                {
                    var (a, b) = await _lds.ReadTwoChannelsAsync(ct).ConfigureAwait(false);
                    var now = DateTime.UtcNow;
                    lock (_gate) _latest = (a, b, now);
                    OnSample?.Invoke(a, b, now);
                }
                catch (OperationCanceledException) { /* ignore */ }
                catch (Exception ex)
                {
                    _log?.Invoke("[LDS][W] polling: " + ex.Message);
                    await Task.Delay(_periodMs, ct);
                }

                await Task.Delay(_periodMs, ct);
            }
        }

        public (double ch1, double ch2, DateTime ts)? TryGetLatest()
        {
            lock (_gate) return _latest;
        }

        public IDisposable PausePolling()
        {
            _paused = true;
            return new ResumeOnDispose(() => _paused = false);
        }

        public async Task<(double ch1, double ch2)> AcquireOnceWhilePausedAsync(CancellationToken ct)
        {
            using (PausePolling())
            {
                // 폴링은 멈춘 상태. 장치에서 즉시 1샷을 읽는다.
                var (a, b) = await _lds.ReadTwoChannelsAsync(ct).ConfigureAwait(false);
                var now = DateTime.UtcNow;
                lock (_gate) _latest = (a, b, now);
                OnSample?.Invoke(a, b, now);
                return (a, b);
            }
        }

        private sealed class ResumeOnDispose : IDisposable
        {
            private readonly Action _resume;
            public ResumeOnDispose(Action resume) => _resume = resume;
            public void Dispose() => _resume?.Invoke();
        }

        public void Dispose() => Stop();
    }

    public interface IOptexLds : IDisposable
    {
        bool Connect(int timeoutMs = 1500);
        void Disconnect();
        bool IsConnected { get; }
        // 동기 1회 읽기
        (double ch1, double ch2) ReadTwoChannels();
        // 비동기 1회 읽기
        Task<(double ch1, double ch2)> ReadTwoChannelsAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Optex CDX(LDS) 이더넷 드라이버(라인 기반 ASCII 프로토콜 기본).
    /// ex) "12.345,67.890\n" → ch1=12.345, ch2=67.890
    /// 필요 시 QueryCommand로 "MEAS?" 등 단일 커맨드 송신 후 응답 라인 파싱으로 사용.
    /// </summary>
    public sealed class OptexCdxEthernet : IOptexLds
    {
        public enum LdsProtocol { AsciiLine, OptexBinary }

        private readonly string _ip;
        private readonly int _port;
        private readonly bool _useSimulator;
        private TcpClient _cli;
        private NetworkStream _ns;
        private readonly object _ioLock = new object();

        public LdsProtocol Protocol { get; set; } = LdsProtocol.OptexBinary;
        public bool IsConnected => _useSimulator ? true : (_cli != null && _cli.Connected);

        public string QueryCommand { get; set; } = null; // "MEAS?\n" 등
        public string NewLine { get; set; } = "\n";

        private const int CMD_BASE = 0x0D60; // 채널1
        private const int CMD_STRIDE = 0x0004; // 채널당 +4
        private static readonly byte[] REQ_PREFIX = new byte[] { 0x30, 0x02 };
        private static readonly byte[] RESP_HDR = new byte[] { 0xB0, 0x04 };
        private const int RESP_LEN = 6; // B0 04 + 4바이트 값

        public OptexCdxEthernet(string ip, int port = 5011, bool useSimulator = false)
        {
            _ip = ip;
            _port = port;
            _useSimulator = useSimulator;
        }

        public bool Connect(int timeoutMs = 1500)
        {
            if (_useSimulator) return true;
            try
            {
                Disconnect();
                _cli = new TcpClient();
                var ar = _cli.BeginConnect(_ip, _port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(timeoutMs))
                {
                    try { _cli.Close(); } catch { }
                    _cli = null;
                    return false;
                }
                _cli.EndConnect(ar);
                _ns = _cli.GetStream();
                _ns.ReadTimeout = timeoutMs;
                _ns.WriteTimeout = timeoutMs;
                return true;
            }
            catch(Exception ex)
            {
                Disconnect();
                return false;
            }
        }

        public void Disconnect()
        {
            if (_useSimulator) return;
            try { _ns?.Dispose(); } catch { }
            try { _cli?.Close(); } catch { }
            _ns = null; _cli = null;
        }

        // =============== 동기 1회 읽기 ===============
        public (double ch1, double ch2) ReadTwoChannels()
        {
            if (_useSimulator) return OptexLdsSimulator.Next();
            if (!IsConnected) throw new IOException("Optex 연결 안 됨");

            lock (_ioLock)
            {
                switch (Protocol)
                {
                    case LdsProtocol.OptexBinary:
                        var a = ReadChannelBinarySync(1);
                        var b = ReadChannelBinarySync(2);
                        return (a, b);

                    case LdsProtocol.AsciiLine:
                        // (선택) 요청 커맨드 송신
                        if (!string.IsNullOrEmpty(QueryCommand))
                        {
                            var cmd = Encoding.ASCII.GetBytes(QueryCommand);
                            _ns.Write(cmd, 0, cmd.Length);
                            _ns.Flush();
                        }
                        string line = ReadLine(_ns, NewLine);
                        var sp = line.Split(new[] { ',', ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (sp.Length < 2) throw new FormatException("Optex 응답 파싱 실패: " + line);
                        var ch1 = double.Parse(sp[0], CultureInfo.InvariantCulture);
                        var ch2 = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        return (ch1, ch2);

                    default: throw new NotSupportedException();
                }
            }
        }

        // 비동기 1회 읽기
        public async Task<(double ch1, double ch2)> ReadTwoChannelsAsync(CancellationToken ct = default)
        {
            if (_useSimulator) return OptexLdsSimulator.Next();
            if (!IsConnected) throw new IOException("Optex 연결 안 됨");

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                // 하나의 요청이 끝날 때까지 직렬화
                // (동일 네트워크스트림에서 요청/응답 프레임을 섞지 않기 위함)
                // Sync와 동일하게 lock 사용
                lock (_ioLock)
                {
                    // 빈 lock 블록으로 진입 동기화만 보장하고, 실제 I/O는 async 메서드에서 수행
                }
            }

            // lock을 async에서 쓰기 위해 별도 크리티컬 섹션 구현
            // 간단히: 전용 세마포어로 직렬화
            // (여기서는 _ioLock으로 Sync/Async 혼용을 피하기 위해 local semaphore 사용)
            // → 구현 간결화를 위해 다시 lock(_ioLock) 안에서 await 안 걸리도록
            //    sync-async 분리: 아래처럼 채널별 async를 순차 await 하되, 그 내부에서 lock 사용

            var v1 = await ReadChannelBinaryAsync(1, ct).ConfigureAwait(false);
            var v2 = await ReadChannelBinaryAsync(2, ct).ConfigureAwait(false);
            return (v1, v2);
        }

        // ---------- Optex Binary (Sync) ----------
        private double ReadChannelBinarySync(int channel)
        {
            if (channel < 1 || channel > 4) throw new ArgumentOutOfRangeException(nameof(channel));
            int cmd = CMD_BASE + (channel - 1) * CMD_STRIDE;
            byte cmdHigh = (byte)((cmd >> 8) & 0xFF);
            byte cmdLow = (byte)(cmd & 0xFF);

            // 송신: 30 02 cmdHigh cmdLow
            var req = new byte[] { REQ_PREFIX[0], REQ_PREFIX[1], cmdHigh, cmdLow };
            _ns.Write(req, 0, req.Length);

            // 수신: B0 04 + 4바이트 raw
            var resp = ReadExact(_ns, RESP_LEN);
            if (resp[0] != RESP_HDR[0] || resp[1] != RESP_HDR[1])
                throw new IOException($"Optex 응답 헤더 불일치: {resp[0]:X2} {resp[1]:X2}");

            int raw = (resp[2] << 24) | (resp[3] << 16) | (resp[4] << 8) | resp[5];
            double mm = raw / 1_000_000.0; // nm → mm
            return mm;
        }

        // ---------- Optex Binary (Async) ----------
        private async Task<double> ReadChannelBinaryAsync(int channel, CancellationToken ct)
        {
            if (_useSimulator) return OptexLdsSimulator.Next().Item1;
            if (!IsConnected) throw new IOException("Optex 연결 안 됨");
            if (channel < 1 || channel > 4) throw new ArgumentOutOfRangeException(nameof(channel));

            int cmd = CMD_BASE + (channel - 1) * CMD_STRIDE;
            byte cmdHigh = (byte)((cmd >> 8) & 0xFF);
            byte cmdLow = (byte)(cmd & 0xFF);
            var req = new byte[] { REQ_PREFIX[0], REQ_PREFIX[1], cmdHigh, cmdLow };

            // 같은 스트림에서의 경쟁 방지
            byte[] resp;
            lock (_ioLock)
            {
                // Write
                _ns.Write(req, 0, req.Length);

                // Read exact
                resp = ReadExact(_ns, RESP_LEN);
            }

            if (resp[0] != RESP_HDR[0] || resp[1] != RESP_HDR[1])
                throw new IOException($"Optex 응답 헤더 불일치: {resp[0]:X2} {resp[1]:X2}");

            int raw = (resp[2] << 24) | (resp[3] << 16) | (resp[4] << 8) | resp[5];
            return raw / 1_000_000.0; // nm → mm
        }

        // ---------- ASCII 라인 공용 ----------
        private static string ReadLine(Stream s, string nl)
        {
            var sb = new StringBuilder(64);
            int b;
            while ((b = s.ReadByte()) >= 0)
            {
                if (b == nl[nl.Length - 1]) break;
                sb.Append((char)b);
            }
            return sb.ToString().Trim();
        }

        // ---------- Exact Reader ----------
        private static byte[] ReadExact(Stream s, int n)
        {
            var buf = new byte[n];
            int off = 0;
            while (off < n)
            {
                int r = s.Read(buf, off, n - off);
                if (r <= 0) throw new EndOfStreamException("LDS 소켓 EOF");
                off += r;
            }
            return buf;
        }

        public void Dispose() => Disconnect();
    }


    /// <summary>오프라인/디버그용 간이 시뮬레이터</summary>
    public static class OptexLdsSimulator
    {
        private static double _t = 0;
        public static (double, double) Next()
        {
            _t += 0.05;
            // ch1: 평탄; ch2: 두께 목적. 아무 파라미터나 넣어 간단 파형.
            double ch1 = 10 + Math.Sin(_t) * 0.2;    // mm
            double ch2 = ch1 + 0.5 + Math.Sin(_t * 0.25) * 0.05; // mm
            return (Math.Round(ch1, 3), Math.Round(ch2, 3));
        }
    }

}
