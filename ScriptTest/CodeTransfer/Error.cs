using Antlr4.Runtime;

namespace CodeTransfer
{
    public class ErrorListener : BaseErrorListener
    {
        public List<string> Errors { get; } = new List<string>();

        public void SyntaxError(IRecognizer recognizer,
            IToken offendingSymbol,
            int line,
            int charPositionInLine,
            string msg,
            RecognitionException e)
        {
            // 收集错误信息
            Errors.Add($"Line {line}:{charPositionInLine} - {msg}");
        }
    }
}