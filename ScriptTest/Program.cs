using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CodeTransfer;
using Pytocs.Core;
using Pytocs.Gui;
using ScriptTest;
using SyntacticSugar; // 用于访问语法树


class Program1
{
    static private Logger _logger = new Logger("python");
    static void Main(string[] args)
    {
        long longValue = 0x3FF0000000000000; // IEEE 754 二进制表示的 1.0
        double doubleValue = BitConverter.Int64BitsToDouble(longValue);

        Console.WriteLine($"Long Value: {longValue}");
        Console.WriteLine($"Interpreted Double: {doubleValue}");


        double floatValue = longValue;

        Console.WriteLine($"Long Value: {longValue}");
        Console.WriteLine($"Converted Float: {floatValue}");

        // tuple.Test();
        list.Test();
        // set.Test();
        dict.Test();
        pyint.TEST();
        // Pyurl.TEST();
        Task.Factory.StartNew((async () =>
        {
           await TestPython3();
        }));
        
        TestTimer.Reload(TestTimer.code);
        int i = 1;
        while (true)
        {
            i++;
            TestTimer.Run();
            if (i == 10000)
            {
                TestTimer.Reload(TestTimer.code2);
            }
        }
    }

    static async Task TestPython3()
    {
       await ConversionUtils.ConvertFolderAsync("py", "cs", _logger);
    }
    public void TestPython()
    {
        
        // 1. 指定待解析的 Python 脚本路径
        string scriptPath = "py\\selector_events.py"; // 替换为你的 Python 脚本路径

        // 2. 读取脚本内容
        string script = File.ReadAllText(scriptPath);

        // 3. 创建输入流
        AntlrInputStream inputStream = new AntlrInputStream(script);

        // 4. 创建词法分析器 (Lexer)
        var lexer = new Python3Lexer(inputStream);

        // 5. 创建词法标记流 (Token Stream)
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);

        // 6. 创建语法分析器 (Parser)
        var parser = new Python3Parser(tokenStream); // 创建一个自定义的错误监听器
        var errorListener = new ErrorListener();

// 为解析器添加错误监听器
        parser.RemoveErrorListeners(); // 移除默认的错误监听器
        parser.AddErrorListener(errorListener);
        // 7. 指定语法规则的入口点
        //    对 Python3Parser 来说，入口规则通常是 "file_input"（取决于你的 .g4 文件定义）
        IParseTree parseTree = parser.file_input();
// 如果有错误，打印错误信息
        if (errorListener.Errors.Any())
        {
            Console.WriteLine("Parsing errors:");
            foreach (var error in errorListener.Errors)
            {
                Console.WriteLine(error);
            }
        }
        else
        {
            Console.WriteLine("Parsing succeeded!");
        }

        var visitor = new PythonToCSharpConverter();
        var csharpCode = visitor.VisitFile_input((Python3Parser.File_inputContext)parseTree);
        // Print the C# code
        Console.WriteLine(
            "//=============================================C#=============================================");
        Console.WriteLine(csharpCode);
        //
        // var listener = new CustomPython3Listener();
        //
        // ParseTreeWalker.Default.Walk(listener, parseTree);
        // var converter = new PythonToCSharpConverter();
        // converter.Visit(parseTree);
        // Console.WriteLine(parseTree.ToStringTree(parser));
    }
}
