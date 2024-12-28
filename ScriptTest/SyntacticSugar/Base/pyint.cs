using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SyntacticSugar
{

    public record struct pyint: IPyObject
    {
        private IntPtr _value; // 核心存储


        // 构造函数（整型）
        public pyint(long value)
        {
            _value = new IntPtr(value);
        }

        // 构造函数（浮点型）
        public pyint(double value)
        {
            _value = new IntPtr((long)value);
        }

        // 构造函数（浮点型）
        public pyint(bool value)
        {
            _value = new IntPtr(value ? 1 : 0);
        }

        // 将 PyNumber 转换为 long
        public long ToLong() => _value.ToInt64();

        // 将 PyNumber 转换为 double
        public double ToDouble() => (double)_value.ToInt64();

        public override string ToString() => _value.ToString();


        #region 接口实现
        
        // 重载加法操作符
        public static pyint operator +(pyint a, pyint b) => new pyint(a.ToLong() + b.ToLong());
        public static pyint operator +(pyint a, long b) => new pyint(a.ToLong() + b);
        public static pyint operator +(pyint a, int b) => new pyint(a.ToLong() + b);
        public static pyint operator +(long a, pyint b) => new pyint(a + b.ToLong());
        public static pyint operator +(int a, pyint b) => new pyint(a + b.ToLong());
        public static pyint operator +(float a, pyint b) => new pyint(a + b.ToDouble());
        public static pyint operator +(pyint a, float b) => new pyint(a.ToDouble() + b);
        public static pyint operator +(pyint a, double b) => new pyint(a.ToDouble() + b);
        public static pyint operator +(double a, pyint b) => new pyint(a + b.ToDouble());

        // 重载减法操作符
        public static pyint operator -(pyint a, pyint b) => new pyint(a.ToLong() - b.ToLong());
        public static pyint operator -(pyint a, long b) => new pyint(a.ToLong() - b);
        public static pyint operator -(pyint a, int b) => new pyint(a.ToLong() - b);
        public static pyint operator -(long a, pyint b) => new pyint(a - b.ToLong());
        public static pyint operator -(int a, pyint b) => new pyint(a - b.ToLong());
        public static pyint operator -(float a, pyint b) => new pyint(a - b.ToDouble());
        public static pyint operator -(pyint a, double b) => new pyint(a.ToDouble() - b);
        public static pyint operator -(pyint a, float b) => new pyint(a.ToDouble() - b);
        public static pyint operator -(double a, pyint b) => new pyint(a - b.ToDouble());

        // 重载乘法操作符
        public static pyint operator *(pyint a, pyint b) => new pyint(a.ToLong() * b.ToLong());
        public static pyint operator *(pyint a, long b) => new pyint(a.ToLong() * b);
        public static pyint operator *(pyint a, int b) => new pyint(a.ToLong() * b);
        public static pyint operator *(long a, pyint b) => new pyint(a * b.ToLong());
        public static pyint operator *(int a, pyint b) => new pyint(a * b.ToLong());
        public static pyint operator *(float a, pyint b) => new pyint(a * b.ToDouble());
        public static pyint operator *(pyint a, double b) => new pyint(a.ToDouble() * b);
        public static pyint operator *(pyint a, float b) => new pyint(a.ToDouble() * b);
        public static pyint operator *(double a, pyint b) => new pyint(a * b.ToDouble());

        // 重载除法操作符
        public static pyint operator /(pyint a, pyint b) => new pyint(a.ToLong() / b.ToLong());
        public static pyint operator /(pyint a, long b) => new pyint(a.ToLong() / b);
        public static pyint operator /(pyint a, int b) => new pyint(a.ToLong() / b);
        public static pyint operator /(long a, pyint b) => new pyint(a / b.ToLong());
        public static pyint operator /(int a, pyint b) => new pyint(a / b.ToLong());
        public static pyint operator /(float a, pyint b) => new pyint(a / b.ToDouble());
        public static pyint operator /(pyint a, double b) => new pyint(a.ToDouble() / b);
        public static pyint operator /(pyint a, float b) => new pyint(a.ToDouble() / b);
        public static pyint operator /(double a, pyint b) => new pyint(a / b.ToDouble());

        // 隐式转换为 long
        public static implicit operator long(pyint number) => number.ToLong();

        // 隐式转换为 double
        public static implicit operator double(pyint number) => number.ToDouble();
        public static implicit operator float(pyint number) => (float)number.ToDouble();

        // 隐式转换为 PyNumber 从 long
        public static implicit operator pyint(long value) => new pyint(value);
        public static implicit operator pyint(int value) => new pyint(value);
        public static implicit operator pyint(uint value) => new pyint(value);
        public static implicit operator pyint(ulong value) => new pyint(value);
        public static implicit operator pyint(byte value) => new pyint(value);
        public static implicit operator pyint(bool value) => new pyint(value);
        public static implicit operator pyint(char value) => new pyint(value);
        public static implicit operator pyint(short value) => new pyint(value);
        public static implicit operator pyint(ushort value) => new pyint(value);

        // 隐式转换为 PyNumber 从 double
        public static implicit operator pyint(double value) => new pyint(value);
        public static implicit operator pyint(float value) => new pyint(value);

        #endregion
        
        public static void TEST()
        {
            // 创建 PyNumber 类型的实例
            pyint a = new pyint(5);
            pyint b = new pyint(3.14);
            pyint c = new pyint(true);
            a /= 5;
            a *= 5; 
            // 测试加法
            pyint addResult1 = a + b;  // 5 + 3.14
            pyint addResult2 = a + 10; // 5 + 10
            pyint addResult3 = 10 + b; // 10 + 3.14

            // 测试减法
            pyint subResult1 = a - b;  // 5 - 3.14
            pyint subResult2 = a - 10; // 5 - 10
            pyint subResult3 = 10 - b; // 10 - 3.14

            // 测试乘法
            pyint mulResult1 = a * b;  // 5 * 3.14
            pyint mulResult2 = a * 10f; // 5 * 10
            pyint mulResult3 = 10 * b; // 10 * 3.14

            // 测试除法
            pyint divResult1 = a / b;  // 5 / 3.14
            pyint divResult2 = a / 10; // 5 / 10
            pyint divResult3 = 10 / b; // 10 / 3.14

            // 输出加法结果
            Console.WriteLine("Add Results:");
            Console.WriteLine(addResult1); // 输出：8.14
            Console.WriteLine(addResult2); // 输出：15
            Console.WriteLine(addResult3); // 输出：13.14

            // 输出减法结果
            Console.WriteLine("Subtract Results:");
            Console.WriteLine(subResult1); // 输出：1.86
            Console.WriteLine(subResult2); // 输出：-5
            Console.WriteLine(subResult3); // 输出：6.86

            // 输出乘法结果
            Console.WriteLine("Multiply Results:");
            Console.WriteLine(mulResult1); // 输出：15.7
            Console.WriteLine(mulResult2); // 输出：50
            Console.WriteLine(mulResult3); // 输出：31.4

            // 输出除法结果
            Console.WriteLine("Divide Results:");
            Console.WriteLine(divResult1); // 输出：1.592356687898089
            Console.WriteLine(divResult2); // 输出：0.5
            Console.WriteLine(divResult3); // 输出：3.184713375796178

            // 测试隐式转换
            long longResult = a;  // 隐式转换为 long
            double doubleResult = b;  // 隐式转换为 double

            Console.WriteLine($"Long Result: {longResult}");  // 输出：5
            Console.WriteLine($"Double Result: {doubleResult}");  // 输出：3.14
        }

        public void __init__()
        {
            throw new NotImplementedException();
        }

        public void __del__()
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

        public int __hash__()
        {
            throw new NotImplementedException();
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

        public IPyObject __radd__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __iadd__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __sub__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __rsub__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __isub__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __mul__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __rmul__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __imul__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __truediv__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __rtruediv__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __itruediv__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __floordiv__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __rfloordiv__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __ifloordiv__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __mod__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __rmod__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __imod__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public IPyObject __pow__(IPyObject other, IPyObject mod = null)
        {
            throw new NotImplementedException();
        }

        public IPyObject __rpow__(IPyObject other, IPyObject mod = null)
        {
            throw new NotImplementedException();
        }

        public IPyObject __ipow__(IPyObject other, IPyObject mod = null)
        {
            throw new NotImplementedException();
        }

        public IPyObject __neg__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __pos()
        {
            throw new NotImplementedException();
        }

        public IPyObject __abs__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __round__(int? n = null)
        {
            throw new NotImplementedException();
        }

        public IPyObject __trunc__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __floor__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __ceil__()
        {
            throw new NotImplementedException();
        }

        public int __int__()
        {
            throw new NotImplementedException();
        }

        public double __float__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __complex__()
        {
            throw new NotImplementedException();
        }

        public int __index__()
        {
            throw new NotImplementedException();
        }

        public string __format__(string format)
        {
            throw new NotImplementedException();
        }

        public byte[] __bytes__()
        {
            throw new NotImplementedException();
        }

        public int __len__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __getitem__(IPyObject key)
        {
            throw new NotImplementedException();
        }

        public void __setitem__(IPyObject key, IPyObject value)
        {
            throw new NotImplementedException();
        }

        public void __delitem__(IPyObject key)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IPyObject> __iter__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __next__()
        {
            throw new NotImplementedException();
        }

        public bool __contains__(IPyObject item)
        {
            throw new NotImplementedException();
        }

        public IPyObject __enter__()
        {
            throw new NotImplementedException();
        }

        public void __exit__(Type excType, object excValue, object traceback)
        {
            throw new NotImplementedException();
        }

        public IPyObject __call__(params IPyObject[] args)
        {
            throw new NotImplementedException();
        }

        public IPyObject __getattr__(string name)
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

        public IPyObject __get__(IPyObject obj, Type type = null)
        {
            throw new NotImplementedException();
        }

        public void __set__(IPyObject obj, IPyObject value)
        {
            throw new NotImplementedException();
        }

        public void __delete__(IPyObject obj)
        {
            throw new NotImplementedException();
        }

        public IPyObject __getstate__()
        {
            throw new NotImplementedException();
        }

        public void __setstate__(IPyObject state)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IPyObject> __await__()
        {
            throw new NotImplementedException();
        }

        public IPyObject __aenter__()
        {
            throw new NotImplementedException();
        }

        public void __aexit__(Type excType, object excValue, object traceback)
        {
            throw new NotImplementedException();
        }

        public bool __bool__()
        {
            throw new NotImplementedException();
        }

        public bool __is__(IPyObject other)
        {
            throw new NotImplementedException();
        }

        public bool __is_not__(IPyObject other)
        {
            throw new NotImplementedException();
        }
    }
}