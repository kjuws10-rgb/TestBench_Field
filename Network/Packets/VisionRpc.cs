using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NetCommon;

namespace StageWin.Driver.Network.Packets
{
    /// <summary>
    /// Stage ↔ Vision RPC 요청/응답 헬퍼 및 라우터 등록.
    /// </summary>
    public static class VisionRpc
    {
        // Stage -> Vision: Glass Align 요청 (3000)
        public static async Task<ST_GLASS_ALIGN_RSP> Request_GlassAlignAsync(
            RpcSession vision, int timeoutMs = 900000000) // Timeout Test PGT
        {
            if (vision == null) throw new ArgumentNullException(nameof(vision));
            var req = new ST_GLASS_ALIGN_REQ() {  }; // 0B
            
            var rsp = await vision.RequestAsync(Cmd.GLASS_ALIGN_REQ, BinMarshal.ToBytes(req), timeoutMs)
                                  .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_GLASS_ALIGN_RSP>(rsp.Body);
        }

        // Stage -> Vision: 2nd Align 요청 (3001)
        public static async Task<ST_2ND_ALIGN_RSP> Request_SecondAlignAsync(
            RpcSession vision, int timeoutMs = 80000000) // PGT Timeout Test
        {
            if (vision == null) throw new ArgumentNullException(nameof(vision));
            var req = new ST_2ND_ALIGN_REQ(); // 0B
            var rsp = await vision.RequestAsync(Cmd.SECOND_ALIGN_REQ, BinMarshal.ToBytes(req), timeoutMs)
                                  .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_2ND_ALIGN_RSP>(rsp.Body);
        }

        // Stage -> Vision: Single Mark Find 요청 (3002)
        public static async Task<ST_MARK_FIND_SINGLE_RSP> Request_MarkFindSingleAsync(
            RpcSession vision, int nLine, int nGlobalR, int nGlobalC, int timeoutMs = 800000)
        {
            if (vision == null) throw new ArgumentNullException(nameof(vision));
            var req = new ST_MARK_FIND_SINGLE_REQ
            {
                nLine = nLine,
                nGlobalR = nGlobalR,
                nGlobalC = nGlobalC
            };
            var rsp = await vision.RequestAsync(Cmd.MARK_FIND_SINGLE, BinMarshal.ToBytes(req), timeoutMs)
                                  .ConfigureAwait(false);
            return BinMarshal.FromBytes<ST_MARK_FIND_SINGLE_RSP>(rsp.Body);
        }

        // Stage -> Vision: Line Mark Find 요청 (3003)
        // Vision 응답은 ST_FIND_DATA가 n개 연속으로 온다고 가정
        // 라인 Find 요청 (파서 교체)
        public static async Task<ST_MARK_FIND_SINGLE_RSP[]> Request_MarkFindLineAsync(
            RpcSession vision, int nDataCount, int timeoutMs = 12000)
        {
            if (vision == null) throw new ArgumentNullException(nameof(vision));
            if (nDataCount <= 0) throw new ArgumentOutOfRangeException(nameof(nDataCount));

            var req = new ST_MARK_FIND_LINE_REQ { nDataCount = nDataCount };
            var rsp = await vision.RequestAsync(Cmd.MARK_FIND_LINE, BinMarshal.ToBytes(req), timeoutMs)
                                  .ConfigureAwait(false);

            return ParseMarkFindLineStrict(rsp.Body);
        }

        // 라인 Find(3003) 응답 파서
        public static ST_MARK_FIND_SINGLE_RSP[] ParseMarkFindLineStrict(byte[] body)
        {
            if (body == null) throw new FormatException("Empty 3003 body");

            // PACK 고정 길이(배열 SizeConst=MAX_LINE_FIND)로만 파싱
            var pack = BinMarshal.FromBytesStrict<ST_MARK_FIND_LINE_RSP_PACK>(body);

            int n = pack.nDataCount;
            if (n <= 0 || n > VisionConst.MAX_LINE_FIND)
                throw new FormatException($"Invalid nDataCount={n}");

            if (pack.nResult == null || pack.dTargetX == null || pack.dTargetY == null ||
                pack.dMarkX == null || pack.dMarkY == null)
                throw new FormatException("3003 PACK arrays missing");

            var arr = new ST_MARK_FIND_SINGLE_RSP[n];
            for (int i = 0; i < n; i++)
            {
                arr[i].nResult = pack.nResult[i];
                arr[i].dTargetX = pack.dTargetX[i];
                arr[i].dTargetY = pack.dTargetY[i];
                arr[i].dMarkX = pack.dMarkX[i];
                arr[i].dMarkY = pack.dMarkY[i];
            }
            return arr;
        }

