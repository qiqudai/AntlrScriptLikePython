using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticSugar
{
    public class PyDict<K, V> : Dictionary<K, V>
    {
        // 使用键值对初始化
        public PyDict() : base() { }
        public PyDict(IEnumerable<KeyValuePair<K, V>> collection) : base(collection) { }

        // 字典推导式（字典推导式是一个功能，但 C# 中需要手动实现）
        public static PyDict<K, V> FromEnumerable(IEnumerable<KeyValuePair<K, V>> collection)
        {
            return new PyDict<K, V>(collection);
        }

        // + 运算符：合并两个字典
        public static PyDict<K, V> operator +(PyDict<K, V> a, PyDict<K, V> b)
        {
            var result = new PyDict<K, V>(a);
            foreach (var kvp in b)
            {
                result[kvp.Key] = kvp.Value; // 如果键重复，后面的值会覆盖前面的
            }
            return result;
        }

        // | 运算符：合并两个字典，right 字典中的值会覆盖 left 字典中的相同键
        public static PyDict<K, V> operator |(PyDict<K, V> left, PyDict<K, V> right)
        {
            // 创建一个新字典，将 left 字典的元素复制过去
            var result = new PyDict<K, V>(left);

            // 遍历 right 字典，并将其键值对添加到结果字典
            foreach (var kvp in right)
            {
                result[kvp.Key] = kvp.Value; // 如果键存在，right 字典的值会覆盖 left 字典的值
            }

            return result;
        }
        // |= 运算符：将 other 的键值对合并到 d 中，若有重复键，other 的值优先
        public static void orEqual(PyDict<K, V> left, pyint right)
        {
            if (right is PyDict<K, V> rightDict)
            {
                // 如果 right 是一个 PyDict 字典，直接合并
                foreach (var kvp in rightDict)
                {
                    left[kvp.Key] = kvp.Value; // 更新字典中的键值对
                }
            }
            else if (right is IEnumerable<KeyValuePair<K, V>> rightEnumerable)
            {
                // 如果 right 是一个键值对的集合，遍历并合并
                foreach (var kvp in rightEnumerable)
                {
                    left[kvp.Key] = kvp.Value; // 更新字典中的键值对
                }
            }
            else
            {
                throw new ArgumentException("The right operand must be a dictionary or an enumerable of key-value pairs.");
            }
        }

        // 解包字典为关键字参数
        public static void unpackAndPrint(Dictionary<string, object> d)
        {
            foreach (var kvp in d)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }

        // 解包多个字典合并
        public static PyDict<K, V> mergeDictionaries(params PyDict<K, V>[] dicts)
        {
            var merged = new PyDict<K, V>();
            foreach (var dict in dicts)
            {
                foreach (var kvp in dict)
                {
                    merged[kvp.Key] = kvp.Value;
                }
            }
            return merged;
        }

        // 模式匹配（C# 9.0 引入了 switch expressions，但不像 Python 完全支持模式匹配）
        public static void matchPattern(PyDict<K, V> dict, K key)
        {
            dict.TryGetValue(key, out var value);
        }

        // 重写 ToString 方法，支持打印字典
        public override string ToString()
        {
            return "{" + string.Join(", ", this.Select(kvp => $"{kvp.Key}: {kvp.Value}")) + "}";
        }

        // 获取指定键的值，如果不存在则返回默认值
        public V get(K key, V defaultValue = default)
        {
            return TryGetValue(key, out var value) ? value : defaultValue;
        }

        // 获取字典的所有键
        public IEnumerable<K> keys() => this.Keys;

        // 获取字典的所有值
        public IEnumerable<V> values() => this.Values;

        // 获取字典的所有键值对
        public IEnumerable<KeyValuePair<K, V>> items() => this;

        // 使用另一个字典更新当前字典
        public void update(PyDict<K, V> otherDict)
        {
            foreach (var kvp in otherDict)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        // 如果键不存在，则设置默认值并返回该值
        public V setdefault(K key, V defaultValue)
        {
            if (TryGetValue(key, out var value))
                return value;
            this[key] = defaultValue;
            return defaultValue;
        }

        // fromkeys() 方法：创建一个新字典，以序列 seq 中元素做字典的键，val 为字典所有键对应的初始值
        public static PyDict<K, V> fromkeys(IEnumerable<K> seq, V val)
        {
            var dict = new PyDict<K, V>();
            dict.EnsureCapacity(seq.Count());
            foreach (var key in seq)
            {
                dict[key] = val;  // 为每个键赋初始值
            }
            return dict;
        }
        // 移除指定键并返回其值；若不存在则可选返回默认值
        public V pop(K key, V defaultValue = default)
        {
            if (!this.TryGetValue(key, out V value))
            {
                return defaultValue;
            }
            // 删除键值对
            this.Remove(key);
            return value;
        }

        // 移除并返回一个（键，值）对，默认为最后一项
        public KeyValuePair<K, V> popitem()
        {
            var lastItem = this.Last();
            this.Remove(lastItem.Key);
            return lastItem;
        }

        // 创建一个新的浅拷贝
        public PyDict<K, V> copy()
        {
            return new PyDict<K, V>(this);
        }

        // 清空字典中的所有条目
        public void clear()
        {
            this.Clear();
        }

        // 测试代码
        public static void Test()
        {
            var dict = new PyDict<string, int>
            {
                { "apple", 1 },
                { "banana", 2 },
                { "cherry", 3 },
                { "date", 4 }
            };

            // 字典推导式 (通过 FromEnumerable 实现)
            var dictComprehension = PyDict<string, int>.FromEnumerable(
                new[] { "apple", "banana", "cherry", "date" }.Select(x => new KeyValuePair<string, int>(x, x.Length))
            );
            Console.WriteLine($"Dict Comprehension: {dictComprehension}");

            // + 运算符测试
            var dict2 = new PyDict<string, int>
            {
                { "elderberry", 5 },
                { "fig", 6 }
            };
            var merged = dict + dict2;
            Console.WriteLine($"Merged: {merged}");

            // .get() 测试
            Console.WriteLine($"Get apple: {dict.get("apple")}");
            Console.WriteLine($"Get unknown: {dict.get("unknown", -1)}");

            // .keys(), .values(), .items() 测试
            Console.WriteLine("Keys:");
            foreach (var key in dict.keys())
            {
                Console.WriteLine(key);
            }
            Console.WriteLine("Values:");
            foreach (var value in dict.values())
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("Items:");
            foreach (var kvp in dict.items())
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            // .update() 测试
            dict.update(new PyDict<string, int> { { "grape", 7 } });
            Console.WriteLine($"After Update: {dict}");

            // .setdefault() 测试
            dict.setdefault("kiwi", 8);
            Console.WriteLine($"After SetDefault: {dict}");

            // .pop() 测试
            var poppedValue = dict.pop("banana", -1);
            Console.WriteLine($"Popped Value: {poppedValue}");
            Console.WriteLine($"After Pop: {dict}");

            // .popitem() 测试
            var poppedItem = dict.popitem();
            Console.WriteLine($"Popped Item: {poppedItem.Key}: {poppedItem.Value}");
            Console.WriteLine($"After PopItem: {dict}");

            // .copy() 测试
            var dictCopy = dict.copy();
            Console.WriteLine($"Copy: {dictCopy}");

            // .clear() 测试
            dict.clear();
            Console.WriteLine($"After Clear: {dict}");

            // 解包测试
            var unpackedDict = new Dictionary<string, object> { { "key", "value" }, { "anotherKey", 42 } };
            PyDict<string, object>.unpackAndPrint(unpackedDict);

            // 合并多个字典
            var mergedDict = PyDict<string, int>.mergeDictionaries(dict, dict2);
            Console.WriteLine($"Merged Dictionaries: {mergedDict}");

            // 模式匹配测试
            // matchPattern(dict);
        }
    }
}
