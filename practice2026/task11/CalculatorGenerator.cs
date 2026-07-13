using System;
using System.Reflection;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public class CalculatorGenerator
    {
        public static ICalculator Create()
        {
            string code = @"
                public class Calculator
                {
                    public int Add(int a,int b) => a + b;
                    public int Minus(int a, int b) => a - b;
                    public int Mul(int a, int b) => a * b;
                    public int Div(int a, int b) => a / b;
                }";
            string modifiedCode = code.Replace("public class Calculator", "public class Calculator : ICalculator");
            string finalCode = $@"
                namespace task11
                {{
                    {modifiedCode}
                }}";
            var syntaxTree = CSharpSyntaxTree.ParseText(finalCode);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a=> !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a=>MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>()
                .ToList();
            var compilation = CSharpCompilation.Create(
                assemblyName: $"DynamicCalculator",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            var type = assembly.GetType("task11.Calculator");
            if (type == null)
            {
                throw new InvalidOperationException("Класс Calculator не найден в скомпилированном коде");
            }
            var instance = Activator.CreateInstance(type);
            return (ICalculator)instance;
        }
    }
}
