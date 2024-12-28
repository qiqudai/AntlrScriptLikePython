using System;
using System.Collections.Generic;

public interface IPyObject
{
    // 构造与析构
    void __init__();
    void __del__();

    // 表示
    string __repr__();
    string __str__();

    // 哈希值
    int __hash__();

    // 等价性比较
    bool __eq__(IPyObject other);
    bool __ne__(IPyObject other);

    // 比较操作符
    bool __lt__(IPyObject other);
    bool __le__(IPyObject other);
    bool __gt__(IPyObject other);
    bool __ge__(IPyObject other);

    // 算术运算符
    IPyObject __add__(IPyObject other);
    IPyObject __radd__(IPyObject other); // 反向加法
    IPyObject __iadd__(IPyObject other); // 增量赋值

    IPyObject __sub__(IPyObject other);
    IPyObject __rsub__(IPyObject other);
    IPyObject __isub__(IPyObject other);

    IPyObject __mul__(IPyObject other);
    IPyObject __rmul__(IPyObject other);
    IPyObject __imul__(IPyObject other);

    IPyObject __truediv__(IPyObject other);
    IPyObject __rtruediv__(IPyObject other);
    IPyObject __itruediv__(IPyObject other);

    IPyObject __floordiv__(IPyObject other);
    IPyObject __rfloordiv__(IPyObject other);
    IPyObject __ifloordiv__(IPyObject other);

    IPyObject __mod__(IPyObject other);
    IPyObject __rmod__(IPyObject other);
    IPyObject __imod__(IPyObject other);

    IPyObject __pow__(IPyObject other, IPyObject mod = null);
    IPyObject __rpow__(IPyObject other, IPyObject mod = null);
    IPyObject __ipow__(IPyObject other, IPyObject mod = null);

    // 单目运算符
    IPyObject __neg__();
    IPyObject __pos();
    IPyObject __abs__();
    IPyObject __round__(int? n = null);
    IPyObject __trunc__();
    IPyObject __floor__();
    IPyObject __ceil__();

    // 类型转换
    int __int__();
    double __float__();
    IPyObject __complex__();
    int __index__();
    string __format__(string format);
    byte[] __bytes__();

    // 容器协议
    int __len__();
    IPyObject __getitem__(IPyObject key);
    void __setitem__(IPyObject key, IPyObject value);
    void __delitem__(IPyObject key);
    IEnumerator<IPyObject> __iter__();
    IPyObject __next__();

    // 成员测试
    bool __contains__(IPyObject item);

    // 上下文管理协议
    IPyObject __enter__();
    void __exit__(Type excType, object excValue, object traceback);

    // 调用对象
    IPyObject __call__(params IPyObject[] args);

    // 属性访问控制
    IPyObject __getattr__(string name);
    void __setattr__(string name, IPyObject value);
    void __delattr__(string name);
    IPyObject __getattribute__(string name);

    // 描述符协议
    IPyObject __get__(IPyObject obj, Type type = null);
    void __set__(IPyObject obj, IPyObject value);
    void __delete__(IPyObject obj);

    // 序列化/反序列化
    IPyObject __getstate__();
    void __setstate__(IPyObject state);

    // 异步支持
    IEnumerator<IPyObject> __await__();
    IPyObject __aenter__();
    void __aexit__(Type excType, object excValue, object traceback);

    // 其他
    bool __bool__();
    bool __is__(IPyObject other);
    bool __is_not__(IPyObject other);
}