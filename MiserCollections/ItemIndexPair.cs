namespace System.Collections.Generic
{
    public struct ItemIndexPair<TItem>
    {
        public readonly TItem Item;
        public readonly int Index;

        public ItemIndexPair(TItem item, int index)
        {
            this.Item = item;
            this.Index = index;
        }
    }
}