using System;
using System.Runtime.InteropServices;

namespace SyntacticSugar;

// 标志类型
public enum TypeFlag: byte
{
    None,
    Integer,
    Float,
    String,
    Frozenset,
    Set,
    List,
    Tuple,
    Dictionary,
    Class,
    FuncPointer,
}
public delegate PyVariable DelegateVarRet(params PyVariable[] Params);
public delegate void DelegateVarVoid(params PyVariable[] Params);
public record struct PyVariable : IDisposable
{
    private pyint _value; // 核心存储
    private TypeFlag _type; // 当前存储内容的类型标志
    private object? _object; // 可选的句柄（用于引用类型）


    // 构造函数（整型）
    public PyVariable(long value)
    {
        _value = new (value);
        _object = null;
        _type = TypeFlag.Integer;
    }
    public PyVariable(int value)
    {
        _value = new (value);
        _object = null;
        _type = TypeFlag.Integer;
    }
    public PyVariable(uint value)
    {
        _value = new (value);
        _object = null;
        _type = TypeFlag.Integer;
    }
    public PyVariable(ulong value)
    {
        _value = new (value);
        _object = null;
        _type = TypeFlag.Integer;
    }


    // 构造函数（浮点型）
    public PyVariable(double value)
    {
        _value = new (value);
        _object = null;
        _type = TypeFlag.Float;
    }
    // 构造函数（浮点型）
    public PyVariable(float value)
    {
        _value = new (value);
        _object = null;
        _type = TypeFlag.Float;
    }

    // 构造函数（字符串）
    public PyVariable(string value)
    {
        _object = (PyString)value;
        _value = 0;
        _type = TypeFlag.String;
    }

    // 构造函数（列表）
    public PyVariable(list value)
    {
        _object = (list)value;
        _value = 0;
        _type = TypeFlag.List;
    }
    
    public PyVariable(PyVariable[] value)
    {
        var li = new list();
        li.AddRange(value);
        _object = li;
        _type = TypeFlag.List;
        _value = 0;
    }
    
    public PyVariable(DelegateVarRet value)
    {
        _object = value;
        _type = TypeFlag.FuncPointer;
        _value = 0;
    }
    
    public PyVariable(IEnumerable<object> value)
    {
        if (value.All(v => v is PyVariable))
        {
            _object = new list(value.OfType<PyVariable>());
            _type = TypeFlag.List;
        }
        else if (value.All(v => v is KeyValuePair<PyVariable, PyVariable>))
        {
            _object = new dict(value.OfType<KeyValuePair<PyVariable, PyVariable>>());
            _type = TypeFlag.Dictionary;
        }
        else
        {
            throw new ArgumentException("Invalid type for PyVariable");
        }
        _value = 0;
    }

    // 构造函数（字典）
    public PyVariable(dict value)
    {
        _object = (dict)value;
        _value = 0;
        _type = TypeFlag.Dictionary;
    }
    // 构造函数（字典）
    public PyVariable(tuple value)
    {
        _object = value;
        _value = 0;
        _type = TypeFlag.Tuple;
    }
    
    // 构造函数（字典）
    public PyVariable(set value)
    {
        _object = (set)value;
        _value = 0;
        _type = TypeFlag.Set;
    }
    // 构造函数（字典）
    public PyVariable(frozenset value)
    {
        _object = (frozenset)value;
        _value = 0;
        _type = value is set ? TypeFlag.Set : TypeFlag.Frozenset;
    }
    // 构造函数（字典）
    public PyVariable(pyclass value)
    {
        _object = value;
        _value = 0;
        _type = TypeFlag.Class;
    }

    public void __call__(params PyVariable[] pyVariables)
    {
        if (_type is TypeFlag.FuncPointer or TypeFlag.Class)
        {
            ((DelegateVarRet)_object)(pyVariables);
        }
    }
    
    // 支持切片操作
    public PyVariable this[int start]
    {
        get
        {
            switch (_type)
            {
                case TypeFlag.List:
                {
                    list li = (list)_object;
                    return li[start];
                }
                case TypeFlag.Set:
                {
                    set li = (set)_object;
                    break;
                }
                default:
                    return "1";
            }

            return 0;
        }
        set
        {
            switch (_type)
            {
                case TypeFlag.List:
                {
                    list li = (list)_object;
                    li[start] = value;
                    return;
                }
                case TypeFlag.Set:
                {
                    set li = (set)_object;
                    li.add(value);
                    break;
                }
            }
        }
    }

    // 支持切片操作
    public PyVariable this[string key]
    {
        get
        {
            switch (_type)
            {
                case TypeFlag.Dictionary:
                {
                    var v = ((dict)_object)[key];
                    return v;
                }
                default:
                    throw new Exception("类型不正确...");
            }
        }
        set
        {
            switch (_type)
            {
                case TypeFlag.None:
                {
                    _object = new dict { [key] = value, };
                    _type = TypeFlag.Dictionary;
                    break;
                }
                case TypeFlag.Dictionary:
                {
                    dict li = (dict)_object;
                    li[key] = value;
                    break;
                }
                default:
                    throw new Exception("类型不正确...");
            }
        }
    }

    // 清理资源
    public void Dispose()
    {
        if (_type == TypeFlag.None) return;
        if (_object != null)
        {
            _object = null;
        }
        _value = IntPtr.Zero;
        _type = TypeFlag.None;
        GC.SuppressFinalize(this);
    }