        // Stage -> Vision : Flying Ready (3004)
        public static async Task<bool> Request_FlyingReadyAsync(
            RpcSession vision, int nDataCount, int nLine, int nCol, int timeoutMs = 80000)
        {
            if (vision == null) throw new ArgumentNullException(nameof(vision));

            var req = new ST_FLYING_READY_REQ
            {
                nDataCount = nDataCount,
                nLine = nLine,
                nCol = nCol
            };
            var rsp = await vision.RequestAsync(Cmd.FLYING_READY, BinMarshal.ToBytes(req), timeoutMs)
                                  .ConfigureAwait(false);
            return rsp.Header.nErrorCode == Err.OK;
        }

        // Stage -> Vision : Flying MoveDone (3007)
        public static async Task<bool> Request_FlyingMoveDoneAsync(
            RpcSession vision, int timeoutMs = 5000)
        {
            if (vision == null) throw new ArgumentNullException(nameof(vision));

            var req = new ST_FLYING_MOVEDONE_REQ(); // 0B
            var rsp = await vision.RequestAsync(Cmd.FLYING_MOVE_DONE, BinMarshal.ToBytes(req), timeoutMs)
                                   .ConfigureAwait(false);
            return rsp.Header.nErrorCode == Err.OK; // 바디 없음, 헤더 코드로 판정
        }


