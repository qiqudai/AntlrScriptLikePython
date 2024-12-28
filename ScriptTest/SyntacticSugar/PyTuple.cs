namespace SyntacticSugar;

public class Pytuple<T> : List<T>
{
    public Pytuple(): base()
    {
    }
    public Pytuple(int capacity): base(capacity)
    {
    }

    public Pytuple(IEnumerable<T> collection) : base(collection)
    {
    }

    // 支持切片操作
    public Pytuple<T> this[int start, int end]
    {
        get
        {
            if (start < 0) start += Count; // 支持负索引
            if (end < 0) end += Count;
            if (start < 0 || end > Count || start > end) throw new ArgumentOutOfRangeException("Slice indices out of range");

            var sliced = new Pytuple<T>();
            for (int i = start; i < end; i++) sliced.Add(base[i]);
            return sliced;
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
    
    // list.append(obj)
    public void append(T obj)
    {
        this.Add(obj);
    }

    // list.count(obj)
    public int count(T obj)
    {
        return this.FindAll(x => EqualityComparer<T>.Default.Equals(x, obj)).Count;
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
    public Pytuple<T> copy()
    {
        return new Pytuple<T>(this);
    }
    
    public static void Test()
    {
        var li = new Pytuple<int> { 1, 2, 4, 3 };
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