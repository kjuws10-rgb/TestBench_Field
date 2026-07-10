using System;
using System.Drawing;
using System.Windows.Forms;
using StageWin.Safety;

namespace StageWin.Etc
{
    public partial class TowerLampForm : Form
    {
        public sealed class Options
        {
            public Func<ProgramMode> ModeProvider { get; set; }
            public Func<bool> HasAlarmProvider { get; set; }                 // Alarm 레벨 활성 여부
            public Func<bool> AutoSeqRunningProvider { get; set; }           // Auto 시퀀스 동작 여부
            public Action<string, bool, string> WriteOutput { get; set; }    // (name, value, source)
        }

        private readonly Options _opt;

        // UI 업데이트 타이머(램프/정책 판정)
        private readonly Timer _tmr = new Timer { Interval = 200 };

        private bool _buzzerSilencedUntilReset = false;   // 사용자 OFF 이후 Reset 전까지 자동 재가동 금지
        private bool _wantBuzzerOn = false;               // 현재 “울려야 하는 의지 상태”(정책 결정값)

        private readonly Timer _tmrBuzzKeep = new Timer { Interval = 300 }; // 지속 재전송(Keep-Alive)
        private readonly Timer _tmrBuzzKick = new Timer { Interval = 120 }; // 초기 버스트 재전송
        private int _buzzKickRemain = 0;                                     // 남은 버스트 횟수(예: 2회)

        private readonly Timer _tmrBlink = new Timer { Interval = 350 };
        private bool _blinkState = false;
        private Color _btnBack0, _btnFore0;
        private readonly Color _blinkBack = Color.FromArgb(220, 60, 60);
        private readonly Color _blinkFore = Color.White;

        // 부저 토글(1초 간격) 전용 타이머 & 상태
        private readonly Timer _tmrBuzzerBeat = new Timer { Interval = 1000 };
        private bool _buzzerState = false;


        // 부저/램프 IO 태그
        private const string OUT_BUZZER_BEEP1 = "Buzzer - Beep 1";
        private const string OUT_BUZZER_BEEP2 = "Buzzer - Beep 2";
        private const string OUT_BUZZER_BEEP3 = "Buzzer - Beep 3";
        private const string OUT_BUZZER_BEEP4 = "Buzzer - Beep 4";

        private const string OUT_LAMP_RED = "Signal Tower - Red";
        private const string OUT_LAMP_YELLOW = "Signal Tower - Yellow";
        private const string OUT_LAMP_GREEN = "Signal Tower - Green";

        // 마지막으로 쓴 IO 상태(채널별 캐싱)
        private bool? _ioRed, _ioYellow, _ioGreen;

