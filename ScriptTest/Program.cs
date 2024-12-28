using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ScriptTest;
using SyntacticSugar; // 用于访问语法树


class Program1
{
    static void Main(string[] args)
    {
        long longValue = 0x3FF0000000000000; // IEEE 754 二进制表示的 1.0
        double doubleValue = BitConverter.Int64BitsToDouble(longValue);

        Console.WriteLine($"Long Value: {longValue}");
        Console.WriteLine($"Interpreted Double: {doubleValue}");

        
        double floatValue = longValue;

        Console.WriteLine($"Long Value: {longValue}");
        Console.WriteLine($"Converted Float: {floatValue}");
        
        list.Test();
        // set.Test();
        dict.Test();
        pyint.TEST();
        // Pyurl.TEST();
        
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
        var parser = new Python3Parser(tokenStream);

        // 7. 指定语法规则的入口点
        //    对 Python3Parser 来说，入口规则通常是 "file_input"（取决于你的 .g4 文件定义）
        IParseTree parseTree = parser.file_input();

        // var listener = new CustomPython3Listener();
        
        // ParseTreeWalker.Default.Walk(listener, parseTree);
        // var converter = new PythonToCSharpConverter();
        // converter.Visit(parseTree);
        // 8. 打印解析树 (可选)
        Console.WriteLine(parseTree.ToStringTree(parser));

        
    }

    class PythonToCSharpConverter : Python3ParserBaseVisitor<string>
    {
        private StringBuilder _csharpCode = new StringBuilder();

        public string GetCSharpCode() => _csharpCode.ToString();

        private int _indentLevel = 0;

        private void Indent() => _indentLevel++;

        private void Dedent() => _indentLevel--;
        public string Translate()
        {
            return _csharpCode.ToString();
        }
        private void AppendLine(string line)
        {
            _csharpCode.AppendLine(new string(' ', _indentLevel * 4) + line);
        }
        
        
    public override string VisitFile_input(Python3Parser.File_inputContext context)
    {
        // foreach (var section in context.section())
        // {
        //     Visit(section);
        // }
        return null;
    }

    public override string VisitFuncdef(Python3Parser.FuncdefContext context)
    {
        var functionName = context.name().GetText();
        var parameters = Visit(context.parameters());
        AppendLine($"public void {functionName}({parameters})");
        AppendLine("{");
        Indent();
        // Visit(context.suite());
        Dedent();
        AppendLine("}");
        return null;
    }

    public override string VisitParameters(Python3Parser.ParametersContext context)
    {
        if (context.typedargslist() != null)
        {
            return Visit(context.typedargslist());
        }
        return string.Empty;
    }

    public override string VisitTypedargslist(Python3Parser.TypedargslistContext context)
    {
        var parameters = new List<string>();
        foreach (var tfpdef in context.tfpdef())
        {
            var parameterName = tfpdef.name().GetText();
            parameters.Add($"object {parameterName}");
        }
        return string.Join(", ", parameters);
    }

    public override string VisitExpr_stmt(Python3Parser.Expr_stmtContext context)
    {
        var left = Visit(context.testlist_star_expr(0));
        var right = Visit(context.testlist_star_expr(1));
        AppendLine($"{left} = {right};");
        return null;
    }

    public override string VisitTestlist_star_expr(Python3Parser.Testlist_star_exprContext context)
    {
        return context.GetText();
    }

    public override string VisitBlock(Python3Parser.BlockContext context)
    {
        foreach (var stmt in context.stmt())
        {
            Visit(stmt);
        }
        return null;
    }

    public override string VisitIf_stmt(Python3Parser.If_stmtContext context)
    {
        var condition = Visit(context.test(0));
        AppendLine($"if ({condition})");
        AppendLine("{");
        Indent();
        // Visit(context.suite(0));
        Dedent();
        AppendLine("}");

        for (int i = 1; i < context.test().Length; i++)
        {
            condition = Visit(context.test(i));
            AppendLine($"else if ({condition})");
            AppendLine("{");
            Indent();
            // Visit(context.suite(i));
            Dedent();
            AppendLine("}");
        }

        if (context.ELSE() != null)
        {
            AppendLine("else");
            AppendLine("{");
            Indent();
            // Visit(context.suite(context.suite().Length - 1));
            Dedent();
            AppendLine("}");
        }

        return null;
    }

    public override string VisitWhile_stmt(Python3Parser.While_stmtContext context)
    {
        var condition = Visit(context.test());
        AppendLine($"while ({condition})");
        AppendLine("{");
        Indent();
        // Visit(context.suite());
        Dedent();
        AppendLine("}");
        return null;
    }

    public override string VisitReturn_stmt(Python3Parser.Return_stmtContext context)
    {
        var value = Visit(context.testlist());
        AppendLine($"return {value};");
        return null;
    }

    public override string VisitAtom(Python3Parser.AtomContext context)
    {
        return context.GetText();
    }
        
        
    }


}
