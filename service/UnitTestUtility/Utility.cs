using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public static class Utility
{
    /// <summary>
    /// Compiles a C# file from a string into an assembly in-memory (with xmldocs) and then parses it with Cecil (and XDocument).
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static (AssemblyDefinition Dll, XDocument Xml) Compile(string code)
    {
        var parseOptions = new CSharpParseOptions()
            .WithKind(SourceCodeKind.Regular)
            .WithLanguageVersion(LanguageVersion.Latest);
        var tree = CSharpSyntaxTree.ParseText(code, parseOptions);
        var compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithOptimizationLevel(OptimizationLevel.Release);
        var compilation = CSharpCompilation.Create("TestInMemoryAssembly", options: compileOptions)
            .AddSyntaxTrees(tree)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        var peStream = new MemoryStream();
        var xmlStream = new MemoryStream();
        var emitResult = compilation.Emit(peStream, xmlDocumentationStream: xmlStream);
        if (!emitResult.Success)
            throw new InvalidOperationException("Compilation failed: " + string.Join("\n", emitResult.Diagnostics));
        peStream.Seek(0, SeekOrigin.Begin);
        xmlStream.Seek(0, SeekOrigin.Begin);
        return (AssemblyDefinition.ReadAssembly(peStream), XDocument.Load(xmlStream));
    }
}