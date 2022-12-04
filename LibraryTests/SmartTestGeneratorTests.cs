using TestsGeneratorLibrary;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibraryTests
{
    public class SmartTestGeneratorTests
    {
        private List<OutputFileInfo> _outputFileInfos1;
        private List<OutputFileInfo> _outputFileInfos2;
        private SyntaxNode _rootFile1;
        private SyntaxNode _rootFile2;
        private SyntaxNode _rootFile3;
        private SyntaxNode _rootFile4;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            TestsGenerator testsGenerator = new TestsGenerator();
            FileReaderWriter fileReaderWriter = new FileReaderWriter();

            _outputFileInfos1 = testsGenerator.CreateTests(await fileReaderWriter.Read("..\\..\\..\\..\\Example\\TestClasses\\Class1.cs"));
            _outputFileInfos2 = testsGenerator.CreateTests(await fileReaderWriter.Read("..\\..\\..\\..\\Example\\TestClasses\\Class3.cs"));

            _rootFile1 = CSharpSyntaxTree
                .ParseText(_outputFileInfos1[0].FileContent)
                .GetRoot();

            _rootFile2 = CSharpSyntaxTree
                .ParseText(_outputFileInfos1[1].FileContent)
                .GetRoot();

            _rootFile3 = CSharpSyntaxTree
                .ParseText(_outputFileInfos2[0].FileContent)
                .GetRoot();

            _rootFile4 = CSharpSyntaxTree
                .ParseText(_outputFileInfos2[1].FileContent)
                .GetRoot();
        }

        [Test]
        public void CreateTests_NumberOfOutputFiles_FourFiles()
        {
            Assert.AreEqual(4, _outputFileInfos1.Count + _outputFileInfos2.Count, "Wrong number of output files.");
        }

        [Test]
        public void CreateTests_SetUpMethod_MethodExists()
        {
            IEnumerable<MethodDeclarationSyntax> methodsClass1 = _rootFile1
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            IEnumerable<MethodDeclarationSyntax> methodsClass2 = _rootFile2
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            IEnumerable<MethodDeclarationSyntax> methodsClass3 = _rootFile3
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            IEnumerable<MethodDeclarationSyntax> methodsClass4 = _rootFile4
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            Assert.AreEqual("SetUp", methodsClass1.First().Identifier.Text, "Invalid fisrt method name in 1st output file.");
            Assert.AreEqual("SetUp", methodsClass2.First().Identifier.Text, "Invalid fisrt method name in 2nd output file.");
            Assert.AreEqual("SetUp", methodsClass3.First().Identifier.Text, "Invalid fisrt method name in 3rd output file.");
            Assert.AreEqual("SetUp", methodsClass4.First().Identifier.Text, "Invalid fisrt method name in 4th output file.");

        }


        [Test]
        public void CreateTests_UsingMoq_UsingExists()
        {

            IEnumerable<UsingDirectiveSyntax> usings = _rootFile1
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>();

            bool isMoqUsing = false;

            foreach (var us in usings)
            {
                if (us.Name.ToString() == "Moq")
                    isMoqUsing = true;
            }

            Assert.IsTrue(isMoqUsing, "Using Moq missing.");
        }

        [Test]
        public void CreateTests_PrivateFields_FieldsExists()
        {
            IEnumerable<FieldDeclarationSyntax> fieldsClass3 = _rootFile3 
                .DescendantNodes()
                .OfType<FieldDeclarationSyntax>();

            Assert.AreEqual("private", fieldsClass3.ElementAt(0).Modifiers.ToString(), "Invalid modifier in 1st field in 3rd output file.");
            Assert.AreEqual("Mock<IInterface>", fieldsClass3.ElementAt(0).Declaration.Type.ToString(), "Invalid type in 1st field in 3rd output file.");
            Assert.AreEqual("_interface", fieldsClass3.ElementAt(0).Declaration.Variables.ElementAt(0).ToString(), "Invalid variable name in 1st field in 3rd output file.");

            Assert.AreEqual("private", fieldsClass3.ElementAt(1).Modifiers.ToString(), "Invalid modifier in 2nd field in 3rd output file.");
            Assert.AreEqual("Class3", fieldsClass3.ElementAt(1).Declaration.Type.ToString(), "Invalid type in 2nd field in 3rd output file.");
            Assert.AreEqual("Class3Obj", fieldsClass3.ElementAt(1).Declaration.Variables.ElementAt(0).ToString(), "Invalid variable name in 2nd field in 3rd output file.");
            
        }

    }
}
