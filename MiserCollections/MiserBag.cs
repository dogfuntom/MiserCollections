namespace MiserCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Unordered collection that never deallocates anything, unless you ask explicitely.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <remarks>
    /// It's cache-friendly.
    /// To iterate use <c>foreach</c> loop — its enumerator made such way that there is no garbage-generating casts.
    /// (Unless you casted the collection to interface).
    /// </remarks>
    public class MiserBag<T> : IEnumerable, ICollection<T>, IEnumerable<T>
    // IList<T> is removed from interfaces, because otherwise this class breaks LSP by not guaranteeing unchanged order of items.
    {
        protected const int firstCapacity = 4;

        protected LinkedList<List<T>> lists = new LinkedList<List<T>>();

        protected int capacity;

        public int Count
        {
            get; protected set;
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [MayProduceGarbage]
        public void Optimize(int unnecessaryReferenceJumpCountThreshold)
        {
            throw new NotImplementedException();
        }

        [MayProduceGarbage]
        public void Optimize()
        {
            throw new NotImplementedException();
        }

        public virtual void Add(T item)
        {
            var addedToExisting = false;

            foreach (var list in this.GetLLEnumerator())
            {
                if (list.Count < list.Capacity)
                {
                    list.Add(item);
                    addedToExisting = true;
                    break;
                }
            }

            if (!addedToExisting)
            {
                AddItemToNewList(item);
            }

            this.Count++;
        }

        /// <summary>
        /// Adds new list with optimal size to <see cref="lists"/>. Adds given item to it.
        /// Doesn't update <see cref="Count"/>!
        /// </summary>
        /// <param name="item">Item to add to new list.</param>
        protected void AddItemToNewList(T item)
        {
            var list = new List<T>(this.lists.First == null ? firstCapacity : this.capacity) { item };
            this.capacity += list.Capacity;
            this.lists.AddLast(list);
        }

        public void Clear()
        {
            //var listNode = this.lists.First;
            //while (listNode != null)
            //{
            //    listNode.Value.Clear();
            //    listNode = listNode.Next;
            //}
            foreach (var list in this.GetLLEnumerator())
            {
                list.Clear();
            }
            this.Count = 0;
        }

        public bool Contains(T item)
        {
            //var listNode = this.lists.First;
            //while (listNode != null)
            //{
            //    if (listNode.Value.Contains(item))
            //        return true;
            //    listNode = listNode.Next;
            //}
            foreach (var list in this.GetLLEnumerator())
            {
                if (list.Contains(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Predicate<T> match)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            //var listNode = this.lists.First;

            //while (listNode != null)
            //{
            //    var list = listNode.Value;
            //    if (list.Remove(item))
            //    {
            //        this.Count--;
            //        return true;
            //    }
            //    listNode = listNode.Next;
            //}

            foreach (var list in this.GetLLEnumerator())
            {
                if (list.Remove(item))
                {
                    this.Count--;
                    return true;
                }
            }

            return false;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this.lists.First);
        }

        protected LLEnumerator GetLLEnumerator()
        {
            return new LLEnumerator(this.lists);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var list in this.lists)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        [Serializable]
        // No interface to force duck-typing, to avoid 2 garbage allocations in every foreach.
        public struct Enumerator
        {
            // TODO: make version checking like in .NET/Mono List<T>:
            // http://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs,9c3d580a8b7a8fe8
            private LinkedListNode<List<T>> node;
            private int index;

            internal Enumerator(LinkedListNode<List<T>> node)
            {
                this.node = node;
                this.index = 0;
                this.Current = default(T);
            }

            public T Current
            {
                get; private set;
            }

            public bool MoveNext()
            {
                if (this.node == null)
                    return false;

                var list = this.node.Value;

                if (((uint)index < (uint)list.Count))
                {
                    Current = list[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                var prevNode = this.node;

                this.node = this.node.Next;

                if (this.node != null)
                {
                    this.index = 0;
                    return this.MoveNext();
                }
                else
                {
                    index = prevNode.Value.Count + 1;
                    Current = default(T);
                    return false;
                }
            }

            public void Reset()
            {
                this.index = 0;
                this.Current = default(T);
            }
        }

        [Serializable]
        // No interface to force duck-typing, to avoid 2 garbage allocations in every foreach.
        protected struct LLEnumerator
        {
            private LinkedListNode<List<T>> currentNode;

            internal LLEnumerator(LinkedList<List<T>> collection)
            {
                this.currentNode = collection.First;
                this.Current = default(List<T>);
            }

            public List<T> Current
            {
                get; private set;
            }

            public bool MoveNext()
            {
                if (currentNode == null)
                    return false;

                this.Current = this.currentNode.Value;

                this.currentNode = this.currentNode.Next;

                return true;
            }

            // little hack to make it possible to foreach using internal enumerator
            public LLEnumerator GetEnumerator()
            {
                return this;
            }
        }
    }
}