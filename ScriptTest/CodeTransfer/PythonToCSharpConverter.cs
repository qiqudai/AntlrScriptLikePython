using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using Antlr4.Runtime.Tree;

namespace CodeTransfer
{

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

        private void AppendLineBody(string line, string body)
        {
            _csharpCode.AppendLine(new string(' ', _indentLevel * 4) + line);
            _csharpCode.AppendLine(new string(' ', _indentLevel * 4) + "{");
            Indent();
            _csharpCode.AppendLine(new string(' ', _indentLevel * 4) + body);
            Dedent();
            _csharpCode.AppendLine(new string(' ', _indentLevel * 4) + "}");
        }

        public override string VisitFile_input(Python3Parser.File_inputContext context)
        {    // Initialize a string to hold the full source code representation
            
            // Iterate through all the statements and newlines
            foreach (var child in context.children)
            {
                switch (child)
                {
                    // If the child is a statement (stmt), visit it
                    case Python3Parser.StmtContext stmtContext:
                        AppendLine(Visit(stmtContext)); // Visit the statement and add it to result
                        break;
                    case ITerminalNode terminalNode:
                    {
                        // 获取 token 类型
                        var tokenType = terminalNode.Symbol.Type;
            
                        // 如果是 NEWLINE（换行符）
                        if (tokenType == Python3Parser.NEWLINE)
                        {
                            AppendLine("");
                        }

                        break;
                    }
                }
            }
            // Return the complete result
            return _csharpCode.ToString();
        }

        public override string VisitFuncdef(Python3Parser.FuncdefContext context)
        {
            // 获取函数名
            var functionName = context.name().GetText();
            // 处理参数列表 (如果存在)
            var parameters = context.parameters() != null ? VisitParameters(context.parameters()) : "()";
            // 处理返回类型注解 (如果存在)
            var returnType = context.test() != null ? Visit(context.test()) : "void";
            // 获取函数体内容
            var functionBody = VisitBlock(context.block());
            AppendLineBody($"public {returnType} {functionName}{parameters}", functionBody);
            return "";
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
            return VisitChildren(context);
        }

        public override string VisitTestlist_star_expr(Python3Parser.Testlist_star_exprContext context)
        {
            return VisitChildren(context);
        }

        public override string VisitBlock(Python3Parser.BlockContext context)
        {
            return string.Join("\n", context.stmt().Select(Visit));
        }

        public override string VisitIf_stmt(Python3Parser.If_stmtContext context)
        {
            var result = new StringBuilder();

            // 获取 "if" 条件表达式
            var condition = Visit(context.test(0)); // 获取第一个 test（即 if 条件）

            // 获取对应的代码块
            var block = Visit(context.block(0)); // 获取第一个 block（即 if 语句体）

            // 生成 C# 代码的 if 部分
            result.AppendLine($"if ({condition})");
            result.AppendLine("{");
            result.AppendLine(block);
            result.AppendLine("}");

            // 遍历处理 "elif" 部分
            for (int i = 1; i < context.test().Count(); i++)  // 从第2个 test（即 elif 条件）开始遍历
            {
                var elifCondition = Visit(context.test(i));  // 获取 elif 条件
                var elifBlock = Visit(context.block(i));  // 获取 elif 语句体

                result.AppendLine($"else if ({elifCondition})");
                result.AppendLine("{");
                result.AppendLine(elifBlock);
                result.AppendLine("}");
            }

            // 处理 "else" 部分（如果存在）
            if (context.block().Count() > context.test().Count())
            {
                var elseBlock = Visit(context.block(context.test().Count()));  // 获取 else 语句体
                result.AppendLine("else");
                result.AppendLine("{");
                result.AppendLine(elseBlock);
                result.AppendLine("}");
            }

            return result.ToString();
        }

        public override string VisitWhile_stmt(Python3Parser.While_stmtContext context)
        {// 获取条件表达式 (test)
            var condition = Visit(context.test());

            // 获取 while 块 (block)
            var whileBlock = VisitBlock(context.block(0));

            // 检查是否有 else 子句
            string elseClause = "";
            if (context.block().Length > 1)
            {
                // 使用布尔标志来控制是否进入 else 子句
                string noBreakFlag = "noBreak";

                // 修改 while 块以设置布尔标志
                whileBlock = $"bool {noBreakFlag} = true;\n{whileBlock.Replace("break;", $"{noBreakFlag} = false; break;")}";

                // 构建 else 子句
                elseClause = $"if ({noBreakFlag}) {{ {VisitBlock(context.block(1))} }}";
            }

            // 构建完整的 while 语句
            var whileStatement = $@"
while ({condition})
{{
    {whileBlock}
}}
{elseClause}";

            return whileStatement;
        }

        public override string VisitReturn_stmt(Python3Parser.Return_stmtContext context)
        {
            var value = Visit(context.testlist());
            return $"return {value};";
        }

        public override string VisitAtom(Python3Parser.AtomContext context)
        {
            return context.GetText();
        }


        public override string VisitLambdef(Python3Parser.LambdefContext context)
        {
            var parameters = context.varargslist() != null ? VisitVarargslist(context.varargslist()) : "";
            // 提取表达式体
            var body = Visit(context.test());
            // 构建 C# lambda 表达式
            return $"{(string.IsNullOrEmpty(parameters) ? "" : $"({parameters})")} => {body}";
        }




        //AI添加2

        public virtual string VisitSingle_input([NotNull] Python3Parser.Single_inputContext context)
        {
            // You can implement this method if you need specific logic for single input statements.
            // As per the current pattern, returning VisitChildren seems appropriate for handling standard parsing.
            return  VisitChildren(context);
        }

        public virtual string VisitDecorator([NotNull] Python3Parser.DecoratorContext context)
        {
            // Decorators are often used in front of functions or methods. 
            // You could handle decorators here if needed.
            return VisitChildren(context);
        }

        public virtual string VisitDecorators([NotNull] Python3Parser.DecoratorsContext context)
        {
            // This handles multiple decorators. Typically, decorators are visited in a list.
            return VisitChildren(context);
        }

        public virtual string VisitDecorated([NotNull] Python3Parser.DecoratedContext context)
        {
            // A decorated function or method. This can be processed by visiting the decorated node itself.
            return VisitChildren(context);
        }

        public virtual string VisitTfpdef([NotNull] Python3Parser.TfpdefContext context)
        {
            // A target function parameter definition, usually specifying type annotations.
            // In many cases, it could be a simple parameter without much processing. 
            return VisitChildren(context);
        }

