using System;
using System.Diagnostics;
using System.Reflection;

namespace LabUno
{
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
    }

    public class MethodTracer : ITracer
    {
        private Stopwatch stopwatch;
        private TraceResult traceResult;

        public void StartTrace()
        {
            stopwatch = new Stopwatch();
            traceResult = new TraceResult();
            stopwatch.Start();
        }

        public void StopTrace()
        {
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            traceResult.ElapsedMilliseconds = elapsed.TotalMilliseconds;

            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            traceResult.MethodName = methodBase.Name;
            traceResult.ClassName = methodBase.DeclaringType.Name;

            PrintTraceResult();
        }

        private void PrintTraceResult()
        {
            Console.WriteLine($"Method: {traceResult.MethodName};");
            Console.WriteLine($"Class: {traceResult.ClassName}();");
            Console.WriteLine($"Execution time: {traceResult.ElapsedMilliseconds} ms;");
            Console.WriteLine();
        }
    }

    public class TraceResult
    {
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public double ElapsedMilliseconds { get; set; }
    }
}

