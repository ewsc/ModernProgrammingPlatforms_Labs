using System.Diagnostics;
using System.Text.Json;

namespace LabUno;

public interface ITracer
{
    void StartTrace();
    void StopTrace();
    string GetTraceResult();
}

public class TraceResult
{
    public string Name { get; set; }
    public string ClassName { get; set; }
    public double ElapsedMilliseconds { get; set; }
    public List<TraceResult> Methods { get; set; }

    public TraceResult()
    {
        Methods = new List<TraceResult>();
    }
}

public class MethodTracer : ITracer
{
    private readonly Stopwatch _stopwatch;
    private readonly Stack<TraceResult> _traceStack;
    private readonly TraceResult _rootTraceResult;
    private TraceResult _currentParent;

    public MethodTracer()
    {
        _stopwatch = new Stopwatch();
        _traceStack = new Stack<TraceResult>();
        _rootTraceResult = new TraceResult();
        _currentParent = _rootTraceResult;
    }

    public void StartTrace()
    {
        _stopwatch.Start();
        var stackFrame = new StackFrame(1);
        var declaringType = stackFrame.GetMethod().DeclaringType;
        var methodName = stackFrame.GetMethod().Name;
        var className = declaringType != null ? declaringType.Name : string.Empty;

        TraceResult traceResult = new TraceResult
        {
            Name = methodName,
            ClassName = className
        };

        _currentParent.Methods.Add(traceResult);
        _traceStack.Push(_currentParent);
        _currentParent = traceResult;
    }

    public void StopTrace()
    {
        _stopwatch.Stop();
        TimeSpan elapsed = _stopwatch.Elapsed;
        _currentParent.ElapsedMilliseconds = elapsed.TotalMilliseconds;

        _currentParent = _traceStack.Pop();
    }

    public string GetTraceResult()
    {
        return SerializeTraceResult(_rootTraceResult);
    }

    private string SerializeTraceResult(TraceResult traceResult)
    {
        var options = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        return JsonSerializer.Serialize(traceResult, options);
    }
}