        public override string VisitVarargslist([NotNull] Python3Parser.VarargslistContext context)
        {
            var parameters = new List<string>();
            bool hasStarArgs = false;
            bool hasDoubleStarArgs = false;

            // 处理普通参数和带默认值的参数
            for (int i = 0; i < context.vfpdef().Length; i++)
            {
                var vfpdef = context.vfpdef(i);
                var defaultValue = context.test(i) != null ? $" = {Visit(context.test(i))}" : "";

                // 如果遇到了 * 或 ** 参数，则停止处理普通参数
                if (hasStarArgs || hasDoubleStarArgs)
                {
                    break;
                }

                parameters.Add($"{Visit(vfpdef)}{defaultValue}");
            }

            // 处理 *args 参数
            if (context.STAR() != null)
            {
                hasStarArgs = true;
                var starArg = context.vfpdef().FirstOrDefault(arg => arg.Parent is Python3Parser.Star_exprContext);
                if (starArg != null)
                {
                    parameters.Add($"params object[] {Visit(starArg)}");
                }
                else
                {
                    parameters.Add("params object[] args"); // 默认名称
                }

                // 继续处理 *args 后面的普通参数（如果有的话）
                for (int i = context.vfpdef().Count(); i < context.vfpdef().Length; i++)
                {
                    var vfpdef = context.vfpdef(i);
                    var defaultValue = context.test(i) != null ? $" = {Visit(context.test(i))}" : "";
                    parameters.Add($"{Visit(vfpdef)}{defaultValue}");
                }
            }

            // 处理 **kwargs 参数
            if (context.POWER() != null)
            {
                hasDoubleStarArgs = true;
                var doubleStarArg = context.vfpdef().LastOrDefault();
                if (doubleStarArg != null)
                {
                    parameters.Add($"Dictionary<string, object> {Visit(doubleStarArg)}");
                }
                else
                {
                    parameters.Add("Dictionary<string, object> kwargs"); // 默认名称
                }
            }

            return string.Join(", ", parameters);
        }

        public override string VisitVfpdef([NotNull] Python3Parser.VfpdefContext context)
        {
            // Process variable function parameter definition.
            return VisitChildren(context); // This is typically a simple node, so the default child visit works.
        }

        public override string VisitStmt([NotNull] Python3Parser.StmtContext context)
        {
            return VisitChildren(context);
        }

        public override string VisitSimple_stmts([NotNull] Python3Parser.Simple_stmtsContext context)
        {
            var statements = context.simple_stmt().Select(Visit).ToList();
            AppendLine(string.Join(";", statements));
            // 用分号连接每个简单语句
            return null;
        }
        
        public override string VisitSimple_stmt([NotNull] Python3Parser.Simple_stmtContext context)
        {
            // Handle a simple statement. Simple statements could be assignment, pass, or others.
            return VisitChildren(context);
        }

        public override string VisitAnnassign([NotNull] Python3Parser.AnnassignContext context)
        {
            var result = ": " + Visit(context.test(0));  // Visit the type annotation (first test).
            // Check if there is an assignment (i.e., an '=' sign).
            if (context.test().Length > 1)
            {
                result += " = " + Visit(context.test(1));  // Visit the assigned value (second test).
            }
            return result;
        }

        public override string VisitAugassign([NotNull] Python3Parser.AugassignContext context)
        {
            return null;
        }

        public override string VisitDel_stmt([NotNull] Python3Parser.Del_stmtContext context)
        {
            // Handle delete statement.
            var target = Visit(context.exprlist());
            return $"delete {target};";
        }

        public override string VisitPass_stmt([NotNull] Python3Parser.Pass_stmtContext context)
        {
            // Handle pass statement (no-op).
            return ";";
        }

        public override string VisitFlow_stmt([NotNull] Python3Parser.Flow_stmtContext context)
        {
            return VisitChildren(context); // This can be extended based on the flow statement type.
        }

        public override string VisitBreak_stmt([NotNull] Python3Parser.Break_stmtContext context)
        {
            // Handle break statement.
            return "break;";
        }

        public override string VisitContinue_stmt([NotNull] Python3Parser.Continue_stmtContext context)
        {
            // Handle the continue statement.
            return "continue;";
        }

        public override string VisitYield_stmt([NotNull] Python3Parser.Yield_stmtContext context)
        {
            // Handle the yield statement.
            var value = Visit(context.GetChild(0));
            return $"yield {value};";
        }

        public override string VisitRaise_stmt([NotNull] Python3Parser.Raise_stmtContext context)
        {
            // Handle the raise statement.
            var exception = context.test() != null ? Visit(context.FROM()) : string.Empty;
            return $"raise {exception};";
        }

        public override string VisitImport_stmt([NotNull] Python3Parser.Import_stmtContext context)
        {
            // Handle import statement.
            return VisitChildren(context); // You can further customize it for specific import handling.
        }

        public override string VisitImport_name([NotNull] Python3Parser.Import_nameContext context)
        {
            // Handle the import name.
            var name = Visit(context.dotted_as_names());
            return $"import {name};";
        }

        public override string VisitImport_from([NotNull] Python3Parser.Import_fromContext context)
        {
            // Handle the 'from ... import ...' statement.
            var module = Visit(context.dotted_name());
            var names = Visit(context.import_as_names());
            return $"from {module} import {names};";
        }

        public override string VisitImport_as_name([NotNull] Python3Parser.Import_as_nameContext context)
        {
            // Handle the 'import ... as ...' part of an import statement.
            var name = context.name().GetValue(0).ToString();
            var alias = context.name(1).GetText();
            return $"{name} as {alias}";
        }

        public override string VisitDotted_as_name([NotNull] Python3Parser.Dotted_as_nameContext context)
        {
            // 获取点分名称 (dotted_name)
            var dottedName = VisitDotted_name(context.dotted_name());

            // 检查是否有 'as' 子句
            if (context.name() == null) return dottedName;
            // 如果有 'as' 子句，则返回 using 别名指令
            var alias = Visit(context.name());
            return $"using {alias} = {dottedName};";

            // 如果没有 'as' 子句，则直接返回点分名称
        }

        public override string VisitImport_as_names([NotNull] Python3Parser.Import_as_namesContext context)
        {
            // Handle the import as names list.
            var result = new StringBuilder();
            foreach (var name in context.import_as_name())
            {
                result.Append(Visit(name) + ", ");
            }

            return result.ToString().TrimEnd(',', ' ');
        }

