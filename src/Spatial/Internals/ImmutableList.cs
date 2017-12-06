namespace MathNet.Spatial.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class ImmutableList<T> : IEnumerable<T>
    {
        internal static readonly ImmutableList<T> Empty = new ImmutableList<T>();

        private readonly T[] data;

        private ImmutableList()
        {
            this.data = new T[0];
        }

        internal ImmutableList(T[] data)
        {
            this.data = data;
        }

        public int Count => this.data.Length;

        public T this[int index] => this.data[index];

        public IEnumerator<T> GetEnumerator() => ((IList<T>)this.data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.data.GetEnumerator();

        internal ImmutableList<T> Add(T value)
        {
            var newData = new T[this.data.Length + 1];

            Array.Copy(this.data, newData, this.data.Length);
            newData[this.data.Length] = value;

            return new ImmutableList<T>(newData);
        }

        internal ImmutableList<T> Remove(T value)
        {
            var i = this.IndexOf(value);
            if (i < 0)
            {
                return this;
            }

            var length = this.data.Length;
            if (length == 1)
            {
                return Empty;
            }

            var newData = new T[length - 1];

            Array.Copy(this.data, 0, newData, 0, i);
            Array.Copy(this.data, i + 1, newData, i, length - i - 1);

            return new ImmutableList<T>(newData);
        }

        private int IndexOf(T value)
        {
            for (var i = 0; i < this.data.Length; ++i)
            {
                if (object.Equals(this.data[i], value))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
