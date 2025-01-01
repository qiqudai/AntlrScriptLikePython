using System.Runtime.InteropServices;

namespace SyntacticSugar;

public class PyList<T> : List<T>, IPyObject
{
    public PyList(): base()
    {
    }
    public PyList(int capacity): base(capacity)
    {
    }

    public PyList(IEnumerable<T> collection) : base(collection)
    {
    }

    public PyList(T[] collection) : base(collection)
    {
    }

    
    // 支持切片操作
    public PyList<T> this[int start, int end]
    {
        get
        {
            if (start < 0) start += Count; // 支持负索引
            if (end < 0) end += Count;
            if (start < 0 || end > Count || start > end) throw new ArgumentOutOfRangeException("Slice indices out of range");

            var sliced = new PyList<T>();
            for (int i = start; i < end; i++) sliced.Add(base[i]);
            return sliced;
        }
    }

    // 支持步长的切片访问器
    public PyList<T> this[int start, int? end, int step]
    {
        get
        {
            var result = new PyList<T>();
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
    
    // list.append(obj)
    public void append(T obj)
    {
        this.Add(obj);
    }
    public int count(T obj)
    {
        return this.Count;
    }

    // list.extend(seq)
    public void extend(IEnumerable<T> seq)
    {
        this.AddRange(seq);
    }

    // list.index(obj)
    public int index(T obj)
    {
        return this.IndexOf(obj);
    }

    // list.insert(index, obj)
    public void insert(int index, T obj)
    {
        this.Insert(index, obj);
    }

    // list.pop([index=-1])
    public T pop(int index = -1)
    {
        if (index == -1)
        {
            index = this.Count - 1;
        }
        T item = this[index];
        this.RemoveAt(index);
        return item;
    }

    // list.remove(obj)
    public void remove(T obj)
    {
        this.Remove(obj);
    }

    // list.reverse()
    public void reverse()
    {
        this.Reverse();
    }

    // list.sort(key=None, reverse=False)
    public void sort(bool reverse = false)
    {
        if (reverse)
        {
            this.Sort((a, b) => Comparer<T>.Default.Compare(b, a)); // Descending
        }
        else
        {
            this.Sort(); // Ascending
        }
    }

    // list.clear()
    public void clear()
    {
        this.Clear();
    }

    // list.copy()
    public PyList<T> copy()
    {
        return new PyList<T>(this);
    }
    
    public static void Test()
    {
        var li = new PyList<int> { 1, 2, 4, 3 };
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
        // Return a list of method names (in this case, the methods of PyList)
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
        if (value is not PyList<T> tuple)
            throw new InvalidOleVariantTypeException("NotImplemented");
        return this.Count >= tuple.Count;
    }

    public IPyObject __getattribute__(string name) {
        // Reflection can be used to get an attribute by name
        var property = this.GetType().GetProperty(name);
        return (IPyObject)property?.GetValue(this);
    }

    public bool __gt__(IPyObject value) {
        if (value is not PyList<T> tuple)
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

    public PyList<T> __iter__() {
        return this;
    }

    public bool __le__(PyList<T> value) {
        return this.Count <= value.Count;
    }

    public long __len__() {
        return this.Count;
    }

    public bool __lt__(PyList<T> value) {
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
        return new PyList<T>();
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
    
    // + 运算符：合并两个列表
    public static PyList<T> operator +(PyList<T> a, IList<T> b)
    {
        a.AddRange(b);
        return a;
    }
    // * 运算符：重复列表
    public static PyList<T> operator *(PyList<T> a, int times)
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
    
}