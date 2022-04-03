/*
Author: Marcin Pietrzykowski
*/

using System.Collections.Generic;

namespace MyCollections {
    public sealed class BinomialHeapNode<TKey, TValue> : BinomialHeapNodeBase<TValue>  {

        internal TKey key;

        public BinomialHeapNode<TKey, TValue> Child { get { return this.child as BinomialHeapNode<TKey, TValue>; } }

        public BinomialHeapDictionary<TKey, TValue> Heap { get { return this.heap as BinomialHeapDictionary<TKey, TValue>; } }

        public BinomialHeapNode<TKey, TValue> Parent { get { return this.parent as BinomialHeapNode<TKey, TValue>; } }

        public BinomialHeapNode<TKey, TValue> Sibling { get { return this.sibling as BinomialHeapNode<TKey, TValue>; } }

        public TKey Key { 
            get { return this.key; }
            set { this.key = value; }
        }

        public BinomialHeapNode(TKey key, TValue value) : base(value){
            this.key = key;
        }
        internal BinomialHeapNode(TKey key, TValue value, BinomialHeapDictionary<TKey, TValue> heap) : this(key, value){
            this.heap = heap;
        }

        internal KeyValuePair<TKey, TValue> CreateKeyValuePair(){
            return new KeyValuePair<TKey, TValue>(this.key, this.value);
        }
    }
}