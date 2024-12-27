namespace ScriptTest;

public class CustomPython3Listener : Python3ParserBaseListener
{
    private Stack<int> indents = new Stack<int>();
    private int currentIndent = 0;

    public override void EnterIf_stmt(Python3Parser.If_stmtContext context)
    {
        base.EnterIf_stmt(context);
        this.startCompound();
    }

    public override void EnterBlock(Python3Parser.BlockContext context)
    {
        base.EnterBlock(context);
        // 处理缩进开始
        this.onIndent();
    }

    public override void ExitBlock(Python3Parser.BlockContext context)
    {
        base.ExitBlock(context);
        // 处理缩进结束
        this.onDedent();
    }

    private void onIndent()
    {
        // 实现缩进处理逻辑
        Console.WriteLine("Handling indent.");
    }

    private void onDedent()
    {
        // 实现取消缩进处理逻辑
        Console.WriteLine("Handling dedent.");
    }

    private void startCompound()
    {
        // 实现开始复合语句的逻辑
        Console.WriteLine("Starting compound statement.");
    }
}
