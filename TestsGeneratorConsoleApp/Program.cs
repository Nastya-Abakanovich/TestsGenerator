using TestsGeneratorLibrary;

namespace TestsGeneratorConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string inputPath = "..\\..\\..\\..\\Example\\TestClasses";
            string outputPath = "..\\..\\..\\..\\Example\\GeneratedTests";
            List<string> fileList = new List<string>();

            fileList.AddRange(Directory.GetFiles(inputPath, "*.cs"));

            Сonveyor сonveyor = new Сonveyor(outputPath, 10, 10, 10);
            await сonveyor.Generate(fileList);
        }
    }
}