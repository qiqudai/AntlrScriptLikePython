using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

public interface ITimer
{
    public void Run(int t);
}

public class DynamicClass: ITimer
{
    public void Run(int n)
    {
        Console.WriteLine("timer Run:" + (n * 2));
    }
}

public static class TestTimer
{
    static private bool reload = true;
    static private List<ITimer> Timers = new ();
    static TestTimer()
    {
    }

    // static public void AddTimer<T>(T timer) where T: ITimer, new()
    // {
    //     Timers.Add(new T());
    // }
    static public void AddTimer(ITimer timer)
    {
        Timers.Add(timer);
    }

    static public void Run()
    {
        foreach (var t in Timers)
        {
            t.Run(1);
        }
    }

    public static string code = @"
using System;

public class DynamicClass: ITimer
{
    public void Run(int n)
    {
        Console.WriteLine(""timer Run:"" + n);
    }
}
";
    public static string code2 = @"
using System;

public class DynamicClass: ITimer
{
    public void Run(int n)
    {
        Console.WriteLine(""timer Run:"" + (n * 2));
    }
}
";
    static ITimer CreateDynamicTimer(string code)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

        // Reference required assemblies
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        string assemblyName = Path.GetRandomFileName();
        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
            Console.WriteLine("Compilation failed:");
            foreach (var diagnostic in result.Diagnostics)
            {
                Console.WriteLine(diagnostic.ToString());
            }

            return null;
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var type = assembly.GetType("DynamicClass");

        return (ITimer)Activator.CreateInstance(type);
    }

    static public void Reload(string code)
    {
        ITimer newTimer = CreateDynamicTimer(code);
        if (Timers.Count == 0)
            AddTimer(newTimer);
    }
    
}