namespace MyCollections {
    public sealed class BinomialHeapNode<T> : BinomialHeapNodeBase<T> {

        public BinomialHeapNode<T> Child { get { return this.child as BinomialHeapNode<T>; } }

        public BinomialHeap<T> Heap { get { return this.heap as BinomialHeap<T>; } }

        public BinomialHeapNode<T> Parent { get { return this.parent as BinomialHeapNode<T>; } }

        public BinomialHeapNode<T> Sibling { get { return this.sibling as BinomialHeapNode<T>; } }

        public BinomialHeapNode(T value) : base(value) {
            this.value = value;
        }   

        internal BinomialHeapNode(T value, BinomialHeap<T> heap) : this(value) {
            this.heap = heap;
        }
    }
}