        public override string VisitDotted_as_names([NotNull] Python3Parser.Dotted_as_namesContext context)
        {
            // Handle multiple dotted names with aliases.
            var result = new StringBuilder();
            foreach (var name in context.dotted_as_name())
            {
                result.Append(Visit(name) + ", ");
            }

            return result.ToString().TrimEnd(',', ' ');
        }

        public override string VisitDotted_name([NotNull] Python3Parser.Dotted_nameContext context)
        {
            // Handle dotted names (e.g., module.submodule).
            return string.Join(".", context.name().Select(n => n.GetText()));
        }

        public override string VisitAssert_stmt([NotNull] Python3Parser.Assert_stmtContext context)
        {
            // 获取第一个 test 上下文（必须存在）
            var condition = Visit(context.test(0));

            // 检查是否有第二个 test（可选的错误消息）
            string errorMessage = null;
            if (context.test().Length > 1)
            {
                errorMessage = Visit(context.test(1));
            }

            // 构建 C# 的断言语句

            var assertStatement =
                // 如果有错误消息，则传递给 Assert 方法
                !string.IsNullOrEmpty(errorMessage) ? $"System.Diagnostics.Debug.Assert({condition}, {errorMessage});" :
                // 如果没有错误消息，只传递条件
                $"System.Diagnostics.Debug.Assert({condition});";
            return assertStatement;
        }

        public override string VisitCompound_stmt([NotNull] Python3Parser.Compound_stmtContext context)
        {
            // 如果是 if 语句
            if (context.if_stmt() != null)
            {
                return VisitIf_stmt(context.if_stmt());
            }
            // 如果是 while 语句
            if (context.while_stmt() != null)
            {
                return VisitWhile_stmt(context.while_stmt());
            }
            // 如果是 for 语句
            if (context.for_stmt() != null)
            {
                return VisitFor_stmt(context.for_stmt());
            }
            // 如果是 try 语句
            if (context.try_stmt() != null)
            {
                return VisitTry_stmt(context.try_stmt());
            }
            // 如果是 with 语句
            if (context.with_stmt() != null)
            {
                return VisitWith_stmt(context.with_stmt());
            }
            // 如果是 match 语句
            if (context.match_stmt() != null)
            {
                return VisitMatch_stmt(context.match_stmt());
            }
            // 如果是函数定义
            if (context.funcdef() != null)
            {
                return VisitFuncdef(context.funcdef());
            }
            // 如果是类定义
            if (context.classdef() != null)
            {
                return VisitClassdef(context.classdef());
            }
            // 如果是装饰器
            if (context.decorated() != null)
            {
                return VisitDecorated(context.decorated());
            }
            // 如果是异步语句
            if (context.async_stmt() != null)
            {
                return VisitAsync_stmt(context.async_stmt());
            }
    
            // 如果没有匹配到任何语句，返回空字符串或其他默认值
            return string.Empty;
        }

        public override string VisitAsync_stmt([NotNull] Python3Parser.Async_stmtContext context)
        {
            // Handle the async statement. If there is any specific syntax for `async`, you can add it here.
            return "async " + VisitChildren(context); // You can customize the visit for async statements as needed.
        }

        public override string VisitFor_stmt([NotNull] Python3Parser.For_stmtContext context)
        {
            // 获取循环变量 (exprlist) 和可迭代对象 (testlist)
            var targets = VisitExprlist(context.exprlist());
            var iterable = VisitTestlist(context.testlist());

            // 获取循环体 (block)
            var body = VisitBlock(context.block(0));

            // 使用临时布尔标志来模拟 Python 的 else 子句行为
            string loopFlag = "loopExecuted";

            // 检查是否有 else 子句
            string elseClause = "";
            if (context.block().Length > 1)
            {
                elseClause = $"if (!{loopFlag}) {{ {VisitBlock(context.block(1))} }}";
            }

            // 构建 C# 的 foreach 循环（假设 exprlist 只有一个目标）
            string forStatement;

            // 如果 exprlist 包含多个目标，则需要特殊处理
            if (context.exprlist().ChildCount > 1)
            {
                // 对于多个目标，可以考虑使用解构赋值（C# 7.0+）
                // 这里简单地使用 var 来声明每个变量
                var targetVars = string.Join(", ", context.exprlist().children.Select(t => "PyVariable"));
                forStatement = $@"
bool {loopFlag} = false;
foreach (({targetVars}) in ({iterable}))
{{
    {loopFlag} = true;
    {body}
}}
{elseClause}";
            }
            else
            {
                // 单个目标的 foreach 循环
                forStatement = $@"
bool {loopFlag} = false;
foreach (var {targets} in ({iterable}))
{{
    {loopFlag} = true;
    {body}
}}
{elseClause}";
            }

            return forStatement;
        }

        public override string VisitTry_stmt([NotNull] Python3Parser.Try_stmtContext context)
        {
            // 获取 try 块
            var tryBlock = VisitBlock(context.block(0));

            // 初始化 catch 和 finally 块
            var catchBlocks = new List<string>();
            string elseBlock = "";
            string finallyBlock = "";

            // 使用布尔标志来控制是否进入 else 子句
            string noExceptionFlag = "noException";

            // 检查是否有 except 子句
            if (context.except_clause().Length > 0)
            {
                for (int i = 0; i < context.except_clause().Length; i++)
                {
                    var exceptClause = context.except_clause(i);
                    var catchBlock = VisitBlock(context.block(i + 1)); // except 子句后的块
                    catchBlocks.Add(VisitExcept_clause(exceptClause));
                }

                // 检查是否有 else 子句
                if (context.block().Length > context.except_clause().Length + 1)
                {
                    elseBlock = $"if ({noExceptionFlag}) {{ {VisitBlock(context.block(context.except_clause().Length + 1))} }}";
                }

                // 检查是否有 finally 子句
                if (context.FINALLY() != null && context.block().Length > context.except_clause().Length + (context.block().Length > context.except_clause().Length + 1 ? 2 : 1))
                {
                    finallyBlock = $"finally {{ {VisitBlock(context.block(context.block().Length - 1))} }}";
                }
            }
            else if (context.FINALLY() != null)
            {
                // 如果只有 finally 子句
                finallyBlock = $"finally {{ {VisitBlock(context.block(1))} }}";
            }

            // 构建完整的 try-catch-finally 语句
            var tryStatement = $@"
bool {noExceptionFlag} = true;
try 
{{ 
    {tryBlock}
}}
{string.Join("\n", catchBlocks)}
{finallyBlock}
{elseBlock}";

            return tryStatement;
        }

