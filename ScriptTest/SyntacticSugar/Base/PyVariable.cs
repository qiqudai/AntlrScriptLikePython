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
    private GCHandle? _handle; // 可选的句柄（用于引用类型）


    // 构造函数（整型）
    public PyVariable(long value)
    {
        _value = new (value);
        _type = TypeFlag.Integer;
    }

    // 构造函数（浮点型）
    public PyVariable(double value)
    {
        _value = new ((long)value);
        _type = TypeFlag.Float;
    }

    // 构造函数（字符串）
    public PyVariable(string value)
    {
        _handle = GCHandle.Alloc(value);
        _value = GCHandle.ToIntPtr(_handle.Value);
        _type = TypeFlag.String;
    }

    // 构造函数（列表）
    public PyVariable(list value)
    {
        _handle = GCHandle.Alloc(value);
        _value = GCHandle.ToIntPtr(_handle.Value);
        _type = TypeFlag.List;
    }

    // 构造函数（字典）
    public PyVariable(dict value)
    {
        _handle = GCHandle.Alloc(value);
        _value = GCHandle.ToIntPtr(_handle.Value);
        _type = TypeFlag.Dictionary;
    }
    // 构造函数（字典）
    public PyVariable(tuple value)
    {
        _handle = GCHandle.Alloc(value);
        _value = GCHandle.ToIntPtr(_handle.Value);
        _type = TypeFlag.Tuple;
    }
    
    // 构造函数（字典）
    public PyVariable(set value)
    {
        _handle = GCHandle.Alloc(value);
        _value = GCHandle.ToIntPtr(_handle.Value);
        _type = TypeFlag.Set;
    }
    // 构造函数（字典）
    public PyVariable(frozenset value)
    {
        _handle = GCHandle.Alloc(value);
        _value = GCHandle.ToIntPtr(_handle.Value);
        _type = value is set ? TypeFlag.Set : TypeFlag.Frozenset;
    }
    // 构造函数（字典）
    public PyVariable(pyclass value)
    {
        _handle = GCHandle.Alloc(value);
        _value = GCHandle.ToIntPtr(_handle.Value);
        _type = TypeFlag.Class;
    }

    // 根据存储的值和目标类型返回值
    public T get<T>()
    {
        object result = _type switch
        {
            TypeFlag.Integer when typeof(T).IsValueType => (long)_value,
            TypeFlag.Float when typeof(T).IsValueType => _value,
            TypeFlag.String when typeof(T) == typeof(string) => _handle.HasValue ? (string)_handle.Value.Target : null,
            TypeFlag.Dictionary when typeof(T) == typeof(dict) => _handle.HasValue ? (dict)_handle.Value.Target : null,
            TypeFlag.Frozenset when typeof(T) == typeof(frozenset) => _handle.HasValue ? (frozenset)_handle.Value.Target : null,
            TypeFlag.Set when typeof(T) == typeof(set) => _handle.HasValue ? (set)_handle.Value.Target : null,
            TypeFlag.Dictionary when typeof(T) == typeof(tuple) => _handle.HasValue ? (tuple)_handle.Value.Target : null,
            _ => throw new InvalidOperationException($"Unsupported conversion: {_type} to {typeof(T)}")
        };

        return (T)result;
    }


    // 清理资源
    public void Dispose()
    {
        if (_type == TypeFlag.None) return;
        if (_handle is { IsAllocated: true })
        {
            _handle.Value.Free();
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
            if (a._handle?.Target is tuple astr && b._handle?.Target is tuple bstr)
            {
                return new PyVariable(astr + bstr);
            }
        }

        if (a._type == TypeFlag.Dictionary || b._type == TypeFlag.Dictionary)
        {
            if (a._handle?.Target is dict a_dict && b._handle?.Target is dict bdict)
            {
                return new PyVariable(a_dict + bdict);
            }
            throw new InvalidOleVariantTypeException("dict只能与dict运算.");
        }
        if (a._type == TypeFlag.Class || b._type == TypeFlag.Class)
        {
            if (a._handle?.Target is pyclass aclass && b._handle?.Target is pyclass bclass)
            {
                return new PyVariable(aclass + bclass);
            }
            throw new InvalidOleVariantTypeException("类只能与类运算.");
        }
        if (a._type == TypeFlag.List || b._type == TypeFlag.List)
        {
            if (a._handle?.Target is list ali && b._handle?.Target is list bli)
            {
                return new PyVariable(ali + bli);
            }
            throw new InvalidOleVariantTypeException("list只能与list运算.");
        }
        if (a._type == TypeFlag.Set || b._type == TypeFlag.Set)
        {
            if (a._handle?.Target is set aset && b._handle?.Target is set bset)
            {
                return new PyVariable(aset + bset); 
            }
            throw new InvalidOleVariantTypeException("Set只能与Set运算.");
        }
        if (a._type == TypeFlag.Tuple || b._type == TypeFlag.Tuple)
        {
            if (a._handle?.Target is tuple atuple && b._handle?.Target is tuple btuple)
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