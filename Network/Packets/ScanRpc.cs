using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NetCommon;

namespace StageWin.Driver.Network.Packets
{
    /// <summary>
    /// Stage → Scan RPC 요청/응답 헬퍼 및
    /// Scan → Stage 수신 패킷에 대한 라우터 등록 빌더.
    /// </summary>
    public static class ScanRpc
    {
        public static async Task<ST_RECIPE_LIST_RSP> Request_RecipeListAsync(
            RpcSession scan, int timeoutMs = 50000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var req = new ST_RECIPE_LIST_REQ(); // 0B
            var rsp = await scan.RequestAsync(Cmd.RECIPE_LIST_REQ, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_RECIPE_LIST_RSP>(rsp.Body);
        }

        public static async Task<string> Request_RecipeAddAsync(
            RpcSession scan, string recipeName, int timeoutMs = 5000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var req = new ST_RECIPE_ADD_REQ { chArrRecipeName = ScanUtil.ToFixedName(recipeName) };
            var rsp = await scan.RequestAsync(Cmd.RECIPE_ADD, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);

            var echo = BinMarshal.FromBytes<ST_RECIPE_ADD_RSP>(rsp.Body);
            return ScanUtil.FromFixedName(echo.chArrRecipeName);
        }

        public static async Task<string> Request_RecipeDeleteAsync(
            RpcSession scan, string recipeName, int timeoutMs = 50000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var req = new ST_RECIPE_DELETE_REQ { chArrRecipeName = ScanUtil.ToFixedName(recipeName) };
            var rsp = await scan.RequestAsync(Cmd.RECIPE_DELETE, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            var echo = BinMarshal.FromBytes<ST_RECIPE_DELETE_RSP>(rsp.Body);
            return ScanUtil.FromFixedName(echo.chArrRecipeName);
        }

        public static async Task<string> Request_RecipeSelectAsync(
            RpcSession scan, string recipeName, int timeoutMs = 50000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var req = new ST_RECIPE_SELECT_REQ { chArrRecipeName = ScanUtil.ToFixedName(recipeName) };
            var rsp = await scan.RequestAsync(Cmd.RECIPE_SELECT, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            var echo = BinMarshal.FromBytes<ST_RECIPE_SELECT_RSP>(rsp.Body);
            return ScanUtil.FromFixedName(echo.chArrRecipeName);
        }

        public static async Task<ST_RECIPE_INFO_RSP> Request_RecipeInfoAsync(
            RpcSession scan, string recipeNameOrNull = null, int timeoutMs = 50000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var name = recipeNameOrNull ?? string.Empty;

            var req = new ST_RECIPE_INFO_REQ
            {
                chArrRecipeName = ScanUtil.ToFixedName(name),
                Items = new ST_PARAM_DATA_REQ[100]
            };
            req.Items[0].chDataName = FixedAsciiUtil.ToFixed(ScanParamKeys.LASER_POWER, 40);
            req.Items[1].chDataName = FixedAsciiUtil.ToFixed(ScanParamKeys.ATTENUATOR_POS, 40);

            var rsp = await scan.RequestAsync(Cmd.RECIPE_INFO_REQ, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_RECIPE_INFO_RSP>(rsp.Body);
        }
        public static async Task<ST_RECIPE_DATA_CHANGE_RSP> Request_RecipeDataChangeAsync(
            RpcSession scan, string recipeName, double power, double atten, int timeoutMs = 50000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));

            var req = new ST_RECIPE_DATA_CHANGE_REQ
            {
                chArrRecipeName = ScanUtil.ToFixedName(recipeName),
                Items = new ST_PARAM_DATA_VAL[100]
            };
            req.Items[0].chDataName = FixedAsciiUtil.ToFixed(ScanParamKeys.LASER_POWER, 40);
            req.Items[0].chDataValue = FixedAsciiUtil.ToFixed(power.ToString(CultureInfo.InvariantCulture), 128);
            req.Items[1].chDataName = FixedAsciiUtil.ToFixed(ScanParamKeys.ATTENUATOR_POS, 40);
            req.Items[1].chDataValue = FixedAsciiUtil.ToFixed(atten.ToString(CultureInfo.InvariantCulture), 128);

            var rsp = await scan.RequestAsync(Cmd.RECIPE_DATA_CHANGE, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_RECIPE_DATA_CHANGE_RSP>(rsp.Body);
        }
        public static async Task<ST_SCAN_STOP_RSP> Request_ScanStopAsync(
            RpcSession scan, int scanNo, int timeoutMs = 30000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var req = new ST_SCAN_STOP_REQ { nScanNo = scanNo };
            var rsp = await scan.RequestAsync(Cmd.SCAN_STOP, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_SCAN_STOP_RSP>(rsp.Body);
        }

        public static async Task<ST_LASER_CONTROL_RSP> Request_LaserControlAsync(
            RpcSession scan, ST_LASER_CONTROL_REQ req, int timeoutMs = 50000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var rsp = await scan.RequestAsync(Cmd.LASER_CONTROL, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_LASER_CONTROL_RSP>(rsp.Body);
        }

        public static Task<ST_LASER_CONTROL_RSP> Request_LaserControlAsync(
            RpcSession scan, int scanNo, long time, double power, double freq,
        int laserOn, double xOffset, double yOffset, int timeoutMs = 50000)
        => Request_LaserControlAsync(scan, new ST_LASER_CONTROL_REQ
        {
            nScanNo = scanNo,
            nTime = time,
            dPower = power,
            dFrequency = freq,
            bLaserOn = laserOn,
            dXOffset = xOffset,
            dYOffset = yOffset
        }, timeoutMs);

        public static async Task<ST_SCAN_STATUS_RSP> Request_ScanStatusAsync(
            RpcSession scan, int timeoutMs = 30000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            var req = new ST_SCAN_STATUS_REQ(); // 0B
            var rsp = await scan.RequestAsync(Cmd.SCAN_STATUS_REQ, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_SCAN_STATUS_RSP>(rsp.Body);
        }

        public static async Task<ST_PROCESS_SCANNING_RSP> Request_ProcessScanningAsync(
            RpcSession scan, int scanNo, ST_DRAW_DATA_LIST[] list, double motorX, double motorY, int timeoutMs = 60000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            if (list == null) list = Array.Empty<ST_DRAW_DATA_LIST>();

            var header = new ST_PROCESS_SCANNING_REQ
            {
                nScanNo = scanNo,
                nDataCount = list.Length,
                dMotorX = motorX,
                dMotorY = motorY
            };

            byte[] body = NetCommon.ScanUtil.BuildBody(header, list);
            var rsp = await scan.RequestAsync(Cmd.PROCESS_SCANNING, body, timeoutMs).ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_PROCESS_SCANNING_RSP>(rsp.Body);
        }

        /// <summary>
        /// 1010 Power Meter Measurement #1
        /// n개 테이블(주파수/타겟파워/Att Min/Max, Range%)을 넘기면
        /// 보정된 Att 위치 배열을 회수.
        /// </summary>
        public static async Task<ST_POWER_MEAS1_RSP> Request_PowerMeter_Measurement1Async(
            RpcSession scan,
            int scanNo,
            double rangePercent,
            ST_POWER_TABLE[] tableList,
            int timeoutMs = 1_200_000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));
            if (tableList == null) tableList = Array.Empty<ST_POWER_TABLE>();

            var head = new ST_POWER_MEAS1_REQ
            {
                nScanNo = scanNo,
                nDataCount = tableList.Length,
                dRange = rangePercent
            };

            byte[] body = ScanUtil.BuildMeas1Body(head, tableList);
            var rsp = await scan.RequestAsync(Cmd.POWER_METER_MEAS_1, body, timeoutMs)
                                .ConfigureAwait(false);

            // 응답 바디: (nScanNo,bResult) + double[n]
            return ScanUtil.ParsePowerMeas1Rsp(rsp.Body);
        }

        /// <summary>
        /// 1011 Power Meter Measurement #2
        /// 단일 조건으로 레이저 출력 측정(Att 지정) → 측정 파워 반환.
        /// </summary>
        public static async Task<ST_POWER_MEAS2_RSP> Request_PowerMeter_Measurement2Async(
            RpcSession scan,
            int scanNo,
            double rangePercent,
            double frequency,
            double attPos,
            int timeoutMs = 10000)
        {
            if (scan == null) throw new ArgumentNullException(nameof(scan));

            var req = new ST_POWER_MEAS2_REQ
            {
                nScanNo = scanNo,
                dRange = rangePercent,
                dFrequency = frequency,
                dAttPos = attPos
            };

            var rsp = await scan.RequestAsync(Cmd.POWER_METER_MEAS_2, BinMarshal.ToBytes(req), timeoutMs)
                                .ConfigureAwait(false);

            return BinMarshal.FromBytes<ST_POWER_MEAS2_RSP>(rsp.Body);
        }


    public static void RegisterHandlers(PacketRouter router,
            Action<string> log,
            Func<ST_MOTOR_MOVE_REQ, int> motorMoveHandler,
            Action<ST_RECIPE_LIST_RSP> recipeRespHandler,
            Func<int, double> motorPosProvider = null,
            Action<ST_SCAN_STATUS_RSP> scanStatusRespHandler = null,
            Action<ST_LASER_CONTROL_RSP> laserControlRespHandler = null,
            Action<ST_SCAN_STOP_RSP> scanStopRespHandler = null,
            Action<ST_PROCESS_SCANNING_RSP> processScanningRespHandler = null,
            Action<ST_POWER_MEAS1_RSP> powerMeas1RespHandler = null,
            Action<ST_POWER_MEAS2_RSP> powerMeas2RespHandler = null)
        {
            if (router == null) throw new ArgumentNullException(nameof(router));

            // 1021 Scan -> Stage : 모터 이동 요청
            router.Register(Cmd.MOTOR_MOVE_FROM_SCAN, async (sess, body) =>
            {
                try
                {
                    var mv = BinMarshal.FromBytes<ST_MOTOR_MOVE_REQ>(body);
                    log?.Invoke($"[SCAN->Stage 1021] Ax={mv.nAxisNo} Type={mv.nMoveType} Val={mv.dMoveValue:F3}");
                    int err = 0;

                    if (motorMoveHandler != null)
                    {
                        try { err = motorMoveHandler(mv); }
                        catch (Exception ex)
                        {
                            sess.Logger.Error("motorMoveHandler exception", ex);
                            return RpcResponse.RespErr(Err.IOERROR);
                        }
                    }
                    var echo = new ST_MOTOR_MOVE_RSP { nAxisNo = mv.nAxisNo };
                    return err == 0 ? RpcResponse.RespOK(BinMarshal.ToBytes(echo))
                                    : RpcResponse.RespErr(err);
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1021 failed: " + ex.Message);
                    return RpcResponse.RespErr(Err.UNHANDLED);
                }
            });

            // 1022 Scan -> Stage : 모터 위치 요청
            router.Register(Cmd.MOTOR_POS_REQUEST, async (sess, body) =>
            {
                try
                {
                    var req = BinMarshal.FromBytes<ST_MOTOR_POS_REQ>(body);
                    double cur = 0; int err = Err.OK;

                    if (motorPosProvider != null)
                    {
                        try { cur = motorPosProvider(req.nAxisNo); }
                        catch (Exception ex) { sess.Logger.Error("motorPosProvider exception", ex); err = Err.IOERROR; }
                    }
                    else err = Err.UNHANDLED;

                    var rsp = new ST_MOTOR_POS_RSP { nAxisNo = req.nAxisNo, nCurPos = cur };
                    return err == Err.OK ? RpcResponse.RespOK(BinMarshal.ToBytes(rsp))
                                         : RpcResponse.RespErr(err);
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1022 failed: " + ex.Message);
                    return RpcResponse.RespErr(Err.UNHANDLED);
                }
            });

            // 1000 Stage->Scan 응답 수신 (OneWay)
            router.Register(Cmd.RECIPE_LIST_REQ, async (sess, body) =>
            {
                try
                {
                    var rl = BinMarshal.FromBytes<ST_RECIPE_LIST_RSP>(body);
                    var names = ScanUtil.ToStrings(rl);
                    log?.Invoke($"[Scan 응답] Recipes {names.Length}개: {string.Join(", ", names)}");
                    recipeRespHandler?.Invoke(rl);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1000(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1001 Stage->Scan 응답 수신 (echo)
            router.Register(Cmd.RECIPE_ADD, async (sess, body) =>
            {
                try
                {
                    var add = BinMarshal.FromBytes<ST_RECIPE_ADD_RSP>(body);
                    var name = ScanUtil.FromFixedName(add.chArrRecipeName);
                    log?.Invoke($"[Scan 응답] RECIPE_ADD echo: {name}");
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1001(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1002 Stage->Scan 응답 수신 (echo)
            router.Register(Cmd.RECIPE_DELETE, async (sess, body) =>
            {
                try
                {
                    var del = BinMarshal.FromBytes<ST_RECIPE_DELETE_RSP>(body);
                    var name = ScanUtil.FromFixedName(del.chArrRecipeName);
                    log?.Invoke($"[Scan 응답] RECIPE_DELETE echo: {name}");
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1002(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1003 Stage->Scan 응답 수신 (echo)
            router.Register(Cmd.RECIPE_SELECT, async (sess, body) =>
            {
                try
                {
                    var rs = BinMarshal.FromBytes<ST_RECIPE_SELECT_RSP>(body);
                    log?.Invoke($"[Scan 응답] RECIPE_SELECT echo: {ScanUtil.FromFixedName(rs.chArrRecipeName)}");
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1003(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1004 Stage->Scan 응답 수신 (echo)
            router.Register(Cmd.RECIPE_INFO_REQ, async (sess, body) =>
            {
                try
                {
                    var v = BinMarshal.FromBytes<ST_RECIPE_INFO_RSP>(body);

                    // 파라미터 값 추출(로그용)
                    double p = 0, a = 0;
                    int n = Math.Max(0, Math.Min(v.nDataCount, v.Items?.Length ?? 0));
                    for (int i = 0; i < n; i++)
                    {
                        var key = FixedAsciiUtil.FromFixed(v.Items[i].chDataName)?.Trim();
                        var val = FixedAsciiUtil.FromFixed(v.Items[i].chDataValue)?.Trim();
                        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val)) continue;

                        if (string.Equals(key, ScanParamKeys.LASER_POWER, StringComparison.OrdinalIgnoreCase))
                            double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out p);
                        else if (string.Equals(key, ScanParamKeys.ATTENUATOR_POS, StringComparison.OrdinalIgnoreCase))
                            double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out a);
                    }

                    var nm = ScanUtil.FromFixedName(v.chArrRecipeName);
                    log?.Invoke($"[Scan 응답] RECIPE_INFO name='{nm}' power={p:F2} att={a:F2}");
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1004(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1005 Stage->Scan 응답 수신 (echo)
            router.Register(Cmd.RECIPE_DATA_CHANGE, async (sess, body) =>
            {
                try
                {
                    var v = BinMarshal.FromBytes<ST_RECIPE_DATA_CHANGE_RSP>(body);
                    var nm = ScanUtil.FromFixedName(v.chArrRecipeName);

                    bool okP = v.Items?.Any(x =>
                        string.Equals(FixedAsciiUtil.FromFixed(x.chDataName).Trim(),
                                      ScanParamKeys.LASER_POWER, StringComparison.OrdinalIgnoreCase)
                        && x.nResult == 1) ?? false;

                    bool okA = v.Items?.Any(x =>
                        string.Equals(FixedAsciiUtil.FromFixed(x.chDataName).Trim(),
                                      ScanParamKeys.ATTENUATOR_POS, StringComparison.OrdinalIgnoreCase)
                        && x.nResult == 1) ?? false;

                    log?.Invoke($"[Scan 응답] RECIPE_DATA_CHANGE '{nm}' POWER={(okP ? "OK" : "NG")}, ATT={(okA ? "OK" : "NG")}");
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1005(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1006 Scan Stop 응답 수신 (echo)
            router.Register(Cmd.SCAN_STOP, async (sess, body) =>
            {
                try
                {
                    var st = BinMarshal.FromBytes<ST_SCAN_STOP_RSP>(body);
                    log?.Invoke($"[Scan 응답] 1006 SCAN_STOP echo ScanNo={st.nScanNo}");
                    scanStopRespHandler?.Invoke(st);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1006(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1007 Laser Control 응답 수신 (echo)
            router.Register(Cmd.LASER_CONTROL, async (sess, body) =>
            {
                try
                {
                    var st = BinMarshal.FromBytes<ST_LASER_CONTROL_RSP>(body);
                    log?.Invoke($"[Scan 응답] 1007 LASER_CONTROL echo ScanNo={st.nScanNo}");
                    laserControlRespHandler?.Invoke(st);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1007(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1008 Scan Status 응답 수신 (echo)
            router.Register(Cmd.SCAN_STATUS_REQ, async (sess, body) =>
            {
                try
                {
                    var st = BinMarshal.FromBytes<ST_SCAN_STATUS_RSP>(body);
                    log?.Invoke($"[Scan 응답] 1008 STATUS = [{string.Join(",", st.nArrScanStatus ?? new int[0])}]");
                    scanStatusRespHandler?.Invoke(st);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1008(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1009 Process Scanning 응답 수신 (echo)
            router.Register(Cmd.PROCESS_SCANNING, async (sess, body) =>
            {
                try
                {
                    var rsp = BinMarshal.FromBytes<ST_PROCESS_SCANNING_RSP>(body);
                    log?.Invoke($"[Scan 응답] 1009 PROCESS_SCANNING echo ScanNo={rsp.nScanNo}");
                    processScanningRespHandler?.Invoke(rsp);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1009(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1010 Power Meter #1 응답 수신 (echo)
            router.Register(Cmd.POWER_METER_MEAS_1, async (sess, body) =>
            {
                try
                {
                    var rsp = ScanUtil.ParsePowerMeas1Rsp(body);
                    int cnt = rsp.dResultAttPos?.Length ?? 0;

                    // 로그 미리보기(최대 5개 값만)
                    int prevN = Math.Min(cnt, 5);
                    var previewArr = new string[prevN];
                    for (int i = 0; i < prevN; i++) previewArr[i] = rsp.dResultAttPos[i].ToString("F3");
                    var preview = string.Join(", ", previewArr);
                    if (cnt > prevN) preview += ", ...";

                    log?.Invoke($"[Scan 응답] 1010 POWER_METER_MEAS_1 ScanNo={rsp.nScanNo} " +
                                $"Result={(rsp.bResult == 1 ? "OK" : "NG")} Count={cnt} " +
                                $"AttPos=[{preview}]");

                    powerMeas1RespHandler?.Invoke(rsp);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1010(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });

            // 1011 Power Meter #2 응답 수신 (echo)
            router.Register(Cmd.POWER_METER_MEAS_2, async (sess, body) =>
            {
                try
                {
                    var rsp = BinMarshal.FromBytes<ST_POWER_MEAS2_RSP>(body);
                    log?.Invoke($"[Scan 응답] 1011 POWER_METER_MEAS_2 ScanNo={rsp.nScanNo} " +
                                $"Result={(rsp.bResult == 1 ? "OK" : "NG")} " +
                                $"MeasPower={rsp.dResultLaserPower:F3}");

                    powerMeas2RespHandler?.Invoke(rsp);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 1011(resp) failed: " + ex.Message);
                    return RpcResponse.OneWay();
                }
            });


            router.Register(Cmd.HEARTBEAT, (sess, body) => Task.FromResult(RpcResponse.OneWay()));
        }
    }
}
