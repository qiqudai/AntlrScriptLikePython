using System.Numerics;
using System.Text;

namespace SyntacticSugar
{


    public static class systemFunc
    {

        // A
        public static int abs(int x) => Math.Abs(x);

        /*
        public static IAsyncEnumerator<T> aiter<T>(IEnumerable<T> collection) =>
            collection.ToAsyncEnumerable().GetAsyncEnumerator();*/

        public static bool all<T>(IEnumerable<T> collection) => collection.All(item => Convert.ToBoolean(item));

        public static T anext<T>(IAsyncEnumerator<T> enumerator) =>
            enumerator.MoveNextAsync().Result ? enumerator.Current : default;

        public static bool any<T>(IEnumerable<T> collection) => collection.Any(item => Convert.ToBoolean(item));
        public static string ascii(object obj)
        {
            // 将对象转换为字符串
            string str = obj.ToString();
    
            // 创建一个 StringBuilder 来构建输出
            var builder = new StringBuilder();

            foreach (char c in str)
            {
                if (c >= 32 && c <= 126)  // 可打印的ASCII字符
                {
                    builder.Append(c);
                }
                else
                {
                    builder.AppendFormat("\\x{0:X2}", (int)c);  // 非ASCII字符用 \xHH 格式表示
                }
            }
            return builder.ToString();
        }


        // B
        public static string bin(int x) => Convert.ToString(x, 2);
        public static bool bool_(object obj) => Convert.ToBoolean(obj);
        public static void breakpoint() => System.Diagnostics.Debugger.Break();
        public static byte[] bytearray(string s) => Encoding.UTF8.GetBytes(s);
        public static byte[] bytes(string s) => Encoding.UTF8.GetBytes(s);

        // C
        public static bool callable(object obj) => obj is Delegate;
        public static string chr(int code) => Convert.ToChar(code).ToString();
        public static Type classmethod(Type target) => target;
        public static string compile(string code, string filename, string mode) => code; // Simplified version.

        public static Complex complex(object real, object imag) =>
            new Complex(Convert.ToDouble(real), Convert.ToDouble(imag));

        // D
        public static void delattr(object obj, string name) => obj.GetType().GetProperty(name)?.SetValue(obj, null);

        public static Dictionary<TKey, TValue> dict<TKey, TValue>(params KeyValuePair<TKey, TValue>[] items) =>
            new Dictionary<TKey, TValue>(items);

        public static IEnumerable<string> dir(object obj) => obj.GetType().GetProperties().Select(p => p.Name);
        public static Tuple<int, int> divmod(int x, int y) => Tuple.Create(x / y, x % y);

        // E
        /*
        public static IEnumerable<T> enumerate<T>(IEnumerable<T> collection) =>
            collection.Select((value, index) => new { index, value });

        public static object eval(string expression) =>
            new System.CodeDom.Compiler.CodeDomProvider().CompileAssemblyFromSource(
                new System.CodeDom.Compiler.CompilerParameters(), expression);
                */

        public static void exec(string code) =>
            System.Reflection.Assembly.Load(new System.Text.UTF8Encoding().GetBytes(code));

        // F
        public static IEnumerable<T> filter<T>(Func<T, bool> predicate, IEnumerable<T> collection) =>
            collection.Where(predicate);

        public static float float_(object obj) => Convert.ToSingle(obj);
        public static string format(object value, string format) => string.Format(format, value);
        public static IReadOnlyCollection<T> frozenset<T>(IEnumerable<T> collection) => new HashSet<T>(collection);

        // G
        public static object getattr(object obj, string name) => obj.GetType().GetProperty(name)?.GetValue(obj);

        public static IDictionary<string, object> globals() => default;

        // H
        public static bool hasattr(object obj, string name) => obj.GetType().GetProperty(name) != null;
        public static int hash(object obj) => obj.GetHashCode();
        public static void help(object obj) => Console.WriteLine(obj);
        public static string hex(int x) => Convert.ToString(x, 16);

