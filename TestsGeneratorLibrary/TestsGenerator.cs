using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.AccessControl;
using System;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;

namespace TestsGeneratorLibrary
{
    public class TestsGenerator
    {
        private static List<UsingDirectiveSyntax> _usingsList = new List<UsingDirectiveSyntax>
        {
            UsingDirective(IdentifierName("System")),
            UsingDirective(QualifiedName(QualifiedName(IdentifierName("System"),IdentifierName("Collections")),
                            IdentifierName("Generic"))),
            UsingDirective(QualifiedName(IdentifierName("System"),IdentifierName("Linq"))),
            UsingDirective(QualifiedName(IdentifierName("System"),IdentifierName("Text"))),
            UsingDirective(QualifiedName(IdentifierName("NUnit"),IdentifierName("Framework"))),
            UsingDirective(IdentifierName("Moq"))
        };

        private SyntaxList<UsingDirectiveSyntax> _usingsSyntaxList = new SyntaxList<UsingDirectiveSyntax>(_usingsList);

        public List<OutputFileInfo> CreateTests(string sourseCode)
        {
            List<ClassInfo> classInfos = GetClassesInfo(sourseCode);

            List < OutputFileInfo > outputInfos = new List<OutputFileInfo>();
            foreach (ClassInfo classInfo in classInfos)
            {
                outputInfos.Add(new OutputFileInfo(classInfo.Name + "Tests", CreateTestClass(classInfo)));
            }

            return outputInfos;
        }

        private string CreateTestClass(ClassInfo classInfo)
        {
            List<FieldDeclarationSyntax> mockField;

            var setUpMethod = CreateSetUpMethod(classInfo, out mockField);

            ClassDeclarationSyntax classSyntax = ClassDeclaration(classInfo.Name + "Tests")
                .AddModifiers(Token(SyntaxKind.PublicKeyword))                
                .AddMembers(mockField.ToArray())
                .AddMembers(FieldDeclaration(VariableDeclaration(IdentifierName(classInfo.Name))
                            .WithVariables(SingletonSeparatedList(VariableDeclarator(
                                           Identifier(classInfo.Name + "Obj")))))
                            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
                        )
                .AddMembers(setUpMethod)
                .AddMembers(CreateTestMethods(classInfo).ToArray())
                .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestFixture"))))));


            return CompilationUnit()
                    .WithUsings(_usingsSyntaxList.Add(UsingDirective(IdentifierName(classInfo.NamespaceName))))
                    .AddMembers(NamespaceDeclaration(IdentifierName(classInfo.NamespaceName + ".Tests"))
                            .WithMembers(new SyntaxList<MemberDeclarationSyntax>(classSyntax)))
                    .NormalizeWhitespace().ToFullString();
        }