        public TowerLampForm(Options opt)
        {
            _opt = opt ?? new Options();

            InitializeComponent(); // 컨트롤 먼저 생성
            Text = "Tower Lamp";

            // 버튼 시각효과가 보이도록(테마색 무시)
            _btnBack0 = btnBuzzerOff.BackColor;
            _btnFore0 = btnBuzzerOff.ForeColor;
            btnBuzzerOff.FlatStyle = FlatStyle.Flat;
            btnBuzzerOff.UseVisualStyleBackColor = false;
            btnBuzzerOff.Text = "Buzzer OFF";
            btnBuzzerOff.Click += (_, __) => BuzzerUserOff();

            // 점멸 타이머
            _tmrBlink.Tick += (_, __) =>
            {
                if (!_wantBuzzerOn) { StopBlink(); return; } // 의지 상태 기준
                _blinkState = !_blinkState;
                btnBuzzerOff.BackColor = _blinkState ? _blinkBack : _btnBack0;
                btnBuzzerOff.ForeColor = _blinkState ? _blinkFore : _btnFore0;
                btnBuzzerOff.Text = _blinkState ? "BUZZER RINGING (Click to Silence)" : "Buzzer OFF";
            };

            // BUZZER 신뢰성 타이머: Keep-Alive
            _tmrBuzzKeep.Tick += (_, __) =>
            {
                if (_wantBuzzerOn) WriteBuzzer(true, "KeepAlive");
            };
            _tmrBuzzKeep.Start();

            // BUZZER 신뢰성 타이머: Kick(초기 버스트 2회)
            _tmrBuzzKick.Tick += (_, __) =>
            {
                if (_buzzKickRemain > 0 && _wantBuzzerOn)
                {
                    WriteBuzzer(true, "Kick");
                    _buzzKickRemain--;
                }
                else
                {
                    _tmrBuzzKick.Stop();
                }
            };

            _tmrBuzzerBeat.Tick += (_, __) =>
            {
                _buzzerState = !_buzzerState;
                TryWrite(OUT_BUZZER_BEEP1, _buzzerState, "Beat");
                // 필요 시 다른 채널도 함께 토글
                // TryWrite(OUT_BUZZER_BEEP2, _buzzerState, "Beat");
                // TryWrite(OUT_BUZZER_BEEP3, _buzzerState, "Beat");
                // TryWrite(OUT_BUZZER_BEEP4, _buzzerState, "Beat");
            };

            // 안전 초기 IO 상태
            ApplyLampIo(red: false, yellow: false, green: false);

            // 주기 갱신
            _tmr.Tick += (_, __) => UpdateTowerLamp();
            _tmr.Start();

            UpdateTowerLamp();
        }

        private void StartBlink()
        {
            _blinkState = false;
            btnBuzzerOff.BackColor = _blinkBack;
            btnBuzzerOff.ForeColor = _blinkFore;
            btnBuzzerOff.Text = "BUZZER RINGING (Click to Silence)";
            if (!_tmrBlink.Enabled) _tmrBlink.Start();
        }

        private void StopBlink()
        {
            _tmrBlink.Stop();
            btnBuzzerOff.BackColor = _btnBack0;
            btnBuzzerOff.ForeColor = _btnFore0;
            btnBuzzerOff.Text = "Buzzer OFF";
        }

        private void SetLamp(bool red, bool yellow, bool green)
        {
            // UI
            lbLampRed.BackColor = red ? Color.Red : Color.Gray;
            lbLampYellow.BackColor = yellow ? Color.Gold : Color.Gray;
            lbLampGreen.BackColor = green ? Color.Lime : Color.Gray;

            // IO(변경된 채널만)
            ApplyLampIo(red, yellow, green);
        }

        private void ApplyLampIo(bool red, bool yellow, bool green)
        {
            if (_ioRed != red) { TryWrite(OUT_LAMP_RED, red, "TowerLamp"); _ioRed = red; }
            if (_ioYellow != yellow) { TryWrite(OUT_LAMP_YELLOW, yellow, "TowerLamp"); _ioYellow = yellow; }
            if (_ioGreen != green) { TryWrite(OUT_LAMP_GREEN, green, "TowerLamp"); _ioGreen = green; }
        }

        private void TryWrite(string name, bool val, string src)
        {
            try { _opt?.WriteOutput?.Invoke(name, val, src); } catch { /* ignore */ }
        }

        // BUZZER IO 일괄 쓰기(패턴 확장 시 여기 수정)
        // BUZZER IO 일괄 쓰기(이제 1초 간격 토글을 관리)
        private void WriteBuzzer(bool on, string src)
        {
            if (on)
            {
                // 이미 동작 중이면 재시작하지 않음(KeepAlive/Kick과 중복 호출 안전)
                if (!_tmrBuzzerBeat.Enabled)
                {
                    // 즉시 1회 ON으로 시작한 뒤 1초마다 토글
                    _buzzerState = true;
                    TryWrite(OUT_BUZZER_BEEP1, true, src);
                    // 필요 시 다른 채널도 즉시 ON
                    // TryWrite(OUT_BUZZER_BEEP2, true, src);
                    // TryWrite(OUT_BUZZER_BEEP3, true, src);
                    // TryWrite(OUT_BUZZER_BEEP4, true, src);

                    _tmrBuzzerBeat.Start();
                }
            }
            else
            {
                // 토글 정지 + 확실히 OFF
                _tmrBuzzerBeat.Stop();
                _buzzerState = false;
                TryWrite(OUT_BUZZER_BEEP1, false, src);
                // TryWrite(OUT_BUZZER_BEEP2, false, src);
                // TryWrite(OUT_BUZZER_BEEP3, false, src);
                // TryWrite(OUT_BUZZER_BEEP4, false, src);
            }
        }
        private void BuzzerUserOff()
        {
            _buzzerSilencedUntilReset = true; // Reset 전까지 자동 재가동 금지
            // 로직은 절대 OFF하지 않지만, 사용자가 누른 경우엔 즉시 OFF
            WriteBuzzer(false, "Manual");
            _wantBuzzerOn = false;            // KeepAlive/점멸 중지
            StopBlink();
        }

