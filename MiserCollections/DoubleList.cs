namespace System.Collections.Generic
{
    using System;
    using System.Collections;
    using MiserCollections;

    public class DoubleList<T> : IEnumerable<ItemIndexPair<T>>
    {
        private MiserList<T> positiveList = new MiserList<T>(); //  0,  1,  2,.. (technically, non-negative instead of “positive”)
        private MiserList<T> negativeList = new MiserList<T>(); // -1, -2, -3,..

        public int From
        {
            get
            {
                return -this.negativeList.Count;
            }
        }

        public int To
        {
            get
            {
                return this.positiveList.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index >= 0)
                    return this.positiveList[index];
                else
                    return this.negativeList[-index - 1];
            }

            set
            {
                if (index >= 0)
                    this.positiveList[index] = value;
                else
                    this.negativeList[-index - 1] = value;
            }
        }

        /// <summary>
        /// Ensures that it's possible to access to given index.
        /// </summary>
        /// <param name="index">Positive or negative index.</param>
        public void EnsureIndex(int index)
        {
            if (index >= 0)
                this.EnsureSize(this.positiveList, index + 1);
            else
                this.EnsureSize(this.negativeList, -index);
        }

        public IEnumerable<int> IndeciesWhere(Predicate<T> condition)
        {
            for (int i = this.negativeList.Count - 1; i >= 0; i--)
            {
                if (condition(this.negativeList[i]))
                    yield return -i - 1;
            }
            for (int i = 0; i < this.positiveList.Count; i++)
            {
                if (condition(this.positiveList[i]))
                    yield return i;
            }
        }

        public int? IndexOf(T item)
        {
            var index = this.positiveList.IndexOf(item);
            if (index >= 0)
                return index;

            index = this.negativeList.IndexOf(item);
            if (index >= 0)
                return -1 - index;

            return null;
        }

        public void Clear()
        {
            positiveList.Clear();
            negativeList.Clear();
        }

        public bool Contains(T item)
        {
            return positiveList.Contains(item)
                || negativeList.Contains(item);
        }

        public bool Remove(T item)
        {
            return positiveList.Remove(item) || negativeList.Remove(item);
        }

        public IEnumerator<ItemIndexPair<T>> GetEnumerator()
        {
            for (int i = this.negativeList.Count - 1; i >= 0; i--)
            {
                yield return new ItemIndexPair<T>(this.negativeList[i], -i - 1);
            }
            for (int i = 0; i < this.positiveList.Count; i++)
            {
                yield return new ItemIndexPair<T>(this.positiveList[i], i);
            }
        }

        private void EnsureSize(MiserList<T> list, int size)
        {
            list.ExtendAndFillToSize(size);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
