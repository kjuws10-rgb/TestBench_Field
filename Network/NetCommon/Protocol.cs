using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCommon
{
    //  공통 프로토콜 
    public enum Cmd : int
    {
        HELLO = 10,

        // Stage(Server) → Scan(Client)
        RECIPE_LIST_REQ = 1000,
        RECIPE_ADD              = 1001,
        RECIPE_DELETE           = 1002,
        RECIPE_SELECT           = 1003,
        RECIPE_INFO_REQ         = 1004,
        RECIPE_DATA_CHANGE      = 1005,
        SCAN_STOP               = 1006,
        LASER_CONTROL           = 1007,
        SCAN_STATUS_REQ         = 1008,
        PROCESS_SCANNING        = 1009,
        POWER_METER_MEAS_1      = 1010,
        POWER_METER_MEAS_2      = 1011,

        // Scan(Client) → Stage(Server)
        MOTOR_MOVE_FROM_SCAN    = 1021, // (기존 1011 → 1021로 이동)
        MOTOR_POS_REQUEST       = 1022, // (기존 1012 → 1022로 이동)

        // Stage <-> Vision
        GLASS_ALIGN_REQ         = 3000,
        SECOND_ALIGN_REQ        = 3001,
        MARK_FIND_SINGLE        = 3002,
        MARK_FIND_LINE          = 3003,
        FLYING_READY            = 3004,
        MOTOR_MOVE_FROM_VISION  = 3005,
        MOTOR_CUR_POS_REQ       = 3006,
        FLYING_MOVE_DONE        = 3007,
        MANUAL_INSPECTION_REQ   = 3008,

        // 공통
        HEARTBEAT = 9000,
    }

    public enum Role : int { Unknown = 0, Vision = 1, Scan = 2 }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HEADER
    {
        public int nCode;       // Cmd
        public int nDataLen;    // Body bytes
        public int nUniqueID;   // Request-Response match
        public int nErrorCode;  // 0=OK
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_HELLO { public int nRole; }

    //  에러 코드 
    public static class Err
    {
        public const int OK = 0;
        public const int UNHANDLED = -1;
        public const int DISCONNECTED = -2;
        public const int TIMEOUT = -3;
        public const int IOERROR = -4;
        public const int ABORTED = -5;
        public const int NOT_FOUND = -404;     
        public const int INVALID_ARG = -400;   
    }

    //  로깅 인터페이스 
    public interface ILogger
    {
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg, Exception ex = null);
    }
    public sealed class NullLogger : ILogger
    {
        public void Info(string msg) { }
        public void Warn(string msg) { }
        public void Error(string msg, Exception ex = null) { }
    }

    //  Struct <-> byte[] 
    public static class BinMarshal
    {
        public static byte[] ToBytes<T>(T st) where T : struct
        {
            int sz = Marshal.SizeOf<T>();
            var buf = new byte[sz];
            IntPtr p = Marshal.AllocHGlobal(sz);
            try { Marshal.StructureToPtr(st, p, true); Marshal.Copy(p, buf, 0, sz); }
            finally { Marshal.FreeHGlobal(p); }
            return buf;
        }

        // 역직렬화: 0B/크기불일치 허용(제로패딩/트림)
        public static T FromBytes<T>(byte[] buf) where T : struct
        {
            int sz = Marshal.SizeOf<T>();
            IntPtr p = Marshal.AllocHGlobal(sz);
            try
            {
                // 1) unmanaged 버퍼를 0으로 초기화
                var zeros = new byte[sz];
                Marshal.Copy(zeros, 0, p, sz);

                // 2) 수신 바디가 있으면 min(len, sz) 만큼만 복사(나머지는 0 패딩 유지)
                if (buf != null && buf.Length > 0)
                {
                    int copy = Math.Min(buf.Length, sz);
                    if (buf.Length != sz)
                        Debug.WriteLine($"[BinMarshal] lenient FromBytes<{typeof(T).Name}>: {buf.Length} -> {sz} (copy {copy}, pad {sz - copy})");
                    Marshal.Copy(buf, 0, p, copy);
                }

                // 3) 0 패딩된 버퍼에서 구조체로 변환
                return (T)Marshal.PtrToStructure(p, typeof(T));
            }
            finally { Marshal.FreeHGlobal(p); }
        }

        // 엄격 모드가 필요할 때 사용할 수 있는 보조 함수
        public static T FromBytesStrict<T>(byte[] buf) where T : struct
        {
            int sz = Marshal.SizeOf<T>();
            if (buf == null || buf.Length != sz)
                throw new ArgumentException($"size mismatch: {buf?.Length ?? 0}!={sz}");
            IntPtr p = Marshal.AllocHGlobal(sz);
            try { Marshal.Copy(buf, 0, p, sz); return (T)Marshal.PtrToStructure(p, typeof(T)); }
            finally { Marshal.FreeHGlobal(p); }
        }
    }

    //  RPC Core 
    public sealed class RpcPacket
    {
        public HEADER Header;
        public byte[] Body;
        public RpcPacket() { }
        public RpcPacket(HEADER h, byte[] b) { Header = h; Body = b; }
    }

    public sealed class RpcResponse
    {
        public bool Respond;
        public int Err;
        public byte[] Body;
        public static RpcResponse OneWay() => new RpcResponse { Respond = false, Err = 0, Body = null };
        public static RpcResponse RespOK(byte[] body = null) => new RpcResponse { Respond = true, Err = 0, Body = body };
        public static RpcResponse RespErr(int err) => new RpcResponse { Respond = true, Err = err, Body = null };
    }

    //  라우터 
    public sealed class PacketRouter
    {
        private readonly ConcurrentDictionary<int, Func<RpcSession, byte[], Task<RpcResponse>>> _map
            = new ConcurrentDictionary<int, Func<RpcSession, byte[], Task<RpcResponse>>>();

        public void Register(Cmd cmd, Func<RpcSession, byte[], Task<RpcResponse>> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _map[(int)cmd] = handler;
        }

        public Task<RpcResponse> DispatchAsync(RpcSession s, Cmd code, byte[] body)
        {
            if (_map.TryGetValue((int)code, out var h))
            {
                try { return h(s, body); }
                catch (Exception ex)
                {
                    s.Logger.Error($"Router handler exception: {code}", ex);
                    return Task.FromResult(RpcResponse.OneWay());
                }
            }
            return Task.FromResult(RpcResponse.RespErr(Err.UNHANDLED));
        }
    }

    public sealed class RpcOptions
    {
        //timeout Test용으로 증가 PGT
        public int DefaultTimeoutMs = 3000000;
        public int MaxRetries = 0;
        public int RetryDelayMs = 200;
        public bool EnableHeartbeat = false;
        public int HeartbeatIntervalMs = 2000;
    }

    public sealed class RpcSession : IDisposable
    {
        readonly TcpClient _cli;
        public NetworkStream Stream => _cli.GetStream();
        public Role Role { get; set; } = Role.Unknown;
        public string Remote => _cli.Client.RemoteEndPoint?.ToString() ?? string.Empty;

        public ILogger Logger { get; set; } = new NullLogger();
        public RpcOptions Options { get; } = new RpcOptions();

        public event Action<RpcSession> Disconnected;
        public event Action<RpcSession, Exception> Error;

        readonly ConcurrentDictionary<long, TaskCompletionSource<RpcPacket>> _pend
            = new ConcurrentDictionary<long, TaskCompletionSource<RpcPacket>>();
        int _uid = 1;
        CancellationTokenSource _cts;
        Timer _hbTimer;

        // 세션 단위 전송 락(헤더+바디 원자적 전송 보장)
        readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public Func<Cmd, byte[], Task<RpcResponse>> OnRequestAsync;

        // RpcSession 클래스 필드 영역에 추가
        public Action<string> NetLog { get; set; } // UI 로그상자/파일 로깅 델리게이트
        public int NetHexPreviewBytes { get; set; } = 64; // HEX 미리보기 최대 바이트
        public bool NetDumpBodies { get; set; } = true;   // 바디 HEX 미리보기 on/off

        readonly ConcurrentDictionary<long, long> _orphan = new ConcurrentDictionary<long, long>();
        const int ORPHAN_TTL_MS = 5_000; // 5초 정도 보관
        void RememberOrphan(long key) => _orphan[key] = Environment.TickCount;

        public RpcSession(TcpClient cli)
        {
            _cli = cli ?? throw new ArgumentNullException(nameof(cli));
            try { _cli.NoDelay = true; } catch { }
        }

        static long Key(int code, int uid) => ((long)code << 32) | (uint)uid;

        string HexPreview(byte[] b)
        {
            if (!NetDumpBodies || b == null || b.Length == 0) return "-";
            int n = Math.Min(b.Length, NetHexPreviewBytes);
            var sb = new StringBuilder(n * 3);
            for (int i = 0; i < n; i++) sb.Append(b[i].ToString("X2")).Append(' ');
            if (b.Length > n) sb.Append("...");
            return sb.ToString().TrimEnd();
        }
        string RoleTag() => $"[{Role}]";
        string Dir(bool tx) => tx ? "▶" : "◀";
        void TraceTx(HEADER h, string phase, byte[] body = null, string note = null)
        {
            var code = (Cmd)h.nCode;
            var line = $"[NET] {Dir(true)} {RoleTag()} {phase,-7} {code,-18} uid={h.nUniqueID,-6} len={h.nDataLen,-5} → {Remote,-21} {(note ?? "")}  hex={HexPreview(body)}";
            try { (NetLog ?? (s => Logger.Info(s))).Invoke(line); } catch { }
        }
        void TraceRx(HEADER h, string phase, byte[] body = null, string note = null, long? rttMs = null)
        {
            var code = (Cmd)h.nCode;
            var rtt = rttMs.HasValue ? $" rtt={rttMs.Value}ms" : "";
            var err = h.nErrorCode != 0 ? $" err={h.nErrorCode}" : "";
            var line = $"[NET] {Dir(false)} {RoleTag()} {phase,-7} {code,-18} uid={h.nUniqueID,-6} len={h.nDataLen,-5}{err} ← {Remote,-21}{rtt} {(note ?? "")}  hex={HexPreview(body)}";
            try { (NetLog ?? (s => Logger.Info(s))).Invoke(line); } catch { }
        }

        bool IsOrphan(long key)
        {
            if (_orphan.TryGetValue(key, out var t))
            {
                if (Environment.TickCount - t <= ORPHAN_TTL_MS) return true;
                _orphan.TryRemove(key, out _);
            }
            return false;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _ = Task.Run(RecvLoop);

            if (Options.EnableHeartbeat)
            {
                _hbTimer = new Timer(async _ =>
                {
                    try { await SendNotifyAsync(Cmd.HEARTBEAT).ConfigureAwait(false); }
                    catch (Exception ex) { Logger.Warn("Heartbeat failed: " + ex.Message); }
                }, null, Options.HeartbeatIntervalMs, Options.HeartbeatIntervalMs);
            }
        }

        public void Stop()
        {
            try { _cts?.Cancel(); } catch { }
            try { _hbTimer?.Dispose(); } catch { }
            try { _cli?.Close(); } catch { }
        }

        public void Dispose() => Stop();

        public async Task<RpcPacket> RequestAsync(Cmd code, byte[] body, int timeoutMs = -1, int? retryOverride = null)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : Options.DefaultTimeoutMs;
            int retries = retryOverride.HasValue ? retryOverride.Value : Options.MaxRetries;

            for (int attempt = 0; ; attempt++)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew(); // RTT 측정
                try
                {
                    EnsureConnected();

                    int uid = Interlocked.Increment(ref _uid);
                    var tcs = new TaskCompletionSource<RpcPacket>(TaskCreationOptions.RunContinuationsAsynchronously);
                    if (!_pend.TryAdd(Key((int)code, uid), tcs))
                        throw new Exception("pending add failed");

                    var h = new HEADER
                    {
                        nCode = (int)code,
                        nDataLen = body?.Length ?? 0,
                        nUniqueID = uid,
                        nErrorCode = 0
                    };
                    var hb = BinMarshal.ToBytes(h);

                    //  TX 트레이스 
                    TraceTx(h, "REQ", body);

                    // 헤더+바디를 임계구역에서 연속 전송
                    await SendFrameAsync(hb, (h.nDataLen > 0 ? body : null)).ConfigureAwait(false);

                    using (var cto = new CancellationTokenSource(timeout))
                    using (cto.Token.Register(() =>
                    {
                        var key = Key((int)code, uid);
                        if (_pend.TryRemove(Key((int)code, uid), out var waiter))
                        {
                            waiter.TrySetException(new TimeoutException($"{code} timed out uid={uid}"));
                            RememberOrphan(key);
                        }
                            
                    }))
                    {
                        var pkt = await tcs.Task.ConfigureAwait(false);

                        sw.Stop();
                        //  RX(응답) 트레이스 
                        TraceRx(pkt.Header, "RSP", pkt.Body, null, sw.ElapsedMilliseconds);
                        return pkt;
                    }
                }
                catch (TimeoutException tex)
                {
                    sw.Stop();
                    Logger.Warn($"Request timeout: {code} (attempt={attempt + 1}) - {tex.Message}");
                    TraceRx(new HEADER { nCode = (int)code, nUniqueID = 0, nDataLen = 0, nErrorCode = Err.TIMEOUT }, "RSP", null, "TIMEOUT", sw.ElapsedMilliseconds);
                    if (attempt >= retries) throw;
                    await Task.Delay(Options.RetryDelayMs).ConfigureAwait(false);
                }
                catch (IOException ioex)
                {
                    Logger.Error($"IO error on request {code}: {ioex.Message}", ioex);
                    Error?.Invoke(this, ioex);
                    if (attempt >= retries) throw;
                    await Task.Delay(Options.RetryDelayMs).ConfigureAwait(false);
                }
                catch (SocketException sex)
                {
                    Logger.Error($"Socket error on request {code}: {sex.Message}", sex);
                    Error?.Invoke(this, sex);
                    if (attempt >= retries) throw;
                    await Task.Delay(Options.RetryDelayMs).ConfigureAwait(false);
                }
                catch (ObjectDisposedException od)
                {
                    Logger.Error($"Disposed during request {code}: {od.Message}", od);
                    Error?.Invoke(this, od);
                    if (attempt >= retries) throw;
                    await Task.Delay(Options.RetryDelayMs).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    Logger.Error($"Unexpected request error {code}: {ex.Message}", ex);
                    Error?.Invoke(this, ex);
                    TraceRx(new HEADER { nCode = (int)code, nUniqueID = 0, nDataLen = 0, nErrorCode = Err.IOERROR }, "RSP", null, "EXCEPTION");
                    if (attempt >= retries) throw;
                    await Task.Delay(Options.RetryDelayMs).ConfigureAwait(false);
                }
            }
        }

        public async Task SendNotifyAsync(Cmd code, byte[] body = null)
        {
            try
            {
                EnsureConnected();
                var h = new HEADER
                {
                    nCode = (int)code,
                    nDataLen = body?.Length ?? 0,
                    nUniqueID = 0,
                    nErrorCode = 0
                };
                var hb = BinMarshal.ToBytes(h);

                //  TX 트레이스 (NOTIFY) 
                TraceTx(h, "NOTIFY", body);

                // 임계구역 전송
                await SendFrameAsync(hb, (h.nDataLen > 0 ? body : null)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendNotify failed: {code}", ex);
                throw;
            }
        }

        async Task SendFrameAsync(byte[] header, byte[] body)
        {
            int hlen = header?.Length ?? 0;
            int blen = body?.Length ?? 0;
            if (hlen + blen == 0) return;

            await _sendLock.WaitAsync().ConfigureAwait(false);
            try
            {
                // 한 번에 합쳐 보내되, body==null/0 도 안전
                var packet = new byte[hlen + blen];
                if (hlen > 0) Buffer.BlockCopy(header, 0, packet, 0, hlen);
                if (blen > 0) Buffer.BlockCopy(body, 0, packet, hlen, blen);

                await Stream.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);
                await Stream.FlushAsync().ConfigureAwait(false);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        async Task SafeWriteAsync(byte[] buf)
        {
            if (buf == null || buf.Length == 0) return;
            await Stream.WriteAsync(buf, 0, buf.Length).ConfigureAwait(false);
        }

        async Task RecvLoop()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    HEADER h;
                    try
                    {
                        var hb = new byte[Marshal.SizeOf<HEADER>()];
                        await ReadExactAsync(Stream, hb, hb.Length).ConfigureAwait(false);
                        h = BinMarshal.FromBytes<HEADER>(hb);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("Recv header failed: " + ex.Message);
                        throw;
                    }

                    byte[] b = h.nDataLen > 0 ? new byte[h.nDataLen] : Array.Empty<byte>();
                    if (h.nDataLen > 0)
                    {
                        try { await ReadExactAsync(Stream, b, b.Length).ConfigureAwait(false); }
                        catch (Exception ex)
                        {
                            Logger.Warn("Recv body failed: " + ex.Message);
                            throw;
                        }
                    }

                    //  RX 수신 즉시 트레이스 (응답/요청 공통) 중복으로 남는 로그 제거.
                    if (h.nUniqueID == 0) TraceRx(h, "REQ_IN", b);

                    // 응답/요청 분기
                    long key = Key(h.nCode, h.nUniqueID);

                    // 응답 매칭
                    if (h.nUniqueID != 0 && _pend.TryRemove(key, out var waiter))
                    {
                        waiter.TrySetResult(new RpcPacket(h, b));
                        continue; // 라우터로 내려가지 않음
                    }

                    // 타임아웃 뒤 늦게 도착한 응답이면 드롭(라우터 금지)
                    if (h.nUniqueID != 0 && IsOrphan(key))
                    {
                        Logger?.Warn($"Drop late/unsolicited response: cmd={(Cmd)h.nCode} uid={h.nUniqueID}, len={b?.Length ?? 0}");
                        // 라우터로 보내지 말 것
                        continue;
                    }

                    // 신규 요청 → 핸들러
                    var code = (Cmd)h.nCode;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            if (OnRequestAsync == null) return;
                            var rr = await OnRequestAsync(code, b).ConfigureAwait(false);
                            if (rr != null && rr.Respond)
                            {
                                var rh = new HEADER
                                {
                                    nCode = h.nCode,
                                    nDataLen = (rr.Body?.Length ?? 0),
                                    nUniqueID = h.nUniqueID,
                                    nErrorCode = rr.Err
                                };
                                var rhb = BinMarshal.ToBytes(rh);

                                //  TX 응답 송신 직전 트레이스 중복 제거 
                                if (code != Cmd.MOTOR_POS_REQUEST) TraceTx(rh, "RSP_OUT", rr.Body);

                                // 응답도 임계구역에서 전송
                                await SendFrameAsync(rhb, (rh.nDataLen > 0 ? rr.Body : null)).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"OnRequest handler failed: {code}", ex);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, ex);
            }
            finally
            {
                try { _cli.Close(); } catch { }
                foreach (var kv in _pend)
                {
                    if (_pend.TryRemove(kv.Key, out var waiter))
                        waiter.TrySetException(new IOException("connection closed"));
                }
                Disconnected?.Invoke(this);
            }
        }

        static async Task ReadExactAsync(NetworkStream s, byte[] buf, int len)
        {
            int off = 0;
            while (off < len)
            {
                int r = await s.ReadAsync(buf, off, len - off).ConfigureAwait(false);
                if (r == 0) throw new IOException("remote closed");
                off += r;
            }
        }

        void EnsureConnected()
        {
            if (_cli == null || !_cli.Connected || !Stream.CanWrite || !Stream.CanRead)
                throw new IOException("not connected");
        }
    }
}