        public override string VisitWith_stmt([NotNull] Python3Parser.With_stmtContext context)
        {
            // 获取所有 with_item
            var withItems = context.with_item();

            // 构建多个 using 语句或嵌套 using 块
            var usings = new List<string>();
            foreach (var item in withItems)
            {
                usings.Add(VisitWith_item(item));
            }

            // 获取 block 内容
            var blockContent = VisitBlock(context.block());

            // 构建完整的 using 语句
            var usingStatement = string.Join("\n", usings.Select(u => $"using ({u})"));
            usingStatement += $"\n{{\n{blockContent}\n}}";

            return usingStatement;
        }

        public override string VisitWith_item([NotNull] Python3Parser.With_itemContext context)
        {
            // 处理 with_item
            var test = Visit(context.test());

            // 检查是否有 'as' 子句
            if (context.expr() != null)
            {
                var asExpr = Visit(context.expr());
                return $"{test} as {asExpr}";
            }
            else
            {
                // 如果没有 'as' 子句，则直接使用表达式
                return test;
            }
        }

        public override string VisitExcept_clause([NotNull] Python3Parser.Except_clauseContext context)
        {
            // 如果存在 test (即指定的异常类型)，则处理它
            if (context.test() != null)
            {
                // 获取异常类型名称
                var exceptionType = VisitTest(context.test());

                // 如果存在 'as' name，则获取异常变量名称
                if (context.name() != null)
                {
                    var exceptionVariable = context.name().GetText();
                    return $"catch ({exceptionType} {exceptionVariable})";
                }
                else
                {
                    // 如果没有 'as' name，则只指定异常类型
                    return $"catch ({exceptionType})";
                }
            }
            else
            {
                // 如果没有指定异常类型，则是默认的 except 子句
                return "catch"; // 或者根据上下文决定是否提供更具体的 catch 块
            }
        }

        public override string VisitMatch_stmt([NotNull] Python3Parser.Match_stmtContext context)
        {
            // 1. 获取 MATCH 关键字并处理
            var result = "match ";
            // 2. 访问 subject_expr（匹配的表达式）
            result += Visit(context.subject_expr());
            // 3. 处理冒号和换行
            result += ":";
            // 4. 处理代码块和缩进
            result += "\n"; // 添加换行符
            // INDENT 表示缩进，case_block 需要处理至少一个 case
            result += new string(' ', 4); // 假设使用4个空格进行缩进
            foreach (var caseBlock in context.case_block())
            {
                result += Visit(caseBlock); // 访问每个 case_block
            }
    
            // 5. 处理 DEDENT（去缩进）
            result += "\n"; // 结束时换行

            return result;
        }

        public override string VisitSubject_expr([NotNull] Python3Parser.Subject_exprContext context)
        {
            var result = string.Empty;

            // 检查 subject_expr 是否匹配第一个模式（有逗号的多个 star_named_expressions）
            if (context.star_named_expressions() != null)
            {
                result += Visit(context.star_named_expressions()); // 访问第一个 star_named_expression
        
            }
            else if (context.test() != null)
            {
                // 否则，直接访问 test 表达式
                result += Visit(context.test());
            }

            return result;
        }

        public override string VisitStar_named_expressions(
            [NotNull] Python3Parser.Star_named_expressionsContext context)
        {
            // 使用 LINQ 来将每个 star_named_expression 转换为字符串，并用逗号分隔
            var result = string.Join(", ", context.star_named_expression().Select(Visit));
            return result;
        }

        public override string VisitStar_named_expression([NotNull] Python3Parser.Star_named_expressionContext context)
        {
            // 如果是解包表达式
            if (context.expr() != null)
            {
                var exprText = VisitExpr(context.expr());

                // 对于解包表达式的处理逻辑
                // 注意: C# 没有直接的解包操作符，需要根据上下文进行特殊处理
                return $"Unpack({exprText});";
            }
            if (context.test() != null)
            {
                // 处理测试表达式
                return VisitTest(context.test());
            }

            throw new NotImplementedException("Unsupported star_named_expression type.");
        }

        public override string VisitCase_block([NotNull] Python3Parser.Case_blockContext context)
        {
            // 访问 patterns 部分
            var patterns = VisitPatterns(context.patterns());
            // 如果存在 guard，则访问 guard 部分
            var guardClause = context.guard() != null ? $" when ({VisitGuard(context.guard())})" : "";
            // 获取 block 内容
            var blockContent = VisitBlock(context.block());
            // 构建 C# 格式的 case_block 表达式
            return $"case {patterns}{guardClause}:\n{blockContent}";
        }

        public override string VisitGuard([NotNull] Python3Parser.GuardContext context)
        {
            var condition = VisitTest(context.test());
            // 构建 C# 格式的 guard 表达式
            return $"when ({condition})";
        }

        public override string VisitPatterns([NotNull] Python3Parser.PatternsContext context)
        {
            return VisitChildren(context);
        }

        public override string VisitPattern([NotNull] Python3Parser.PatternContext context)
        {
            return VisitChildren(context);
        }

        public override string VisitAs_pattern([NotNull] Python3Parser.As_patternContext context)
        {
            // 访问 or_pattern 部分
            var orPattern = VisitOr_pattern(context.or_pattern());

            // 访问 pattern_capture_target 部分
            var captureTarget = VisitPattern_capture_target(context.pattern_capture_target());

            // 构建 C# 格式的 as_pattern 表达式
            return $"{orPattern} when ({captureTarget})";
        }

        public override string VisitOr_pattern([NotNull] Python3Parser.Or_patternContext context)
        {
            // Handle 'or' pattern (e.g., matching either one of several patterns).
            // 获取所有 closed_pattern 并用 ' || ' 连接
            var patterns = context.closed_pattern().Select(VisitClosed_pattern);
            return string.Join(" || ", patterns);
        }

        public override string VisitClosed_pattern([NotNull] Python3Parser.Closed_patternContext context)
        {
            // Handle closed patterns (e.g., a specific value match).
            return Visit(context.capture_pattern()); // Closed patterns typically match specific values.
        }

        public override string VisitLiteral_pattern([NotNull] Python3Parser.Literal_patternContext context)
        {
            // Handle literal patterns (e.g., matching a number or string literal).
            return Visit(context.GetChild(0));
        }

        public override string VisitLiteral_expr([NotNull] Python3Parser.Literal_exprContext context)
        {
            // Handle literal expressions in patterns.
            return Visit(context.GetChild(0)); // Literal expressions are usually just simple tests.
        }