        // I
        public static int id(object obj) => obj.GetHashCode();

        public static string input(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        public static int int_(object obj) => Convert.ToInt32(obj);
        public static bool isinstance(object obj, Type type) => type.IsInstanceOfType(obj);
        public static bool issubclass(Type type, Type parent) => type.IsSubclassOf(parent);
        public static IEnumerable<T> iter<T>(IEnumerable<T> collection) => collection;
        public static int len<T>(IEnumerable<T> collection) => collection.Count();
        public static List<T> list<T>(params T[] items) => new List<T>(items);

        public static IDictionary<string, object> locals() => default;

        // M
        public static IEnumerable<T> map<T>(Func<T, T> func, IEnumerable<T> collection) => collection.Select(func);
        public static T max<T>(IEnumerable<T> collection) => collection.Max();
        public static Memory<byte> memoryview(byte[] array) => new Memory<byte>(array);
        public static T min<T>(IEnumerable<T> collection) => collection.Min();

        // N
        public static T next<T>(IEnumerator<T> enumerator) => enumerator.MoveNext() ? enumerator.Current : default;

        // O
        public static object object_() => new object();
        public static string oct(int x) => Convert.ToString(x, 8);
        public static StreamReader open(string filename) => new StreamReader(filename);
        public static int ord(string s) => Convert.ToInt32(s[0]);

        // P
        public static int pow(int x, int y) => (int)Math.Pow(x, y);
        
        public static void Print(object[] args = null, string sep = " ", string end = "\n", TextWriter file = null,
            bool? flush = null)
        {
            // 默认输出到 Console
            file ??= Console.Out;
            // 如果没有参数，打印 end
            if (args == null || args.Length == 0)
            {
                file.Write(end);
                return;
            }
            // 将所有参数转换为字符串，并使用 sep 连接
            string output = string.Join(sep, Array.ConvertAll(args, arg => arg.ToString()));
            // 写入输出流
            file.Write(output);
            // 如果 flush 为 true，则强制刷新
            if (flush.HasValue && flush.Value)
            {
                file.Flush();
            }
            // 在输出的末尾添加 end
            file.Write(end);
        }
        
        public static object property(object getter) => getter;

        // R
        public static IEnumerable<int> range(int start, int stop, int step = 1) => Enumerable
            .Range(start, (stop - start + step - 1) / step).Where(x => (x - start) % step == 0);

        public static string repr(object obj) => obj.ToString();
        public static IEnumerable<T> reversed<T>(IEnumerable<T> collection) => collection.Reverse();
        public static long round(double value) => (long)Math.Round(value);

        // S
        /*public static HashSet<T> set<T>(params T[] items) => new HashSet<T>(items);*/

        public static void setattr(object obj, string name, object value) =>
            obj.GetType().GetProperty(name)?.SetValue(obj, value);

        /*public static object slice(int start, int stop, int step = 1) => new Range(start, stop);*/
        public static IEnumerable<T> sorted<T>(IEnumerable<T> collection) => collection.OrderBy(x => x);
        public static T staticmethod<T>(T method) => method;
        public static string str(object obj) => obj.ToString();

        /*public static T sum<T>(IEnumerable<T> collection) where T: struct =>
            collection.Aggregate((T)null, (a, b) => (T)a + (T)b);

        public static object super(object obj) => obj.GetType().BaseType;*/

        // T
        /*public static Tuple<T1, T2> tuple<T1, T2>(T1 item1, T2 item2) => Tuple.Create(item1, item2);
        public static Type type(object obj) => obj.GetType();*/

        // V
        /*public static IDictionary<string, object> vars(object obj) =>
            obj.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj));

        // Z
        public static IEnumerable<Tuple<T1, T2>> zip<T1, T2>(IEnumerable<T1> collection1, IEnumerable<T2> collection2)
        {
            return collection1.Zip(collection2, (item1, item2) => Tuple.Create(item1, item2));
        }*/
    }
}