# Compile, Load and Run

This is a quick sample application demonstrating how you can compile C# code on the fly, load it into an `AssemblyLoadContext` and execute it.

For example:

```cmd
> dotnet run
Enter C# code type 'end' to finish: 
Console.WriteLine("Hello World");
end
Compiling ... Runnning ... 
Hello World

Enter C# code type 'end' to finish:
Console.WriteLine(DateTime.Now);
end
Compiling ... Runnning ... 
8/22/2024 4:13:49 PM

Enter C# code type 'end' to finish:
```