        // Vision -> Stage 라우팅 빌더
        public static void RegisterHandlers(PacketRouter router,
            Action<string> log,
            Func<ST_MOTOR_MOVE_VISION_REQ, int> motorMoveHandler,
            Action<ST_GLASS_ALIGN_RSP> alignRespHandler,
            Func<double[]> motorCurPosProvider = null,                          
            Action<ST_2ND_ALIGN_RSP> secondAlignRespHandler = null,             
            Action<ST_MARK_FIND_SINGLE_RSP> markFindSingleRespHandler = null,   
            Action<ST_MARK_FIND_SINGLE_RSP[]> markFindLineRespHandler = null,
            Action<ST_FLYING_READY_REQ> flyingReadyReqHandler = null   ,
            Action<ST_MANUAL_INSPECTION_REQ> manualInspectionReqHandler = null
        )
        {
            if (router == null) throw new ArgumentNullException(nameof(router));
            // 3005 Vision->Stage : Motor Move
            router.Register(Cmd.MOTOR_MOVE_FROM_VISION, async (sess, body) =>
            {
                try
                {
                    var mv = BinMarshal.FromBytes<ST_MOTOR_MOVE_VISION_REQ>(body);
                    log?.Invoke($"[VISION->Stage 3005] Type={mv.nMoveType} X={mv.dMoveX} Y={mv.dMoveY} T={mv.dMoveT}");
                    int err = 0;
                    if (motorMoveHandler != null)
                    {
                        try { err = motorMoveHandler(mv); }
                        catch (Exception ex) { sess.Logger.Error("motorMoveHandler exception", ex); err = Err.IOERROR; }
                    }
                    var echo = new ST_MOTOR_MOVE_VISION_RSP { nResult = (err == 0 ? 1 : 0) };
                    return (err == 0) ? RpcResponse.RespOK(BinMarshal.ToBytes(echo))
                                      : RpcResponse.RespErr(err);
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 3005 failed: " + ex.Message);
                    return RpcResponse.RespErr(Err.UNHANDLED);
                }
            });
            // 3006 Vision->Stage : Motor Cur Pos
            router.Register(Cmd.MOTOR_CUR_POS_REQ, async (sess, body) =>
            {
                try
                {
                    double[] pos = new[] { 0.0, 0.0, 0.0 };
                    int err = Err.OK;
                    try
                    {
                        var p = motorCurPosProvider?.Invoke();
                        if (p != null && p.Length >= 3) pos = new[] { p[0], p[1], p[2] };
                    }
                    catch (Exception ex) { sess.Logger.Error("motorCurPosProvider exception", ex); err = Err.IOERROR; }

                    var rsp = new ST_MOTOR_CUR_POS_RSP { dMotorCurPos = pos };
                    return (err == Err.OK) ? RpcResponse.RespOK(BinMarshal.ToBytes(rsp))
                                           : RpcResponse.RespErr(err);
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 3006 failed: " + ex.Message);
                    return RpcResponse.RespErr(Err.UNHANDLED);
                }
            });
            // 3000 Stage->Vision : Glass Align
            router.Register(Cmd.GLASS_ALIGN_REQ, async (sess, body) =>
            {
                try
                {
                    var res = BinMarshal.FromBytes<ST_GLASS_ALIGN_RSP>(body);
                    log?.Invoke($"[Vision 응답] GlassAlign: Result={(res.nResult == 1 ? "OK" : "FAIL")} Judge={(res.nJudgement == 1 ? "OK" : "NG")} " +
                                $"Err(X,Y,T)=({res.dAlignErrorX:F3},{res.dAlignErrorY:F3},{res.dAlignErrorT:F3})");
                    alignRespHandler?.Invoke(res);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex) { log?.Invoke("Handle 3000(resp) failed: " + ex.Message); return RpcResponse.OneWay(); }
            });
            // 3001 Stage->Vision : 2nd Align
            router.Register(Cmd.SECOND_ALIGN_REQ, async (sess, body) =>
            {
                try
                {
                    var res = BinMarshal.FromBytes<ST_2ND_ALIGN_RSP>(body);
                    log?.Invoke($"[Vision 응답] 2ndAlign: Result={(res.nResult == 1 ? "OK" : "FAIL")} Judge={(res.nJudgement == 1 ? "OK" : "NG")} " +
                                $"Err(X,Y,T)=({res.dAlignErrorX:F3},{res.dAlignErrorY:F3},{res.dAlignErrorT:F3})");
                    secondAlignRespHandler?.Invoke(res);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex) { log?.Invoke("Handle 3001(resp) failed: " + ex.Message); return RpcResponse.OneWay(); }
            });
            // 3002 Stage->Vision : Mark Find (Single)
            router.Register(Cmd.MARK_FIND_SINGLE, async (sess, body) =>
            {
                try
                {
                    var res = BinMarshal.FromBytes<ST_MARK_FIND_SINGLE_RSP>(body);
                    log?.Invoke($"[Vision 응답] MarkFind(1): res={res.nResult}, " +
                                $"Target=({res.dTargetX:F3}px,{res.dTargetY:F3}px), " +
                                $"Mark=({res.dMarkX:F3}px,{res.dMarkY:F3}px)");
                    markFindSingleRespHandler?.Invoke(res);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex) { log?.Invoke("Handle 3002(resp) failed: " + ex.Message); return RpcResponse.OneWay(); }
            });
            // 3003 Stage->Vision : Mark Find (Line)
            router.Register(Cmd.MARK_FIND_LINE, async (sess, body) =>
            {
                try
                {
                    var list = ParseMarkFindLineStrict(body);
                    log?.Invoke($"[Vision 응답] MarkFind(Line) {list.Length}개");
                    markFindLineRespHandler?.Invoke(list);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex) { log?.Invoke("Handle 3003(resp) failed: " + ex.Message); return RpcResponse.OneWay(); }
            });
            // 3004 Stage->Vision : Flying Ready
            router.Register(Cmd.FLYING_READY, async (sess, body) =>
            {
                try
                {
                    ST_FLYING_READY_REQ req = default;
                    if (body != null && body.Length > 0)
                        req = BinMarshal.FromBytes<ST_FLYING_READY_REQ>(body);

                    // 3002 스타일로 "한 줄" 고정 로그
                    log?.Invoke($"[Vision 요청] FlyingReady: Line={req.nLine}, VecC={req.nCol}, Count={req.nDataCount}");

                    // (옵션) 파일 저장/표준 Logger도 동일하게 남기고 싶으면 sess.Logger 사용
                    // sess.Logger.Info($"[FV-READY] Line={req.nLine}, VecC={req.nCol}, Count={req.nDataCount}");

                    flyingReadyReqHandler?.Invoke(req);
                    return RpcResponse.OneWay();
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 3004(FlyingReady) failed: " + ex.Message);
                    return RpcResponse.RespErr(Err.UNHANDLED);
                }
            });
            // 3007 Stage->Vision : Flying Movedone
            router.Register(Cmd.FLYING_MOVE_DONE, async (sess, body) =>
            {
                // Vision의 RSP 수신 (0B)
                var _ = BinMarshal.FromBytes<ST_FLYING_MOVEDONE_RSP>(body);
                log?.Invoke("[Vision 응답] 3007 FLYING_MOVE_DONE_RSP");
                return RpcResponse.OneWay();
            });
            // 3008 Vision->Stage : Manual Inspection
            router.Register(Cmd.MANUAL_INSPECTION_REQ, async (sess, body) =>
            {
                try
                {
                    var req = BinMarshal.FromBytes<ST_MANUAL_INSPECTION_REQ>(body);

                    log?.Invoke($"[VISION->Stage 3008] ManualInspection: res={req.nResult}, " +
                                $"Target=({req.dTargetX:F3}px,{req.dTargetY:F3}px), " +
                                $"Mark=({req.dMarkX:F3}px,{req.dMarkY:F3}px)");

                    manualInspectionReqHandler?.Invoke(req);

                    var rsp = new ST_MANUAL_INSPECTION_RSP { };
                    return RpcResponse.RespOK(BinMarshal.ToBytes(rsp));
                }
                catch (Exception ex)
                {
                    log?.Invoke("Handle 3008 failed: " + ex.Message);
                    return RpcResponse.RespErr(Err.UNHANDLED);
                }
            });
            router.Register(Cmd.HEARTBEAT, (sess, body) => Task.FromResult(RpcResponse.OneWay()));
        }
    }
}
