using System.Diagnostics;
using System.Runtime.InteropServices;
using FastMember;

namespace SyntacticSugar;

public class Pytuple<T> : List<T>, IPyObject
{
    public Pytuple()
    {
    }
    public Pytuple(Pytuple<T> t): base(t)
    {
        
    }

    public Pytuple(object tuple)
    {
        if (tuple is ValueTuple)
        {
        }
    }
    // 构造函数：支持任意值元组初始化
    public Pytuple(params T[] items)
    {
        if (items.Length > 0)
        {
            AddRange(items);
        }
    }
    
    public Pytuple(Pytuple<T> t, int start, int end)
    {
        if (start < 0 || end > t.Count || start > end)
        {
            throw new ArgumentOutOfRangeException("Invalid slice indices.");
        }
        AddRange(t.GetRange(start, end - start));
    }

    // 支持切片操作
    public Pytuple<T> this[int start, int end]
    {
        get
        {
            if (start < 0) start += Count; // 支持负索引
            if (end < 0) end += Count;
            if (end >= Count) end = Count - 1;
            if (start < 0 || end > Count || start > end) throw new ArgumentOutOfRangeException("Slice indices out of range");
            return new Pytuple<T>(this, start, end);
        }
    }

    // 支持步长的切片访问器
    public Pytuple<T> this[int start, int? end, int step]
    {
        get
        {
            var result = new Pytuple<T>();
            int count = this.Count;
            int actualEnd = end ?? count;

            if (step > 0)
            {
                for (int i = start; i < actualEnd && i < count; i += step)
                    result.Add(this[i]);
            }
            else if (step < 0)
            {
                for (int i = start; i > actualEnd && i >= 0; i += step)
                    result.Add(this[i]);
            }

            return result;
        }
    }
    // 重写 ToString 方法，支持打印
    public void extend(IList<T> a)
    {
        AddRange(a);
    }
    // + 运算符：合并两个列表
    public static Pytuple<T> operator +(Pytuple<T> a, IList<T> b)
    {
        a.AddRange(b);
        return a;
    }
    // * 运算符：重复列表
    public static Pytuple<T> operator *(Pytuple<T> a, int times)
    {
        for (int i = 0; i < times; i++)
        {
            a.AddRange(a);
        }
        return a;
    }
    // 重写 ToString 方法，支持打印
    public override string ToString()
    {
        return "[" + string.Join(", ", this) + "]";
    }
    
    // list.count(obj)
    public int count(T obj)
    {
        return this.FindAll(x => EqualityComparer<T>.Default.Equals(x, obj)).Count;
    }


    // list.index(obj)
    public int index(T obj)
    {
        return this.IndexOf(obj);
    }

    // list.copy()
    public Pytuple<T> copy()
    {
        return new Pytuple<T>(this);
    }
    
    // 方法示例（自动生成的方法）
    public void __add__(T value) {
        this.Add(value);
    }

    public bool __contains__(T key) {
        return this.Contains(key);
    }

    public void __delattr__(string name) {
        // C# does not have direct attribute deletion; you might need reflection for this
        Console.WriteLine("Method __delattr__ not directly applicable in C#.");
    }

    public void __delitem__(int key) {
        this.RemoveAt(key);
    }

    public List<string> __dir__() {
        // Return a list of method names (in this case, the methods of Pytuple)
        return new List<string> { "__add__", "__contains__", "__delattr__", "__delitem__", "__iter__", "__len__", "__str__", "__repr__", "__eq__" };
    }

    public bool __eq__(IPyObject value) {
        return this.Equals(value);
    }

    public long __hash__()
    {
        return this.GetHashCode();
    }

    public string __format__(string format_spec) {
        // Formatting strings are usually used for string representations
        return string.Join(", ", this);
    }

    public bool __ge__(IPyObject value)
    {
        if (value is not Pytuple<T> tuple)
            throw new InvalidOleVariantTypeException("NotImplemented");
        return this.Count >= tuple.Count;
    }

    public IPyObject __getattribute__(string name) {
        // Reflection can be used to get an attribute by name
        var property = this.GetType().GetProperty(name);
        return (IPyObject)property?.GetValue(this);
    }

    public bool __gt__(IPyObject value) {
        if (value is not Pytuple<T> tuple)
            throw new InvalidOleVariantTypeException("NotImplemented");
        return this.Count > tuple.Count;
    }

    public int __iadd__(T value) {
        this.Add(value);
        return this.Count;
    }

    public int __imul__(T value) {
        this.Add(value);
        return this.Count;
    }

    public void __init__(params PyVariable[] args) {
        // Constructor logic in Python typically
        Console.WriteLine("Initialization with args: " + string.Join(", ", args));
    }

    public Pytuple<T> __iter__() {
        return this;
    }

    public bool __le__(Pytuple<T> value) {
        return this.Count <= value.Count;
    }

    public long __len__() {
        return this.Count;
    }

    public bool __lt__(Pytuple<T> value) {
        return this.Count < value.Count;
    }

    public void __mul__(T value) {
        this.Add(value);
    }

    public bool __ne__(object value) {
        return !this.Equals(value);
    }

    public object __new__(params object[] args) {
        // C# does not have an equivalent __new__ function directly
        return new Pytuple<T>();
    }

    public void __reduce__() {
        Console.WriteLine("Method __reduce__ is not applicable in C#.");
    }

    public void __reduce_ex__(int protocol) {
        Console.WriteLine("Method __reduce_ex__ is not applicable in C#.");
    }

    public string __repr__() {
        return "[" + string.Join(", ", this) + "]";
    }

    public void __reversed__() {
        this.Reverse();
    }

    public void __rmul__(T value) {
        this.Add(value);
    }

    public void __setattr__(string name, object value) {
        // C# doesn't have direct equivalent for setting attributes dynamically
        Console.WriteLine($"Attribute {name} cannot be dynamically set in C#.");
    }

    public void __setitem__(int key, T value) {
        this[key] = value;
    }

    public int __sizeof__() {
        // Returning the size of the object in memory is not trivial in C#.
        return System.Runtime.InteropServices.Marshal.SizeOf(this);
    }

    public string __str__() {
        return string.Join(", ", this);
    }

    public void __subclasshook__(Type subclass) {
        Console.WriteLine("Method __subclasshook__ is not directly applicable in C#.");
    }
    
    
    public static void Test()
    {
        tuple li = new ((1, 2, 4, 3, 5, 6, 7, 8, 98, 9, 2, 213, 213, 213, 1, 2, 4, 3, 5, 6, 7, 8, 98, 9, 2, 213, 213, 213,
            1, 2, 4, 3, 5, 6, 7, 8, 98, 9, 2, 213, (1,23,4), 213));
        var t1 = (1, 2, 3);
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