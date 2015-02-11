namespace MiserCollections
{
    using System;
    using System.Collections.Generic;

    public class MiserList<T> : MiserBag<T>, IList<T>
    {
        public override void Add(T item)
        {
            var last = this.lists.Last;

            var addToNewList = true;

            if (last != null)
            {
                var list = last.Value;
                if (list.Count < list.Capacity)
                {
                    list.Add(item);
                    addToNewList = false;
                }
            }

            if (addToNewList)
            {
                this.AddItemToNewList(item);
            }

            this.Count++;
        }

        public T this[int index]
        {
            get
            {
                var currentListStartIndex = 0;
                foreach (var list in this.GetLLEnumerator())
                {
                    if (index >= currentListStartIndex + list.Count)
                    {
                        currentListStartIndex += list.Count;
                    }
                    else
                    {
                        return list[index - currentListStartIndex];
                    }
                }

                throw new ArgumentOutOfRangeException("index");
            }

            set
            {
                var currentListStartIndex = 0;
                foreach (var list in this.GetLLEnumerator())
                {
                    if (index >= currentListStartIndex + list.Count)
                    {
                        currentListStartIndex += list.Count;
                    }
                    else
                    {
                        list[index - currentListStartIndex] = value;
                        return;
                    }
                }

                throw new ArgumentOutOfRangeException("index");
            }
        }

        public int IndexOf(T item)
        {
            var currentListStartIndex = 0;
            foreach (var list in this.GetLLEnumerator())
            {
                var index = list.IndexOf(item);
                if (index != -1)
                    return index + currentListStartIndex;

                currentListStartIndex += list.Count;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extends the size of this list to specified size, filling new elements with default values.
        /// </summary>
        /// <param name="size">Desired size.</param>
        internal void ExtendAndFillToSize(int size)
        {
            //// extend capacity
            //if (this.capacity < size)
            //    this.capacity = Math.Max(this.capacity * 2, size);

            //// fill empty positions with nulls
            //while (this.Count < size)
            //{
            //    this.Add(default(T));
            //}

            // fill last list first until it's full or size is achieved (if it exists)
            if (this.lists.Last != null)
            {
                var lastList = this.lists.Last.Value;
                var lastListFirstIndex = this.Count - lastList.Count;

                var dest = Math.Min(lastListFirstIndex + lastList.Capacity, size);
                for (int i = lastListFirstIndex; i < dest; i++)
                {
                    lastList.Add(default(T));
                    this.Count++;
                }
            }

            // check if disired size is achieved already
            var leftUntilDesiredSize = size - this.Count;
            if (leftUntilDesiredSize <= 0)
                return;

            // add new list and fill it until size is achieved
            var usualNewListCapacity = this.lists.First == null ? firstCapacity : this.capacity;
            var optimalNewListCapacity = Math.Max(leftUntilDesiredSize, usualNewListCapacity);

            var list = new List<T>(optimalNewListCapacity);
            this.lists.AddLast(list);
            this.capacity += list.Capacity;

            for (int i = 0; i < leftUntilDesiredSize; i++)
            {
                this.lists.Last.Value.Add(default(T));
                this.Count++;
            }
        }
    }
}