    public override string ToString() 
    {
        switch (_type)
        {
            case TypeFlag.None:
                return "None";
            case TypeFlag.List:
                return ((list)_object).ToString();
            case TypeFlag.Dictionary:
                return ((dict)_object).ToString();
            // 添加其他类型的 ToString 处理
            case TypeFlag.Integer:
            case TypeFlag.Float:
                return _value.ToString();
            case TypeFlag.String:
                return (PyString)_object;
            case TypeFlag.Frozenset:
                return ((frozenset)_object).ToString();
            case TypeFlag.Set:
                return ((set)_object).ToString();
            case TypeFlag.Tuple:
                return ((tuple)_object).ToString();
            case TypeFlag.Class:
                return ((pyclass)_object).ToString();
            case TypeFlag.FuncPointer:
                return ((Delegate)_object).Method.Name;
            default:
                return "Unknown Type";
        }
    }



    #region 运算符重载

    // 运算符重载：加法
    public static PyVariable operator +(PyVariable a, PyVariable b)
    {
        if (a._type == TypeFlag.String || b._type == TypeFlag.String)
        {
            if (a._object is tuple astr && b._object is tuple bstr)
            {
                return new PyVariable(astr + bstr);
            }
        }

        if (a._type == TypeFlag.Dictionary || b._type == TypeFlag.Dictionary)
        {
            if (a._object is dict a_dict && b._object is dict bdict)
            {
                return new PyVariable(a_dict + bdict);
            }
            throw new InvalidOleVariantTypeException("dict只能与dict运算.");
        }
        if (a._type == TypeFlag.Class || b._type == TypeFlag.Class)
        {
            if (a._object is pyclass aclass && b._object is pyclass bclass)
            {
                return new PyVariable(aclass + bclass);
            }
            throw new InvalidOleVariantTypeException("类只能与类运算.");
        }
        if (a._type == TypeFlag.List || b._type == TypeFlag.List)
        {
            if (a._object is list ali && b._object is list bli)
            {
                return new PyVariable(ali + bli);
            }
            throw new InvalidOleVariantTypeException("list只能与list运算.");
        }
        if (a._type == TypeFlag.Set || b._type == TypeFlag.Set)
        {
            if (a._object is set aset && b._object is set bset)
            {
                return new PyVariable(aset + bset); 
            }
            throw new InvalidOleVariantTypeException("Set只能与Set运算.");
        }
        if (a._type == TypeFlag.Tuple || b._type == TypeFlag.Tuple)
        {
            if (a._object is tuple atuple && b._object is tuple btuple)
            {
                return new PyVariable(atuple + btuple);
            }
            throw new InvalidOleVariantTypeException("tuple只能与tuple运算.");
        }
        
        if ((a._type == TypeFlag.Float) || (b._type == TypeFlag.Float))
        {
            return new PyVariable((double)a._value + (double)b._value);
        }
        if (a._type == TypeFlag.Integer && b._type == TypeFlag.Integer)
        {
            return new PyVariable(a._value + a._value);
        }
        throw new InvalidOperationException("Unsupported types for + operation.");
    }
    
    // 隐式转换为 long
    public static implicit operator long(PyVariable number) => (long)number._value;
    public static implicit operator int(PyVariable number) => (int)number._value;
    public static implicit operator uint(PyVariable number) => (uint)number._value;
    public static implicit operator short(PyVariable number) => (short)number._value;
    public static implicit operator ushort(PyVariable number) => (ushort)number._value;
    public static implicit operator byte(PyVariable number) => (byte)number._value;
    public static implicit operator sbyte(PyVariable number) => (sbyte)number._value;
    public static implicit operator ulong(PyVariable number) => (ulong)number._value;
    // 隐式转换为 double
    //public static implicit operator double(PyVariable number) => number;
    public static implicit operator float(PyVariable number) => (float)number._value;
    public static implicit operator double(PyVariable number) => number._value;
    public static implicit operator decimal(PyVariable number) => (decimal)number._value;
    public static implicit operator list(PyVariable v) => v._type == TypeFlag.List ? (list)v._object : null;
    public static implicit operator set(PyVariable v) => v._type == TypeFlag.Set ? (set)v._object : null;
    public static implicit operator frozenset(PyVariable v) => v._type == TypeFlag.Frozenset ? (frozenset)v._object : null;
    public static implicit operator dict(PyVariable v) => v._type == TypeFlag.Dictionary ? (dict)v._object : null;
    public static implicit operator string(PyVariable v) => v._type == TypeFlag.String ? (PyString)v._object : null;
    public static implicit operator Delegate(PyVariable v) => v._type == TypeFlag.FuncPointer ? (Delegate)v._object : null;

    // 隐式转换为 PyNumber 从 long
    public static implicit operator PyVariable(long value) => new (value);
    public static implicit operator PyVariable(int value) => new (value);
    public static implicit operator PyVariable(uint value) => new (value);
    public static implicit operator PyVariable(ulong value) => new (value);
    public static implicit operator PyVariable(byte value) => new (value);
    public static implicit operator PyVariable(bool value) => new (value ? 1 : 0);
    public static implicit operator PyVariable(char value) => new ((int)value);
    public static implicit operator PyVariable(short value) => new (value);
    public static implicit operator PyVariable(ushort value) => new (value);

    // 隐式转换为 PyNumber 从 double
    public static implicit operator PyVariable(list value) => new (value);
    public static implicit operator PyVariable(set value) => new (value);

    public static implicit operator PyVariable(dict value) => new (value);
    public static implicit operator PyVariable(frozenset value) => new (value);
    public static implicit operator PyVariable(string value) => new (value);
    public static implicit operator PyVariable(PyString value) => new (value);
    public static implicit operator PyVariable(PyVariable[] values) => new (values);
    public static implicit operator PyVariable(DelegateVarRet values) => new (values);
    
    #endregion
}