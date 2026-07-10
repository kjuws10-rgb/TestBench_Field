using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using StageWin.Etc;
using StageWin.WagoIO;
using StageWin.Driver.Motion;
using StageWin.Safety;

namespace StageWin.UI
{
    public partial class OverviewForm : Form
    {
        #region ======= 통신상태, 옵션 =======
        public sealed class MessageBoxAlarmSink : IAlarmSink
        {
            private readonly IWin32Window _owner;
            public MessageBoxAlarmSink(IWin32Window owner) { _owner = owner; }
            public void Notify(string title, string message)
                => MessageBox.Show(_owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public sealed class Options
        {
            // 장비 Online/Simulation 상태 (널이면 false로 처리)
            public Func<bool> AcsOnline { get; set; } = () => false;
            public Func<bool> AcsSim { get; set; } = () => false;
            public Func<bool> AjinOnline { get; set; } = () => false;
            public Func<bool> AjinSim { get; set; } = () => false;
            public Func<bool> LdsOnline { get; set; } = () => false;
            public Func<bool> LdsSim { get; set; } = () => false;
            public Func<bool> WagoOnline { get; set; } = () => false;
            public Func<bool> WagoSim { get; set; } = () => false;

            // 제어/데이터 소스
            public IMotionController AcsMotion { get; set; }
            public IOForm IoForm { get; set; }
            public AjinMotionAdapter AjinMotion { get; set; }
            public Func<bool> LaserOn { get; set; } = () => false;
            public Func<Axis, double> Setpoint { get; set; } = _ => 0.0;
            public Func<ProgramMode> ModeProvider { get; set; } = () => ProgramMode.Manual;
        }
        private sealed class OutputBinding
        {
            public string Name;
            public Button OnBtn;
            public Button OffBtn;
            public Label StateLed;
        }
        #endregion

        #region ======= 필드, IO네임 =======
        private readonly IAlarmSink _uiAlarm; // MessageBox용 Sink

        // Motion 샘플링 상태
        private DateTime _lastSample = DateTime.MinValue;
        private double _prevX, _prevY, _prevZ, _prevT;

        // RMS 계산용 고정 길이 버퍼
        private const int RMS_WINDOW = 30;
        private readonly double[] _vx = new double[RMS_WINDOW];
        private readonly double[] _vy = new double[RMS_WINDOW];
        private readonly double[] _vz = new double[RMS_WINDOW];
        private readonly double[] _vt = new double[RMS_WINDOW];
        private int _vIndex = 0;         // 순환 인덱스
        private int _vFilled = 0;        // 채워진 샘플 개수(초기 과도 구간 보정)

        // Setpoint 제공자 보관 (CmdVel 표시에 사용)
        private Func<Axis, double> _setpoint;

        // 축별 Servo/InPos/Alarm IO 이름(필요시 여기만 수정)
        private const string IO_X_HOMED = "Review X Homed";
        private const string IO_X_SERVO = "Review X Servo";
        private const string IO_X_INPOS = "Review X InPos";
        private const string IO_X_ALARM = "Review X Alarm";

        private const string IO_Y_HOMED = "Main Y Homed";
        private const string IO_Y_SERVO = "Main Y Servo";
        private const string IO_Y_INPOS = "Main Y InPos";
        private const string IO_Y_ALARM = "Main Y Alarm";

        private const string IO_Z_HOMED = "Maint Z Homed";
        private const string IO_Z_SERVO = "Maint Z Servo";
        private const string IO_Z_INPOS = "Maint Z InPos";
        private const string IO_Z_ALARM = "Maint Z Alarm";

        private const string IO_T_HOMED = "Theta Homed";
        private const string IO_T_SERVO = "Theta Servo";
        private const string IO_T_INPOS = "Theta InPos";
        private const string IO_T_ALARM = "Theta Alarm";

        private const string IO_LASER_STATUS = "Laser ON / OFF Status";      // X102D
        private const string OUT_LASER_LED = "Laser On LED Lamp";            // Y1023

        private Func<bool> _acsOnline, _acsSim, _ajinOnline, _ajinSim, _ldsOnline, _ldsSim, _wagoOnline, _wagoSim;
        private IMotionController _acsMotion;
        private IOForm _io;
        private AjinMotionAdapter _ajinMotion;

        // Safety
        private ISafetyContext _ctx;
        private readonly SafetyRouter _router = new SafetyRouter();
        private double _curActX, _curActY, _curActZ, _curActT;

        // 주기 갱신
        private readonly Timer _tmr = new Timer { Interval = 200 };
        private readonly DateTime _startupAt = DateTime.UtcNow;
        private static readonly TimeSpan IO_WARMUP = TimeSpan.FromSeconds(2.0);

        // Grip UI Lock
        private Panel _infoBanner;
        private Label _infoBannerText;
        private volatile bool _uiLocked = false;
        private bool IsUiLocked() => _uiLocked;
        public bool UiLocked => _uiLocked;
        private DateTime _uiLockLastChanged = DateTime.MinValue;
        public event Action<bool> UiLockChanged;     // true=잠금, false=해제
        private DigitalStatus _prevModeKey = DigitalStatus.Unknown;
        private bool _latchedTempY = false;
        private bool _latchedTempX = false;
        private bool IsWagoSim => SafetyPolicy.WagoSimProvider?.Invoke() == true;
        private readonly List<OutputBinding> _bindings = new List<OutputBinding>();
        #endregion

        #region ======= 초기화, UI관련 함수 =======
        public OverviewForm(Options opt)
        {
            InitializeComponent();

            _uiAlarm = new MessageBoxAlarmSink(this);
            ApplyOptions(opt);

            // Ajin 부하율 설정: Z(0), Theta(1)축 처음에 셋팅 진행. Reference Torque load ratio(2번)
            try
            {
                if (_ajinOnline?.Invoke() == true)
                {
                    CAXM.AxmStatusSetReadServoLoadRatio(0, 2); // Z
                    CAXM.AxmStatusSetReadServoLoadRatio(1, 2); // Theta
                }
            }
            catch { }

            // 버튼 핸들러 연결
            WireButtons();

            // 출력 바인딩(버튼 상태/색 갱신용)
            RegisterBindings();

            // 라벨 초기화
            this.InitLedLabel(ledReviewX_Servo);
            this.InitLedLabel(ledReviewX_InPos);
            this.InitLedLabel(ledReviewX_Alarm);

            this.InitLedLabel(ledMainY_Servo);
            this.InitLedLabel(ledMainY_InPos);
            this.InitLedLabel(ledMainY_Alarm);

            this.InitLedLabel(ledMaintZ_Servo);
            this.InitLedLabel(ledMaintZ_InPos);
            this.InitLedLabel(ledMaintZ_AmpAlarm);

            this.InitLedLabel(ledTheta_Servo);
            this.InitLedLabel(ledTheta_InPos);
            this.InitLedLabel(ledTheta_AmpAlarm);

            this.InitLedLabel(ledPmFan_Front1);
            this.InitLedLabel(ledPmFan_Front2);
            this.InitLedLabel(ledPmFan_Side1);
            this.InitLedLabel(ledPmFan_Side2);
            this.InitLedLabel(ledPmFan_Rear1);
            this.InitLedLabel(ledPmFan_Rear2);

            this.InitLedLabel(ledUiFan_Front1);
            this.InitLedLabel(ledUiFan_Front2);
            this.InitLedLabel(ledUiFan_Side1);
            this.InitLedLabel(ledUiFan_Side2);
            this.InitLedLabel(ledUiFan_Rear1);
            this.InitLedLabel(ledUiFan_Rear2);

            this.InitLedLabel(ledGripSwitchEms);
            this.InitLedLabel(ledPmRackEms);
            this.InitLedLabel(ledChamberEms1);
            this.InitLedLabel(ledChamberEms2);
            this.InitLedLabel(ledChamberEms3);

            this.InitLedLabel(ledModeKey);

            this.InitLedLabel(ledDoorFront1);
            this.InitLedLabel(ledDoorFront2);
            this.InitLedLabel(ledDoorRear1);
            this.InitLedLabel(ledDoorRear2);
            this.InitLedLabel(ledDoorSide1);
            this.InitLedLabel(ledDoorSide2);

            this.InitLedLabel(ledLaserStatus);
            this.InitLedLabel(ledAcs1Main);
            this.InitLedLabel(ledAcs2Y);
            this.InitLedLabel(ledAcs2X);
            this.InitLedLabel(ledRot1Main);
            this.InitLedLabel(ledRot2Theta);
            this.InitLedLabel(ledRot2MaintZ12);

            this.InitLedLabel(ledFrontDoor1_LockState);
            this.InitLedLabel(ledFrontDoor2_LockState);
            this.InitLedLabel(ledRearDoor1_LockState);
            this.InitLedLabel(ledRearDoor2_LockState);
            this.InitLedLabel(ledSideDoor1_LockState);
            this.InitLedLabel(ledSideDoor2_LockState);
            this.InitLedLabel(ledCleanBoothLed);

            this.InitLedLabel(ledItkEmsAllOk);
            this.InitLedLabel(ledItkAjinMcAllOk);
            this.InitLedLabel(ledItkAcsMcAllOk);
            this.InitLedLabel(ledItkStageMoveOk);
            this.InitLedLabel(ledItkLaserReady);
            this.InitLedLabel(ledItkModeKey);

            EnsureInfoBanner();
            _tmr.Tick += (_, __) => RefreshOverview();
            _tmr.Start();
            try { tmrRefresh?.Start(); } catch { }
        }
        private void InitLedLabel(Label l)
        {
            if (l == null) return;
            l.AutoSize = false;
            if (l.Size.Width == 0 || l.Size.Height == 0) l.Size = new Size(13, 13);
            l.BackColor = Color.DarkGray;
            l.BorderStyle = BorderStyle.FixedSingle;
            l.Text = string.Empty;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Margin = Padding.Empty;
            l.Padding = Padding.Empty;
        }
        private void ApplyOptions(Options o)
        {
            _acsOnline = o.AcsOnline; _acsSim = o.AcsSim;
            _ajinOnline = o.AjinOnline; _ajinSim = o.AjinSim;
            _ldsOnline = o.LdsOnline; _ldsSim = o.LdsSim;
            _wagoOnline = o.WagoOnline; _wagoSim = o.WagoSim;
            SafetyPolicy.WagoSimProvider = _wagoSim ?? (() => false);
            _acsMotion = o.AcsMotion;
            _ajinMotion = o.AjinMotion;
            _io = o.IoForm;
            _ctx = new OverviewSafetyContext(
                _acsMotion,
                name => VirtualBus.DigitalInputs.TryGet(name, out var v) && v.RawBit,
                name => VirtualBus.DigitalOutputs.TryGet(name, out var v) && v.RawBit,
                o.LaserOn,
                o.ModeProvider,
                () => _curActX,     // X Actual Velocity provider
                () => _curActY,     // Y Actual Velocity provider
                () => _curActZ,     // MaintZ Actual Velocity provider
                () => _curActT      // Theta Actual Velocity provider
            );
            _setpoint = o.Setpoint;
            if (_io != null) _io.BindProgramModeProvider(o.ModeProvider ?? (() => ProgramMode.Manual));
        }
        private void EnsureInfoBanner()
        {
            if (_infoBanner != null) return;

            _infoBanner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 34,
                BackColor = Color.Moccasin,
                Padding = new Padding(8, 7, 8, 7),
                Visible = false
            };
            _infoBannerText = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(this.Font, FontStyle.Bold),
                AutoEllipsis = true,
                Text = "TEACH 모드: Grip Switch를 눌러야 조작이 활성화됩니다."
            };
            _infoBanner.Controls.Add(_infoBannerText);
            this.Controls.Add(_infoBanner);
            _infoBanner.BringToFront();
        }
        private void PulseBanner()
        {
            EnsureInfoBanner();
            _infoBanner.Visible = true;
            var old = _infoBanner.BackColor;
            _infoBanner.BackColor = Color.LightSalmon;
            var t = new Timer { Interval = 300 };
            t.Tick += (s, e) =>
            {
                _infoBanner.BackColor = old;
                t.Stop(); t.Dispose();
            };
            t.Start();
        }
        public void AddOverviewLog(string message)
        {
            if (lstOverview == null) return;
            if (lstOverview.InvokeRequired)
            {
                lstOverview.BeginInvoke(new Action(() => AddOverviewLog(message)));
                return;
            }
            string line = $"{DateTime.Now:HH:mm:ss}  {message}";
            lstOverview.Items.Add(line);
            if (lstOverview.Items.Count > 500) lstOverview.Items.RemoveAt(0);
            lstOverview.TopIndex = lstOverview.Items.Count - 1;
        }
        private static bool IsFreshDI(string name, TimeSpan? maxAge = null)
        {
            if (maxAge == null) maxAge = TimeSpan.FromSeconds(3);
            if (StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet(name, out var v))
            {
                if (v.UpdatedAt == default) return false;
                return (DateTime.UtcNow - v.UpdatedAt) <= maxAge.Value;
            }
            return false;
        }
        private bool IoReady(params string[] names)
        {
            if ((DateTime.UtcNow - _startupAt) < IO_WARMUP) return false;
            foreach (var n in names) if (!IsFreshDI(n)) return false;
            return true;
        }
        #endregion

