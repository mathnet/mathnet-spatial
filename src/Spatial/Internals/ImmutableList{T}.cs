namespace MathNet.Spatial.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    internal sealed class ImmutableList<T> : IEnumerable<T>
    {
        internal static readonly ImmutableList<T> Empty = new ImmutableList<T>(new T[0]);

        private readonly T[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableList{T}"/> class.
        /// </summary>
        /// <param name="data">The</param>
        private ImmutableList(T[] data)
        {
            this.data = data;
        }

        internal int Count => this.data.Length;

        internal T this[int index] => this.data[index];

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => ((IList<T>)this.data).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.data.GetEnumerator();

        [Pure]
        internal ImmutableList<T> Add(T value)
        {
            var newData = new T[this.data.Length + 1];
            Array.Copy(this.data, newData, this.data.Length);
            newData[this.data.Length] = value;
            return new ImmutableList<T>(newData);
        }

        [Pure]
        internal ImmutableList<T> AddRange(ICollection<T> values)
        {
            var newData = new T[this.data.Length + values.Count];
            Array.Copy(this.data, newData, this.data.Length);
            values.CopyTo(newData, this.data.Length);
            return new ImmutableList<T>(newData);
        }

        [Pure]
        internal ImmutableList<T> Remove(T value)
        {
            var i = Array.IndexOf(this.data, value);
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

        internal T[] GetRawData() => this.data;
    }
}
