namespace LabUno;

public class Foo
{
    private Bar _bar;
    private ITracer _tracer;

    internal Foo(ITracer tracer)
    {
        _tracer = tracer;
        _bar = new Bar(_tracer);
    }

    public void MyMethod()
    {
        _tracer.StartTrace();
        _bar.InnerMethod();
        _tracer.StopTrace();
    }
}

public class Bar
{
    private ITracer _tracer;

    internal Bar(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void InnerMethod()
    {
        _tracer.StartTrace();
        _tracer.StopTrace();
    }
}

public class Program
{
    static void Main()
    {
        ITracer tracer = new MethodTracer();

        tracer.StartTrace();
        Foo foo = new Foo(tracer);
        foo.MyMethod();
        tracer.StopTrace();

        string jsonResult = tracer.GetTraceResult();
        Console.WriteLine(jsonResult);
        File.WriteAllText("results.json", jsonResult);
    }
}