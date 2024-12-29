using System;
using System.Collections.Frozen;
using System.Collections.Generic;
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
    Class
}
public record struct PyVariable : IDisposable
{
    private pyint _value; // 核心存储
    private TypeFlag _type; // 当前存储内容的类型标志
    private IPyObject? _handle; // 可选的句柄（用于引用类型）


    // 构造函数（整型）
    public PyVariable(long value)
    {
        _value = new (value);
        _handle = null;
        _type = TypeFlag.Integer;
    }

    // 构造函数（浮点型）
    public PyVariable(double value)
    {
        _value = new ((long)value);
        _handle = null;
        _type = TypeFlag.Float;
    }

    // 构造函数（字符串）
    public PyVariable(string value)
    {
        _handle = (PyString)value;
        _value = 0;
        _type = TypeFlag.String;
    }

    // 构造函数（列表）
    public PyVariable(list value)
    {
        _handle = (list)value;
        _value = 0;
        _type = TypeFlag.List;
    }

    // 构造函数（字典）
    public PyVariable(dict value)
    {
        _handle = (dict)value;
        _value = 0;
        _type = TypeFlag.Dictionary;
    }
    // 构造函数（字典）
    public PyVariable(tuple value)
    {
        _handle = value;
        _value = 0;
        _type = TypeFlag.Tuple;
    }
    
    // 构造函数（字典）
    public PyVariable(set value)
    {
        _handle = (set)value;
        _value = 0;
        _type = TypeFlag.Set;
    }
    // 构造函数（字典）
    public PyVariable(frozenset value)
    {
        _handle = (frozenset)value;
        _value = 0;
        _type = value is set ? TypeFlag.Set : TypeFlag.Frozenset;
    }
    // 构造函数（字典）
    public PyVariable(pyclass value)
    {
        _handle = value;
        _value = 0;
        _type = TypeFlag.Class;
    }

    public pyint? get()
    {
        switch (_type)
        {
            case SyntacticSugar.TypeFlag.None:
                return null;
            case SyntacticSugar.TypeFlag.Integer:
            case SyntacticSugar.TypeFlag.Float:
                return _value;
            default:
                throw new InvalidOleVariantTypeException("var not value type");
        }
    }
    
    // 根据存储的值和目标类型返回值
    public T? get<T>() where T : class
    {
        switch (_type)
        {
            case SyntacticSugar.TypeFlag.None:
                return null;
            case SyntacticSugar.TypeFlag.String:
                return (T?)(object?)(_handle as PyString);
            case SyntacticSugar.TypeFlag.Frozenset:
                return (T?)(object?)(_handle as frozenset);
            case SyntacticSugar.TypeFlag.Set:
                return (T?)(object?)(_handle as set);
            case SyntacticSugar.TypeFlag.List:
                return (T?)(object?)(_handle as list);
            case SyntacticSugar.TypeFlag.Tuple:
                return (T?)(object?)(_handle as tuple);
            case SyntacticSugar.TypeFlag.Dictionary:
                return (T?)(object?)(_handle as dict);
            case SyntacticSugar.TypeFlag.Class:
                return (T?)(object?)(_handle as PyClass);
            case TypeFlag.Integer:
            case TypeFlag.Float:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // 清理资源
    public void Dispose()
    {
        if (_type == TypeFlag.None) return;
        if (_handle != null)
        {
            _handle = null;
        }
        _value = IntPtr.Zero;
        _type = TypeFlag.None;
        GC.SuppressFinalize(this);
    }

    public override string ToString() => _type != TypeFlag.None ? _value.ToString() : "null";



    #region 运算符重载

    // 运算符重载：加法
    public static PyVariable operator +(PyVariable a, PyVariable b)
    {
        if (a._type == TypeFlag.String || b._type == TypeFlag.String)
        {
            if (a._handle is tuple astr && b._handle is tuple bstr)
            {
                return new PyVariable(astr + bstr);
            }
        }

        if (a._type == TypeFlag.Dictionary || b._type == TypeFlag.Dictionary)
        {
            if (a._handle is dict a_dict && b._handle is dict bdict)
            {
                return new PyVariable(a_dict + bdict);
            }
            throw new InvalidOleVariantTypeException("dict只能与dict运算.");
        }
        if (a._type == TypeFlag.Class || b._type == TypeFlag.Class)
        {
            if (a._handle is pyclass aclass && b._handle is pyclass bclass)
            {
                return new PyVariable(aclass + bclass);
            }
            throw new InvalidOleVariantTypeException("类只能与类运算.");
        }
        if (a._type == TypeFlag.List || b._type == TypeFlag.List)
        {
            if (a._handle is list ali && b._handle is list bli)
            {
                return new PyVariable(ali + bli);
            }
            throw new InvalidOleVariantTypeException("list只能与list运算.");
        }
        if (a._type == TypeFlag.Set || b._type == TypeFlag.Set)
        {
            if (a._handle is set aset && b._handle is set bset)
            {
                return new PyVariable(aset + bset); 
            }
            throw new InvalidOleVariantTypeException("Set只能与Set运算.");
        }
        if (a._type == TypeFlag.Tuple || b._type == TypeFlag.Tuple)
        {
            if (a._handle is tuple atuple && b._handle is tuple btuple)
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
    
    #endregion
}