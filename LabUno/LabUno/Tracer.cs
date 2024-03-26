using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace LabUno
{
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        string GetTraceResult();
    }
    
    public interface ITraceResultWriter
    {
        void WriteTraceResult(string result);
    }

    public class ConsoleTraceResultWriter : ITraceResultWriter
    {
        public void WriteTraceResult(string result)
        {
            Console.WriteLine(result);
        }
    }

    public class FileTraceResultWriter : ITraceResultWriter
    {
        private readonly string _filePath;

        public FileTraceResultWriter(string filePath)
        {
            _filePath = filePath;
        }

        public void WriteTraceResult(string result)
        {
            File.WriteAllText(_filePath, result);
        }
    }

    public class TraceResult
    {
        public string Name { get; }
        public string ClassName { get; }
        public double ElapsedMilliseconds { get; set; }
        public List<TraceResult> Methods { get; }

        public TraceResult()
        {
            Methods = new List<TraceResult>();
        }

        public TraceResult(string name, string className, double elapsedMilliseconds, List<TraceResult> methods)
        {
            Name = name;
            ClassName = className;
            ElapsedMilliseconds = elapsedMilliseconds;
            Methods = methods;
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
            lock (_traceStack)
            {
                _stopwatch.Start();
                var stackFrame = new StackFrame(1);
                var declaringType = stackFrame.GetMethod().DeclaringType;
                var methodName = stackFrame.GetMethod().Name;
                var className = declaringType != null ? declaringType.Name : string.Empty;

                TraceResult traceResult = new TraceResult(methodName, className, 0, new List<TraceResult>());

                _currentParent.Methods.Add(traceResult);
                _traceStack.Push(_currentParent);
                _currentParent = traceResult;
            }
        }

        public void StopTrace()
        {
            lock (_traceStack)
            {
                _stopwatch.Stop();
                TimeSpan elapsed = _stopwatch.Elapsed;
                _currentParent.ElapsedMilliseconds = elapsed.TotalMilliseconds;

                _currentParent = _traceStack.Pop();
            }
        }

        public string GetTraceResult()
        {
            return SerializeTraceResult(_rootTraceResult);
        }

        private string SerializeTraceResult(TraceResult traceResult)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = true
            };
            string jsonResult = JsonSerializer.Serialize(traceResult, jsonOptions);

            var xmlSerializer = new XmlSerializer(typeof(TraceResult));
            using (var writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, traceResult);
                string xmlResult = writer.ToString();

            }

            return jsonResult;
        }
    }
}