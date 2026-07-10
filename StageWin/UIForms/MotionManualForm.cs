using StageWin.Driver.Motion;
using StageWin.Etc;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using StageWin.Safety;
using StageWin.UIForms;

namespace StageWin.UI
{
    public partial class MotionManualForm : Form
    {
        // 기본생성자 후 AttachMotion에서 주입
        private IMotionController _motion;   // null 가능
        private IAcsStatus _acs;             // null 가능
        private readonly Timer _tmr = new Timer { Interval = 100 };
        private bool _eventsWired = false;
        private CancellationTokenSource _p2pXcts, _p2pYcts;
        private bool _p2pXRunning = false, _p2pYRunning = false;

        private AjinMotionAdapter _ajin;
        private AjinHomeParams _zHomeParams = new AjinHomeParams();
        private CancellationTokenSource _p2pZcts, _p2pTcts;
        private bool _p2pZRunning = false, _p2pTRunning = false;
        private int _alarmBlinkTick = 0;
        private readonly Color _btnAlarmBlinkColor = Color.OrangeRed;
        private readonly Color _btnNormalColor = SystemColors.Control;
        public Func<ISafetyContext> GetSafetyContext { get; set; }  // Safety 컨텍스트 주입
        private volatile bool _safetyStopping = false;
        private ISafetyContext _fallbackCtx;
        public Func<ProgramMode> ModeProvider { get; set; }
        private Func<(bool minus, bool plus)> MakeLimitSnapshotProvider(AjinAxis ax)
        {
            return () =>
            {
                AxisIoState s = _ajin?.IoSenseProvider?.Invoke(ax) ?? default;  // non-nullable 로 강제
                return (s.LimitMinus, s.LimitPlus);
            };
        }


        // 어디서든 호출할 수 있게 헬퍼 추가
        private ISafetyContext TryGetSafetyCtx()
        {
            // 1순위: 외부에서 주입된 컨텍스트 사용
            var ctx = GetSafetyContext?.Invoke();
            if (ctx != null) return ctx;

            // 2순위: 폴백 컨텍스트 — 반드시 실제 ProgramMode를 끌어오도록 수정
            if (_fallbackCtx == null)
            {
                _fallbackCtx = new BasicSafetyContext(
                    modeGetter: () => (ModeProvider != null ? ModeProvider() : ProgramMode.Manual)
                );
            }
            return _fallbackCtx;
        }
        private bool IsAxisHomed(Axis ax)
        {
            return _acs != null && _acs.IsServoOn(ax) && _acs.IsHomeDone(ax);
        }
        private bool IsAjinAxisHomed(AjinAxis ax)
        {
            return _ajin != null && _ajin.IsConnected && _ajin.IsServoOn(ax) && _ajin.IsHomed(ax);
        }
        private bool GuardAcsAxisHomed(Axis ax, string action)
        {
            return true;
            if (IsAxisHomed(ax)) return true;
            BeginInvoke(new Action(() =>
                MessageBox.Show(this, $"{ax} 축이 Home 완료 상태가 아닙니다.\n(Servo ON + Home Done 필요)  동작: {action}",
                    "Home Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
            return false;
        }
        private bool GuardAjinAxisHomed(AjinAxis ax, string action)
        {
            if (IsAjinAxisHomed(ax)) return true;
            BeginInvoke(new Action(() =>
                MessageBox.Show(this, $"{ax} 축이 Home 완료 상태가 아닙니다.\n(Servo ON + Home Done 필요)  동작: {action}",
                    "Home Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
            return false;
        }
        private bool GuardAjinLimitDirection(AjinAxis ax, int dir, string action)
        {
            var sense = MakeLimitSnapshotProvider(ax);
            var r = StageWin.Safety.SafetyPolicy.CheckLimitDirection(sense, dir, ax.ToString());
            if (!r.Allowed)
            {
                BeginInvoke(new Action(() =>
                    MessageBox.Show(this, $"{action} 불가: {r.Reason}", "Limit Interlock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                return false;
            }
            return true;
        }

        //  기본 생성자(디자이너/런타임 공용) 
        public MotionManualForm()
        {
            InitializeComponent();

            // 보기 옵션(필요없으면 제거 가능)
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(10);

            // 모션 미부착 시 조작 버튼 비활성화
            SetMotionControlsEnabled(false);
            SetAjinControlsEnabled(false);

            this.FormClosing += (s, e) =>
            {
                try { _tmr?.Stop(); }       catch { }
                try { _p2pXcts?.Cancel(); } catch { }
                try { _p2pYcts?.Cancel(); } catch { }
                try { _p2pZcts?.Cancel(); } catch { }   
                try { _p2pTcts?.Cancel(); } catch { }   
            };
        }

        //  런타임에 모션 주입 
        public void AttachMotion(IMotionController motion)
        {
            if (motion == null) throw new ArgumentNullException(nameof(motion));

            _motion = motion;
            _acs = motion as IAcsStatus;

            if (!_eventsWired)
            {
                WireAcsEvents();           // 이벤트는 한 번만 묶음
                _eventsWired = true;
            }

            SetMotionControlsEnabled(true);

            _tmr.Tick -= Tmr_Tick;
            _tmr.Tick += Tmr_Tick;
            _tmr.Start();
            SafeUiPoll();
        }

        // Ajin 보드 Attach (Maint-Z / Theta-T)
        public void AttachAjin(AjinMotionAdapter ajin)
        {
            if (ajin == null) throw new ArgumentNullException(nameof(ajin));
            _ajin = ajin;

            // 외부 IO 모드 on + 센서 읽기 주입 + 알람 구독
            _ajin.UseExternalIoForHomeAndLimits = true;

            // 실제 프로젝트의 Wago 매핑에 맞게 구현하세요.
            _ajin.IoSenseProvider = WagoIoSense.Build(WagoIoSense.ReadByAddressFlexible);

            _ajin.IoAlarm += (ax, msg) =>
            {
                if (!IsDisposed) BeginInvoke(new Action(() => ShowWarn(msg)));
            };

            WireAjinEvents();
            SetAjinControlsEnabled(true);

            _tmr.Tick -= Tmr_Tick;
            _tmr.Tick += Tmr_Tick;
            _tmr.Start();
            SafeUiPoll();
        }
        private void Tmr_Tick(object sender, EventArgs e) => SafeUiPoll();


        // == 이벤트 바인딩 (AttachMotion에서만 호출) ==
        private void WireAcsEvents()
        {
            // X
            btnXMove.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.X, "X Move")) return;
                await DoMoveAsync(Axis.X, numXTarget.Value, rXAbs.Checked, numXSpeed.Value, numXAcc.Value, numXDec.Value); };
            btnXStop.Click += async (_, __) => await SafeCall(_motion.StopAsync(Axis.X));
            btnXHome.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                using (StageWin.Etc.BlinkNotifier.Show(this, "X 축 Home 동작 중..."))
                {
                    await SafeCall(_motion.HomeAsync(Axis.X));
                }};
            btnXServo.Click += async (_, __) => await ToggleServoAsync(Axis.X);

            btnXJogMinus.MouseDown += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.X, "X Jog -")) return;
                await SafeCall(_motion.JogStartAsync(Axis.X, positive: false, (double)numXJog.Value)); };
            btnXJogMinus.MouseUp += async (_, __) => await SafeCall(_motion.JogStopAsync(Axis.X));
            btnXJogPlus.MouseDown += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.X, "X Jog +")) return;
                await SafeCall(_motion.JogStartAsync(Axis.X, positive: true, (double)numXJog.Value)); };
            btnXJogPlus.MouseUp += async (_, __) => await SafeCall(_motion.JogStopAsync(Axis.X));

