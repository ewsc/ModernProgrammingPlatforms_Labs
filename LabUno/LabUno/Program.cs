using System;

namespace LabUno
{
    
    public class Program
    {
        static void Main()
        {
            ITracer tracer = new MethodTracer();

            tracer.StartTrace();
            Foo foo = new Foo(tracer);
            foo.MyMethod();
            tracer.StopTrace();

            string traceResult = tracer.GetTraceResult();

            ITraceResultWriter consoleWriter = new ConsoleTraceResultWriter();
            consoleWriter.WriteTraceResult(traceResult);

            ITraceResultWriter fileWriter = new FileTraceResultWriter("text.json");
            fileWriter.WriteTraceResult(traceResult);
        }
    }

    public class Foo
    {
        private readonly ITracer _tracer;

        public Foo(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void MyMethod()
        {
            _tracer.StartTrace();
            // Code of MyMethod
            _tracer.StopTrace();
        }
    }
}