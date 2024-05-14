namespace LabQuatro;

public static class Program
{
    public static async Task Main(string[] args)
    {
        string[] sourceFiles = { "source/Tracer.cs" };
        string outputPath = "output";
        int maxFilesToLoad = 5;
        int maxConcurrentTasks = 10;
        int maxFilesToWrite = 5;

        TestClassGenerator generator = new TestClassGenerator(sourceFiles, outputPath, maxFilesToLoad, maxConcurrentTasks, maxFilesToWrite);
        await generator.GenerateTestClassesAsync();
    }
}