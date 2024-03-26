using NUnit.Framework;
using System;
using System.IO;
using System.Text.Json;

namespace LabUno.Tests
{
    [TestFixture]
    public class MethodTracerTests
    {
        private MethodTracer _methodTracer;
        private ITracer _tracer;

        [SetUp]
        public void Setup()
        {
            _methodTracer = new MethodTracer();
            _tracer = (ITracer)_methodTracer;
        }
        
        [Test]
        public void StartTrace_StopTrace_GetTraceResult_ReturnsValidTraceResult()
        {
            string expectedMethodName = "TestMethod";
            string expectedClassName = "TestClass";

            _tracer.StartTrace();
            TestClass.TestMethod();
            _tracer.StopTrace();
            string traceResult = _tracer.GetTraceResult();

            Assert.That(traceResult, Is.Not.Null);
            TraceResult result = JsonSerializer.Deserialize<TraceResult>(traceResult);

        }

        [Test]
        public void WriteTraceResult_ConsoleTraceResultWriter_WritesToConsole()
        {
            string traceResult = "Test Trace Result";
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            ConsoleTraceResultWriter consoleWriter = new ConsoleTraceResultWriter();
            consoleWriter.WriteTraceResult(traceResult);
            string output = stringWriter.ToString();
            Assert.That(traceResult + Environment.NewLine, Is.EqualTo(output));
        }

        [Test]
        public void WriteTraceResult_FileTraceResultWriter_WritesToFile()
        {
            string traceResult = "Test Trace Result";
            string filePath = "test.txt";
            FileTraceResultWriter fileWriter = new FileTraceResultWriter(filePath);
            fileWriter.WriteTraceResult(traceResult);
            string fileContent = File.ReadAllText(filePath);
            Assert.That(traceResult, Is.EqualTo(fileContent));
            File.Delete(filePath);
        }
    }

    public class TestClass
    {
        public static void TestMethod()
        {
        }
    }
}