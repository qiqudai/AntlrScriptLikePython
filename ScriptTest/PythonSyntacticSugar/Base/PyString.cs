using System.Runtime.InteropServices;
using System.Text;

namespace SyntacticSugar;

public class PyString: IPyObject
{
    // 创建一个 ThreadLocal<StringBuilder>，每个线程都有自己的 StringBuilder 副本
    public static ThreadLocal<StringBuilder> threadLocalStringBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());
    private string _value;
    // 构造函数
    public PyString(string value)
    {
        _value = value;
    }
    // 构造函数：从 char 创建
    public PyString(char value)
    {
        _value = value.ToString();
    }

    // 构造函数：从 byte[] 创建
    public PyString(byte[] value)
    {
        _value = Encoding.UTF8.GetString(value);
    }

    // 构造函数：从 long 创建
    public PyString(long value)
    {
        __init__(value);
    }

    // 构造函数：从 float 创建
    public PyString(float value)
    {
        __init__(value);
    }

    // 构造函数：从 double 创建
    public PyString(double value)
    {
        __init__(value);
    }
    
    // 构造函数：从 char 数组创建
    public PyString(char[] value)
    {
        _value = new string(value);
    }

    // 构造函数：通过指定重复次数创建字符串
    public PyString(int repeatCount, char value)
    {
        _value = new string(value, repeatCount);
    }
    // 构造函数：通过指定重复次数创建字符串
    public PyString(string value, long repeatCount)
    {
        threadLocalStringBuilder.Value.Clear();

        if (repeatCount is > 0 and < 100000)
        {
            for (long i = 0; i < repeatCount; i++)
            {
                threadLocalStringBuilder.Value.Append(value);
            }

            _value = threadLocalStringBuilder.Value.ToString();
        }
    }

    // 重载 * 操作符（字符串重复）
    public static string operator *(PyString a, long repeatCount)
    {
        if (repeatCount < 0)
        {
            throw new ArgumentException("Repeat count cannot be negative");
        }
        return new PyString(a._value, repeatCount);
    }
    


    // 重载 + 操作符（字符串拼接）
    public static string operator +(PyString a, PyString b)
    {
        return a.__add__(b);
    }

    // 重载 + 操作符（字符串拼接）
    public static string operator +(PyString a, string b)
    {
        return a.__add__(b);
    }

    // 重载 + 操作符（字符串拼接）
    public static string operator +(string a, PyString b)
    {
        return ((PyString)a).__add__(b);
    }

    public static PyString operator *(long repeatCount, PyString a)
    {
        return a.__mul__(repeatCount); // 调用上面的重载
    }

    // 比较操作符（Python 中字符串比较按字典序）
    public static bool operator ==(PyString a, PyString b)
    {
        return a._value == b._value;
    }

    public static bool operator ==(PyString a, pyint b)
    {
        if (long.TryParse(a, out var ia))
            return ia == b;
        return false;
    }
    public static bool operator ==(pyint a, PyString b)
    {
        if (long.TryParse(b, out var ib))
            return ib == a;
        return false;
    }

    public static bool operator !=(PyString a, PyString b)
    {
        return !(a == b);
    }
    public static bool operator !=(PyString a, pyint b)
    {
        return !(a == b);
    }
    public static bool operator !=(pyint a, PyString b)
    {
        return !(a == b);
    }

    public static bool operator <(PyString a, PyString b)
    {
        return string.Compare(a._value, b._value, StringComparison.Ordinal) < 0;
    }

    public static bool operator >(PyString a, PyString b)
    {
        return string.Compare(a._value, b._value, StringComparison.Ordinal) > 0;
    }

    public static bool operator <=(PyString a, PyString b)
    {
        return a < b || a == b;
    }

    public static bool operator >=(PyString a, PyString b)
    {
        return a > b || a == b;
    }

    // 隐式转换为 string
    public static implicit operator string(PyString pyString)
    {
        return pyString._value;
    }

    // 隐式转换为 PyString 从 string
    public static implicit operator PyString(string value)
    {
        return new PyString(value);
    }


    
    
    
    
    
    // __add__方法：字符串连接
    public PyString __add__(PyString value)
    {
        return new PyString(_value + value._value);
    }

    // __class__方法：返回类名
    public string __class__()
    {
        return this.GetType().Name;
    }

    // __contains__方法：检查是否包含
    public bool __contains__(PyString key)
    {
        return _value.Contains(key._value);
    }

    // __delattr__方法：移除属性（模拟）
    public void __delattr__(PyString name)
    {
        // C# 不支持动态属性删除，这里我们只是模拟
        Console.WriteLine("Attribute " + name._value + " deleted.");
    }

    // __dir__方法：返回对象的方法和属性
    public List<string> __dir__()
    {
        return this.GetType().GetMethods().Select(m => m.Name).ToList();
    }

    // __eq__方法：检查相等
    public bool __eq__(PyString value)
    {
        return _value == value._value;
    }

    // __format__方法：格式化字符串
    public string __format__(PyString format_spec)
    {
        return string.Format(format_spec._value, _value);
    }

    // __ge__方法：大于等于比较
    public bool __ge__(PyString value)
    {
        return string.Compare(_value, value._value) >= 0;
    }

    // __getattribute__方法：获取属性
    public string __getattribute__(PyString name)
    {
        return _value.GetType().GetProperty(name._value)?.GetValue(_value).ToString() ?? "Attribute not found";
    }

    // __getitem__方法：获取索引
    public char __getitem__(PyString key)
    {
        return _value[key._value.Length];
    }

    // __getnewargs__方法：返回初始化参数
    public Tuple<string> __getnewargs__()
    {
        return new Tuple<string>(_value);
    }

    // __gt__方法：大于比较
    public bool __gt__(PyString value)
    {
        return string.Compare(_value, value._value) > 0;
    }

    // __hash__方法：计算哈希值
    public long __hash__()
    {
        return _value.GetHashCode();
    }

    public bool __eq__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public bool __ne__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public bool __lt__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public bool __le__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public bool __gt__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public bool __ge__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public IPyObject __add__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public IPyObject __sub__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public IPyObject __mul__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public IPyObject __truediv__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public IPyObject __mod__(IPyObject other)
    {
        throw new NotImplementedException();
    }

    public string __format__(string format)
    {
        throw new NotImplementedException();
    }

    // __repr__方法：返回对象的字符串表示
    public void __init__(IPyObject o)
    {
        
    }
    public void __init__(pyint i)
    {
        
    }

    // __init_subclass__方法：用于初始化子类
    public static void __init_subclass__()
    {
        Console.WriteLine("Subclass initialized");
    }

    // __iter__方法：返回字符迭代器
    public IEnumerable<char> __iter__()
    {
        return _value.ToCharArray();
    }

    // __le__方法：小于等于比较
    public bool __le__(PyString value)
    {
        return string.Compare(_value, value._value) <= 0;
    }

    // __len__方法：获取长度
    public long __len__()
    {
        return _value.Length;
    }

    public bool __contains__(IPyObject item)
    {
        throw new NotImplementedException();
    }

    public void __setattr__(string name, IPyObject value)
    {
        throw new NotImplementedException();
    }

    public void __delattr__(string name)
    {
        throw new NotImplementedException();
    }

    public IPyObject __getattribute__(string name)
    {
        throw new NotImplementedException();
    }

    // __lt__方法：小于比较
    public bool __lt__(PyString value)
    {
        return string.Compare(_value, value._value) < 0;
    }

    // __mod__方法：字符串格式化
    public string __mod__(PyString value)
    {
        return string.Format(_value, value._value);
    }

    // __mul__方法：重复字符串
    public PyString __mul__(pyint value)
    {
        return new PyString(_value, value);
    }

    // __ne__方法：不等比较
    public bool __ne__(PyString value)
    {
        return _value != value._value;
    }

    // __new方法：创建新对象
    public static PyString __new__(PyString kwargs)
    {
        return new PyString(kwargs._value);
    }

    // __reduce__方法：序列化对象
    public string __reduce__()
    {
        return _value;
    }

    // __reduce_ex方法：带协议版本的序列化
    public string __reduce_ex__(PyString protocol)
    {
        return _value + protocol._value;
    }

    public string __repr__()
    {
        threadLocalStringBuilder.Value.Clear();
        threadLocalStringBuilder.Value.Append('\'');
        threadLocalStringBuilder.Value.Append(_value);
        threadLocalStringBuilder.Value.Append('\'');
        return threadLocalStringBuilder.Value.ToString();
    }

    // __rmod方法：反向字符串格式化
    public string __rmod__(PyString value)
    {
        return string.Format(value._value, _value);
    }

    // __rmul方法：反向乘法操作
    public PyString __rmul__(pyint value)
    {
        return this.__mul__(value);
    }

    // __setattr__方法：设置属性
    public void __setattr__(PyString name, PyString value)
    {
        // 模拟动态属性设置
        Console.WriteLine($"Attribute {name._value} set to {value._value}");
    }

    // __sizeof__方法：返回对象大小
    public long __sizeof__()
    {
        return Marshal.SizeOf(_value);
    }

    // __str__方法：返回字符串表示
    public string __str__()
    {
        return _value;
    }

    // __subclasshook__方法：用于动态检查子类
    public static void __subclasshook__()
    {
        Console.WriteLine("Subclass hook called");
    }

    // capitalize方法：首字母大写
    public string capitalize()
    {
        if(Char.IsLetter(_value[0]))
        {
            threadLocalStringBuilder.Value.Clear();
            threadLocalStringBuilder.Value.Append(_value[0]);
            threadLocalStringBuilder.Value.Append(_value, 1, _value.Length);
            return threadLocalStringBuilder.Value.ToString();
        }
        return _value;
    }

    // casefold方法：无差异大小写转换
    public string casefold()
    {
        return _value.ToLower();
    }

    // center方法：居中对齐
    public string center(pyint width, PyString fillchar)
    {
        char f = fillchar._value[0];
        return _value.PadLeft(((int)width + _value.Length) / 2, f).PadRight(width, f);
    }

    // count方法：计数出现次数
    public int count()
    {
        return _value.Count(c => c == 'a'); // 假设查找字母 'a' 的出现次数
    }

    // encode方法：编码字符串
    public byte[] encode(PyString encoding, PyString errors)
    {
        return System.Text.Encoding.GetEncoding(encoding._value).GetBytes(_value);
    }

    // endswith方法：检查字符串结尾
    public bool endswith(PyString suffix)
    {
        return _value.EndsWith(suffix._value);
    }

    // expandtabs方法：扩展制表符
    public string expandtabs(int tabsize)
    {
        return _value.Replace("\t", new string(' ', tabsize));
    }

    // find方法：查找子串
    public int find(PyString sub)
    {
        return _value.IndexOf(sub._value);
    }

    // format方法：格式化字符串
    public string format(params object[] args)
    {
        return string.Format(_value, args);
    }

    // format_map方法：映射格式化
    public string format_map(PyString mapping)
    {
        return string.Format(_value, mapping._value);
    }

    // index方法：获取子串索引
    public int index(PyString sub)
    {
        return _value.IndexOf(sub._value);
    }

    // isalnum方法：检查是否是字母数字
    public bool isalnum()
    {
        return _value.All(char.IsLetterOrDigit);
    }

    // isalpha方法：检查是否是字母
    public bool isalpha()
    {
        return _value.All(char.IsLetter);
    }

    // isascii方法：检查是否是 ASCII 字符
    public bool isascii()
    {
        return _value.All(c => c <= 127);
    }

    // isdecimal方法：检查是否是十进制字符
    public bool isdecimal()
    {
        return _value.All(char.IsDigit);
    }

    // isdigit方法：检查是否是数字字符
    public bool isdigit()
    {
        return _value.All(char.IsDigit);
    }

    // isidentifier方法：检查是否是有效标识符
    public bool isidentifier()
    {
        return !string.IsNullOrEmpty(_value) && char.IsLetter(_value[0]) && _value.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    // islower方法：检查是否全部小写
    public bool islower()
    {
        return _value.All(char.IsLower);
    }

    // isnumeric方法：检查是否是数字
    public bool isnumeric()
    {
        return _value.All(char.IsDigit);
    }

    // isprintable方法：检查是否可打印
    public bool isprintable()
    {
        return _value.All(char.IsControl) == false;
    }

    // isspace方法：检查是否为空白字符
    public bool isspace()
    {
        return _value.All(char.IsWhiteSpace);
    }

    // istitle方法：检查是否标题格式
    public bool istitle()
    {
        return _value.Split(' ').All(word => char.IsUpper(word[0]));
    }

    // isupper方法：检查是否全部大写
    public bool isupper()
    {
        return _value.All(char.IsUpper);
    }

    // join方法：将可迭代对象的元素连接为字符串
    public string join(PyString iterable)
    {
        return string.Join(_value, iterable._value);
    }

    // ljust方法：左对齐
    public string ljust(int width, PyString fillchar)
    {
        return _value.PadRight(width, fillchar._value[0]);
    }

    // lower方法：转换为小写
    public string lower()
    {
        return _value.ToLower();
    }

    // lstrip方法：去除左边的字符
    public string lstrip(PyString chars)
    {
        return _value.TrimStart(chars._value.ToCharArray());
    }

    // maketrans方法：生成字符映射表
    public static Dictionary<char, char> maketrans(PyString from, PyString to)
    {
        var translation = new Dictionary<char, char>();
        for (int i = 0; i < from._value.Length; i++)
        {
            translation[from._value[i]] = to._value[i];
        }
        return translation;
    }

    // partition方法：分割字符串
    public Tuple<string, string, string> partition(PyString sep)
    {
        int index = _value.IndexOf(sep._value);
        if (index == -1)
            return new Tuple<string, string, string>(_value, "", "");
        else
            return new Tuple<string, string, string>(_value.Substring(0, index), sep._value, _value.Substring(index + sep._value.Length));
    }

    // replace方法：替换子串
    public string replace(PyString oldstr, PyString newstr, int count = -1)
    {
        // 将输入字符串转换为C#的string类型
        string oldValue = oldstr._value;
        string newValue = newstr._value;

        // 如果没有指定替换次数，或者指定的是负数，则执行所有替换
        if (count < 0)
            return _value.Replace(oldValue, newValue);
        if (count == 0)
            return _value;

        // 创建一个 StringBuilder 用于构建结果字符串
        var sb = threadLocalStringBuilder.Value;
        sb.Clear();
        int startIndex = 0;
        int replacedCount = 0;

        // 遍历原始字符串，并替换指定次数
        while (startIndex < _value.Length)
        {
            int index = _value.IndexOf(oldValue, startIndex, StringComparison.Ordinal);

            // 如果没有找到匹配的子串，直接结束替换
            if (index == -1)
            {
                sb.Append(_value.Substring(startIndex));
                break;
            }

            // 添加替换部分
            sb.Append(_value.Substring(startIndex, index - startIndex));

            // 添加新的子串
            sb.Append(newValue);

            // 更新开始搜索的位置，并增加替换次数
            startIndex = index + oldValue.Length;
            replacedCount++;

            // 如果已经达到了指定的替换次数，退出
            if (replacedCount >= count)
            {
                sb.Append(_value.Substring(startIndex)); // 添加剩余部分
                break;
            }
        }

        return sb.ToString();
    }

    // rfind方法：从右查找子串
    public int rfind(PyString sub)
    {
        return _value.LastIndexOf(sub._value, StringComparison.Ordinal);
    }

    // rindex方法：从右获取子串索引
    public int rindex(PyString sub)
    {
        return _value.LastIndexOf(sub._value, StringComparison.Ordinal);
    }

    // rjust方法：右对齐
    public string rjust(int width, PyString fillchar)
    {
        return _value.PadLeft(width, fillchar._value[0]);
    }

    // rpartition方法：从右分割字符串
    public Tuple<string, string, string> rpartition(PyString sep)
    {
        int index = _value.LastIndexOf(sep._value, StringComparison.Ordinal);
        if (index == -1)
            return new Tuple<string, string, string>("", "", _value);
        else
            return new Tuple<string, string, string>(_value.Substring(0, index), sep._value, _value.Substring(index + sep._value.Length));
    }

    // rsplit方法：从右分割字符串
    public List<string> rsplit(PyString sep, int maxsplit = -1)
    {
        return _value.Split([sep._value], StringSplitOptions.None).Take(maxsplit).ToList();
    }

    // rstrip方法：去除右边的字符
    public string rstrip(PyString chars)
    {
        return _value.TrimEnd(chars._value.ToCharArray());
    }

    // split方法：分割字符串
    public List<string> split(PyString sep, int maxsplit = -1)
    {
        return _value.Split(new string[] { sep._value }, StringSplitOptions.None).Take(maxsplit).ToList();
    }

    // splitlines方法：按行分割字符串
    public List<string> splitlines(int keepends)
    {
        return _value.Split(new[] { "\n" }, StringSplitOptions.None).ToList();
    }

    // startswith方法：检查是否以某个子串开头
    public bool startswith(PyString prefix)
    {
        return _value.StartsWith(prefix._value);
    }

    // strip方法：去除两边的字符
    public string strip(PyString[] chars)
    {
        return _value.Trim(chars.SelectMany(c => c._value).ToArray());
    }

    // swapcase方法：交换大小写
    public string swapcase()
    {
        var sb = threadLocalStringBuilder.Value;
        sb.Clear();
        // 遍历每个字符并交换大小写
        foreach (var c in _value)
        {
            // 如果字符是小写字母，则转换为大写字母
            if (char.IsLower(c))
            {
                sb.Append(char.ToUpper(c));
            }
            // 如果字符是大写字母，则转换为小写字母
            else if (char.IsUpper(c))
            {
                sb.Append(char.ToLower(c));
            }
            else
            {
                sb.Append(c);  // 保持其他字符不变
            }
        }
        return sb.ToString();
    }

    // title方法：首字母大写
    public string title()
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_value);
    }

    // translate方法：字符替换
    public string translate(PyString table)
    {
        return new string(_value.Select(c => table._value.Contains(c.ToString()) ? table._value[0] : c).ToArray());
    }

    // upper方法：转换为大写
    public string upper()
    {
        return _value.ToUpper();
    }

    // zfill方法：零填充
    public string zfill(int width)
    {
        return _value.PadLeft(width, '0');
    }
}