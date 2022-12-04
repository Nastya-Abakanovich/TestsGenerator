using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLibrary
{
    public class ClassInfo
    {
        public string Name { get; set; }
        public string NamespaceName { get; set; }
        public List<MethodInfo> Methods { get; set; }
        public IEnumerable<ConstructorDeclarationSyntax> Constructors { get; set; }

        public ClassInfo() 
        {
            Name = "";
            NamespaceName = "";
            Methods = new List<MethodInfo>();
            Constructors = new List<ConstructorDeclarationSyntax>();
        }

        public ClassInfo(string name, string namespaceName, List<MethodInfo> methods, IEnumerable<ConstructorDeclarationSyntax> constructors)
        {
            Name = name;
            NamespaceName = namespaceName;
            Methods = methods;
            Constructors = constructors;
        }
    }
}
