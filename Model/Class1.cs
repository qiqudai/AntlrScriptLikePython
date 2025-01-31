using System.Text;
namespace Model
{
    // 通用基类模板
    public abstract class DynamicBase
    {
        // 模拟动态字段，支持自定义扩展字段
        private List<object> _dynamicFields;

        protected DynamicBase(int fieldCount)
        {
            _dynamicFields = new List<object>(fieldCount);
        }

        // 高效访问动态字段，通过数组索引操作
        protected object GetField(int index) => _dynamicFields[index];
        protected void SetField(int index, object value)
        {
            // 确保索引不超过当前 list 的大小
            if (index >= _dynamicFields.Count)
            {
                // 动态增加字段
                for (int i = _dynamicFields.Count; i <= index; i++)
                {
                    _dynamicFields.Add(null); // 默认值可以设置为 null
                }
            }
            _dynamicFields[index] = value;
        }
    }
    
// 代码生成的具体子类
    public class MyClass : DynamicBase
    {
        // 提前定义静态字段索引
        private const int NameIndex = 0;
        private const int ValueIndex = 1;

        public MyClass() : base(2) { }

        // 定义静态字段的访问器
        public string Name
        {
            get => (string)GetField(NameIndex);
            set => SetField(NameIndex, value);
        }

        public int Value
        {
            get => (int)(GetField(ValueIndex) ?? 0);
            set => SetField(ValueIndex, value);
        }

        public void Display()
        {
            Console.WriteLine($"{Name}: {Value}");
        }
    }
    public partial class Class1
    {
        public Class1()
        { }

        private string _s = "";
        private int _i = 0;
        private decimal _d = 0;
        private StringBuilder _sb = new StringBuilder();
        public string S
        {
            set { _s = value; }
            get { return _s; }
        }

        public int I
        {
            set { _i = value; }
            get { return _i; }
        }

        public decimal D
        {
            set { _d = value; }
            get { return _d; }
        }

        public StringBuilder SB
        {
            set { _sb = value; }
            get { return _sb; }
        }

        public int func()
        {
            int s = 0;
            for (int i = 0; i < 1000; i++)
            {
                s = s + i;
            }
            return s;
        }
    }
}