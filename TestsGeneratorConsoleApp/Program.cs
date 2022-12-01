using TestsGeneratorLibrary;

namespace TestsGeneratorConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestsGenerator test = new TestsGenerator();
            test.CreateTests();
        }
    }
}