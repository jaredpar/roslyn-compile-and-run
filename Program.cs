using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


var count = 0;
var text = new StringBuilder();
while (true)
{
    Console.WriteLine("Enter C# code type 'end' to finish: ");
    text.Length = 0;
    while (Console.ReadLine() is var line && line != "end")
    {
        text.AppendLine(line);
    }

    var assemblyName = $"assembly{count++}";
    try
    {
        Console.Write("Compiling ... ");
        var stream = Compile(text.ToString(), assemblyName);
        Console.WriteLine("Runnning ... ");
        LoadAndRun(stream, assemblyName);
        Console.WriteLine();
    }
    catch (Exception e)
    {
        Console.WriteLine();
        Console.WriteLine(e.Message);
        Console.WriteLine();
    }
}

Stream Compile(string code, string assemblyName)
{
    var fullCode = $$"""
        using System;
        using System.Collections.Generic;

        class Lib
        {
            static void Go()
            {
                {{code}}
            }
        }
        """;

    var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
    var compilation = CSharpCompilation.Create(
        assemblyName,
        [CSharpSyntaxTree.ParseText(fullCode)],
        Net80.References.All,
        options);

    var peStream = new MemoryStream();
    var emitResult = compilation.Emit(peStream);
    if (!emitResult.Success)
    {
        throw new Exception(GetMessage(emitResult.Diagnostics));
    }

    peStream.Position = 0;
    return peStream;
}

void LoadAndRun(Stream stream, string assemblyName)
{
    var alc = new AssemblyLoadContext(assemblyName);
    var assembly = alc.LoadFromStream(stream);
    var libType = assembly
        .GetTypes()
        .Where(x => x.Name == "Lib")
        .Single();
    var method = libType.GetMethod("Go", BindingFlags.Static | BindingFlags.NonPublic);
    method!.Invoke(null, null);
}

string GetMessage(IEnumerable<Diagnostic> diagnostics)
{
    var builder = new StringBuilder();
    builder.AppendLine("Compilation failed with the following errors:");
    foreach (var d in diagnostics)
    {
        builder.AppendLine(d.ToString());
    }
    return builder.ToString();
}