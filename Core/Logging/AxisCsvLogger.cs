using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Core.Logging
{
    public sealed class AxisCsvLogger : IDisposable
    {
        public sealed class Options
        {
            public Func<AxisRawData> SampleProvider { get; set; }
            public int IntervalMs { get; set; } = 200;
        }

        public struct AxisRawData
        {
            public DateTime Timestamp;

            public double XPos;
            public double XVel;
            public double XAcc;
            public double XRms;

            public bool XServoOn;
            public bool XHome;

            public double YPos;
            public double YVel;
            public double YAcc;
            public double YRms;

            public bool YServoOn;
            public bool YHome;

            public double ZPos;
            public double ZCmdVel;
            public double ZRms;

            public bool ZServoOn;
            public bool ZHome;

            public double TPos;
            public double TCmdVel;
            public double TRms;

            public bool TServoOn;
            public bool THome;
        }

        readonly object _sync = new object();
        Timer _timer;
        Options _opt;

        DateTime _openDate = DateTime.MinValue;
        StreamWriter _sw;

        static string Csv(params object[] values)
        {
            return string.Join(",", values.Select(v =>
            {
                if (v == null) return string.Empty;
                if (v is IFormattable f) return f.ToString(null, CultureInfo.InvariantCulture);
                return v.ToString();
            }));
        }

        public void Start(Options opt)
        {
            if (opt == null) throw new ArgumentNullException(nameof(opt));
            if (opt.SampleProvider == null) throw new ArgumentNullException(nameof(opt.SampleProvider));
            if (opt.IntervalMs < 10) throw new ArgumentOutOfRangeException(nameof(opt.IntervalMs));

            lock (_sync)
            {
                StopInternal();
                _opt = opt;
                _timer = new Timer(_ => Tick(), null, 0, _opt.IntervalMs);
            }
        }

        public void Stop()
        {
            lock (_sync) StopInternal();
        }

        void StopInternal()
        {
            try { _timer?.Dispose(); } catch { }
            _timer = null;

            try { _sw?.Dispose(); } catch { }
            _sw = null;

            _openDate = DateTime.MinValue;
        }

        void Tick()
        {
            AxisRawData AxisData;
            try
            {
                AxisData = _opt.SampleProvider();
            }
            catch
            {
                return;
            }

            try
            {
                lock (_sync)
                {
                    EnsureWriter(AxisData.Timestamp);

                    double xVel = AxisData.XVel;
                    double yVel = AxisData.YVel;

                    double zVel = AxisData.ZCmdVel;
                    double tVel = AxisData.TCmdVel;

                    double xAcc = AxisData.XAcc;
                    double yAcc = AxisData.YAcc;

                    _sw.WriteLine(Csv(
                        AxisData.Timestamp.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture),
                        AxisData.XPos, xVel, xAcc, AxisData.XRms,
                        AxisData.XServoOn ? 1 : 0, AxisData.XHome ? 1 : 0,

                        AxisData.YPos, yVel, yAcc, AxisData.YRms,
                        AxisData.YServoOn ? 1 : 0, AxisData.YHome ? 1 : 0,

                        AxisData.ZPos, AxisData.ZCmdVel, AxisData.ZRms,
                        AxisData.ZServoOn ? 1 : 0, AxisData.ZHome ? 1 : 0,

                        AxisData.TPos, AxisData.TCmdVel, AxisData.TRms,
                        AxisData.TServoOn ? 1 : 0, AxisData.THome ? 1 : 0
                    ));
                    _sw.Flush();
                }
            }
            catch
            {
            }
        }

        void EnsureWriter(DateTime ts)
        {
            var day = ts.Date;
            if (_sw != null && _openDate == day) return;

            try { _sw?.Dispose(); } catch { }
            _sw = null;

            string dir = Path.Combine(Logger.LogDirectory, "AxisLog");
            Directory.CreateDirectory(dir);

            string path = Path.Combine(dir, ts.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".csv");
            bool writeHeader = !File.Exists(path);

            _sw = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), new UTF8Encoding(false));
            //Data 
            if (writeHeader)
            {
                _sw.WriteLine("Timestamp,X_Pos,X_Vel,X_Acc,X_Rms,X_ServoOn,X_Home,Y_Pos,Y_Vel,Y_Acc,Y_Rms,Y_ServoOn,Y_Home,Z_Pos,Z_CmdVel,Z_Rms,Z_ServoOn,Z_Home,Theta_Pos,Theta_CmdVel,Theta_Rms,Theta_ServoOn,Theta_Home");
                _sw.Flush();
            }

            _openDate = day;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