        #region ======= Refresh 루프, IO & Safety Auto 로직 =======
        private void RefreshOverview()
        {
            try
            {
                // ===== 공통 시간 샘플링 =====
                var now = DateTime.UtcNow;
                double dt = (_lastSample == DateTime.MinValue) ? 0.0 : (now - _lastSample).TotalSeconds;
                if (dt < 1e-6) dt = 0.0; // 첫 샘플 보호

                // ===================== Review X =====================
                double posX = _acsMotion?.GetAPosition(Axis.X) ?? 0.0;
                lblReviewX_Pos.Text = posX.ToString("F3");

                double actX = (dt > 0.0) ? (posX - _prevX) / dt : 0.0;
                _vx[_vIndex] = actX;
                lblReviewX_ActVel.Text = actX.ToString("F3");
                _curActX = actX;

                double cmdX = _setpoint?.Invoke(Axis.X) ?? 0.0;
                lblReviewX_CmdVel.Text = cmdX.ToString("F3");

                // RMS(X) – ACS 컨트롤러에서 직접 읽기
                var acsStatus = _acsMotion as StageWin.Driver.Motion.IAcsStatus;
                double rmsX = acsStatus?.GetRms(Axis.X) ?? 0.0;
                lblReviewX_Rms.Text = rmsX.ToString("F3");

                // LED(X)
                SetAcsAxisLeds(ledReviewX_Homed, ledReviewX_Servo, ledReviewX_InPos, ledReviewX_Alarm, Axis.X,
                               IO_X_HOMED, IO_X_SERVO, IO_X_INPOS, IO_X_ALARM);

                // ===================== Main Y ======================
                double posY = _acsMotion?.GetAPosition(Axis.Y) ?? 0.0;
                lblMainY_Pos.Text = posY.ToString("F3");

                double actY = (dt > 0.0) ? (posY - _prevY) / dt : 0.0;
                _vy[_vIndex] = actY;
                lblMainY_ActVel.Text = actY.ToString("F3");
                _curActY = actY;

                double cmdY = _setpoint?.Invoke(Axis.Y) ?? 0.0;
                lblMainY_CmdVel.Text = cmdY.ToString("F3");

                // RMS(Y)
                double rmsY = acsStatus?.GetRms(Axis.Y) ?? 0.0;
                lblMainY_Rms.Text = rmsY.ToString("F3");

                SetAcsAxisLeds(ledMainY_Homed, ledMainY_Servo, ledMainY_InPos, ledMainY_Alarm, Axis.Y,
                               IO_Y_HOMED, IO_Y_SERVO, IO_Y_INPOS, IO_Y_ALARM);

                // ===== Maint Z (Ajin) =====
                double posZ = _ajinMotion?.GetActPos(AjinAxis.MaintZ1) ?? 0.0;
                lblMaintZ_Pos.Text = posZ.ToString("F3");

                double actZ = (dt > 0.0) ? (posZ - _prevZ) / dt : 0.0;
                _vz[_vIndex] = actZ;
                lblMaintZ_ActVel.Text = actZ.ToString("F3");
                _curActZ = actZ;

                double cmdZ = _ajinMotion?.GetCmdVel(AjinAxis.MaintZ1) ?? 0.0;
                lblMaintZ_CmdVel.Text = cmdZ.ToString("F3");

                double zLoad = 0.000;
                CAXM.AxmStatusReadServoLoadRatio(0, ref zLoad);
                lblMaintZ_Rms.Text = Math.Abs(zLoad).ToString("F3");

                SetAjinAxisLeds(ledMaintZ_Homed, ledMaintZ_Servo, ledMaintZ_InPos, ledMaintZ_AmpAlarm,
                                AjinAxis.MaintZ1, IO_Z_HOMED, IO_Z_SERVO, IO_Z_INPOS, IO_Z_ALARM);

                // ===== Theta (Ajin) =====
                double posT = _ajinMotion?.GetActPos(AjinAxis.T) ?? 0.0;
                lblTheta_Pos.Text = posT.ToString("F5");

                double actT = (dt > 0.0) ? (posT - _prevT) / dt : 0.0;
                _vt[_vIndex] = actT;
                lblTheta_ActVel.Text = actT.ToString("F3");
                _curActT = actT;

                double cmdT = _ajinMotion?.GetCmdVel(AjinAxis.T) ?? 0.0;
                lblTheta_CmdVel.Text = cmdT.ToString("F3");

                double tLoad = 0.000;
                CAXM.AxmStatusReadServoLoadRatio(1, ref tLoad);
                lblTheta_Rms.Text = Math.Abs(tLoad).ToString("F3");

                SetAjinAxisLeds(ledTheta_Homed, ledTheta_Servo, ledTheta_InPos, ledTheta_AmpAlarm,
                                AjinAxis.T, IO_T_HOMED, IO_T_SERVO, IO_T_INPOS, IO_T_ALARM);

                // ===== 여기부터는 기존 입력/출력 LED 갱신 그대로 유지 =====
                SetLed(ledPmFan_Front1, In("PM Rack Fan Status - Front 1"));
                SetLed(ledPmFan_Front2, In("PM Rack Fan Status - Front 2"));
                SetLed(ledPmFan_Side1, In("PM Rack Fan Status - Side 1"));
                SetLed(ledPmFan_Side2, In("PM Rack Fan Status - Side 2"));
                SetLed(ledPmFan_Rear1, In("PM Rack Fan Status - Rear 1"));
                SetLed(ledPmFan_Rear2, In("PM Rack Fan Status - Rear 2"));

                SetLed(ledUiFan_Front1, In("UI Rack Fan Status - Front 1"));
                SetLed(ledUiFan_Front2, In("UI Rack Fan Status - Front 2"));
                SetLed(ledUiFan_Rear1, In("UI Rack Fan Status - Rear 1"));
                SetLed(ledUiFan_Rear2, In("UI Rack Fan Status - Rear 2"));
                SetLed(ledUiFan_Side1, In("UI Rack Fan Status - Side 1"));
                SetLed(ledUiFan_Side2, In("UI Rack Fan Status - Side 2"));

                SetLed(ledGripSwitchEms, In("Grip Switch EMS"));
                SetLed(ledPmRackEms, In("PM Rack EMS Switch"));
                SetLed(ledChamberEms1, In("Chamber EMS Switch Status 1"));
                SetLed(ledChamberEms2, In("Chamber EMS Switch Status 2"));
                SetLed(ledChamberEms3, In("Chamber EMS Switch Status 3"));
                SetLed(ledModeKey, In("Mode Key Status"));

                SetLed(ledDoorFront1, In("Clean Booth Front Door 1 Status"));
                SetLed(ledDoorFront2, In("Clean Booth Front Door 2 Status"));
                SetLed(ledDoorRear1, In("Clean Booth Rear Door 1 Status"));
                SetLed(ledDoorRear2, In("Clean Booth Rear Door 2 Status"));
                SetLed(ledDoorSide1, In("Clean Booth Side Door 1 Status"));
                SetLed(ledDoorSide2, In("Clean Booth Side Door 2 Status"));

                SetLed(ledLaserStatus, In("Laser ON / OFF Status"));

                SetLed(ledAcs1Main, In("ACS 1'st MC Status - Main"));
                SetLed(ledAcs2Y, In("ACS 2'nd MC Status - Y AXIS"));
                SetLed(ledAcs2X, In("ACS 2'nd MC Status - X AXIS"));
                SetLed(ledRot1Main, In("Rotary Motor 1'st MC Status - Main"));
                SetLed(ledRot2Theta, In("Rotary Motor 2'nd MC Status - Theta Motor"));
                SetLed(ledRot2MaintZ12, In("Rotary Motor 2'nd MC Status - Maint Z 1/2"));

                // Laser Lamp 점멸(입력+출력 미러링)
                SetLed(ledLaserStatus, In("Laser ON / OFF Status"));

                // 출력 상태 LED
                SetLed(ledFrontDoor1_LockState, !Out("Clean Booth Front Door 1 Lock / Unlock"));
                SetLed(ledFrontDoor2_LockState, !Out("Clean Booth Front Door 2 Lock / Unlock"));
                SetLed(ledRearDoor1_LockState, !Out("Clean Booth Rear Door 1 Lock / Unlock"));
                SetLed(ledRearDoor2_LockState, !Out("Clean Booth Rear Door 2 Lock / Unlock"));
                SetLed(ledSideDoor1_LockState, !Out("Clean Booth Door Side 1 Lock / Unlock"));
                SetLed(ledSideDoor2_LockState, !Out("Clean Booth Door Side 2 Lock / Unlock"));
                SetLed(ledCleanBoothLed, !Out("Clean Booth LED Lamp"));

                SetLed(ledACSMc1_OutputState, Out(SafetyPolicy.OUT_ACS_MC1));
                SetLed(ledACSMc2_OutputYState, Out(SafetyPolicy.OUT_ACS_MC2_Y));
                SetLed(ledACSMc2_OutputXState, Out(SafetyPolicy.OUT_ACS_MC2_X));
                SetLed(ledAjinMc2_OutputZState, Out(SafetyPolicy.OUT_AJIN_MC2_Z));
                SetLed(ledAjinMc2_OutputTState, Out(SafetyPolicy.OUT_AJIN_MC2_T));

                SetLed(ledY1Motor_AirCoolingVvState, Out(SafetyPolicy.OUT_COOLING_Y1));
                SetLed(ledY2Motor_AirCoolingVvState, Out(SafetyPolicy.OUT_COOLING_Y2));
                SetLed(ledXMotor_AirCoolingVvState, Out(SafetyPolicy.OUT_COOLING_X));
                SetLed(ledLaserBox_AirCoolingVvState, Out(SafetyPolicy.OUT_COOLING_LASER));

                UpdateOutputBindingsVisual();

                // ===================== ITK 종합 상태 LED =====================
                if (IsWagoSim) return;

                bool emsAny =
                    In(SafetyPolicy.IN_EMS_GRIP_SWITCH) ||
                    In(SafetyPolicy.IN_EMS_PM_RACK) ||
                    In(SafetyPolicy.IN_EMS_CHAMBER_SWITCH1) ||
                    In(SafetyPolicy.IN_EMS_CHAMBER_SWITCH2) ||
                    In(SafetyPolicy.IN_EMS_CHAMBER_SWITCH3);
                SetLed(ledItkEmsAllOk, !emsAny);

                bool ajinMcAllOk =
                    In("Rotary Motor 1'st MC Status - Main") &&
                    In("Rotary Motor 2'nd MC Status - Theta Motor") &&
                    In("Rotary Motor 2'nd MC Status - Maint Z 1/2");
                SetLed(ledItkAjinMcAllOk, ajinMcAllOk);

                bool acsMcAllOk = Out(SafetyPolicy.OUT_ACS_MC_ALLOK) || Out("ACS MC ALL ON OK");
                SetLed(ledItkAcsMcAllOk, acsMcAllOk);

                bool stageMoveOk = Out(SafetyPolicy.OUT_STAGE_MOVING_OK) || Out("Stage Moving OK Interlock");
                SetLed(ledItkStageMoveOk, stageMoveOk);

                SetLed(ledItkLaserReady, In("Laser Ass'y Status (All Ready)"));
                SetLed(ledItkModeKey, In("Mode Key Status"));

                bool mc1 = In(SafetyPolicy.IN_ACS_MC1_MAIN);
                bool mc2y = In(SafetyPolicy.IN_ACS_MC2_Y);
                bool mc2x = In(SafetyPolicy.IN_ACS_MC2_X);
                bool rotMain = In(SafetyPolicy.IN_AJIN_MC1_MAIN);

                bool laserOn = In(IO_LASER_STATUS);
                bool lampNow = Out(OUT_LASER_LED);

                // (1) ACS 1차측 OFF → 2차측 자동 OFF
                if (IoReady(SafetyPolicy.IN_ACS_MC1_MAIN))
                {
                    if (!mc1 && _io != null)
                    {
                        if (Out(SafetyPolicy.OUT_ACS_MC2_Y))
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_ACS_MC2_Y, false, source: "Auto");
                        if (Out(SafetyPolicy.OUT_ACS_MC2_X))
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_ACS_MC2_X, false, source: "Auto");
                    }
                }