        // Reset 이후 재무장(무음 해제) – Form1에서 Reset 직후 호출
        public void ArmBuzzerAfterReset()
        {
            _buzzerSilencedUntilReset = false;
            // 여기서는 바로 켜지지 않음. 다음 UpdateTowerLamp에서 alarm이면 켜짐.
        }

        private void EnsureBuzzerOn()
        {
            if (_buzzerSilencedUntilReset) return; // 사용자가 무음한 상태면 금지

            if (!_wantBuzzerOn)
            {
                // 알람 최초 감지 → 즉시 ON + 버스트 + KeepAlive + 점멸
                _wantBuzzerOn = true;
                WriteBuzzer(true, "AlarmRise");
                _buzzKickRemain = 2;     // 추가로 2회 더 밀어넣기(총 3회)
                _tmrBuzzKick.Start();
                StartBlink();
            }
            // 이미 _wantBuzzerOn이면 KeepAlive가 주기적으로 재전송함.
        }

        private void UpdateTowerLamp()
        {
            var mode = _opt?.ModeProvider?.Invoke() ?? ProgramMode.Manual;
            bool alarm = _opt?.HasAlarmProvider?.Invoke() ?? false;
            bool autoOn = _opt?.AutoSeqRunningProvider?.Invoke() ?? false;

            // 우선순위: RED(알람) > GREEN(Auto 동작 중) > YELLOW(Semi/Auto 모드) > OFF
            if (alarm)
            {
                SetLamp(red: true, yellow: false, green: false);
                EnsureBuzzerOn(); // 기존 그대로
                return;
            }

            // Auto 모드 & 실제 Auto 시퀀스 동작 중이면 GREEN 유지 (기존 동작)
            if (mode == ProgramMode.Auto && autoOn)
            {
                SetLamp(red: false, yellow: false, green: true);
                return;
            }

            // 요구사항: 모드가 Semi 또는 Auto 이기만 해도 노란색 램프 ON
            if (mode == ProgramMode.SemiAuto || mode == ProgramMode.Auto)
            {
                SetLamp(red: false, yellow: true, green: false);
                return;
            }

            // 나머지(Manual 등)는 모두 OFF
            SetLamp(red: false, yellow: false, green: false);
        }

        public void RefreshNow() => UpdateTowerLamp();

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try { _tmr.Stop(); _tmr.Dispose(); } catch { }
            try { _tmrBlink.Stop(); _tmrBlink.Dispose(); } catch { }
            try { _tmrBuzzKeep.Stop(); _tmrBuzzKeep.Dispose(); } catch { }
            try { _tmrBuzzKick.Stop(); _tmrBuzzKick.Dispose(); } catch { }
            try { _tmrBuzzerBeat.Stop(); _tmrBuzzerBeat.Dispose(); } catch { }

            try
            {
                // 안전 종료시 모두 OFF (사용자 정책과 무관하게 앱 종료 시는 강제 OFF)
                WriteBuzzer(false, "Dispose");
                ApplyLampIo(false, false, false);
            }
            catch { /* ignore */ }

            base.OnFormClosed(e);
        }
    }
}
