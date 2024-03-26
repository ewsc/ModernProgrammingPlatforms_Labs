using System;

namespace LabUno
{
    
    public class Program
    {

        public static void ProgramMethod(ITracer _tracer)
        {
            _tracer.StartTrace();
            _tracer.StopTrace();
        
        }
        static void Main()
        {
            ITracer tracer = new MethodTracer();

            tracer.StartTrace();
            Foo foo = new Foo(tracer);
            foo.FooMethod();
            tracer.StopTrace();

            ProgramMethod(tracer);
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

        public void FooMethod()
        {
            _tracer.StartTrace();
            Boo boo = new Boo(_tracer);
            boo.BooMethod();
            _tracer.StopTrace();
        }
    }
    
    public class Boo
    {
        private readonly ITracer _tracer;

        public Boo(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void BooMethod()
        {
            _tracer.StartTrace();
            _tracer.StopTrace();
        }
    }
}