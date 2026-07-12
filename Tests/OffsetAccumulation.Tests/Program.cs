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
                SetAppliedOffsetStoresFinalOffsetForLegacyConsumers();
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
            AssertNear(0.013, current.ErrX, "success stores accumulated X in ErrX");
            AssertNear(-0.016, current.ErrY, "success stores accumulated Y in ErrY");
            AssertNear(0.003, current.MeasuredErrX, "success preserves measured X");
            AssertNear(0.004, current.MeasuredErrY, "success preserves measured Y");
            AssertTrue(current.HasAppliedOffset, "success marks applied offset");
            AssertTrue(current.HasMeasuredErr, "success marks measured error");
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
                ErrX = 0.013,
                ErrY = -0.016,
                MeasuredErrX = 0.003,
                MeasuredErrY = 0.004,
                HasMeasuredErr = true,
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
            AssertNear(0.013, current.ErrX, "same measurement stores applied X in ErrX");
            AssertNear(-0.016, current.ErrY, "same measurement stores applied Y in ErrY");
            AssertNear(0.003, current.MeasuredErrX, "same measurement preserves measured X");
            AssertNear(0.004, current.MeasuredErrY, "same measurement preserves measured Y");
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
            AssertNear(0.010, current.ErrX, "failure stores preserved X in ErrX");
            AssertNear(-0.020, current.ErrY, "failure stores preserved Y in ErrY");
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
            AssertNear(0.015, current.ErrX, "legacy fallback stores accumulated X in ErrX");
            AssertNear(0.037, current.ErrY, "legacy fallback stores accumulated Y in ErrY");
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
            AssertNear(0.0, current.ErrX, "reset clears ErrX offset");
            AssertNear(0.0, current.ErrY, "reset clears ErrY offset");
            AssertTrue(!current.HasAppliedOffset, "reset clears applied flag");
        }

        private static void SetAppliedOffsetStoresFinalOffsetForLegacyConsumers()
        {
            var result = new MeasResult
            {
                FindResult = 1,
                ErrX = 0.001,
                ErrY = 0.002
            };

            result.SetAppliedOffset(0.123, -0.456);

            AssertNear(0.123, result.AppliedOffsetX, "set applied offset X");
            AssertNear(-0.456, result.AppliedOffsetY, "set applied offset Y");
            AssertNear(0.123, result.ErrX, "set applied offset stores X in ErrX");
            AssertNear(-0.456, result.ErrY, "set applied offset stores Y in ErrY");
            AssertTrue(result.HasAppliedOffset, "set applied offset marks applied flag");
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