        public override string VisitComplex_number([NotNull] Python3Parser.Complex_numberContext context)
        {
            // Handle complex number literals.
            var realPart = Visit(context.imaginary_number()); // Real part of the complex number.
            var imaginaryPart = Visit(context.signed_real_number()); // Imaginary part of the complex number.
            return $"{realPart} + {imaginaryPart}i"; // Format as "realPart + imaginaryParti".
        }

        public override string VisitSigned_number([NotNull] Python3Parser.Signed_numberContext context)
        {
            // Handle signed numbers (e.g., -10, +5).
            var sign = context.MINUS() != null ? "-" : (context.MINUS() != null ? "+" : "");
            var number = Visit(context.NUMBER());
            return $"{sign}{number}";
        }

        public override string VisitSigned_real_number([NotNull] Python3Parser.Signed_real_numberContext context)
        {
            // Handle signed real numbers (e.g., -10.5, +3.14).
            var sign = context.MINUS() != null ? "-" : (context.MINUS() != null ? "+" : "");
            var realNumber = Visit(context.real_number());
            return $"{sign}{realNumber}";
        }

        public override string VisitReal_number([NotNull] Python3Parser.Real_numberContext context)
        {
            // Handle real number literals (e.g., 10.5, -3.14).
            return Visit(context.NUMBER()); // Real numbers are typically just number expressions.
        }

        public override string VisitImaginary_number([NotNull] Python3Parser.Imaginary_numberContext context)
        {
            // Handle imaginary number literals (e.g., 3j, -2j).
            var number = Visit(context.NUMBER());
            return $"{number}j"; // Imaginary numbers are suffixed with 'j'.
        }

        public override string VisitCapture_pattern([NotNull] Python3Parser.Capture_patternContext context)
        {
            // Handle capture patterns (e.g., when you want to capture the value in a match statement).
            return Visit(context.pattern_capture_target());
        }

        public override string VisitPattern_capture_target(
            [NotNull] Python3Parser.Pattern_capture_targetContext context)
        {
            // 获取目标名称，确保不是 '_'
            var targetName = context.name().GetText();
            if (targetName == "_")
            {
                throw new ArgumentException("Pattern capture target cannot be '_'.");
            }

            // 可能需要进一步处理 CannotBeDotLpEq 约束
            // 这里假设 CannotBeDotLpEq 是一个验证函数，具体实现取决于需求
            // this.CannotBeDotLpEq();

            return targetName;
        }

        public override string VisitWildcard_pattern([NotNull] Python3Parser.Wildcard_patternContext context)
        {
            // Handle wildcard patterns (e.g., `_` for any value).
            return "_"; // Wildcard is represented by the underscore in Python.
        }

        public override string VisitValue_pattern([NotNull] Python3Parser.Value_patternContext context)
        {
            // Handle value patterns (e.g., literal values to match).
            return Visit(context.GetToken(0, 0)); // Value patterns are literal expressions.
        }

        public override string VisitAttr([NotNull] Python3Parser.AttrContext context)
        {
            // Handle attribute access (e.g., `obj.attr`).
            var obj = Visit(context.name(0)); // Visit the object.
            var attr = Visit(context.DOT(1)); // Visit the attribute.
            return $"{obj}.{attr}"; // Combine object and attribute.
        }

