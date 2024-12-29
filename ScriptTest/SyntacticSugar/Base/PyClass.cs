using System.Runtime.InteropServices;

namespace SyntacticSugar
{

    public class PyClass : IPyObject
    {
        // 构造函数（字典）
        public PyClass(set value)
        {
            __init__();
        }

        // 构造函数（字典）
        public PyClass(frozenset value)
        {
            __init__();
        }

        // 构造函数（字典）
        public PyClass(pyclass value)
        {
            __init__();
        }

        ~PyClass()
        {
            __del__();
        }
        

        // 字符串表示
        public override string ToString() => __str__();
        public override bool Equals(object obj) => __eq__((PyClass)obj);

        

        #region 接口实现

        // + 运算符：
        public static PyClass operator +(PyClass a, PyClass b)
        {
            return a.__add__(b);
        }

        // / 运算符：
        public static PyClass operator /(PyClass a, PyClass b)
        {
            return a.__mod__(b);
        }

        // - 运算符：
        public static PyClass operator -(PyClass a, PyClass b)
        {
            return a.__sub__(b);
        }

        // * 运算符：
        public static PyClass operator *(PyClass a, PyClass b)
        {
            return a.__mul__(b);
        }

        public void __weakref__(){}
        

        public PyClass __add__(PyClass other)
        {
            throw new NotImplementedException();
        }

        

        public PyClass __sub__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public PyClass __mul__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public PyClass __truediv__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public PyClass __mod__(PyClass other)
        {
            throw new NotImplementedException();
        }


        // 构造与析构
        public void __init__()
        {
        }


        public void __del__()
        {
        }

        

        #endregion

        public void __init__(IPyObject o)
        {
            throw new NotImplementedException();
        }


        public string __repr__()
        {
            throw new NotImplementedException();
        }

        public string __str__()
        {
            throw new NotImplementedException();
        }

        public long __hash__()
        {
            throw new NotImplementedException();
        }

        public bool __eq__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public bool __ne__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public bool __lt__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public bool __le__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public bool __gt__(PyClass other)
        {
            throw new NotImplementedException();
        }

        public bool __ge__(PyClass other)
        {
            throw new NotImplementedException();
        }
        
        public string __format__(string format)
        {
            throw new NotImplementedException();
        }

        public long __len__()
        {
            throw new NotImplementedException();
        }

        public bool __contains__(PyClass item)
        {
            throw new NotImplementedException();
        }

        public void __setattr__(string name, PyClass value)
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
    }
}