namespace TestsGeneratorLibrary
{
    public class ClassInfo
    {
        public string Name { get; set; }
        public string NamespaceName { get; set; }
        public List<string> MethodNames { get; set; }

        public ClassInfo() 
        {
            Name = "";
            NamespaceName = "";
            MethodNames = new List<string>();
        }

        public ClassInfo(string name, string namespaceName, List<string> methodNames)
        {
            Name = name;
            NamespaceName = namespaceName;
            MethodNames = methodNames;
        }
    }
}
