namespace SyntacticSugar;

public class Pylist<T> : Pytuple<T>
{
    // 重写 ToString 方法，支持打印
    public override string ToString()
    {
        return "[" + string.Join(", ", this) + "]";
    }
    
    
    public static void Test()
    {
        var li = new Pylist<int> { 1, 2, 4, 3 };
        //倒数
        var a = li[^1];
        //完整拷贝
        var b = li[..];
        // 相当于 Python 的 li[1:3]
        var slice1 = li[1..3]; // 返回 [2, 4]

        // 相当于 Python 的 li[2:]
        var slice2 = li[2..]; // 返回 [4, 3]

        // 相当于 Python 的 li[:3]
        var slice3 = li[..3]; // 返回 [1, 2, 4]

        // 相当于 Python 的 li[::2]
        var slice4 = li[0, null, 2]; // 返回 [1, 4]

        // 相当于 Python 的 li[::-1]
        var slice5 = li[li.Count - 1, -1, -1]; // 返回 [3, 4, 2, 1]

        Console.WriteLine($"Slice1: [{string.Join(", ", slice1)}]");
        Console.WriteLine($"Slice2: [{string.Join(", ", slice2)}]");
        Console.WriteLine($"Slice3: [{string.Join(", ", slice3)}]");
        Console.WriteLine($"Slice4: [{string.Join(", ", slice4)}]");
        Console.WriteLine($"Slice5: [{string.Join(", ", slice5)}]");
        Console.WriteLine($"Slice5: [{string.Join(", ", b)}]");
    }
}