public class Program
{
    static void Main()
    {
        ITracer tracer = new MethodTracer();

        tracer.StartTrace();
        ExampleMethod();
        tracer.StopTrace();
    }

    static void ExampleMethod()
    {
        for (int i = 0; i < 100000000; i++)
        {
            // Do some computation
        }
    }
}