        public override string VisitName_or_attr([NotNull] Python3Parser.Name_or_attrContext context)
        {
            // Handle name or attribute access (e.g., `name` or `obj.attr`).
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0)); // If it's a single name, just return the name.
            }
            else
            {
                var obj = Visit(context.attr()); // Visit the object part.
                var attr = Visit(context.name()); // Visit the attribute part.
                return $"{obj}.{attr}"; // Combine object and attribute.
            }
        }

        public override string VisitGroup_pattern([NotNull] Python3Parser.Group_patternContext context)
        {
            // Handle grouped patterns (e.g., patterns inside parentheses).
            var pattern = Visit(context.pattern());
            return $"({pattern})"; // Group the pattern inside parentheses.
        }

        public override string VisitSequence_pattern([NotNull] Python3Parser.Sequence_patternContext context)
        {
            // Handle sequence patterns (e.g., matching sequences of values).
            var patterns = new List<string>();
            foreach (var p in context.children)
            {
                patterns.Add(Visit(p)); // Visit each pattern in the sequence.
            }

            return $"[{string.Join(", ", patterns)}]"; // Join the patterns with commas and wrap in square brackets.
        }

        public override string VisitOpen_sequence_pattern([NotNull] Python3Parser.Open_sequence_patternContext context)
        {
            // Handle open-ended sequence patterns (e.g., `[a, b, *rest]`).
            var patterns = new List<string>();
            foreach (var p in context.children)
            {
                patterns.Add(Visit(p)); // Visit each fixed pattern in the sequence.
            }

            patterns.Add("*" + Visit(context.maybe_sequence_pattern())); // Handle the starred element (`*rest`).
            return $"[{string.Join(", ", patterns)}]"; // Join with commas and wrap in square brackets.
        }

        public override string VisitMaybe_sequence_pattern(
            [NotNull] Python3Parser.Maybe_sequence_patternContext context)
        {
            // Handle possibly empty sequence patterns (e.g., `[a, b, ...]`).
            var patterns = new List<string>();
            foreach (var p in context.children)
            {
                patterns.Add(Visit(p)); // Visit each pattern in the sequence.
            }

            return $"[{string.Join(", ", patterns)} ...]"; // Indicate the possibility of more elements.
        }

        public override string VisitMaybe_star_pattern([NotNull] Python3Parser.Maybe_star_patternContext context)
        {
            // Handle possibly starred sequence patterns (e.g., `[a, *rest, b]`).
            var patterns = new List<string>();
            foreach (var p in context.children)
            {
                patterns.Add(Visit(p)); // Visit each pattern in the sequence.
            }

            patterns.Add("*" + Visit(context.pattern())); // Handle the starred pattern part.
            return $"[{string.Join(", ", patterns)}]"; // Join with commas and wrap in square brackets.
        }

        public override string VisitStar_pattern([NotNull] Python3Parser.Star_patternContext context)
        {
            // Handle star patterns (e.g., `*rest` in sequence patterns).
            var pattern = Visit(context.children[0]); // Visit the pattern that follows the star (`*rest`).
            return $"*{pattern}"; // Return the pattern with the '*' symbol.
        }

        public override string VisitMapping_pattern([NotNull] Python3Parser.Mapping_patternContext context)
        {
            // Handle mapping patterns (e.g., matching dictionaries).
            var items = new List<string>();
            foreach (var item in context.children)
            {
                items.Add(Visit(item)); // Visit each item pattern in the mapping.
            }

            return $"{{{string.Join(", ", items)}}}"; // Join the items with commas and wrap in curly braces.
        }

        public override string VisitItems_pattern([NotNull] Python3Parser.Items_patternContext context)
        {
            // Handle individual items in mapping patterns (e.g., key-value pairs).
            var key = Visit(context.key_value_pattern(0)); // Visit the key of the item.
            var value = Visit(context.key_value_pattern(1)); // Visit the value of the item.
            return $"{key}: {value}"; // Format as "key: value".
        }

        public override string VisitKey_value_pattern([NotNull] Python3Parser.Key_value_patternContext context)
        {
            // Handle key-value pair patterns in mapping patterns (e.g., `key: value`).
            var key = Visit(context.COLON()); // Visit the key in the key-value pair.
            var value = Visit(context.pattern()); // Visit the value in the key-value pair.
            return $"{key}: {value}"; // Format as "key: value".
        }

        public override string VisitDouble_star_pattern([NotNull] Python3Parser.Double_star_patternContext context)
        {
            // Handle double-star patterns (e.g., `**rest` for capturing arbitrary keyword arguments).
            var pattern = Visit(context.pattern_capture_target()); // Visit the pattern following the double star.
            return $"**{pattern}"; // Add the double star to the pattern.
        }

        public override string VisitClass_pattern([NotNull] Python3Parser.Class_patternContext context)
        {
            // Handle class patterns in match statements (e.g., `class` in pattern matching).
            var className = Visit(context.positional_patterns()); // Visit the class pattern.
            return $"{className}"; // Return the class pattern (as a type or class).
        }

        public override string VisitPositional_patterns([NotNull] Python3Parser.Positional_patternsContext context)
        {
            // Handle positional patterns (e.g., matching positional arguments).
            var patterns = new List<string>();
            foreach (var pattern in context.pattern())
            {
                patterns.Add(Visit(pattern)); // Visit each positional pattern.
            }

            return $"({string.Join(", ", patterns)})"; // Join patterns with commas and wrap in parentheses.
        }

        public override string VisitKeyword_patterns([NotNull] Python3Parser.Keyword_patternsContext context)
        {
            // Handle keyword patterns (e.g., matching keyword arguments).
            var patterns = new List<string>();
            foreach (var keywordPattern in context.keyword_pattern())
            {
                patterns.Add(Visit(keywordPattern)); // Visit each keyword pattern.
            }

            return $"{{{string.Join(", ", patterns)}}}"; // Join keyword patterns with commas and wrap in curly braces.
        }

        public override string VisitKeyword_pattern([NotNull] Python3Parser.Keyword_patternContext context)
        {
            // Handle a single keyword pattern (e.g., `key=value` in matching).
            var key = Visit(context.name()); // Visit the key.
            var value = Visit(context.ASSIGN()); // Visit the value.
            return $"{key}={value}"; // Format as "key=value".
        }

        public override string VisitTest([NotNull] Python3Parser.TestContext context)
        {
            // 如果有三目运算符结构，则进行特殊处理
            if (context.or_test().Length > 1 && context.QUEST() != null && context.COLON() != null)
            {
                var condition = Visit(context.or_test(0));
                var trueValue = Visit(context.or_test(1));
                var falseValue = Visit(context.test());

                return $"{condition} ? {trueValue} : {falseValue}";
            }
            else if (context.lambdef() != null)
            {
                // 处理 lambda 表达式的情况
                return Visit(context.lambdef());
            }
            else
            {
                // 对于非三目运算符的情况，直接访问 or_test
                return Visit(context.or_test(0));
            }
        }

        public override string VisitTest_nocond([NotNull] Python3Parser.Test_nocondContext context)
        {
            // Handle test expressions that are unconditional (no conditional).
            return VisitChildren(context);
        }

        public override string VisitLambdef_nocond([NotNull] Python3Parser.Lambdef_nocondContext context)
        {
            // Handle lambda definitions without conditionals.
            var parameters = Visit(context.varargslist()); // Visit the parameters of the lambda function.
            var body = Visit(context.test_nocond()); // Visit the body of the lambda function.
            return $"({parameters}) => {body}"; // Format as a lambda function.
        }

        public override string VisitOr_test([NotNull] Python3Parser.Or_testContext context)
        {
            // Handle logical OR tests (e.g., `a or b`).
            var left = Visit(context.and_test(0)); // Visit the left side of the OR expression.
            for (int i = 1; i < context.and_test().Length; i++)
            {
                var right = Visit(context.and_test(i)); // Visit the right side of the OR expression.
                left = $"{left} || {right}"; // Combine them with the "or" operator.
            }

            return left; // Return the final OR expression.
        }

        public override string VisitAnd_test([NotNull] Python3Parser.And_testContext context)
        {
            // Handle logical AND tests (e.g., `a and b`).
            var left = Visit(context.not_test(0)); // Visit the left side of the AND expression.
            for (int i = 1; i < context.not_test().Length; i++)
            {
                var right = Visit(context.not_test(i)); // Visit the right side of the AND expression.
                left = $"{left} && {right}"; // Combine them with the "and" operator.
            }

            return left; // Return the final AND expression.
        }

        public override string VisitNot_test([NotNull] Python3Parser.Not_testContext context)
        {
            // Handle logical NOT tests (e.g., `not a`).
            if (context.NOT() != null)
            {
                var expr = Visit(context.not_test()); // Visit the expression after the "not".
                return $"!{expr}"; // Return the NOT expression.
            }

            return Visit(context.comparison()); // Return the comparison if no NOT operator.
        }

        public override string VisitComparison([NotNull] Python3Parser.ComparisonContext context)
        {
            // Start with the first expression in the comparison.
            var result = Visit(context.expr(0));

            // Process all subsequent comparison operators and expressions.
            for (int i = 1; i < context.expr().Length; i++)
            {
                var compOp = Visit(context.comp_op(i - 1)); // Get the comparison operator (e.g., '<', '==', 'is', etc.)
                var expr = Visit(context.expr(i)); // Get the expression on the right-hand side.

                // Append the operator and the next expression to the result.
                result += " " + compOp + " " + expr;
            }

            return result;
        }

        public override string VisitComp_op([NotNull] Python3Parser.Comp_opContext context)
        {
            // Handle the comparison operators (e.g., `==`, `<`, `>`, etc.).
            return context.GetText(); // Return the operator as a string.
        }

        public override string VisitStar_expr([NotNull] Python3Parser.Star_exprContext context)
        {
            return $"*{Visit(context.expr())}"; // Return the star expression.
        }

        public override string VisitExpr([NotNull] Python3Parser.ExprContext context)
        {
            // 处理原子表达式（如常量、变量等）
            if (context.atom_expr() != null)
            {
                return Visit(context.atom_expr());
            }

            // 处理 '**' 运算符（幂运算）
            if (context.expr().Length == 2 && context.GetChild(1).GetText() == "**")
            {
                var left = Visit(context.expr(0));
                var right = Visit(context.expr(1));
                return $"{left} ** {right}";
            }

            // 处理一元运算符（'+'、'-'、'~'）
            if (context.expr().Length == 1 && context.GetChild(0).GetText() == "+" ||
                context.GetChild(0).GetText() == "-" || context.GetChild(0).GetText() == "~")
            {
                var op = context.GetChild(0).GetText();
                var operand = Visit(context.expr(0));
                return $"{op}{operand}";
            }

            // 处理乘除法运算符（'*'、'@'、'/'、'%'）
            if (context.expr().Length == 2 && (context.GetChild(1).GetText() == "*" ||
                                               context.GetChild(1).GetText() == "@" ||
                                               context.GetChild(1).GetText() == "/" ||
                                               context.GetChild(1).GetText() == "%" ||
                                               context.GetChild(1).GetText() == "/."))
            {
                var left = Visit(context.expr(0));
                var op = context.GetChild(1).GetText();
                var right = Visit(context.expr(1));
                return $"{left} {op} {right}";
            }

            // 处理加减法运算符（'+'、'-'）
            if (context.expr().Length == 2 &&
                (context.GetChild(1).GetText() == "+" || context.GetChild(1).GetText() == "-"))
            {
                var left = Visit(context.expr(0));
                var op = context.GetChild(1).GetText();
                var right = Visit(context.expr(1));
                return $"{left} {op} {right}";
            }

            // 处理位移运算符（'<<'、'>>'）
            if (context.expr().Length == 2 &&
                (context.GetChild(1).GetText() == "<<" || context.GetChild(1).GetText() == ">>"))
            {
                var left = Visit(context.expr(0));
                var op = context.GetChild(1).GetText();
                var right = Visit(context.expr(1));
                return $"{left} {op} {right}";
            }

            // 处理位与运算符（'&'）
            if (context.expr().Length == 2 && context.GetChild(1).GetText() == "&")
            {
                var left = Visit(context.expr(0));
                var right = Visit(context.expr(1));
                return $"{left} & {right}";
            }

            // 处理位异或运算符（'^'）
            if (context.expr().Length == 2 && context.GetChild(1).GetText() == "^")
            {
                var left = Visit(context.expr(0));
                var right = Visit(context.expr(1));
                return $"{left} ^ {right}";
            }

            // 处理位或运算符（'|'）
            if (context.expr().Length == 2 && context.GetChild(1).GetText() == "|")
            {
                var left = Visit(context.expr(0));
                var right = Visit(context.expr(1));
                return $"{left} | {right}";
            }

            // 处理后缀自增（i++）
            if (context.INCREMENT() != null && context.expr().Length == 1)
            {
                var operand = Visit(context.expr(0));
                return $"{operand}++";
            }

            // 处理前缀自增（++i）
            if (context.INCREMENT() != null && context.expr().Length == 1 && context.GetChild(0).GetText() == "++")
            {
                var operand = Visit(context.expr(0));
                return $"++{operand}";
            }

            return string.Empty;
        }

        public override string VisitAtom_expr([NotNull] Python3Parser.Atom_exprContext context)
        {
            // Handle atom expressions (e.g., literals, variables, or grouped expressions).
            return Visit(context.atom()); // Visit the atom expression (which may be a literal or variable).
        }

        public override string VisitName([NotNull] Python3Parser.NameContext context)
        {
            // Handle variable names (e.g., `a`, `b`, `x`, etc.).
            return context.GetText(); // Return the name as a string.
        }