            // Y
            btnYMove.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.Y, "Y Move")) return;
                await DoMoveAsync(Axis.Y, numYTarget.Value, rYAbs.Checked, numYSpeed.Value, numYAcc.Value, numYDec.Value); };
            btnYStop.Click += async (_, __) => await SafeCall(_motion.StopAsync(Axis.Y));
            btnYHome.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                using (StageWin.Etc.BlinkNotifier.Show(this, "Y 축 Home 동작 중..."))
                {
                    await SafeCall(_motion.HomeAsync(Axis.Y));
                }};
            btnYServo.Click += async (_, __) => await ToggleServoAsync(Axis.Y);

            btnYJogMinus.MouseDown += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.Y, "Y Jog -")) return;
                await SafeCall(_motion.JogStartAsync(Axis.Y, positive: false, (double)numYJog.Value)); };
            btnYJogMinus.MouseUp += async (_, __) => await SafeCall(_motion.JogStopAsync(Axis.Y));
            btnYJogPlus.MouseDown += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.Y, "Y Jog +")) return;
                await SafeCall(_motion.JogStartAsync(Axis.Y, positive: true, (double)numYJog.Value)); };
            btnYJogPlus.MouseUp += async (_, __) => await SafeCall(_motion.JogStopAsync(Axis.Y));

            // P2P (X/Y)
            btnXP2PStart.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.X, "X P2P")) return;
                await StartP2PAsync(Axis.X); };
            btnXP2PStop.Click += async (_, __) => await StopP2PAsync(Axis.X);
            btnYP2PStart.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                if (!GuardAcsAxisHomed(Axis.Y, "Y P2P")) return;
                await StartP2PAsync(Axis.Y); };
            btnYP2PStop.Click += async (_, __) => await StopP2PAsync(Axis.Y);
        }
        private bool _ajinEventsWired = false;
        private void WireAjinEvents()
        {
            if (_ajinEventsWired) return;

            // === Maint-Z (AjinAxis.MaintZ) ===
            btnMZMove.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.MaintZ1, "MaintZ Move")) return;
                await DoMoveAjinAsync(AjinAxis.MaintZ1, numMZTarget.Value, rMZAbs.Checked, numMZSpeed.Value, numMZAcc.Value, numMZDec.Value); };
            btnMZStop.Click += (_, __) => SafeCallAjin(() => _ajin.Stop(AjinAxis.MaintZ1));
            btnMZHome.Click += async (_, __) =>
            {
                if (!GuardAllowAxisMove()) return;
                // 홈 전 알람 검사 → 리셋 → 진행
                if (!await EnsureNoAjinAlarmBeforeAction(AjinAxis.MaintZ1, "Maint-Z Home")) return;
                using (StageWin.Etc.BlinkNotifier.Show(this, "Maint-Z 축 Home 동작 중..."))
                    await SafeCallAjinAsync(_ajin.HomeAsync(AjinAxis.MaintZ1, BuildHomeParamsFromZUi()));
                    _ajin.PosErrZeroSet(AjinAxis.MaintZ1);
            };
            btnMZServo.Click += async (_, __) => await ToggleServoAjinAsync(AjinAxis.MaintZ1);
            btnMZJogMinus.MouseDown += (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.MaintZ1, "MaintZ Jog -")) return;
                if (!GuardAjinLimitDirection(AjinAxis.MaintZ1, -1, "MaintZ Jog -")) return;
                SafeCallAjin(() => _ajin.JogStart(AjinAxis.MaintZ1, positive: false, (double)numMZJog.Value, (double)numMZAcc.Value, (double)numMZDec.Value)); };
            btnMZJogMinus.MouseUp += (_, __) => SafeCallAjin(() => _ajin.Stop(AjinAxis.MaintZ1));
            btnMZJogPlus.MouseDown += (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.MaintZ1, "MaintZ Jog +")) return;
                if (!GuardAjinLimitDirection(AjinAxis.MaintZ1, +1, "MaintZ Jog +")) return;
                SafeCallAjin(() => _ajin.JogStart(AjinAxis.MaintZ1, positive: true, (double)numMZJog.Value, (double)numMZAcc.Value, (double)numMZDec.Value)); };
            btnMZJogPlus.MouseUp += (_, __) => SafeCallAjin(() => _ajin.Stop(AjinAxis.MaintZ1));
            btnMZP2PStart.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.MaintZ1, "MaintZ P2P")) return;
                await StartP2PAjinAsync(AjinAxis.MaintZ1); };
            btnMZP2PStop.Click += async (_, __) => await StopP2PAjinAsync(AjinAxis.MaintZ1);
            btnMZAlarmReset.Click += async (_, __) => await ResetAjinAlarmAsync(AjinAxis.MaintZ1, showResult: true);

            // === Theta-T (AjinAxis.T) ===
            btnTMove.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.T, "Theta Move")) return;
                await DoMoveAjinAsync(AjinAxis.T, numTTarget.Value, rTAbs.Checked, numTSpeed.Value, numTAcc.Value, numTDec.Value); };
            btnTStop.Click += (_, __) => SafeCallAjin(() => _ajin.Stop(AjinAxis.T));
            btnTHome.Click += async (_, __) =>
            {
                if (!GuardAllowAxisMove()) return;
                if (!await EnsureNoAjinAlarmBeforeAction(AjinAxis.T, "Theta Home")) return;
                using (StageWin.Etc.BlinkNotifier.Show(this, "Theta 축 Home 동작 중..."))
                    await SafeCallAjinAsync(_ajin.HomeAsync(AjinAxis.T, BuildHomeParamsFromTUi()));
                //_ajin.PosErrZeroSet(AjinAxis.T);

            };
            btnTServo.Click += async (_, __) => await ToggleServoAjinAsync(AjinAxis.T);
            btnTJogMinus.MouseDown += (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.T, "Theta Jog -")) return;
                if (!GuardAjinLimitDirection(AjinAxis.T, -1, "Theta Jog -")) return;
                SafeCallAjin(() => _ajin.JogStart(AjinAxis.T, positive: false, (double)numTJog.Value, (double)numTAcc.Value, (double)numTDec.Value)); };
            btnTJogMinus.MouseUp += (_, __) => SafeCallAjin(() => _ajin.Stop(AjinAxis.T));
            btnTJogPlus.MouseDown += (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.T, "Theta Jog +")) return;
                if (!GuardAjinLimitDirection(AjinAxis.T, +1, "Theta Jog +")) return;
                SafeCallAjin(() => _ajin.JogStart(AjinAxis.T, positive: true, (double)numTJog.Value, (double)numTAcc.Value, (double)numTDec.Value)); };
            btnTJogPlus.MouseUp += (_, __) => SafeCallAjin(() => _ajin.Stop(AjinAxis.T));
            btnTP2PStart.Click += async (_, __) => {
                if (!GuardAllowAxisMove()) return;
                //if (!GuardAjinAxisHomed(AjinAxis.T, "Theta P2P")) return;
                await StartP2PAjinAsync(AjinAxis.T); };
            btnTP2PStop.Click += async (_, __) => await StopP2PAjinAsync(AjinAxis.T);
            btnTAlarmReset.Click += async (_, __) => await ResetAjinAlarmAsync(AjinAxis.T, showResult: true);

            _ajinEventsWired = true;
        }

        private AjinHomeParams BuildHomeParamsFromZUi()
        {
            // Z 홈 UI 컨트롤 이름 예시: cmbZHomeDir, cmbZHomeSignal, cmbZZPhase, numZHomeClrMs, numZHomeOffset, numZHomeV1 ... 등
            // 네가 쓴 실제 컨트롤 이름으로 매핑만 바꿔줘.
            return new AjinHomeParams
            {
                //Direction = (cmbZHomeDir?.SelectedIndex ?? 0),
                //HomeSignal = (uint)(cmbZHomeSignal?.SelectedIndex ?? 0),
                //UseZPhase = (uint)(cmbZZPhase?.SelectedIndex ?? 0),
                //ClearTimeMs = (double)(numZHomeClrMs?.Value ?? 0),
                //Offset = (double)(numZHomeOffset?.Value ?? 0),
                //
                //Vel1st = (double)(numZHomeV1?.Value ?? 10),
                //Vel2nd = (double)(numZHomeV2?.Value ?? 5),
                //Vel3rd = (double)(numZHomeV3?.Value ?? 1),
                //VelLast = (double)(numZHomeVLast?.Value ?? 0.5),
                //Acc1st = (double)(numZHomeA1?.Value ?? 100),
                //Acc2nd = (double)(numZHomeA2?.Value ?? 50),
            };
        }

        private AjinHomeParams BuildHomeParamsFromTUi()
        {
            return new AjinHomeParams
            {
                //Direction = (cmbTHomeDir?.SelectedIndex ?? 0),
                //HomeSignal = (uint)(cmbTHomeSignal?.SelectedIndex ?? 0),
                //UseZPhase = (uint)(cmbTZPhase?.SelectedIndex ?? 0),
                //ClearTimeMs = (double)(numTHomeClrMs?.Value ?? 0),
                //Offset = (double)(numTHomeOffset?.Value ?? 0),
                //
                //Vel1st = (double)(numTHomeV1?.Value ?? 10),
                //Vel2nd = (double)(numTHomeV2?.Value ?? 5),
                //Vel3rd = (double)(numTHomeV3?.Value ?? 1),
                //VelLast = (double)(numTHomeVLast?.Value ?? 0.5),
                //Acc1st = (double)(numTHomeA1?.Value ?? 100),
                //Acc2nd = (double)(numTHomeA2?.Value ?? 50),
            };
        }
        private bool GuardAllowAxisMove(string src = "Manual")
        {
            var ctx = TryGetSafetyCtx();
            if (ctx == null) return true; // 컨텍스트 없으면 통과(필요 시 false로 바꿔도 됨)

            var eval = SafetyPolicy.CheckGlobalMotionInterlockForAxis(ctx);
            if (!eval.Allowed)
            {
                var msg = eval.Reason ?? "도어 인터락으로 축 동작이 차단되었습니다.";
                BeginInvoke(new Action(() =>
                    MessageBox.Show(this, msg, "Safety", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                return false;
            }
            return true;
        }
        private async Task ToggleServoAjinAsync(AjinAxis ax)
        {
            if (_ajin == null || !_ajin.IsConnected) return;
            try
            {
                if (_ajin.IsServoOn(ax)) _ajin.ServoOff(ax);
                else _ajin.ServoOn(ax);
            }
            catch (Exception ex) { ShowWarn(ex.Message); }
            finally { SafeUiPoll(); }
        }

        private async Task DoMoveAjinAsync(AjinAxis ax, decimal target, bool abs, decimal v, decimal a, decimal d)
        {
            if (_ajin == null || !_ajin.IsConnected) return;
            try
            {
                int dir = 0;
                if (abs)
                {
                    double cur = _ajin.GetActPos(ax);
                    dir = Math.Sign((double)target - cur);
                }
                else
                {
                    dir = Math.Sign((double)target); // 상대이동: delta 부호
                }
                if (dir != 0 && !GuardAjinLimitDirection(ax, dir, $"{ax} Move")) return;

                if (abs)
                    await _ajin.MoveAbsAsync(ax, (double)target, (double)v, (double)a, (double)d);
                else
                    await _ajin.MoveRelAsync(ax, (double)target, (double)v, (double)a, (double)d);
            }
            catch (Exception ex) { ShowWarn("Ajin Move failed: " + ex.Message); }
        }

        // P2P 시작/정지
        private async Task StartP2PAjinAsync(AjinAxis ax)
        {
            if (_ajin == null || !_ajin.IsConnected) return;

            // UI 바인딩
            decimal a, b, v, acc, dec, dwell, cnt;
            Label lb;
            Button btnStart, btnStop;

            if (ax == AjinAxis.MaintZ1)
            {
                if (_p2pZRunning) return;
                a = numMZP2PA.Value; b = numMZP2PB.Value; v = numMZP2PVel.Value; acc = numMZP2PAcc.Value; dec = numMZP2PDec.Value; dwell = numMZP2PDwell.Value; cnt = numMZP2PCount.Value;
                lb = lbMZP2PStatus; btnStart = btnMZP2PStart; btnStop = btnMZP2PStop;
                _p2pZcts?.Dispose(); _p2pZcts = new CancellationTokenSource();
                _p2pZRunning = true;
            }
            else
            {
                if (_p2pTRunning) return;
                a = numTP2PA.Value; b = numTP2PB.Value; v = numTP2PVel.Value; acc = numTP2PAcc.Value; dec = numTP2PDec.Value; dwell = numTP2PDwell.Value; cnt = numTP2PCount.Value;
                lb = lbTP2PStatus; btnStart = btnTP2PStart; btnStop = btnTP2PStop;
                _p2pTcts?.Dispose(); _p2pTcts = new CancellationTokenSource();
                _p2pTRunning = true;
            }

            btnStart.Enabled = false; btnStop.Enabled = true;
            lb.Text = "Running...";

            var cts = (ax == AjinAxis.MaintZ1) ? _p2pZcts : _p2pTcts;
            var token = cts.Token;

            try
            {
                int total = Math.Max(0, (int)cnt);
                if ((double)Math.Abs(a - b) < 1e-9 || total == 0)
                {
                    await _ajin.MoveAbsAsync(ax, (double)a, (double)v, (double)acc, (double)dec, ct: token);
                    SetTextSafe(lb, "Done");
                    return;
                }

                // 시작은 항상 A
                await _ajin.MoveAbsAsync(ax, (double)a, (double)v, (double)acc, (double)dec, ct: token);
                await Task.Delay((int)dwell, token);

                for (int i = 1; i <= total; i++)
                {
                    token.ThrowIfCancellationRequested();

                    // A -> B
                    await _ajin.MoveAbsAsync(ax, (double)b, (double)v, (double)acc, (double)dec, ct: token);
                    await Task.Delay((int)dwell, token);

                    // B -> A
                    await _ajin.MoveAbsAsync(ax, (double)a, (double)v, (double)acc, (double)dec, ct: token);
                    if (i < total) await Task.Delay((int)dwell, token);

                    // 카운트는 A 복귀 시점에 증가
                    SetTextSafe(lb, $"Running... {i} / {total}");
                }

                SetTextSafe(lb, "Done"); // 마지막 위치는 A
            }
            catch (TaskCanceledException)
            {
                SetTextSafe(lb, "Canceled");
            }
            catch (Exception ex)
            {
                SetTextSafe(lb, "Error: " + ex.Message);
            }
            finally
            {
                if (ax == AjinAxis.MaintZ1) _p2pZRunning = false; else _p2pTRunning = false;
                UiEnableSafe(btnStart, true);
                UiEnableSafe(btnStop, false);
            }
        }

        private async Task StopP2PAjinAsync(AjinAxis ax)
        {
            try
            {
                if (ax == AjinAxis.MaintZ1)
                {
                    _p2pZcts?.Cancel();
                    SafeCallAjin(() => _ajin.Stop(AjinAxis.MaintZ1));
                }
                else
                {
                    _p2pTcts?.Cancel();
                    SafeCallAjin(() => _ajin.Stop(AjinAxis.T));
                }
            }
            catch (Exception ex) { ShowWarn(ex.Message); }
        }
        private void UpdateAlarmResetButton(Button btn, bool alarmOn)
        {
            if (btn == null) return;

            if (alarmOn)
            {
                // 100ms 타이머 틱마다 색상 토글(점멸)
                btn.BackColor = (_alarmBlinkTick % 2 == 0) ? _btnAlarmBlinkColor : _btnNormalColor;
                btn.Enabled = true; // 알람 때만 눌러 의미 있게
            }
            else
            {
                btn.BackColor = _btnNormalColor;
                btn.Enabled = false; // 알람 없으면 비활성(원하면 true로 바꿔도 됨)
            }
        }
        private async Task<bool> ResetAjinAlarmAsync(AjinAxis axis, bool showResult = true)
        {
            if (_ajin == null || !_ajin.IsConnected) return false;

            try
            {
                // HW 리셋은 Block 호출 → 백그라운드에서 수행
                bool ok = await Task.Run(() => _ajin.ResetAlarm(axis));
                if (showResult)
                {
                    if (ok) BeginInvoke(new Action(() =>
                        MessageBox.Show(this, $"{axis} 축 알람이 해제되었습니다.", "Alarm Reset", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    else BeginInvoke(new Action(() =>
                        MessageBox.Show(this, $"{axis} 축 알람 해제 실패. 드라이버 상태를 확인하세요.", "Alarm Reset", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
                return ok;
            }
            catch (Exception ex)
            {
                ShowWarn("Alarm reset error: " + ex.Message);
                return false;
            }
            finally
            {
                SafeUiPoll();
            }
        }
        private async Task<bool> EnsureNoAjinAlarmBeforeAction(AjinAxis axis, string actionLabel)
        {
            if (_ajin == null || !_ajin.IsConnected) return true;

            if (_ajin.IsAlarm(axis))
            {
                // 사용자 인지용 팝업(안내) + 자동 리셋 시도
                var dr = DialogResult.OK;
                try
                {
                    dr = MessageBox.Show(
                        this,
                        $"{axis} 축에서 서보 알람이 감지되었습니다.\n\n" +
                        $"[조치] {actionLabel} 실행 전 알람을 리셋합니다.",
                        "Ajin Alarm",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);
                }
                catch { /* headless 안전 */ }

                if (dr != DialogResult.OK) return false;

                var ok = await ResetAjinAlarmAsync(axis, showResult: false);
                if (!ok)
                {
                    ShowWarn($"{axis} 축 알람 리셋 실패로 '{actionLabel}'을(를) 중단합니다.");
                    return false;
                }
            }
            return true;
        }
        private async Task SafeCallAjinAsync(Task t)
        {
            try { await t; } catch (Exception ex) { ShowWarn(ex.Message); }
        }
        private void SafeCallAjin(Action act)
        {
            try { act(); } catch (Exception ex) { ShowWarn(ex.Message); }
        }

        private void SetAjinControlsEnabled(bool enabled)
        {
            // Maint-Z
            if (btnMZMove != null) btnMZMove.Enabled = enabled;
            if (btnMZStop != null) btnMZStop.Enabled = enabled;
            if (btnMZHome != null) btnMZHome.Enabled = enabled;
            if (btnMZServo != null) btnMZServo.Enabled = enabled;
            if (btnMZJogMinus != null) btnMZJogMinus.Enabled = enabled;
            if (btnMZJogPlus != null) btnMZJogPlus.Enabled = enabled;

            if (numMZTarget != null) numMZTarget.Enabled = enabled;
            if (numMZSpeed != null) numMZSpeed.Enabled = enabled;
            if (numMZAcc != null) numMZAcc.Enabled = enabled;
            if (numMZDec != null) numMZDec.Enabled = enabled;
            if (numMZJog != null) numMZJog.Enabled = enabled;
            if (rMZAbs != null) rMZAbs.Enabled = enabled;
            if (rMZRel != null) rMZRel.Enabled = enabled;

            if (btnMZP2PStart != null) btnMZP2PStart.Enabled = enabled;
            if (btnMZP2PStop != null) btnMZP2PStop.Enabled = false;

            if (numMZP2PA != null) numMZP2PA.Enabled = enabled;
            if (numMZP2PB != null) numMZP2PB.Enabled = enabled;
            if (numMZP2PVel != null) numMZP2PVel.Enabled = enabled;
            if (numMZP2PAcc != null) numMZP2PAcc.Enabled = enabled;
            if (numMZP2PDec != null) numMZP2PDec.Enabled = enabled;
            if (numMZP2PDwell != null) numMZP2PDwell.Enabled = enabled;
            if (numMZP2PCount != null) numMZP2PCount.Enabled = enabled;

            // Theta-T
            if (btnTMove != null) btnTMove.Enabled = enabled;
            if (btnTStop != null) btnTStop.Enabled = enabled;
            if (btnTHome != null) btnTHome.Enabled = enabled;
            if (btnTServo != null) btnTServo.Enabled = enabled;
            if (btnTJogMinus != null) btnTJogMinus.Enabled = enabled;
            if (btnTJogPlus != null) btnTJogPlus.Enabled = enabled;

            if (numTTarget != null) numTTarget.Enabled = enabled;
            if (numTSpeed != null) numTSpeed.Enabled = enabled;
            if (numTAcc != null) numTAcc.Enabled = enabled;
            if (numTDec != null) numTDec.Enabled = enabled;
            if (numTJog != null) numTJog.Enabled = enabled;
            if (rTAbs != null) rTAbs.Enabled = enabled;
            if (rTRel != null) rTRel.Enabled = enabled;

            if (btnTP2PStart != null) btnTP2PStart.Enabled = enabled;
            if (btnTP2PStop != null) btnTP2PStop.Enabled = false;

            if (numTP2PA != null) numTP2PA.Enabled = enabled;
            if (numTP2PB != null) numTP2PB.Enabled = enabled;
            if (numTP2PVel != null) numTP2PVel.Enabled = enabled;
            if (numTP2PAcc != null) numTP2PAcc.Enabled = enabled;
            if (numTP2PDec != null) numTP2PDec.Enabled = enabled;
            if (numTP2PDwell != null) numTP2PDwell.Enabled = enabled;
            if (numTP2PCount != null) numTP2PCount.Enabled = enabled;
        }
        private async Task ToggleServoAsync(Axis ax)
        {
            if (_motion == null) return;
            try
            {
                if (_acs != null && _acs.IsServoOn(ax))
                    await SafeCall(_motion.ServoOffAsync(ax));
                else
                    await SafeCall(_motion.ServoOnAsync(ax));
            }
            finally { SafeUiPoll(); }
        }
        private async Task DoMoveAsync(Axis ax, decimal target, bool abs, decimal v, decimal a, decimal d)
        {
            if (_motion == null) return; // 안전가드
            try
            {
                var p = _motion.GetProfile(ax);
                _motion.SetProfile(ax, (double)v, (double)a, (double)d);
                if (abs)
                    await _motion.MoveAbsAsync(ax, (double)target, (double)v, (double)a, (double)d);
                else
                    await _motion.MoveRelAsync(ax, (double)target, (double)v, (double)a, (double)d);
            }
            catch (Exception ex) { ShowWarn("Move failed: " + ex.Message); }
        }

        private async Task StartP2PAsync(Axis ax)
        {
            if (_motion == null) return;

            // UI 값 준비
            decimal a, b, v, acc, dec, dwell, cnt;
            Label lb;
            Button btnStart, btnStop;

            if (ax == Axis.X)
            {
                if (_p2pXRunning) return;
                a = numXP2PA.Value; b = numXP2PB.Value; v = numXP2PVel.Value; acc = numXP2PAcc.Value; dec = numXP2PDec.Value; dwell = numXP2PDwell.Value; cnt = numXP2PCount.Value;
                lb = lbXP2PStatus; btnStart = btnXP2PStart; btnStop = btnXP2PStop;
                _p2pXcts?.Dispose(); _p2pXcts = new CancellationTokenSource();
                _p2pXRunning = true;
            }
            else
            {
                if (_p2pYRunning) return;
                a = numYP2PA.Value; b = numYP2PB.Value; v = numYP2PVel.Value; acc = numYP2PAcc.Value; dec = numYP2PDec.Value; dwell = numYP2PDwell.Value; cnt = numYP2PCount.Value;
                lb = lbYP2PStatus; btnStart = btnYP2PStart; btnStop = btnYP2PStop;
                _p2pYcts?.Dispose(); _p2pYcts = new CancellationTokenSource();
                _p2pYRunning = true;
            }

            btnStart.Enabled = false; btnStop.Enabled = true;
            lb.Text = "Running...";

            var cts = (ax == Axis.X) ? _p2pXcts : _p2pYcts;
            var token = cts.Token;

            try
            {
                // (A) 바쁨이면 잠깐 대기(최대 3초)
                var t0 = Environment.TickCount;
                while (_motion.IsBusy(ax))
                {
                    if (Environment.TickCount - t0 > 3000) break;
                    await Task.Delay(20, token);
                }

                // (B) 프로파일 저장/적용
                var p0 = _motion.GetProfile(ax);
                _motion.SetProfile(ax, (double)v, (double)acc, (double)dec);

                // (C) 동일지점 방어
                int total = Math.Max(0, (int)cnt);
                if ((double)Math.Abs(a - b) < 1e-9 || total == 0)
                {
                    // A==B이거나 카운트 0이면 A로만 이동하고 종료
                    await _motion.MoveAbsAsync(ax, (double)a, (double)v, (double)acc, (double)dec);
                    if (!IsDisposed) BeginInvoke(new Action(() => lb.Text = "Done"));
                    _motion.SetProfile(ax, p0.Velocity, p0.Acceleration, p0.Deceleration);
                    return;
                }

                // (D) 시작점 정렬: 항상 A에서 시작
                await _motion.MoveAbsAsync(ax, (double)a, (double)v, (double)acc, (double)dec);
                await Task.Delay((int)dwell, token);

                // (E) 1카운트 = A→B→A 완주
                for (int i = 1; i <= total; i++)
                {
                    token.ThrowIfCancellationRequested();

                    // A -> B
                    await _motion.MoveAbsAsync(ax, (double)b, (double)v, (double)acc, (double)dec);
                    await Task.Delay((int)dwell, token);

                    // B -> A
                    await _motion.MoveAbsAsync(ax, (double)a, (double)v, (double)acc, (double)dec);
                    if (i < total) await Task.Delay((int)dwell, token); // 마지막 A에서는 굳이 대기 안 해도 됨(원하면 유지)

                    // 카운트는 A에 복귀한 시점에 갱신
                    if (!IsDisposed) BeginInvoke(new Action(() => lb.Text = $"Running... {i} / {total}"));
                }

                // (F) 프로파일 복원 및 완료
                _motion.SetProfile(ax, p0.Velocity, p0.Acceleration, p0.Deceleration);
                if (!IsDisposed) BeginInvoke(new Action(() => lb.Text = "Done"));
            }
            catch (TaskCanceledException)
            {
                if (!IsDisposed) BeginInvoke(new Action(() => lb.Text = "Canceled"));
            }
            catch (Exception ex)
            {
                if (!IsDisposed) BeginInvoke(new Action(() => lb.Text = "Error: " + ex.Message));
            }
            finally
            {
                if (ax == Axis.X) _p2pXRunning = false; else _p2pYRunning = false;
                if (!IsDisposed) BeginInvoke(new Action(() => { btnStart.Enabled = true; btnStop.Enabled = false; }));
            }
        }

        private async Task StopP2PAsync(Axis ax)
        {
            try
            {
                if (ax == Axis.X)
                {
                    _p2pXcts?.Cancel();
                    await SafeCall(_motion.StopAsync(Axis.X)); // 즉시 감속 정지
                }
                else
                {
                    _p2pYcts?.Cancel();
                    await SafeCall(_motion.StopAsync(Axis.Y));
                }
            }
            catch (Exception ex)
            {
                ShowWarn(ex.Message);
            }
        }

        private async Task SafeCall(Task t)
        {
            try { await t; } catch (Exception ex) { ShowWarn(ex.Message); }
        }

        private void SafeUiPoll()
        {
            if (_motion != null)
            {
                try
                {
                    // X
                    double x = _motion.GetPosition(Axis.X);
                    double tX = _motion.GetTargetPosition(Axis.X);
                    double AX = _motion.GetAPosition(Axis.X);
                    bool xBusy = _motion.IsBusy(Axis.X);
                    lbXPos.Text = $"{x:F3} mm";
                    lblXtPos.Text = $"{tX:F3} mm";
                    lblXAPos.Text = $"{AX:F3} mm";
                    Chip(chipXMove, xBusy);
                    if (_acs != null)
                    {
                        Chip(chipXServo, _acs.IsServoOn(Axis.X));
                        Chip(chipXInpos, _acs.IsInPosition(Axis.X));
                    }

                    // Y
                    double y = _motion.GetPosition(Axis.Y);
                    double tY = _motion.GetTargetPosition(Axis.Y);
                    double AY = _motion.GetAPosition(Axis.Y);
                    bool yBusy = _motion.IsBusy(Axis.Y);
                    lbYPos.Text = $"{y:F3} mm";
                    lblYtPos.Text = $"{tY:F3} mm";
                    lblYAPos.Text = $"{AY:F3} mm";
                    Chip(chipYMove, yBusy);
                    if (_acs != null)
                    {
                        Chip(chipYServo, _acs.IsServoOn(Axis.Y));
                        Chip(chipYInpos, _acs.IsInPosition(Axis.Y));
                    }

                    // 프로파일 표시
                    //var px = _motion.GetProfile(Axis.X);
                    //numXSpeed.Value = Clamp(px.Velocity, numXSpeed);
                    //numXAcc.Value = Clamp(px.Acceleration, numXAcc);
                    //numXDec.Value = Clamp(px.Deceleration, numXDec);
                    //
                    //var py = _motion.GetProfile(Axis.Y);
                    //numYSpeed.Value = Clamp(py.Velocity, numYSpeed);
                    //numYAcc.Value = Clamp(py.Acceleration, numYAcc);
                    //numYDec.Value = Clamp(py.Deceleration, numYDec);

                    Chip(chipXHomeStatus, _acs.IsServoOn(Axis.X) && _acs.IsHomeDone(Axis.X));
                    Chip(chipYHomeStatus, _acs.IsServoOn(Axis.Y) && _acs.IsHomeDone(Axis.Y));
                }
                catch { /* ignore */ }
            }

            // --- Ajin(Z/T) 폴링 추가 ---
            if (_ajin != null && _ajin.IsConnected)
            {
                try
                {
                    // Z
                    double z = _ajin.GetActPos(AjinAxis.MaintZ1);
                    bool zBusy = _ajin.IsInMotion(AjinAxis.MaintZ1);
                    lbMZPos.Text = $"{z:F3} mm";
                    Chip(chipMZMove, zBusy);
                    Chip(chipMZServo, _ajin.IsServoOn(AjinAxis.MaintZ1));
                    // in-pos 칩은 별도 신호가 없으므로 '정지 상태' 근사
                    Chip(chipMZInpos, !zBusy);

                    // T
                    double t = _ajin.GetActPos(AjinAxis.T);
                    bool tBusy = _ajin.IsInMotion(AjinAxis.T);
                    lbTPos.Text = $"{t:F5} mm";
                    Chip(chipTMove, tBusy);
                    Chip(chipTServo, _ajin.IsServoOn(AjinAxis.T));
                    Chip(chipTInpos, !tBusy);

                    Chip(chipMZHomeStatus, _ajin.IsServoOn(AjinAxis.MaintZ1) && _ajin.IsHomed(AjinAxis.MaintZ1));
                    Chip(chipTHomeStatus, _ajin.IsServoOn(AjinAxis.T) && _ajin.IsHomed(AjinAxis.T));

                    // --- Ajin(Z/T) 폴링 추가 내부의 조그 Enable 갱신 부분만 교체 ---
                    var zSense = MakeLimitSnapshotProvider(AjinAxis.MaintZ1);
                    var zJog = StageWin.Safety.SafetyPolicy.GetJogInterlock(zSense);
                    if (btnMZJogPlus != null) btnMZJogPlus.Enabled = zJog.AllowPlus;
                    if (btnMZJogMinus != null) btnMZJogMinus.Enabled = zJog.AllowMinus;

                    var tSense = MakeLimitSnapshotProvider(AjinAxis.T);
                    var tJog = StageWin.Safety.SafetyPolicy.GetJogInterlock(tSense);
                    if (btnTJogPlus != null) btnTJogPlus.Enabled = tJog.AllowPlus;
                    if (btnTJogMinus != null) btnTJogMinus.Enabled = tJog.AllowMinus;

                    bool zAlarm = _ajin.IsAlarm(AjinAxis.MaintZ1);
                    bool tAlarm = _ajin.IsAlarm(AjinAxis.T);
                    _alarmBlinkTick++;
                    UpdateAlarmResetButton(btnMZAlarmReset, zAlarm);
                    UpdateAlarmResetButton(btnTAlarmReset, tAlarm);
                }
                catch { /* ignore */ }
            }

            try
            {
                var ctx = TryGetSafetyCtx();
                if (ctx != null && !_safetyStopping)
                {
                    if (SafetyPolicy.ShouldForceAxesStop(ctx, out var why))
                    {
                        _safetyStopping = true;
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                // 진행 중 루프/조그 취소
                                _p2pXcts?.Cancel(); _p2pYcts?.Cancel();
                                _p2pZcts?.Cancel(); _p2pTcts?.Cancel();

                                // X/Y 정지 (Servo는 끄지 않음)
                                if (_motion != null)
                                {
                                    try { await _motion.JogStopAsync(Axis.X); } catch { }
                                    try { await _motion.JogStopAsync(Axis.Y); } catch { }
                                    try { await _motion.StopAsync(Axis.X); } catch { }
                                    try { await _motion.StopAsync(Axis.Y); } catch { }
                                }
                                // Ajin 정지 (Servo 유지)
                                if (_ajin != null && _ajin.IsConnected)
                                {
                                    try { _ajin.Stop(AjinAxis.MaintZ1); } catch { }
                                    try { _ajin.Stop(AjinAxis.T); } catch { }
                                }

                                BeginInvoke(new Action(() =>
                                {
                                    // 필요 시 상태표시
                                    // MessageBox.Show(this, why, "Safety", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }));
                            }
                            finally { _safetyStopping = false; }
                        });
                    }
                }
            }
            catch { /* ignore */ }
        }


        private static void Chip(Label chip, bool on)
        {
            chip.BackColor = on ? Color.FromArgb(200, 235, 255) : Color.Gainsboro;
            chip.ForeColor = on ? Color.Black : Color.DimGray;
            chip.BorderStyle = BorderStyle.FixedSingle;
        }

        private static decimal Clamp(double v, NumericUpDown n)
        {
            var d = (decimal)v;
            if (d < n.Minimum) d = n.Minimum;
            if (d > n.Maximum) d = n.Maximum;
            return Math.Round(d, n.DecimalPlaces);
        }

        private void ZHomeParamButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new ZHomeParamForm(_zHomeParams))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _zHomeParams = dlg.Result ?? _zHomeParams;
                }
            }
        }

        private void SetTextSafe(Control c, string text)
        {
            if (c == null) return;
            if (c.IsDisposed || c.Disposing) return;
            if (!c.IsHandleCreated) return;

            try
            {
                if (c.InvokeRequired) c.BeginInvoke(new Action<Control, string>(SetTextSafe), c, text);
                else c.Text = text;
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }
        private void UiEnableSafe(Control c, bool enabled)
        {
            if (c == null) return;
            if (c.IsDisposed || c.Disposing || !c.IsHandleCreated) return;
            try
            {
                if (c.InvokeRequired) c.BeginInvoke(new Action<Control, bool>(UiEnableSafe), c, enabled);
                else c.Enabled = enabled;
            }
            catch { }
        }
        private void ShowWarn(string msg) =>
            BeginInvoke(new Action(() =>
                MessageBox.Show(this, msg, "Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning)));

        // 모션이 붙기 전엔 조작 버튼/입력 비활성화
        private void SetMotionControlsEnabled(bool enabled)
        {
            if (btnXMove != null) btnXMove.Enabled = enabled;
            if (btnXStop != null) btnXStop.Enabled = enabled;
            if (btnXHome != null) btnXHome.Enabled = enabled;
            if (btnXServo != null) btnXServo.Enabled = enabled;
            if (btnXJogMinus != null) btnXJogMinus.Enabled = enabled;
            if (btnXJogPlus != null) btnXJogPlus.Enabled = enabled;

            if (btnYMove != null) btnYMove.Enabled = enabled;
            if (btnYStop != null) btnYStop.Enabled = enabled;
            if (btnYHome != null) btnYHome.Enabled = enabled;
            if (btnYServo != null) btnYServo.Enabled = enabled;
            if (btnYJogMinus != null) btnYJogMinus.Enabled = enabled;
            if (btnYJogPlus != null) btnYJogPlus.Enabled = enabled;

            if (numXTarget != null) numXTarget.Enabled = enabled;
            if (numYTarget != null) numYTarget.Enabled = enabled;
            if (numXSpeed != null) numXSpeed.Enabled = enabled;
            if (numYSpeed != null) numYSpeed.Enabled = enabled;
            if (numXAcc != null) numXAcc.Enabled = enabled;
            if (numYAcc != null) numYAcc.Enabled = enabled;
            if (numXDec != null) numXDec.Enabled = enabled;
            if (numYDec != null) numYDec.Enabled = enabled;
            if (numXJog != null) numXJog.Enabled = enabled;
            if (numYJog != null) numYJog.Enabled = enabled;
            if (rXAbs != null) rXAbs.Enabled = enabled;
            if (rXRel != null) rXRel.Enabled = enabled;
            if (rYAbs != null) rYAbs.Enabled = enabled;
            if (rYRel != null) rYRel.Enabled = enabled;

            if (btnXP2PStart != null) btnXP2PStart.Enabled = enabled;
            if (btnXP2PStop != null) btnXP2PStop.Enabled = false;
            if (btnYP2PStart != null) btnYP2PStart.Enabled = enabled;
            if (btnYP2PStop != null) btnYP2PStop.Enabled = false;

            if (numXP2PA != null) numXP2PA.Enabled = enabled;
            if (numXP2PB != null) numXP2PB.Enabled = enabled;
            if (numXP2PVel != null) numXP2PVel.Enabled = enabled;
            if (numXP2PAcc != null) numXP2PAcc.Enabled = enabled;
            if (numXP2PDec != null) numXP2PDec.Enabled = enabled;
            if (numXP2PDwell != null) numXP2PDwell.Enabled = enabled;
            if (numXP2PCount != null) numXP2PCount.Enabled = enabled;

            if (numYP2PA != null) numYP2PA.Enabled = enabled;
            if (numYP2PB != null) numYP2PB.Enabled = enabled;
            if (numYP2PVel != null) numYP2PVel.Enabled = enabled;
            if (numYP2PAcc != null) numYP2PAcc.Enabled = enabled;
            if (numYP2PDec != null) numYP2PDec.Enabled = enabled;
            if (numYP2PDwell != null) numYP2PDwell.Enabled = enabled;
            if (numYP2PCount != null) numYP2PCount.Enabled = enabled;
        }
    }
}
