using System;
using StageWin.Core.Recipe;

namespace OffsetAccumulation.Tests
{
    internal static class Program
    {
        private const double Epsilon = 1e-12;

        private static int Main()
        {
            try
            {
                AccumulatesSuccessfulMeasurement();
                ReapplyingSameMeasurementDoesNotDoubleAccumulate();
                PreservesPreviousOffsetOnFailedMeasurement();
                UsesLegacyErrAsPreviousOffset();
                ResetClearsAppliedOffset();
                Console.WriteLine("Offset accumulation tests passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        private static void AccumulatesSuccessfulMeasurement()
        {
            var previous = new MeasResult
            {
                AppliedOffsetX = 0.010,
                AppliedOffsetY = -0.020,
                HasAppliedOffset = true
            };
            var current = new MeasResult
            {
                FindResult = 1,
                ErrX = 0.003,
                ErrY = 0.004
            };

            current.AccumulateAppliedOffset(previous, resetAppliedOffsets: false);

            AssertNear(0.013, current.AppliedOffsetX, "success accumulates X");
            AssertNear(-0.016, current.AppliedOffsetY, "success accumulates Y");
            AssertTrue(current.HasAppliedOffset, "success marks applied offset");
        }

        private static void ReapplyingSameMeasurementDoesNotDoubleAccumulate()
        {
            var previous = new MeasResult
            {
                FindResult = 1,
                TargetX = 10.0,
                TargetY = 20.0,
                MeasX = 9.7,
                MeasY = 19.6,
                ErrX = 0.003,
                ErrY = 0.004,
                AppliedOffsetX = 0.013,
                AppliedOffsetY = -0.016,
                HasAppliedOffset = true
            };
            var current = new MeasResult
            {
                FindResult = 1,
                TargetX = 10.0,
                TargetY = 20.0,
                MeasX = 9.7,
                MeasY = 19.6,
                ErrX = 0.003,
                ErrY = 0.004
            };

            current.AccumulateAppliedOffset(previous, resetAppliedOffsets: false);

            AssertNear(0.013, current.AppliedOffsetX, "same measurement preserves X");
            AssertNear(-0.016, current.AppliedOffsetY, "same measurement preserves Y");
            AssertTrue(current.HasAppliedOffset, "same measurement keeps applied flag");
        }

        private static void PreservesPreviousOffsetOnFailedMeasurement()
        {
            var previous = new MeasResult
            {
                AppliedOffsetX = 0.010,
                AppliedOffsetY = -0.020,
                HasAppliedOffset = true
            };
            var current = new MeasResult
            {
                FindResult = 0,
                ErrX = 0.100,
                ErrY = 0.100
            };

            current.AccumulateAppliedOffset(previous, resetAppliedOffsets: false);

            AssertNear(0.010, current.AppliedOffsetX, "failure preserves X");
            AssertNear(-0.020, current.AppliedOffsetY, "failure preserves Y");
            AssertTrue(current.HasAppliedOffset, "failure preserves applied flag");
        }

        private static void UsesLegacyErrAsPreviousOffset()
        {
            var legacyPrevious = new MeasResult
            {
                ErrX = 0.020,
                ErrY = 0.030,
                HasAppliedOffset = false
            };
            var current = new MeasResult
            {
                FindResult = 1,
                ErrX = -0.005,
                ErrY = 0.007
            };

            current.AccumulateAppliedOffset(legacyPrevious, resetAppliedOffsets: false);

            AssertNear(0.015, current.AppliedOffsetX, "legacy fallback accumulates X");
            AssertNear(0.037, current.AppliedOffsetY, "legacy fallback accumulates Y");
            AssertTrue(current.HasAppliedOffset, "legacy fallback marks applied offset");
        }

        private static void ResetClearsAppliedOffset()
        {
            var previous = new MeasResult
            {
                AppliedOffsetX = 0.010,
                AppliedOffsetY = 0.020,
                HasAppliedOffset = true
            };
            var current = new MeasResult
            {
                FindResult = 1,
                ErrX = 0.003,
                ErrY = 0.004
            };

            current.AccumulateAppliedOffset(previous, resetAppliedOffsets: true);

            AssertNear(0.0, current.AppliedOffsetX, "reset clears X");
            AssertNear(0.0, current.AppliedOffsetY, "reset clears Y");
            AssertTrue(!current.HasAppliedOffset, "reset clears applied flag");
        }

        private static void AssertNear(double expected, double actual, string name)
        {
            if (Math.Abs(expected - actual) > Epsilon)
                throw new InvalidOperationException($"{name}: expected {expected}, actual {actual}");
        }

        private static void AssertTrue(bool condition, string name)
        {
            if (!condition)
                throw new InvalidOperationException(name);
        }
    }
}
