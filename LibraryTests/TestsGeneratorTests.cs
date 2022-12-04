using TestsGeneratorLibrary;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibraryTests
{
    public class TestsGeneratorTests
    {
        private List<OutputFileInfo> _outputFileInfos;
        private SyntaxNode _rootFile1;
        private SyntaxNode _rootFile2;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            TestsGenerator testsGenerator = new TestsGenerator();
            FileReaderWriter fileReaderWriter = new FileReaderWriter();
            string sourceCode = await fileReaderWriter.Read("..\\..\\..\\..\\Example\\TestClasses\\Class1.cs");
            _outputFileInfos = testsGenerator.CreateTests(sourceCode);

            _rootFile1 = CSharpSyntaxTree
                .ParseText(_outputFileInfos[0].FileContent)
                .GetRoot();

            _rootFile2 = CSharpSyntaxTree
                .ParseText(_outputFileInfos[1].FileContent)
                .GetRoot();
        }

        [Test]
        public void CreateTests_NumberOfOutputFiles_TwoFiles()
        {
            Assert.AreEqual(2, _outputFileInfos.Count, "Wrong number of output files.");
        }

        [Test]
        public void CreateTests_NameOfOutputFiles_ClassNameTests()
        {
            Assert.AreEqual("Class1Tests", _outputFileInfos[0].Filename, "Wrong name of the first output file.");
            Assert.AreEqual("Class2Tests", _outputFileInfos[1].Filename, "Wrong name of the second output file.");
        }

        [Test]
        public void CreateTests_NamespaceName_NamespaceNameTests()
        {
            IEnumerable<NamespaceDeclarationSyntax> namespaciesClass1 = _rootFile1
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>();

            string namespaceName1 = namespaciesClass1.First().Name.ToString();

            IEnumerable<NamespaceDeclarationSyntax> namespaciesClass2 = _rootFile2
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>();

            string namespaceName2 = namespaciesClass2.First().Name.ToString();

            Assert.AreEqual("TestClasses.Tests", namespaceName1, "Invalid namespace name in first output file.");
            Assert.AreEqual("TestClasses.Tests", namespaceName2, "Invalid namespace name in second output file.");

        }

        [Test]
        public void CreateTests_ClassName_ClassNameTests()
        {

            IEnumerable<ClassDeclarationSyntax> class1 = _rootFile1
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>();

            string className1 = class1.First().Identifier.Text;

            IEnumerable<ClassDeclarationSyntax> class2 = _rootFile2
                 .DescendantNodes()
                 .OfType<ClassDeclarationSyntax>();

            string className2 = class2.First().Identifier.Text;

            Assert.AreEqual("Class1Tests", className1, "Invalid class name in first output file.");
            Assert.AreEqual("Class2Tests", className2, "Invalid class name in second output file.");
        }

        [Test]
        public void CreateTests_MethodName_MethodNameTest()
        {
            IEnumerable<MethodDeclarationSyntax> methodsClass = _rootFile1
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();
            
            Assert.AreEqual("Method1Test", methodsClass.ElementAt(1).Identifier.Text, "Invalid fisrt method name in first output file.");
            Assert.AreEqual("Method2Test", methodsClass.ElementAt(2).Identifier.Text, "Invalid second method name in fisrt output file.");
        }

        [Test]
        public void CreateTests_DuplicateMethodName_MethodName_NumberTest()
        {
            IEnumerable<MethodDeclarationSyntax> methodsClass = _rootFile2
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            Assert.AreEqual("Method_1Test", methodsClass.ElementAt(1).Identifier.Text, "Invalid fisrt method name in second output file.");
            Assert.AreEqual("Method_2Test", methodsClass.ElementAt(2).Identifier.Text, "Invalid second method name in second output file.");
        }
    }
}
