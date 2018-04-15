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
        var tree = CSharpSyntaxTree.ParseText(code, parseOptions);
        var compilation = sharedCompilation.Clone().AddSyntaxTrees(tree);
        var peStream = new MemoryStream();
        var xmlStream = new MemoryStream();
        var emitResult = compilation.Emit(peStream, xmlDocumentationStream: xmlStream);
        if (!emitResult.Success)
            throw new InvalidOperationException("Compilation failed: " + string.Join("\n", emitResult.Diagnostics));
        peStream.Seek(0, SeekOrigin.Begin);
        xmlStream.Seek(0, SeekOrigin.Begin);
        return (AssemblyDefinition.ReadAssembly(peStream), XDocument.Load(xmlStream));
    }

    private static readonly CSharpParseOptions parseOptions = new CSharpParseOptions()
        .WithKind(SourceCodeKind.Regular)
        .WithLanguageVersion(LanguageVersion.Latest);
    private static readonly CSharpCompilationOptions compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithOptimizationLevel(OptimizationLevel.Release)
        .WithAllowUnsafe(enabled: true);
    private static readonly CSharpCompilation sharedCompilation = CSharpCompilation.Create("TestInMemoryAssembly")
        .WithOptions(compileOptions)
        .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
}