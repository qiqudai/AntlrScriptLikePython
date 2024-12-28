using System.Runtime.InteropServices;

namespace SyntacticSugar
{

    public class PyClass : IPyObject, IDisposable
    {
        private IntPtr _value; // 核心存储
        private TypeFlag _type; // 当前存储内容的类型标志

        private GCHandle? _handle; // 可选的句柄（用于引用类型）

        // 构造函数（字典）
        public PyClass(set value)
        {
            __init__();
            _handle = GCHandle.Alloc(value);
            _value = GCHandle.ToIntPtr(_handle.Value);
            _type = TypeFlag.Set;
        }

        // 构造函数（字典）
        public PyClass(frozenset value)
        {
            __init__();
            _handle = GCHandle.Alloc(value);
            _value = GCHandle.ToIntPtr(_handle.Value);
            _type = value is set ? TypeFlag.Set : TypeFlag.Frozenset;
        }

        // 构造函数（字典）
        public PyClass(pyclass value)
        {
            __init__();
            _handle = GCHandle.Alloc(value);
            _value = GCHandle.ToIntPtr(_handle.Value);
            _type = TypeFlag.Class;
        }

        ~PyClass()
        {
            __del__();
        }
        

        // 字符串表示
        public override string ToString() => __str__();
        public override bool Equals(object obj) => __eq__((PyClass)obj);

        


        // 清理资源
        public void Dispose()
        {
            if (_type == TypeFlag.None) return;
            __del__();
            if (_handle is { IsAllocated: true })
            {
                _handle.Value.Free();
                _handle = null;
            }

            _value = IntPtr.Zero;
            _type = TypeFlag.None;
            GC.SuppressFinalize(this);
        }

        #region 接口实现



        // 构造与析构
        public void __init__()
        {
        }


        public void __del__()
        {
        }

        public string __repr__() => $"{GetType().Name}()";
        public string __str__() => __repr__();

        public long __hash__() => base.GetHashCode();

        // 等价性比较
        public bool __eq__(IPyObject other) => ReferenceEquals(this, other);
        public bool __ne__(IPyObject other) => !__eq__(other);

        // 比较操作符
        public bool __lt__(IPyObject other) => throw new NotImplementedException("__lt__ not implemented");
        public bool __le__(IPyObject other) => throw new NotImplementedException("__le__ not implemented");
        public bool __gt__(IPyObject other) => throw new NotImplementedException("__gt__ not implemented");
        public bool __ge__(IPyObject other) => throw new NotImplementedException("__ge__ not implemented");

        // 算术运算符
        public IPyObject __add__(IPyObject other) => throw new NotImplementedException("__add__ not implemented");
        public IPyObject __radd__(IPyObject other) => __add__(other); // 反向加法

        public IPyObject __iadd__(IPyObject other) =>
            throw new NotImplementedException("__iadd__ not implemented"); // 增量赋值

        public IPyObject __sub__(IPyObject other) => throw new NotImplementedException("__sub__ not implemented");
        public IPyObject __rsub__(IPyObject other) => throw new NotImplementedException("__rsub__ not implemented");
        public IPyObject __isub__(IPyObject other) => throw new NotImplementedException("__isub__ not implemented");

        public IPyObject __mul__(IPyObject other) => throw new NotImplementedException("__mul__ not implemented");
        public IPyObject __rmul__(IPyObject other) => __mul__(other);
        public IPyObject __imul__(IPyObject other) => throw new NotImplementedException("__imul__ not implemented");

        public IPyObject __truediv__(IPyObject other) =>
            throw new NotImplementedException("__truediv__ not implemented");

        public IPyObject __rtruediv__(IPyObject other) =>
            throw new NotImplementedException("__rtruediv__ not implemented");

        public IPyObject __itruediv__(IPyObject other) =>
            throw new NotImplementedException("__itruediv__ not implemented");

        public IPyObject __floordiv__(IPyObject other) =>
            throw new NotImplementedException("__floordiv__ not implemented");

        public IPyObject __rfloordiv__(IPyObject other) =>
            throw new NotImplementedException("__rfloordiv__ not implemented");

        public IPyObject __ifloordiv__(IPyObject other) =>
            throw new NotImplementedException("__ifloordiv__ not implemented");

        public IPyObject __mod__(IPyObject other) => throw new NotImplementedException("__mod__ not implemented");
        public IPyObject __rmod__(IPyObject other) => throw new NotImplementedException("__rmod__ not implemented");
        public IPyObject __imod__(IPyObject other) => throw new NotImplementedException("__imod__ not implemented");

