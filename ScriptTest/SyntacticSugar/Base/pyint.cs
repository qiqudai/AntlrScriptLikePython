using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SyntacticSugar
{

    
    public record struct pyint
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
        public static pyint operator /(pyint a, double b) => new pyint(a.ToDouble() / b);
        public static pyint operator /(pyint a, float b) => new pyint(a.ToDouble() / b);
        public static pyint operator /(double a, pyint b) => new pyint(a / b.ToDouble());
        public static pyint operator /(float a, pyint b) => new pyint(a / b.ToDouble());

        // 隐式转换为 long
        public static implicit operator long(pyint number) => number.ToLong();

        // 隐式转换为 double
        //public static implicit operator double(pyint number) => number.ToDouble();
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
            pyint divResult4 = a / 1.0;  // 5 / 3.14
            pyint divResult2 = (a / 10.1); // 5 / 10
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

        public string __repr__()
        {
            return _value.ToString();
        }

        public string __str__()
        {
            return _value.ToString();
        }

        public long __hash__()
        {
            return _value;
        }
        public bool __eq__(pyint other)
        {
            return _value == other._value;
        }

        public bool __ne__(pyint other)
        {
            return _value != other._value;
        }

        public bool __lt__(pyint other)
        {
            return _value < other._value;
        }

        public bool __le__(pyint other)
        {
            return _value <= other._value;
        }

        public bool __gt__(pyint other)
        {
            return _value > other._value;
        }

        public bool __ge__(pyint other)
        {
            return _value >= other._value;
        }
        public pyint __add__(pyint other)
        {
            return new pyint((IntPtr)(this._value + other._value));
        }

        public pyint __radd__(pyint other)
        {
            return __add__(other);  // Reverse addition is same as normal addition
        }

        public pyint __iadd__(pyint other)
        {
            _value = (IntPtr)(this._value + other._value);
            return this;
        }

        public pyint __sub__(pyint other)
        {
            return new pyint((IntPtr)(this._value - other._value));
        }

        public pyint __rsub__(pyint other)
        {
            return new pyint((IntPtr)(other._value - this._value));
        }

        public pyint __isub__(pyint other)
        {
            _value = (IntPtr)(this._value - other._value);
            return this;
        }

        public pyint __mul__(pyint other)
        {
            return new pyint((IntPtr)(this._value * other._value));
        }

        public pyint __rmul__(pyint other)
        {
            return __mul__(other);  // Reverse multiplication is same as normal multiplication
        }

        public pyint __imul__(pyint other)
        {
            _value = (IntPtr)(this._value * other._value);
            return this;
        }

        public pyint __truediv__(pyint other)
        {
            if (other._value == 0) throw new DivideByZeroException();
            return new pyint((IntPtr)(this._value / other._value));
        }

        public pyint __rtruediv__(pyint other)
        {
            if (this._value == 0) throw new DivideByZeroException();
            return new pyint((IntPtr)(other._value / this._value));
        }

        public pyint __itruediv__(pyint other)
        {
            if (other._value == 0) throw new DivideByZeroException();
            _value = (IntPtr)(this._value / other._value);
            return this;
        }

        public pyint __floordiv__(pyint other)
        {
            return new pyint((IntPtr)(this._value / other._value));
        }

        public pyint __rfloordiv__(pyint other)
        {
            return new pyint((IntPtr)(other._value / this._value));
        }

        public pyint __ifloordiv__(pyint other)
        {
            _value = (IntPtr)(this._value / other._value);
            return this;
        }

        public pyint __mod__(pyint other)
        {
            return new pyint((IntPtr)(this._value % other._value));
        }

        public pyint __rmod__(pyint other)
        {
            return new pyint((IntPtr)(other._value % this._value));
        }

        public pyint __imod__(pyint other)
        {
            _value = (IntPtr)(this._value % other._value);
            return this;
        }

        public pyint __pow__(pyint other, pyint mod)
        {
            return new pyint((IntPtr)(Math.Pow(this._value, other._value) % mod._value));
        }

        public pyint __rpow__(pyint other, pyint mod)
        {
            return new pyint((IntPtr)(Math.Pow(other._value, this._value) % mod._value));
        }

        public pyint __ipow__(pyint other, pyint mod)
        {
            _value = (IntPtr)(Math.Pow(this._value, other._value) % mod._value);
            return this;
        }

        public pyint __neg__()
        {
            return new pyint(-this._value);
        }
        public string __format__(string format)
        {
            return _value.ToString(format);
        }
        public byte[] __bytes__()
        {
            return BitConverter.GetBytes(_value);
        }
        public int __len__()
        {
            return sizeof(long);
        }


        public void __delattr__(string name)
        {
            _value = IntPtr.Zero;
        }

        public bool __getattribute__(string name)
        {
            return true;
        }

    }
}