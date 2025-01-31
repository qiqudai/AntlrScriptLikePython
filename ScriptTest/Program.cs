using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CodeTransfer;
// using Pytocs.Core;
using SyntacticSugar; // 用于访问语法树


using System.Collections;

using System.Collections.Generic;

using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Model;

partial class Program
{
    // static private Logger _logger = new Logger("python");
    static void Main(string[] args)
    {
        long longValue = 0x3FF0000000000000; // IEEE 754 二进制表示的 1.0
        double doubleValue = BitConverter.Int64BitsToDouble(longValue);

        // TestDynamic();
        // TestClass1();
        // Console.WriteLine($"Long Value: {longValue}");
        // Console.WriteLine($"Interpreted Double: {doubleValue}");


        double floatValue = longValue;

        // Console.WriteLine($"Long Value: {longValue}");
        // Console.WriteLine($"Converted Float: {floatValue}");

        // tuple.Test();
        list.Test();
        // set.Test();
        dict.Test();
        pyint.TEST();
        // Pyurl.TEST();
        TestPython();
        return;
        Task.Factory.StartNew((async () =>
        {
           await TestPython3();
        }));
        TestTimer.Reload(TestTimer.code);
        int i = 1;
        while (true)
        {
            i++;
            TestTimer.Run();
            if (i == 10000)
            {
                TestTimer.Reload(TestTimer.code2);
            }
        }
    }

    static async Task TestPython3()
    {
       // await ConversionUtils.ConvertFolderAsync("py", "cs", _logger);
    }
    public static void TestPython()
    {
        
        // 1. 指定待解析的 Python 脚本路径
        string scriptPath = "py\\basec.py"; // 替换为你的 Python 脚本路径

        // 2. 读取脚本内容
        string script = File.ReadAllText(scriptPath);

        // 3. 创建输入流
        AntlrInputStream inputStream = new AntlrInputStream(script);

        // 4. 创建词法分析器 (Lexer)
        var lexer = new Python3Lexer(inputStream);

        // 5. 创建词法标记流 (Token Stream)
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);

        // 6. 创建语法分析器 (Parser)
        var parser = new Python3Parser(tokenStream); // 创建一个自定义的错误监听器
        var errorListener = new ErrorListener();

// 为解析器添加错误监听器
        parser.RemoveErrorListeners(); // 移除默认的错误监听器
        parser.AddErrorListener(errorListener);
        // 7. 指定语法规则的入口点
        //    对 Python3Parser 来说，入口规则通常是 "file_input"（取决于你的 .g4 文件定义）
        IParseTree parseTree = parser.file_input();
// 如果有错误，打印错误信息
        if (errorListener.Errors.Any())
        {
            Console.WriteLine("Parsing errors:");
            foreach (var error in errorListener.Errors)
            {
                Console.WriteLine(error);
            }
        }
        else
        {
            Console.WriteLine("Parsing succeeded!");
        }