                // (2) Ajin 1차측 OFF → 2차측 자동 OFF
                if (IoReady(SafetyPolicy.IN_AJIN_MC1_MAIN))
                {
                    if (!rotMain && _io != null)
                    {
                        if (Out(SafetyPolicy.OUT_AJIN_MC2_Z))
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_AJIN_MC2_Z, false, source: "Auto");
                        if (Out(SafetyPolicy.OUT_AJIN_MC2_T))
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_AJIN_MC2_T, false, source: "Auto");
                    }
                }

                // (3.1) Y100F 자동 동기화(ALL OK)
                if (IoReady(SafetyPolicy.IN_ACS_MC1_MAIN, SafetyPolicy.IN_ACS_MC2_Y, SafetyPolicy.IN_ACS_MC2_X) && _io != null)
                {
                    bool allOkCond = mc1 && mc2y && mc2x;
                    bool curAllOk = Out(SafetyPolicy.OUT_ACS_MC_ALLOK);

                    if (allOkCond != curAllOk)
                    {
                        _io.TryWriteOutputByName(SafetyPolicy.OUT_ACS_MC_ALLOK, allOkCond, source: "Auto");
                        curAllOk = allOkCond;
                    }

                    if (!curAllOk && _acsMotion != null)
                    {
                        var acsStat = _acsMotion as StageWin.Driver.Motion.IAcsStatus;
                        if (acsStat == null || acsStat.IsServoOn(StageWin.Driver.Motion.Axis.Y))
                            _ = _acsMotion.ServoOffAsync(StageWin.Driver.Motion.Axis.Y);
                        if (acsStat == null || acsStat.IsServoOn(StageWin.Driver.Motion.Axis.X))
                            _ = _acsMotion.ServoOffAsync(StageWin.Driver.Motion.Axis.X);
                    }
                }

                // (3.2) Y100E 자동 동기화(Stage Moving OK)
                if (_io != null && IoReady(
                        SafetyPolicy.IN_ACS_MC1_MAIN,
                        SafetyPolicy.IN_ACS_MC2_Y,
                        SafetyPolicy.IN_ACS_MC2_X,
                        SafetyPolicy.IN_MOTOR_TEMP_Y1,
                        SafetyPolicy.IN_MOTOR_TEMP_Y2,
                        SafetyPolicy.IN_MOTOR_TEMP_X))
                {
                    mc1 = In(SafetyPolicy.IN_ACS_MC1_MAIN);
                    mc2y = In(SafetyPolicy.IN_ACS_MC2_Y);
                    mc2x = In(SafetyPolicy.IN_ACS_MC2_X);

                    bool y1Ok = In(SafetyPolicy.IN_MOTOR_TEMP_Y1);
                    bool y2Ok = In(SafetyPolicy.IN_MOTOR_TEMP_Y2);
                    bool xOk = In(SafetyPolicy.IN_MOTOR_TEMP_X);

                    bool movingOkCond = mc1 && mc2y && mc2x && y1Ok && y2Ok && xOk;

                    if (((y1Ok || y2Ok) == false) && _acsMotion != null)
                    {
                        var acsStat = _acsMotion as StageWin.Driver.Motion.IAcsStatus;
                        if (acsStat == null || acsStat.IsServoOn(StageWin.Driver.Motion.Axis.Y))
                            _ = _acsMotion.ServoOffAsync(StageWin.Driver.Motion.Axis.Y);
                    }
                    if ((xOk == false) && _acsMotion != null)
                    {
                        var acsStat = _acsMotion as StageWin.Driver.Motion.IAcsStatus;
                        if (acsStat == null || acsStat.IsServoOn(StageWin.Driver.Motion.Axis.X))
                            _ = _acsMotion.ServoOffAsync(StageWin.Driver.Motion.Axis.X);
                    }

                    bool curMovingOk = Out(SafetyPolicy.OUT_STAGE_MOVING_OK);

                    if (movingOkCond != curMovingOk)
                        _io.TryWriteOutputByName(SafetyPolicy.OUT_STAGE_MOVING_OK, movingOkCond, source: "Auto");
                }

                // (4) Laser 상태 → LED 자동 미러링
                if (_io != null && IsFreshDI(IO_LASER_STATUS))
                {
                    if (laserOn != lampNow)
                        _io.TryWriteOutputByName(OUT_LASER_LED, laserOn, source: "Auto");
                }

                // (5) Mode Key → Auto 전환 시 Y100B 자동 ON
                if (_io != null)
                {
                    var mk = _ctx.GetInputStatus(SafetyPolicy.IN_MODE_KEY_NAME);
                    bool changedToAuto = (_prevModeKey != DigitalStatus.Auto) && (mk == DigitalStatus.Auto);

                    if (changedToAuto && IoReady(
                            SafetyPolicy.IN_DOOR_FRONT1, SafetyPolicy.IN_DOOR_FRONT2,
                            SafetyPolicy.IN_DOOR_REAR1, SafetyPolicy.IN_DOOR_REAR2,
                            SafetyPolicy.IN_DOOR_SIDE1, SafetyPolicy.IN_DOOR_SIDE2))
                    {
                        var intent = SafetyIntent.ForAutoKey("OverviewForm");
                        var r = SafetyPolicy.EvaluateEx(_ctx, intent);

                        if (r.Allowed)
                        {
                            bool cur = Out(SafetyPolicy.OUT_LASER_INTERLOCK_OK);
                            if (!cur)
                                _io.TryWriteOutputByName(SafetyPolicy.OUT_LASER_INTERLOCK_OK, true, source: "Auto");
                        }
                        else if (!r.SuppressPopup)
                        {
                            _uiAlarm?.Notify("Safety Interlock", r.Reason ?? "인터락 조건 불만족으로 동작 차단.");
                        }
                    }

                    _prevModeKey = mk;
                }

                // (6) 도어 열림/닫힘에 따른 Y100B 강제 ON/OFF
                if (_io != null && IoReady(
                        SafetyPolicy.IN_DOOR_FRONT1, SafetyPolicy.IN_DOOR_FRONT2,
                        SafetyPolicy.IN_DOOR_REAR1, SafetyPolicy.IN_DOOR_REAR2,
                        SafetyPolicy.IN_DOOR_SIDE1, SafetyPolicy.IN_DOOR_SIDE2))
                {
                    bool anyDoorOpen =
                        In(SafetyPolicy.IN_DOOR_FRONT1) ||
                        In(SafetyPolicy.IN_DOOR_FRONT2) ||
                        In(SafetyPolicy.IN_DOOR_REAR1) ||
                        In(SafetyPolicy.IN_DOOR_REAR2) ||
                        In(SafetyPolicy.IN_DOOR_SIDE1) ||
                        In(SafetyPolicy.IN_DOOR_SIDE2);

                    if (anyDoorOpen)
                    {
                        bool curOk = Out(SafetyPolicy.OUT_LASER_INTERLOCK_OK);
                        if (curOk)
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_LASER_INTERLOCK_OK, false, source: "Auto");
                    }
                    else
                    {
                        bool curOk = Out(SafetyPolicy.OUT_LASER_INTERLOCK_OK);
                        if (!curOk)
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_LASER_INTERLOCK_OK, true, source: "Auto");
                    }
                }

                // (6-EMS) EMS 동작 시 Interlock 강제 OFF
                if (_io != null && IoReady(
                        SafetyPolicy.IN_EMS_GRIP_SWITCH,
                        SafetyPolicy.IN_EMS_PM_RACK,
                        SafetyPolicy.IN_EMS_CHAMBER_SWITCH1,
                        SafetyPolicy.IN_EMS_CHAMBER_SWITCH2,
                        SafetyPolicy.IN_EMS_CHAMBER_SWITCH3))
                {
                    bool emsActive =
                        In(SafetyPolicy.IN_EMS_GRIP_SWITCH) ||
                        In(SafetyPolicy.IN_EMS_PM_RACK) ||
                        In(SafetyPolicy.IN_EMS_CHAMBER_SWITCH1) ||
                        In(SafetyPolicy.IN_EMS_CHAMBER_SWITCH2) ||
                        In(SafetyPolicy.IN_EMS_CHAMBER_SWITCH3);

                    if (emsActive)
                    {
                        if (Out(SafetyPolicy.OUT_STAGE_MOVING_OK))
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_STAGE_MOVING_OK, false, source: "Auto");

                        if (Out(SafetyPolicy.OUT_LASER_INTERLOCK_OK))
                            _io.TryWriteOutputByName(SafetyPolicy.OUT_LASER_INTERLOCK_OK, false, source: "Auto");
                    }
                }

                // (7) Linear Motor Temp Alarm 처리 (ON=정상, OFF=알람)
                if (_io != null && IoReady(
                        SafetyPolicy.IN_MOTOR_TEMP_Y1,
                        SafetyPolicy.IN_MOTOR_TEMP_Y2,
                        SafetyPolicy.IN_MOTOR_TEMP_X))
                {
                    bool y1Ok = In(SafetyPolicy.IN_MOTOR_TEMP_Y1);
                    bool y2Ok = In(SafetyPolicy.IN_MOTOR_TEMP_Y2);
                    bool xOk = In(SafetyPolicy.IN_MOTOR_TEMP_X);

                    bool tripY = !(y1Ok && y2Ok);
                    bool tripX = !xOk;

                    if (tripY && !_latchedTempY)
                    {
                        _latchedTempY = true;
                        TryStopAxis(StageWin.Driver.Motion.Axis.Y);
                        var rY = SafetyPolicy.EvaluateEx(_ctx, SafetyIntent.ForMotorTemp("OverviewForm"));
                        if (!rY.SuppressPopup)
                            _uiAlarm?.Notify("Safety Interlock", rY.Reason ?? "Linear Motor Temp Alarm(Y) 감지.");
                        TryServoOffAxis(StageWin.Driver.Motion.Axis.Y);
                        _io.TryWriteOutputByName(SafetyPolicy.OUT_STAGE_MOVING_OK, false, source: "Auto");
                    }

                    if (tripX && !_latchedTempX)
                    {
                        _latchedTempX = true;
                        TryStopAxis(StageWin.Driver.Motion.Axis.X);
                        var rX = SafetyPolicy.EvaluateEx(_ctx, SafetyIntent.ForMotorTemp("OverviewForm"));
                        if (!rX.SuppressPopup)
                            _uiAlarm?.Notify("Safety Interlock", rX.Reason ?? "Linear Motor Temp Alarm(X) 감지.");
                        TryServoOffAxis(StageWin.Driver.Motion.Axis.X);
                        _io.TryWriteOutputByName(SafetyPolicy.OUT_STAGE_MOVING_OK, false, source: "Auto");
                    }

                    if (!tripY) _latchedTempY = false;
                    if (!tripX) _latchedTempX = false;
                }

                // ===== 상태 저장 (다음 샘플 대비) =====
                _prevX = posX; _prevY = posY; _prevZ = posZ; _prevT = posT;
                _lastSample = now;

                _vIndex = (_vIndex + 1) % RMS_WINDOW;
                if (_vFilled < RMS_WINDOW) _vFilled++;

                UpdateUiLockByInputs();
            }
            catch { }
        }
        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            ApplyBadge(lblAcs,  "ACS Motion",    _acsOnline(),   _acsSim());
            ApplyBadge(lblAjin, "Ajin Motion",   _ajinOnline(),  _ajinSim());
            ApplyBadge(lblLds,  "LDS",           _ldsOnline(),   _ldsSim());
            ApplyBadge(lblWago, "WAGO IO",       _wagoOnline(),  _wagoSim());
        }
        private static void ApplyBadge(Label label, string title, bool online, bool sim)
        {
            if (sim)
            {
                label.Text = $"{title}: Simulation";
                label.BackColor = Color.PaleGreen;
                label.ForeColor = Color.Black;
                return;
            }
            if (online)
            {
                label.Text = $"{title}: Online";
                label.BackColor = Color.PaleGreen;
                label.ForeColor = Color.Black;
            }
            else
            {
                label.Text = $"{title}: Offline";
                label.BackColor = Color.Salmon;
                label.ForeColor = Color.White;
            }
        }
        #endregion

        #region ===== IO Read/Write, LED Helpers, UI Lock, Button Wiring
        private bool In(string name)
            => VirtualBus.DigitalInputs.TryGet(name, out var v) && v.RawBit;
        private bool Out(string name)
            => VirtualBus.DigitalOutputs.TryGet(name, out var v) && v.RawBit;
        private void SetLed(Label led, bool on)
        {
            if (led == null) return;
            led.BackColor = on ? Color.LimeGreen : Color.DarkGray;
            led.BorderStyle = BorderStyle.FixedSingle;
        }
        // ACS(X/Y) 축용
        private void SetAcsAxisLeds(Label homedLed, Label servoLed, Label inposLed, Label alarmLed,
                                   Axis axis, string ioHomed, string ioServo, string ioInPos, string ioAlarm)
        {
            bool home = In(ioHomed);
            bool servo = In(ioServo);
            bool inpos = In(ioInPos);
            bool alarm = In(ioAlarm);

            var st = _acsMotion as IAcsStatus;
            if (st != null)
            {
                try { home = st.IsHomeDone(axis) || home; } catch { }
                try { servo = st.IsServoOn(axis) || servo; } catch { }
                try { inpos = st.IsInPosition(axis) || inpos; } catch { }
            }

            SetLed(homedLed, home);
            SetLed(servoLed, servo);
            SetLed(inposLed, inpos);
            SetLed(alarmLed, alarm);
        }
        // Ajin 축(Z, T)용
        private void SetAjinAxisLeds(Label homedLed, Label servoLed, Label inposLed, Label alarmLed,
                                    AjinAxis axis, string ioHomed, string ioServo, string ioInPos, string ioAlarm)
        {
            bool servo = In(ioServo);
            bool inpos = In(ioInPos);
            bool home = false;
            bool alarm = false;

            var aj = _ajinMotion as StageWin.Etc.IAjinStatus;
            if (aj != null)
            {
                try { home = aj.IsHomed(axis); } catch { }
                try { servo = servo || aj.IsServoOn(axis); } catch { }
                try { alarm = aj.IsAlarm(axis); } catch { }
            }
            else
            {
                try { home = In(ioHomed); } catch { }
                try { alarm = In(ioAlarm); } catch { }
            }

            const double velEps = 0.001;
            if (axis == AjinAxis.MaintZ1)
                inpos = Math.Abs(_curActZ) < velEps;
            else if (axis == AjinAxis.T)
                inpos = Math.Abs(_curActT) < velEps;

            if (homedLed != null) SetLed(homedLed, home);
            if (servoLed != null) SetLed(servoLed, servo);
            if (inposLed != null) SetLed(inposLed, inpos);
            if (alarmLed != null) SetLed(alarmLed, alarm);
        }
        private void TryStopAxis(StageWin.Driver.Motion.Axis axis)
        {
            try
            {
                var t = _acsMotion?.GetType();
                if (t == null) return;
                var halt = t.GetMethod("HaltAsync", new Type[] { typeof(StageWin.Driver.Motion.Axis) });
                var stop = t.GetMethod("StopAsync", new Type[] { typeof(StageWin.Driver.Motion.Axis) });
                var dstop = t.GetMethod("DecelStopAsync", new Type[] { typeof(StageWin.Driver.Motion.Axis) });
                var m = halt ?? stop ?? dstop;
                if (m != null) _ = m.Invoke(_acsMotion, new object[] { axis });
            }
            catch { }
        }
        private void TryServoOffAxis(StageWin.Driver.Motion.Axis axis)
        {
            try
            {
                if (_acsMotion == null) return;
                var st = _acsMotion as StageWin.Driver.Motion.IAcsStatus;
                if (st == null || st.IsServoOn(axis))
                    _ = _acsMotion.ServoOffAsync(axis);
            }
            catch { }
        }
        private void UpdateUiLockByInputs()
        {
            var mk = _ctx.GetInputStatus(SafetyPolicy.IN_MODE_KEY_NAME);
            var ge = _ctx.GetInputStatus(SafetyPolicy.IN_GRIP_SWITCH_ENABLE);

            bool isTeach = (mk == DigitalStatus.Teach);
            bool gripEnabled = (ge == DigitalStatus.Enable);
            bool wantLock = isTeach && !gripEnabled;

            //wantLock = false;   // Test KJW
            //gripEnabled = true; // Test KJW

            ApplyUiLockState(wantLock, isTeach, gripEnabled);
        }
        private void ApplyUiLockState(bool lockNow, bool isTeach, bool gripEnabled)
        {
            bool changed = (_uiLocked != lockNow);
            _uiLocked = lockNow;
            _uiLockLastChanged = DateTime.UtcNow;
            UpdateInfoBanner(isTeach, gripEnabled);
            UpdateOutputBindingsVisual();
            try { _io?.HintRefreshInputs(SafetyPolicy.IN_MODE_KEY_NAME, SafetyPolicy.IN_GRIP_SWITCH_ENABLE); } catch { }
            if (changed) UiLockChanged?.Invoke(_uiLocked);
        }
        private void UpdateInfoBanner(bool isTeach, bool gripEnabled)
        {
            EnsureInfoBanner();

            if (_uiLocked)
            {
                _infoBanner.Visible = true;
                _infoBannerText.Text = "TEACH 모드: Grip Switch를 눌러야 조작이 활성화됩니다.";
            }
            else
            {
                _infoBanner.Visible = false;
            }
        }
        private void TryWriteWithSafety(string outputName, bool desired)
        {
            if (IsUiLocked())
            {
                PulseBanner(); return;
            }
            if (_io == null) { _uiAlarm.Notify("IO", "IOForm 연결이 없습니다."); return; }
            var intent = _router.ResolveForOutputWrite(outputName, desired, source: "OverviewForm");
            bool isDoor = outputName?.IndexOf(SafetyPolicy.DOOR_KEYWORD, StringComparison.OrdinalIgnoreCase) >= 0;
            bool isUnlock = (isDoor && desired == true);
            if (isUnlock) _io.HintRefreshInputs(SafetyPolicy.IN_LASER_STATUS, SafetyPolicy.IN_MODE_KEY_NAME);
            if (intent != null)
            {
                var r = SafetyPolicy.EvaluateEx(_ctx, intent);
                if (!r.Allowed)
                {
                    if (!r.SuppressPopup) _uiAlarm.Notify("Safety Interlock", r.Reason ?? "인터락 조건 불만족으로 동작 차단.");
                    return;
                }
            }
            if (!_io.TryWriteOutputByName(outputName, desired, source: "OverviewForm"))
            {
                _uiAlarm.Notify("IO", $"출력 실패: {outputName}");
                return;
            }
            foreach (var b in _bindings)
                if (string.Equals(b.Name, outputName, StringComparison.OrdinalIgnoreCase))
                {
                    if (b.StateLed != null)
                    {
                        bool doorBinding = b.Name?.IndexOf(SafetyPolicy.DOOR_KEYWORD, StringComparison.OrdinalIgnoreCase) >= 0;
                        SetLed(b.StateLed, doorBinding ? !desired : desired);
                    }
                    break;
                }
            UpdateOutputBindingsVisual();
        }
        private void RegisterBindings()
        {
            _bindings.Clear();
            _bindings.Add(new OutputBinding { Name = "Clean Booth Front Door 1 Lock / Unlock", OnBtn = btnFrontDoor1_Unlock, OffBtn = btnFrontDoor1_Lock, StateLed = ledFrontDoor1_LockState });
            _bindings.Add(new OutputBinding { Name = "Clean Booth Front Door 2 Lock / Unlock", OnBtn = btnFrontDoor2_Unlock, OffBtn = btnFrontDoor2_Lock, StateLed = ledFrontDoor2_LockState });
            _bindings.Add(new OutputBinding { Name = "Clean Booth Rear Door 1 Lock / Unlock", OnBtn = btnRearDoor1_Unlock, OffBtn = btnRearDoor1_Lock, StateLed = ledRearDoor1_LockState });
            _bindings.Add(new OutputBinding { Name = "Clean Booth Rear Door 2 Lock / Unlock", OnBtn = btnRearDoor2_Unlock, OffBtn = btnRearDoor2_Lock, StateLed = ledRearDoor2_LockState });
            _bindings.Add(new OutputBinding { Name = "Clean Booth Door Side 1 Lock / Unlock", OnBtn = btnSideDoor1_Unlock, OffBtn = btnSideDoor1_Lock, StateLed = ledSideDoor1_LockState });
            _bindings.Add(new OutputBinding { Name = "Clean Booth Door Side 2 Lock / Unlock", OnBtn = btnSideDoor2_Unlock, OffBtn = btnSideDoor2_Lock, StateLed = ledSideDoor2_LockState });
            _bindings.Add(new OutputBinding { Name = "Clean Booth LED Lamp", OnBtn = btnCleanBoothLed_On, OffBtn = btnCleanBoothLed_Off, StateLed = ledCleanBoothLed });

            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_ACS_MC1, OnBtn = btnACSMc1_OutputOn, OffBtn = btnACSMc1_OutputOff, StateLed = ledACSMc1_OutputState });
            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_ACS_MC2_Y, OnBtn = btnACSMc2_OutputYOn, OffBtn = btnACSMc2_OutputYOff, StateLed = ledACSMc2_OutputYState });
            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_ACS_MC2_X, OnBtn = btnACSMc2_OutputXOn, OffBtn = btnACSMc2_OutputXOff, StateLed = ledACSMc2_OutputXState });
            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_AJIN_MC2_Z, OnBtn = btnAjinMc2_OutputZOn, OffBtn = btnAjinMc2_OutputZOff, StateLed = ledAjinMc2_OutputZState });
            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_AJIN_MC2_T, OnBtn = btnAjinMc2_OutputTOn, OffBtn = btnAjinMc2_OutputTOff, StateLed = ledAjinMc2_OutputTState });

            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_COOLING_Y1, OnBtn = btnY1Motor_AirCoolingVvOpen, OffBtn = btnY1Motor_AirCoolingVvClose, StateLed = ledY1Motor_AirCoolingVvState });
            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_COOLING_Y2, OnBtn = btnY2Motor_AirCoolingVvOpen, OffBtn = btnY2Motor_AirCoolingVvClose, StateLed = ledY2Motor_AirCoolingVvState });
            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_COOLING_X, OnBtn = btnXMotor_AirCoolingVvOpen, OffBtn = btnXMotor_AirCoolingVvClose, StateLed = ledXMotor_AirCoolingVvState });
            _bindings.Add(new OutputBinding { Name = SafetyPolicy.OUT_COOLING_LASER, OnBtn = btnLaserBox_AirCoolingVvOpen, OffBtn = btnLaserBox_AirCoolingVvClose, StateLed = ledLaserBox_AirCoolingVvState });
        }
        private void UpdateOutputBindingsVisual()
        {
            bool locked = IsUiLocked();

            foreach (var b in _bindings)
            {
                bool raw = Out(b.Name);
                bool isDoor = b.Name?.IndexOf("Door", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isCooling = b.Name?.IndexOf("Cooling V/V", StringComparison.OrdinalIgnoreCase) >= 0;
                bool onState = raw;
                string onCaption = isDoor ? "Unlock" : (isCooling ? "Open" : "On");
                string offCaption = isDoor ? "Lock" : (isCooling ? "Close" : "Off");
                if (b.OnBtn != null)
                {
                    b.OnBtn.Text = onCaption + (onState ? " (ON)" : "");
                    b.OnBtn.BackColor = onState ? Color.PaleGreen : SystemColors.Control;
                    b.OnBtn.Enabled = !locked && !onState;
                }
                if (b.OffBtn != null)
                {
                    b.OffBtn.Text = offCaption + (!onState ? " (ON)" : "");
                    b.OffBtn.BackColor = !onState ? Color.PaleGreen : SystemColors.Control;
                    b.OffBtn.Enabled = !locked && onState;
                }
                if (b.StateLed != null)
                {
                    SetLed(b.StateLed, isDoor ? !raw : raw);
                }
            }
        }
        private void WireButtons()
        {
            // Door Front 1
            btnFrontDoor1_Lock.Click += (s, e) => TryWriteWithSafety("Clean Booth Front Door 1 Lock / Unlock", false);
            btnFrontDoor1_Unlock.Click += (s, e) => TryWriteWithSafety("Clean Booth Front Door 1 Lock / Unlock", true);
            // Door Front 2
            btnFrontDoor2_Lock.Click += (s, e) => TryWriteWithSafety("Clean Booth Front Door 2 Lock / Unlock", false);
            btnFrontDoor2_Unlock.Click += (s, e) => TryWriteWithSafety("Clean Booth Front Door 2 Lock / Unlock", true);
            // Door Rear 1
            btnRearDoor1_Lock.Click += (s, e) => TryWriteWithSafety("Clean Booth Rear Door 1 Lock / Unlock", false);
            btnRearDoor1_Unlock.Click += (s, e) => TryWriteWithSafety("Clean Booth Rear Door 1 Lock / Unlock", true);
            // Door Rear 2
            btnRearDoor2_Lock.Click += (s, e) => TryWriteWithSafety("Clean Booth Rear Door 2 Lock / Unlock", false);
            btnRearDoor2_Unlock.Click += (s, e) => TryWriteWithSafety("Clean Booth Rear Door 2 Lock / Unlock", true);
            // Door Side 1
            btnSideDoor1_Lock.Click += (s, e) => TryWriteWithSafety("Clean Booth Door Side 1 Lock / Unlock", false);
            btnSideDoor1_Unlock.Click += (s, e) => TryWriteWithSafety("Clean Booth Door Side 1 Lock / Unlock", true);
            // Door Side 2
            btnSideDoor2_Lock.Click += (s, e) => TryWriteWithSafety("Clean Booth Door Side 2 Lock / Unlock", false);
            btnSideDoor2_Unlock.Click += (s, e) => TryWriteWithSafety("Clean Booth Door Side 2 Lock / Unlock", true);
            // LED Lamp
            btnCleanBoothLed_On.Click += (s, e) => TryWriteWithSafety("Clean Booth LED Lamp", true);
            btnCleanBoothLed_Off.Click += (s, e) => TryWriteWithSafety("Clean Booth LED Lamp", false);

            // ACS 1차측 MC
            btnACSMc1_OutputOn.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_ACS_MC1, true);
            btnACSMc1_OutputOff.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_ACS_MC1, false);
            // ACS 2차측 Main Y
            btnACSMc2_OutputYOn.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_ACS_MC2_Y, true);
            btnACSMc2_OutputYOff.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_ACS_MC2_Y, false);
            // ACS 2차측 Review X
            btnACSMc2_OutputXOn.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_ACS_MC2_X, true);
            btnACSMc2_OutputXOff.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_ACS_MC2_X, false);

            // Ajin 2차측 Maint Z
            btnAjinMc2_OutputZOn.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_AJIN_MC2_Z, true);
            btnAjinMc2_OutputZOff.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_AJIN_MC2_Z, false);
            // Ajin 2차측 Theta
            btnAjinMc2_OutputTOn.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_AJIN_MC2_T, true);
            btnAjinMc2_OutputTOff.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_AJIN_MC2_T, false);

            // Cooling V/V
            btnY1Motor_AirCoolingVvOpen.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_Y1, true);
            btnY1Motor_AirCoolingVvClose.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_Y1, false);
            btnY2Motor_AirCoolingVvOpen.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_Y2, true);
            btnY2Motor_AirCoolingVvClose.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_Y2, false);
            btnXMotor_AirCoolingVvOpen.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_X, true);
            btnXMotor_AirCoolingVvClose.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_X, false);
            btnLaserBox_AirCoolingVvOpen.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_LASER, true);
            btnLaserBox_AirCoolingVvClose.Click += (s, e) => TryWriteWithSafety(SafetyPolicy.OUT_COOLING_LASER, false);
        }
        #endregion
    }

    public sealed class OverviewSafetyContext : ISafetyContext
    {
        private readonly IMotionController _motion;
        private readonly Func<string, bool> _readInput;
        private readonly Func<string, bool> _readOutput;
        private readonly Func<bool> _laserOnProvider;
        private readonly Func<ProgramMode> _modeProvider;
        private readonly Func<double> _actVelXProvider;
        private readonly Func<double> _actVelYProvider;
        private readonly Func<double> _actVelMaintZ;
        private readonly Func<double> _actVelTheta;
        private DateTime _lastXTime, _lastYTime;
        private double _lastXPos, _lastYPos;
        private static readonly string[] ModeKeyAliases = new[]
        {
            "Mode Key Status", "Mode Key", "Mode Select Status", "Mode Selector",
            "Mode Switch Status", "Mode Switch", "Mode Key Switch Status"
        };
        private static readonly string[] GripEnableAliases = new[]
        {
            "Grip Switch Enable IO", "Grip Switch Enable", "Grip Enable",
            "Enabling Switch", "Teach Grip Enable", "Pendant Enable"
        };
        private static bool IsGripEnable(string name)
            => name?.IndexOf("grip", StringComparison.OrdinalIgnoreCase) >= 0
            || name?.IndexOf("enable", StringComparison.OrdinalIgnoreCase) >= 0;

        public OverviewSafetyContext(
            IMotionController motion,
            Func<string, bool> readInput,
            Func<string, bool> readOutput,
            Func<bool> laserOnProvider,
            Func<ProgramMode> modeProvider,
            Func<double> actVelXProvider,
            Func<double> actVelYProvider,
            Func<double> actVelMaintZ,
            Func<double> actVelTheta)
        {
            _motion = motion;
            _readInput = readInput;
            _readOutput = readOutput;
            _laserOnProvider = laserOnProvider;
            _modeProvider = modeProvider ?? (() => ProgramMode.Manual);
            _actVelXProvider = actVelXProvider ?? (() => 0.0);
            _actVelYProvider = actVelYProvider ?? (() => 0.0);
            _actVelMaintZ = actVelMaintZ ?? (() => 0.0);
            _actVelTheta = actVelTheta ?? (() => 0.0);
        }
        public ProgramMode Mode => _modeProvider();
        public string CurrentProgram => Mode.ToString().ToUpperInvariant();
        public double GetXActualVelocity()
        {
            double v = _actVelXProvider();
            if (Math.Abs(v) > 1e-6) return v;

            if (_motion != null)
            {
                var now = DateTime.UtcNow;
                double pos = _motion.GetPosition(Axis.X);
                if (_lastXTime == default) { _lastXTime = now; _lastXPos = pos; return 0.0; }
                double dt = (now - _lastXTime).TotalSeconds;
                double vel = (dt > 1e-6) ? (pos - _lastXPos) / dt : 0.0;
                _lastXTime = now; _lastXPos = pos;
                return vel;
            }
            return 0.0;
        }
        public double GetYActualVelocity()
        {
            double v = _actVelYProvider();
            if (Math.Abs(v) > 1e-6) return v;

            if (_motion != null)
            {
                var now = DateTime.UtcNow;
                double pos = _motion.GetPosition(Axis.Y);
                if (_lastYTime == default) { _lastYTime = now; _lastYPos = pos; return 0.0; }
                double dt = (now - _lastYTime).TotalSeconds;
                double vel = (dt > 1e-6) ? (pos - _lastYPos) / dt : 0.0;
                _lastYTime = now; _lastYPos = pos;
                return vel;
            }
            return 0.0;
        }
        public double GetMaintZActualVelocity() => _actVelMaintZ();
        public double GetThetaActualVelocity() => _actVelTheta();
        public bool GetInput(string name) => _readInput?.Invoke(name) ?? false;
        public bool GetOutput(string name) => _readOutput?.Invoke(name) ?? false;
        public DigitalStatus GetInputStatus(string name)
        {
            if (VirtualBus.DigitalInputs.TryGet(name, out var v))
            {
                var st = v.ReadValue;
                if (st != DigitalStatus.Unknown) return st;

                if (TryParseFromLabel(v.Label, out st)) return st;
                return v.RawBit ? DigitalStatus.On : DigitalStatus.Off;
            }
            if (IsModeKey(name))
            {
                foreach (var alias in ModeKeyAliases)
                {
                    if (VirtualBus.DigitalInputs.TryGet(alias, out var mv))
                    {
                        var st = mv.ReadValue;
                        if (st != DigitalStatus.Unknown) return st;
                        if (TryParseFromLabel(mv.Label, out st)) return st;
                        return mv.RawBit ? DigitalStatus.On : DigitalStatus.Off;
                    }
                }
            }
            if (IsGripEnable(name) || string.Equals(name, SafetyPolicy.IN_GRIP_SWITCH_ENABLE, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var alias in GripEnableAliases)
                {
                    if (VirtualBus.DigitalInputs.TryGet(alias, out var gv))
                    {
                        var st2 = gv.ReadValue;
                        if (st2 != DigitalStatus.Unknown) return st2;
                        if (TryParseFromLabel(gv.Label, out st2)) return st2;
                        return gv.RawBit ? DigitalStatus.On : DigitalStatus.Off;
                    }
                }
            }
            return DigitalStatus.Unknown;
        }
        public DigitalStatus GetOutputStatus(string name)
        {
            if (VirtualBus.DigitalOutputs.TryGet(name, out var v))
            {
                var st = v.ReadValue;
                if (st != DigitalStatus.Unknown) return st;
                if (TryParseFromLabel(v.Label, out st)) return st;
                return v.RawBit ? DigitalStatus.On : DigitalStatus.Off;
            }
            return DigitalStatus.Unknown;
        }
        public string GetInputLabel(string name)
        {
            if (VirtualBus.DigitalInputs.TryGet(name, out var v)) return v.Label;
            if (IsModeKey(name))
            {
                foreach (var alias in ModeKeyAliases)
                    if (VirtualBus.DigitalInputs.TryGet(alias, out var mv)) return mv.Label;
            }
            return "N/A";
        }
        public string GetOutputLabel(string name)
        {
            if (VirtualBus.DigitalOutputs.TryGet(name, out var v)) return v.Label;
            return "N/A";
        }
        public bool IsLaserOn() => _laserOnProvider?.Invoke() ?? false;
        private static bool IsModeKey(string name)
            => name?.IndexOf("mode", StringComparison.OrdinalIgnoreCase) >= 0;
        private static bool TryParseFromLabel(string label, out DigitalStatus status)
        {
            status = DigitalStatus.Unknown;
            if (string.IsNullOrWhiteSpace(label)) return false;

            var s = label.Trim().ToUpperInvariant();
            if (s.Contains("TEACH") || s.Contains("MANUAL") || s.Contains("HAND"))
            {
                status = DigitalStatus.Teach; return true;
            }
            if (s.Contains("AUTO") || s.Contains("AUTOMATIC"))
            {
                status = DigitalStatus.Auto; return true;
            }
            if (s.Contains("LOCK")) { status = DigitalStatus.Lock; return true; }
            if (s.Contains("UNLOCK")) { status = DigitalStatus.Unlock; return true; }
            if (s == "ON" || s.Contains("ENABLE")) { status = DigitalStatus.On; return true; }
            if (s == "OFF" || s.Contains("DISABLE")) { status = DigitalStatus.Off; return true; }
            if (s == "NA" || s == "N/A" || s == "NONE") { status = DigitalStatus.None; return true; }
            return false;
        }
    }
}