        private MethodDeclarationSyntax CreateSetUpMethod(ClassInfo classInfo, out List<FieldDeclarationSyntax> mockField)
        {            

            var constructors = classInfo.Constructors
                .OrderByDescending(ct => ct.ParameterList.Parameters.Count);

            ConstructorDeclarationSyntax interfaceConstructor = null;
            foreach (var constructor in constructors)
            {
                foreach (var arg in constructor.ParameterList.Parameters)
                {
                    if (arg.Type.ToString()[0] == 'I' && char.IsUpper(arg.Type.ToString()[1]))
                    {
                        interfaceConstructor = constructor;
                        break;
                    }
                }
            }

            mockField = new List<FieldDeclarationSyntax>();
            var mockInitialization = new List<ExpressionStatementSyntax>();
            var objArgsInitialization = new List<ArgumentSyntax>();

            if (interfaceConstructor != null)
            {
                var interfaceParameters = interfaceConstructor.ParameterList.Parameters;
                foreach (var param in interfaceParameters)
                {
                    if (param.Type.ToString()[0] == 'I' && char.IsUpper(param.Type.ToString()[1]))
                    {
                        mockField.Add(FieldDeclaration(VariableDeclaration(GenericName(Identifier("Mock"))
                            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(param.Type.ToString())))))
                             .WithVariables(SingletonSeparatedList(VariableDeclarator(
                                Identifier(param.Identifier.Text)))))
                            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword))));

                    }
                    else
                    {
                        mockField.Add(FieldDeclaration(VariableDeclaration(param.Type)
                                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(param.Identifier.Text)))))
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword))));
                    }
                }

                var initializationParameters = interfaceConstructor.ParameterList.Parameters;

                foreach (var param in initializationParameters)
                {
                    if (param.Type.ToString()[0] == 'I' && char.IsUpper(param.Type.ToString()[1]))
                    {
                        mockInitialization.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName(param.Identifier.Text), ObjectCreationExpression(
                                    GenericName(Identifier("Mock"))
                                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(
                                    IdentifierName(param.Type.ToString())))))
                                .WithArgumentList(ArgumentList()))));
                    }
                    else
                    {
                        mockInitialization.Add(ExpressionStatement(AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression, IdentifierName(param.Identifier.Text), 
                            LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)))));

                    }
                }

                var argParameters = interfaceConstructor.ParameterList.Parameters;

                foreach(var param in argParameters)
                {
                    if (param.Type.ToString()[0] == 'I' && char.IsUpper(param.Type.ToString()[1]))
                    {
                        objArgsInitialization.Add(Argument(MemberAccessExpression(
                               SyntaxKind.SimpleMemberAccessExpression,
                               IdentifierName(param.Identifier.Text),
                               IdentifierName("Object"))));
                    }
                    else
                    {
                        objArgsInitialization.Add(Argument(IdentifierName(param.Identifier.Text)));
                    }

                }

            }

           return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                        Identifier("SetUp"))
                    .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("OneTimeSetUp"))))))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithBody(Block(mockInitialization)
                    .AddStatements(ExpressionStatement(AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression, IdentifierName(classInfo.Name + "Obj"),
                        ObjectCreationExpression(IdentifierName(classInfo.Name))
                    .WithArgumentList(ArgumentList().AddArguments(objArgsInitialization.ToArray()))))));

        }

        private List<MemberDeclarationSyntax> CreateTestMethods(ClassInfo classInfo)
        {
            List<MemberDeclarationSyntax> testMethods = new List<MemberDeclarationSyntax>();
            
            var assertFail = ExpressionStatement(InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,IdentifierName("Assert"), IdentifierName("Fail")))
                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(
                    SyntaxKind.StringLiteralExpression, Literal("autogenerated")))))));      

            foreach (var method in classInfo.Methods)
            {
                List<LocalDeclarationStatementSyntax> varInitialization = null;                

                if (method.MethodSyntax.ParameterList.Parameters.Count > 0)
                {
                    varInitialization = method.MethodSyntax.ParameterList.Parameters.Select(param =>
                    {
                        return LocalDeclarationStatement(VariableDeclaration(param.Type)
                            .WithVariables( SingletonSeparatedList(VariableDeclarator(Identifier(param.Identifier.Text))
                            .WithInitializer(EqualsValueClause( LiteralExpression(
                                SyntaxKind.DefaultLiteralExpression,Token(SyntaxKind.DefaultKeyword)))))));
                    }).ToList();
                }  

                List<StatementSyntax> body = new List<StatementSyntax>();
                
                if (varInitialization != null)
                {
                    body.AddRange(varInitialization);
                }                    
                body.AddRange(CreateActAndAccertBodyPart(method, classInfo.Name));                
                body.Add(assertFail);

                MethodDeclarationSyntax testMethod = MethodDeclaration(PredefinedType(
                        Token(SyntaxKind.VoidKeyword)), method.TestMethodName + "Test")
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .WithAttributeLists(SingletonList(AttributeList(
                        SingletonSeparatedList(Attribute(IdentifierName("Test"))))))
                    .WithBody(Block(body));

                testMethods.Add(testMethod);
            }        

            return testMethods;
        }

        private List<StatementSyntax> CreateActAndAccertBodyPart(MethodInfo method, String className)
        {
            List<StatementSyntax> result = new List<StatementSyntax>();

            var callArguments = ArgumentList();
            StatementSyntax invokeMethod;
            LocalDeclarationStatementSyntax initializationExpected = null;
            ExpressionStatementSyntax assertInvoke = null;

            if (method.MethodSyntax.ParameterList.Parameters.Count > 0)
            {
                foreach (var p in method.MethodSyntax.ParameterList.Parameters)
                {
                    callArguments = callArguments.AddArguments(SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[] { Argument(IdentifierName(p.Identifier.Text)) }).ToArray());
                }
            }

            if (method.MethodSyntax.ReturnType.ToString().Equals("void"))
            {
                invokeMethod = ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                  IdentifierName(Identifier(className + "Obj")), IdentifierName(method.MethodSyntax.Identifier.Text)))
                .WithArgumentList(callArguments));

                result.Add(invokeMethod);
            }
            else
            {
                invokeMethod = LocalDeclarationStatement(VariableDeclaration(method.MethodSyntax.ReturnType)
                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("actual"))
                .WithInitializer(EqualsValueClause(InvocationExpression(MemberAccessExpression(
                   SyntaxKind.SimpleMemberAccessExpression, IdentifierName(Identifier(className + "Obj")),
                   IdentifierName(method.MethodSyntax.Identifier.Text)))
               .WithArgumentList(callArguments))))));

                initializationExpected = LocalDeclarationStatement(VariableDeclaration(method.MethodSyntax.ReturnType)
                            .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("expected"))
                            .WithInitializer(EqualsValueClause(LiteralExpression(
                                SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)))))));


                var arguments = ArgumentList();

                assertInvoke = ExpressionStatement(InvocationExpression(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Assert"), IdentifierName("AreEqual")))
                                .WithArgumentList(arguments.AddArguments(new List<ArgumentSyntax> {
                                        Argument(IdentifierName("actual")), Argument(IdentifierName("expected")) }.ToArray())));

                result.Add(invokeMethod);
                result.Add(initializationExpected);
                result.Add(assertInvoke);
            }

            return result;
        }

        private List<ClassInfo> GetClassesInfo(string code)
        {
            SyntaxNode root = CSharpSyntaxTree.ParseText(code).GetRoot();
            var classInfoList = new List<ClassInfo>();

            IEnumerable<NamespaceDeclarationSyntax> namespacies = root
            .DescendantNodes()
            .OfType<NamespaceDeclarationSyntax>();            

            foreach (var ns in namespacies)
            {
                string namespaceName = ns.Name.ToString();

                IEnumerable<ClassDeclarationSyntax> classes = ns
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)));

                foreach (var cl in classes)
                {
                    IEnumerable<MethodDeclarationSyntax> methods = cl
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)));

                    var methodInfos = methods.Select(method => new MethodInfo(method.Identifier.Text, method)).ToList(); 
                    
                    var duplicateMethods = methodInfos.GroupBy(x => x.TestMethodName)
                                            .Where(g => g.Count() > 1)
                                            .ToDictionary(x => x.Key, x => x.Count());

                    foreach (var duplicate in duplicateMethods)
                    {
                        for (int i = 1; i <= duplicate.Value; i++)
                        {
                            methodInfos[methodInfos.FindIndex(method => method.TestMethodName == duplicate.Key)].TestMethodName += "_" + i;
                        }
                    }

                    classInfoList.Add(new ClassInfo(cl.Identifier.Text, namespaceName, methodInfos, 
                        cl.DescendantNodes().OfType<ConstructorDeclarationSyntax>()));
                }
            }

            return classInfoList;
        }
    }
}