// 辅助方法：构建 LINQ 查询表达式
// 辅助方法：构建 LINQ 查询表达式
        private string BuildLinqQuery(IEnumerable<string> elements, Python3Parser.Comp_forContext compFor)
        {
            // 构建 LINQ 查询表达式逻辑
            // 注意：这取决于具体的应用场景，此处仅提供示例逻辑
            var fromClause = VisitComp_for(compFor);
            var elementExpressions = string.Join(", ", elements);

            // 假设我们总是返回一个列表推导式
            return $"from {fromClause} select new[] {{ {elementExpressions} }}";
        }

        public override string VisitTestlist_comp([NotNull] Python3Parser.Testlist_compContext context)
        {
            // 分别获取 test 和 star_expr 上下文
            var tests = context.test().Select(Visit).ToList();
            var starExprs = context.star_expr().Select(VisitStar_expr).ToList();

            // 检查是否有 comp_for 子句
            if (context.comp_for() != null)
            {
                // 如果有 comp_for 子句，则构建 LINQ 查询表达式
                var queryExpression = BuildLinqQuery(tests.Concat(starExprs), context.comp_for());
                return queryExpression;
            }

            // 如果没有 comp_for 子句，则构建逗号分隔的表达式列表
            var elements = tests.Concat(starExprs).ToList();
            return string.Join(", ", elements);
        }

        public override string VisitTrailer([NotNull] Python3Parser.TrailerContext context)
        {
            // Handle function calls, indexing, or attribute access
            if (context.OPEN_PAREN() != null)
            {
                // Handle function call: '(' arglist? ')'
                var args = context.arglist() != null ? Visit(context.arglist()) : string.Empty;
                return $"({args})";
            }
            else if (context.OPEN_BRACK() != null)
            {
                // Handle indexing: '[' subscriptlist ']'
                var subscripts = Visit(context.subscriptlist());
                return $"[{subscripts}]";
            }
            else if (context.DOT() != null)
            {
                // Handle attribute access: '.' name
                var name = Visit(context.name());
                return $".{name}";
            }

            throw new InvalidOperationException("Unsupported trailer type.");
        }

        public override string VisitSubscriptlist([NotNull] Python3Parser.SubscriptlistContext context)
        {
            // Handle the list of subscripts (e.g., for array indexing).
            return string.Join(", ", context.subscript_()
                .Select(Visit) // Visit each subscript.
                .ToList()); // Return them as a comma-separated string.
        }

        public override string VisitSubscript_([NotNull] Python3Parser.Subscript_Context context)
        {
            // 处理下标逻辑，这取决于具体的下标类型
            if (context.test() != null && context.sliceop() == null)
            {
                // 简单下标
                return VisitTest(context.test(0));
            }

            if (context.sliceop() == null) throw new InvalidOperationException("Unexpected subscript type.");
            // 切片操作
            var lowerBound = context.test(0) != null ? VisitTest(context.test(0)) : "";
            var upperBound = context.test(1) != null ? VisitTest(context.test(1)) : "";
            var step = context.test(2) != null ? VisitTest(context.test(2)) : "";

            return $"{lowerBound}..{upperBound}..{step}";

        }

        public override string VisitSliceop([NotNull] Python3Parser.SliceopContext context)
        {
            // Start with the slice separator ':'
            var slice = "..";
            // If there is a stop index, visit it.
            if (context.test() != null)
            {
                slice += Visit(context.test()); // Visit the stop index expression.
            }

            // Return the slice representation.
            return slice;
        }

        public override string VisitExprlist([NotNull] Python3Parser.ExprlistContext context)
        {
            // Handle a list of expressions (e.g., arguments in a function call).
            var expressions = context.expr()
                .Select(Visit) // Visit each expression.
                .ToList();
            return string.Join(", ", expressions); // Return them as a comma-separated string.
        }

        public override string VisitTestlist([NotNull] Python3Parser.TestlistContext context)
        {
            // Handle a list of tests (e.g., expressions or conditions).
            var tests = context.test()
                .Select(Visit) // Visit each test.
                .ToList();
            return string.Join(", ", tests); // Return them as a comma-separated string.
        }

        public override string VisitDictorsetmaker([NotNull] Python3Parser.DictorsetmakerContext context)
        {
            if (context.GetText().Contains(':') || context.GetText().Contains("**"))
            {
                var entries = new List<string>();
                if (context.comp_for() != null)
                {
                    entries.Add(Visit(context.comp_for()));
                }
                else
                {
                    foreach (var child in context.children)
                    {
                        if (child.GetText() == "," || child.GetText() == ":")
                        {
                            continue;
                        }

                        entries.Add(Visit(child));
                    }
                }

                return $"{{ {string.Join(", ", entries)} }}";
            }

            var elements = new List<string>();

            if (context.comp_for() != null)
            {
                // Process the comprehension.
                elements.Add(Visit(context.comp_for()));
            }
            else
            {
                // Process static elements or unpacked sets.
                foreach (var child in context.children)
                {
                    if (child.GetText() == ",")
                        continue;
                    elements.Add(Visit(child));
                }
            }

            // Return the formatted set entries.
            return $"{{ {string.Join(", ", elements)} }}";
        }

        public override string VisitClassdef([NotNull] Python3Parser.ClassdefContext context)
        {
            // Class name
            var className = Visit(context.name());

            // Argument list (optional)
            var baseClasses = context.arglist() != null 
                ? $"({Visit(context.arglist())})" 
                : string.Empty;

            // Block content
            var blockContent = Visit(context.block());

            // Combine into a full class definition
            return $"class {className}{baseClasses}:\n{blockContent}";
        }

        public override string VisitArglist([NotNull] Python3Parser.ArglistContext context)
        {
            // Handle the argument list (e.g., in function definitions or calls).
            var args = context.argument()
                .Select(Visit) // Visit each argument.
                .ToList();
            return string.Join(", ", args); // Return them as a comma-separated string.
        }

        public override string VisitArgument([NotNull] Python3Parser.ArgumentContext context)
        {
            if (context.comp_for() != null)
                // Handle a positional argument with a comprehension (e.g., `x for x in iterable`).
                return $"{Visit(context.test(0))} {Visit(context.comp_for())}";
            if (context.GetChild(1)?.GetText() == "=")
                // Handle keyword arguments (e.g., `name=value`).
                return $"{Visit(context.test(0))}={Visit(context.test(1))}";
            if (context.GetChild(0)?.GetText() == "**")
                // Handle dictionary unpacking (e.g., `**kwargs`).
                return $"**{Visit(context.test(0))}";
            return context.GetChild(0)?.GetText() == "*" ?
                // Handle iterable unpacking (e.g., `*args`).
                $"*{Visit(context.test(0))}" :
                // Handle a simple positional argument.
                Visit(context.test(0));
        }

        public override string VisitComp_iter([NotNull] Python3Parser.Comp_iterContext context)
        {
            // Handle comprehension iterators (e.g., `for` loops in list comprehensions).
            return VisitChildren(context); // Visit the `for` loop part of the comprehension.
        }

        public override string VisitComp_for([NotNull] Python3Parser.Comp_forContext context)
        {
            var result = new StringBuilder();

            // Handle 'ASYNC' if it exists
            if (context.ASYNC() != null)
            {
                result.Append("async ");
            }

            // Add the 'for' keyword
            result.Append("for ");

            // Visit the exprlist (the variables being iterated over)
            result.Append(Visit(context.exprlist()));

            // Add 'in'
            result.Append(" in ");

            // Visit the or_test (the iterable)
            result.Append(Visit(context.or_test()));

            // Handle the optional comp_iter (nested comprehension or further conditions)
            if (context.comp_iter() == null) return result.ToString();
            result.Append(" ");
            result.Append(Visit(context.comp_iter()));

            return result.ToString();
        }

        public override string VisitComp_if([NotNull] Python3Parser.Comp_ifContext context)
        {
            var result = new StringBuilder();

            // Add the 'if' keyword
            result.Append("if ");

            // Visit the condition (test_nocond)
            result.Append(Visit(context.test_nocond()));

            // Handle the optional comp_iter (nested comprehension or further conditions)
            if (context.comp_iter() == null) return result.ToString();
            result.Append(" ");
            result.Append(Visit(context.comp_iter()));

            return result.ToString();
        }

        public override string VisitEncoding_decl([NotNull] Python3Parser.Encoding_declContext context)
        {
            return "";
        }

        public override string VisitYield_expr([NotNull] Python3Parser.Yield_exprContext context)
        {
            // Handle yield expressions (e.g., `yield x`).
            return $"yield {Visit(context.YIELD())}"; // Return the yield expression.
        }

        public override string VisitYield_arg([NotNull] Python3Parser.Yield_argContext context)
        {
            // Handle yield argument (e.g., `yield from x`).
            return $"yield from {Visit(context.test())}"; // Return the "yield from" expression.
        }

        public override string VisitStrings([NotNull] Python3Parser.StringsContext context)
        {
            // Handle string literals (e.g., `"hello"`).
            return $"\"{context.GetText()}\""; // Return the string wrapped in quotes.
        }

    }
}