using System;
using System.Threading;
using Xunit.Runners;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        static readonly object ConsoleLock = new object();

        // Use an event to know when we're done
        static readonly ManualResetEvent Finished = new ManualResetEvent(false);

        // Start out assuming success; we'll set this to 1 if we get a failed test
        static int _result;

        static int Main()
        {
            using (var runner = AssemblyRunner.WithAppDomain(typeof(Tests.BaseTests).Assembly.Location))
            {
                runner.OnDiscoveryComplete = OnDiscoveryComplete;
                runner.OnExecutionComplete = OnExecutionComplete;
                runner.OnTestFailed = OnTestFailed;
                runner.OnTestSkipped = OnTestSkipped;

                Console.WriteLine("Discovering...");
                //runner.Start(typeof(Tests.IntTests).FullName);
                //Finished.WaitOne();

                //Finished.Reset();
                //runner.Start(typeof(Tests.StringTests).FullName);
                //Finished.WaitOne();

                //Finished.Reset();
                //runner.Start(typeof(Tests.ObjectTests).FullName);
                //Finished.WaitOne();

                //Finished.Reset();
                //runner.Start(typeof(Tests.UIntTests).FullName);
                //Finished.WaitOne();

                //Finished.Reset();
                //runner.Start(typeof(Tests.DoubleTests).FullName);
                //Finished.WaitOne();

                Finished.Reset();
                runner.Start(typeof(Tests.BoolTests).FullName);
                Finished.WaitOne();

                Finished.Dispose();

                return _result;
            }
        }

        static void OnDiscoveryComplete(DiscoveryCompleteInfo info)
        {
            lock (ConsoleLock)
                Console.WriteLine($"Running {info.TestCasesToRun} of {info.TestCasesDiscovered} tests...");
        }

        static void OnExecutionComplete(ExecutionCompleteInfo info)
        {
            lock (ConsoleLock)
                Console.WriteLine($"Finished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s ({info.TestsFailed} failed, {info.TestsSkipped} skipped)");

            Finished.Set();
        }

        static void OnTestFailed(TestFailedInfo info)
        {
            lock (ConsoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("[FAIL] {0}: {1}", info.TestDisplayName, info.ExceptionMessage);
                if (info.ExceptionStackTrace != null)
                    Console.WriteLine(info.ExceptionStackTrace);

                Console.ResetColor();
            }

            _result = 1;
        }

        static void OnTestSkipped(TestSkippedInfo info)
        {
            lock (ConsoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
                Console.ResetColor();
            }
        }
    }
}