        public IPyObject __pow__(IPyObject other, IPyObject mod = null) =>
            throw new NotImplementedException("__pow__ not implemented");

        public IPyObject __rpow__(IPyObject other, IPyObject mod = null) =>
            throw new NotImplementedException("__rpow__ not implemented");

        public IPyObject __ipow__(IPyObject other, IPyObject mod = null) =>
            throw new NotImplementedException("__ipow__ not implemented");

        // 单目运算符
        public IPyObject __neg__() => throw new NotImplementedException("__neg__ not implemented");
        public IPyObject __pos()
        {
            throw new NotImplementedException();
        }

        public IPyObject __pos__() => this;
        public IPyObject __abs__() => throw new NotImplementedException("__abs__ not implemented");
        public IPyObject __round__(long? n = null)
        {
            throw new NotImplementedException();
        }

        public IPyObject __round__(int? n = null) => throw new NotImplementedException("__round__ not implemented");
        public IPyObject __trunc__() => throw new NotImplementedException("__trunc__ not implemented");
        public IPyObject __floor__() => throw new NotImplementedException("__floor__ not implemented");
        public IPyObject __ceil__() => throw new NotImplementedException("__ceil__ not implemented");

        // 类型转换
        public long __int__() => throw new NotImplementedException("__int__ not implemented");
        public double __float__() => throw new NotImplementedException("__float__ not implemented");
        public IPyObject __complex__() => throw new NotImplementedException("__complex__ not implemented");
        public long __index__() => throw new NotImplementedException("__index__ not implemented");
        public string __format__(string format) => throw new NotImplementedException("__format__ not implemented");
        public byte[] __bytes__() => throw new NotImplementedException("__bytes__ not implemented");

        // 容器协议
        public long __len__() => throw new NotImplementedException("__len__ not implemented");
        public IPyObject __getitem__(IPyObject key) => throw new NotImplementedException("__getitem__ not implemented");

        public void __setitem__(IPyObject key, IPyObject value) =>
            throw new NotImplementedException("__setitem__ not implemented");

        public void __delitem__(IPyObject key) => throw new NotImplementedException("__delitem__ not implemented");
        public IEnumerator<IPyObject> __iter__() => throw new NotImplementedException("__iter__ not implemented");
        public IPyObject __next__() => throw new NotImplementedException("__next__ not implemented");

        // 成员测试
        public bool __contains__(IPyObject item) => throw new NotImplementedException("__contains__ not implemented");

        // 上下文管理协议
        public IPyObject __enter__() => this;

        public void __exit__(Type excType, object excValue, object traceback)
        {
        }

        // 调用对象
        public IPyObject __call__(params IPyObject[] args) =>
            throw new NotImplementedException("__call__ not implemented");

        // 属性访问控制
        public IPyObject __getattr__(string name) => throw new NotImplementedException("__getattr__ not implemented");

        public void __setattr__(string name, IPyObject value) =>
            throw new NotImplementedException("__setattr__ not implemented");

        public void __delattr__(string name) => throw new NotImplementedException("__delattr__ not implemented");

        public IPyObject __getattribute__(string name) =>
            throw new NotImplementedException("__getattribute__ not implemented");

        // 描述符协议
        public IPyObject __get__(IPyObject obj, Type type = null) =>
            throw new NotImplementedException("__get__ not implemented");

        public void __set__(IPyObject obj, IPyObject value) =>
            throw new NotImplementedException("__set__ not implemented");

        public void __delete__(IPyObject obj) => throw new NotImplementedException("__delete__ not implemented");

        // 序列化/反序列化
        public IPyObject __getstate__() => throw new NotImplementedException("__getstate__ not implemented");
        public void __setstate__(IPyObject state) => throw new NotImplementedException("__setstate__ not implemented");

        // 异步支持
        public IEnumerator<IPyObject> __await__() => throw new NotImplementedException("__await__ not implemented");
        public IPyObject __aenter__() => throw new NotImplementedException("__aenter__ not implemented");

        public void __aexit__(Type excType, object excValue, object traceback) =>
            throw new NotImplementedException("__aexit__ not implemented");

        // 其他
        public bool __bool__() => true;
        public bool __is__(IPyObject other) => ReferenceEquals(this, other);
        public bool __is_not__(IPyObject other) => !ReferenceEquals(this, other);

        #endregion
    }
}