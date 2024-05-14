using TestGenerator;

namespace LabQuatro;

public class Program
{
    public static async Task Main(string[] args)
    {
        List<string> sourceFiles = new List<string>
        {
            "source/Tracer.cs",
            // ...
        };

        string outputPath = "output/";

        var generator = new NUnitTestGenerator();
        await generator.GenerateTestsAsync(sourceFiles, outputPath);
    }
}