        var visitor = new PythonToCSharpConverter();
        var csharpCode = visitor.VisitFile_input((Python3Parser.File_inputContext)parseTree);
        // Print the C# code
        Console.WriteLine(
            "//=============================================C#=============================================");
        Console.WriteLine(csharpCode);
        //
        // var listener = new CustomPython3Listener();
        //
        // ParseTreeWalker.Default.Walk(listener, parseTree);
        // var converter = new PythonToCSharpConverter();
        // converter.Visit(parseTree);
        // Console.WriteLine(parseTree.ToStringTree(parser));
    }


    public class DynamicSample
    {
        public string Name { get; set; }
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
    
    public static void TestDynamic()
    {
        DynamicSample dynamicSample = new DynamicSample();
        //通过反射得到DynamicSample的方法
        var add = dynamicSample.GetType().GetMethod("Add");
        Stopwatch watch = new Stopwatch();
        watch.Start();
        for (int i = 0; i < 100000; i++)
        {
            int re = (int)add.Invoke(dynamicSample, new object[] { 1, 2 });
        }
        watch.Stop();
        Console.WriteLine("反射：" + watch.ElapsedMilliseconds);//200毫秒左右
        
        
        dynamic dynamicSample2 = new DynamicSample();
        watch.Restart();
        for (int i = 0; i < 100000; i++)
        {
            int re = dynamicSample2.Add(1, 2);
        }
        watch.Stop();
        Console.WriteLine("动态：" +watch.ElapsedMilliseconds);//50毫秒左右
        
        
        watch.Restart();
        for (int i = 0; i < 100000; i++)
        {
            int re = dynamicSample.Add(1, 2);
        }
        watch.Stop();
        Console.WriteLine("原生：" +watch.ElapsedMilliseconds);//50毫秒左右
    }
    
        static void TestClass1()
        {
            const int count = 100000;
            Console.WriteLine("count="+count);

            Stopwatch sw = new Stopwatch();
            StringBuilder test = new StringBuilder();
            sw.Restart();
            {
                Class1 m = new Class1();
                m.SB = new StringBuilder("StringBuilder");
                for (int i = 0; i < count; i++)
                {
                    m.D = 100;
                    m.I = 200;
                    m.S = "string";

                    test.Append(m.D);
                    test.Append(m.I);
                    test.Append(m.S);
                    test.Append(m.SB);
                }
            }
            sw.Stop();
            Console.WriteLine("直接创建,赋值并访问内存中对象属性：   " + sw.ElapsedTicks);
            //Console.WriteLine("直接创建,赋值并访问内存中对象属性：   " + sw.ElapsedTicks);
            long basic = sw.ElapsedTicks;

            sw.Restart();
            {
                var t = typeof(Class1);
                ConstructorInfo ct = t.GetConstructor(new Type[] { });
                var m = ct.Invoke(null);
                test = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    PropertyInfo pd = t.GetProperty("D");
                    pd.SetValue(m, 100m, null);
                    PropertyInfo pi = t.GetProperty("I");
                    pi.SetValue(m, 200, null);
                    PropertyInfo ps = t.GetProperty("S");
                    ps.SetValue(m, "string", null);
                    PropertyInfo psb = t.GetProperty("StringBuilder");

                    PropertyInfo[] props = m.GetType().GetProperties();
                    foreach (var p in props)
                    {
                        test.Append(p.GetValue(m, null));
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("反射创建，赋值并访问内存中对象属性：   " + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");


            test = new StringBuilder();
            {
                sw.Restart();
                dynamic dm = new Class1();
                dm.SB = new StringBuilder("StringBuilder");
                for (int i = 0; i < count; i++)
                {
                    dm.D = 100;
                    dm.I = 200;
                    dm.S = "string";

                    test.Append(dm.D);
                    test.Append(dm.I);
                    test.Append(dm.S);
                    test.Append(dm.SB);
                }
            }
            sw.Stop();
            Console.WriteLine("dynamic直接创建，赋值并访问内存中对象属性：" + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");

            test = new StringBuilder();
            {
                sw.Restart();
                dynamic dm = new ExpandoObject();
                dm.SB = new StringBuilder("StringBuilder");
                for (int i = 0; i < count; i++)
                {
                    dm.D = 100;
                    dm.I = 200;
                    dm.S = "string";

                    test.Append(dm.D);
                    test.Append(dm.I);
                    test.Append(dm.S);
                    test.Append(dm.SB);
                }
            }
            sw.Stop();
            Console.WriteLine("ExpandoObject直接创建，赋值并访问内存中对象属性：" + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");

            test = new StringBuilder();
            {
                sw.Restart();
                dynamic dm = new Class1();
                dm.SB = new StringBuilder("StringBuilder");
                for (int i = 0; i < count; i++)
                {
                    dm.D = 100;
                    dm.I = 200;
                    dm.S = "string";

                    test.Append(dm.D);
                    test.Append(dm.I);
                    test.Append(dm.S);
                    test.Append(dm.SB);
                }
            }
            sw.Stop();
            Console.WriteLine("ExpandoObject直接创建，赋值并访问内存中对象属性：" + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");

            test = new StringBuilder();
            {
                var t = typeof(Class1);
                ConstructorInfo ct = t.GetConstructor(new Type[] { });
                dynamic dm = ct.Invoke(null);
                dm.SB = new StringBuilder("StringBuilder");
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    dm.D = 100;
                    dm.I = 200;
                    dm.S = "string";
                    test.Append(dm.D);
                    test.Append(dm.I);
                    test.Append(dm.S);
                    test.Append(dm.SB);
                }
            }
            sw.Stop();
            Console.WriteLine("dynamic反射创建，赋值并访问内存中对象属性：" + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");

            test = new StringBuilder();
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                Class1 m = new Class1();
                test.Append(m.func());
            }
            sw.Stop();
            Console.WriteLine("直接调用内存中对象方法：   " + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");


            sw.Restart();
            test = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                var t = typeof(Class1);
                MethodInfo meth = t.GetMethod("func");
                ConstructorInfo ct = t.GetConstructor(new Type[] { });
                test.Append(meth.Invoke(ct.Invoke(null), null));
            }
            sw.Stop();
            Console.WriteLine("反射调用内存中对象方法：   " + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");


            test = new StringBuilder();
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                dynamic dm = new Class1();
                test.Append(dm.func());
            }
            sw.Stop();
            Console.WriteLine("dynamic直接调用内存中对象方法：" + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");

            test = new StringBuilder();
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                var t = typeof(Class1);
                ConstructorInfo ct = t.GetConstructor(new Type[] { });
                dynamic dm = ct.Invoke(null);
                test.Append(dm.func());
            }
            sw.Stop();
            Console.WriteLine("dynamic反射调用内存中对象方法：" + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");



            Console.WriteLine("###############################");

            sw.Restart();
            test = new StringBuilder();
            for (int i = 0; i < count; i++)
            {

                Assembly a = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + @"\Model.dll");
                var t = a.GetType("Model.Class1");
                ConstructorInfo ct = t.GetConstructor(new Type[] { });
                var rm = ct.Invoke(null);
                PropertyInfo pd = t.GetProperty("D");
                pd.SetValue(rm, 100m, null);
                PropertyInfo pi = t.GetProperty("I");
                pi.SetValue(rm, 200, null);
                PropertyInfo ps = t.GetProperty("S");
                ps.SetValue(rm, "string", null);
                PropertyInfo psb = t.GetProperty("SB");
                psb.SetValue(rm, new StringBuilder("StringBuilder"), null);

                PropertyInfo[] props = t.GetProperties();
                foreach (var p in props)
                {
                    test.Append(p.GetValue(rm, null));
                }
            }
            sw.Stop();
            Console.WriteLine("使用反射加载程序集，创建，赋值，访问，对象属性：   " + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");

            dynamic ta = (1, 2, 3, 4, "fff", "ddd", (1, 2, 3));

            sw.Restart();
            test = new StringBuilder();
            for (int i = 0; i < count; i++)
            {

                Assembly a = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + @"\Model.dll");
                var t = a.GetType("Model.Class1");
                ConstructorInfo ct = t.GetConstructor(new Type[] { });
                dynamic dm = ct.Invoke(null);
                dm.D = 100m;
                dm.I = 200;
                dm.S = "string";
                dm.SB = new StringBuilder("StringBuilder");

                test.Append(dm.D);
                test.Append(dm.I);
                test.Append(dm.S);
                test.Append(dm.SB);

            }
            sw.Stop();
            Console.WriteLine("dynamic赋值，使用反射加载程序集，创建，赋值，访问，对象属性：   " + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");
            
            sw.Restart();
            dict dict = new dict();
            test = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                dict["D"] = 100;
                dict["I"] = 200;
                dict["S"] = "string";
                dict["SB"] = new StringBuilder("StringBuilder").ToString();
                test.Append(dict["D"].ToString());
                test.Append(dict["I"].ToString());
                test.Append(dict["S"].ToString());
                test.Append(dict["SB"].ToString());
            }
            sw.Stop();
            Console.WriteLine("dict调用内存中对象方法：   " + sw.ElapsedTicks + "(" + (float)sw.ElapsedTicks / basic + ")");

        }
}

