using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticSugar
{
    // 可变集合 set
    public class Pyfrozenset<T> : HashSet<T>
    {
        // 创建 set 类的构造函数
        public Pyfrozenset() : base()
        {
        }

        public Pyfrozenset(IEnumerable<T> collection) : base(collection)
        {
        }

        // isdisjoint(other)
        public bool isdisjoint(Pyfrozenset<T> other)
        {
            return !this.Any(item => other.Contains(item));
        }

        // issubset(other)
        public bool issubset(Pyfrozenset<T> other)
        {
            return this.All(item => other.Contains(item));
        }

        // < 运算符（检测集合是否为真子集）
        public bool issubset_strict(Pyfrozenset<T> other)
        {
            return this.issubset(other) && this.Count != other.Count;
        }

        // issuperset(other)
        public bool issuperset(Pyfrozenset<T> other)
        {
            return other.All(item => this.Contains(item));
        }

        // > 运算符（检测集合是否为真超集）
        public bool issuperset_strict(Pyfrozenset<T> other)
        {
            return this.issuperset(other) && this.Count != other.Count;
        }

        // union(*others)
        public Pyfrozenset<T> union(params Pyfrozenset<T>[] others)
        {
            var result = new Pyfrozenset<T>(this);
            foreach (var other in others)
            {
                result.UnionWith(other);
            }

            return result;
        }

        // intersection(*others)
        public Pyfrozenset<T> intersection(params Pyfrozenset<T>[] others)
        {
            var result = new Pyfrozenset<T>(this);
            foreach (var other in others)
            {
                result.IntersectWith(other);
            }

            return result;
        }

        // difference(*others)
        public Pyfrozenset<T> difference(params Pyfrozenset<T>[] others)
        {
            var result = new Pyfrozenset<T>(this);
            foreach (var other in others)
            {
                result.ExceptWith(other);
            }

            return result;
        }

        // symmetric_difference(other)
        public Pyfrozenset<T> symmetric_difference(Pyfrozenset<T> other)
        {
            var result = new Pyfrozenset<T>(this);
            result.SymmetricExceptWith(other);
            return result;
        }

        // copy()
        public Pyfrozenset<T> copy()
        {
            return new Pyfrozenset<T>(this);
        }
    }

    public class Pyset<T> : Pyfrozenset<T>
    {
        
        public Pyset() : base()
        {
        }
        
        public Pyset(IEnumerable<T> collection) : base(collection)
        {
        }
        
        // update(*others)
        public void update(params Pyfrozenset<T>[] others)
        {
            foreach (var other in others)
            {
                this.UnionWith(other);
            }
        }

        // intersection_update(*others)
        public void intersection_update(params Pyfrozenset<T>[] others)
        {
            foreach (var other in others)
            {
                this.IntersectWith(other);
            }
        }

        // difference_update(*others)
        public void difference_update(params Pyfrozenset<T>[] others)
        {
            foreach (var other in others)
            {
                this.ExceptWith(other);
            }
        }

        // symmetric_difference_update(other)
        public void symmetric_difference_update(Pyfrozenset<T> other)
        {
            this.SymmetricExceptWith(other);
        }

        // add(elem)
        public void add(T elem)
        {
            this.Add(elem);
        }

        // remove(elem)
        public void remove(T elem)
        {
            this.Remove(elem);
        }

        // discard(elem)
        public void discard(T elem)
        {
            this.Remove(elem); // discard 是不抛出异常的
        }

        // pop()
        public T pop()
        {
            if (this.Count == 0) throw new InvalidOperationException("Set is empty.");
            var element = this.First();
            this.Remove(element);
            return element;
        }

        // clear()
        public void clear()
        {
            this.Clear();
        }
    }
    
}
