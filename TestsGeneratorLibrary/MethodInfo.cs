using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLibrary
{
    public class MethodInfo
    {
        public string TestMethodName { get; set; }
        public MethodDeclarationSyntax MethodSyntax { get; set; }

        public MethodInfo(string testMethodName, MethodDeclarationSyntax methodSyntax)
        {
            TestMethodName = testMethodName;
            MethodSyntax = methodSyntax;
        }
    }
}
