/*
Author: Marcin Pietrzykowski
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace MyCollections {
    public sealed class BinomialHeap<T> : BinomialHeapBase<T>, ICollection<T>, IReadOnlyCollection<T>, ICollection {

        public struct Enumerator : IEnumerator<T> {
            private BinomialHeapNodeBase<T> current;

            private readonly BinomialHeap<T> heap;

            private readonly int version;

            public T Current {
                get {
                    if (this.current != null) {
                        return this.current.value;
                    }
                    else {
                        return default;
                    }
                }
            }

            object IEnumerator.Current => this.Current;

            internal Enumerator(BinomialHeap<T> heap) {
                this.current = null;
                this.heap = heap;
                this.version = heap.version;
            }

            public void Dispose() { }

            public bool MoveNext() {
                if (this.version != this.heap.version) {
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                }

                if (this.current == null) {
                    this.current = heap.root;
                } 
                else {
                    if (current.child != null) {
                        current = current.child;
                    }
                    else if (current.sibling != null) {
                        current = current.sibling;
                    }
                    else {
                        current = current.parent.sibling;
                    }
                }

                if (this.current == null){
                    return false;
                }
                else {
                    return true;
                }
            }

            void IEnumerator.Reset() {
                if (this.version != this.heap.version) {
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                }
                
                this.current = null;
            }
        }

        private object syncRoot;
        
        public T Min {
            get { 
                if (this.count == 0) {
                    return default;
                } 
                else {
                    return this.min.value; 
                }
            }
        }

        int ICollection.Count => this.count;
        
        bool ICollection.IsSynchronized => false;

        public BinomialHeapNode<T> MinNode {
            get { 
                if (this.count == 0) {
                    //similar action in Set<T>
                    return default;
                } 
                else {
                    return this.min as BinomialHeapNode<T>; 
                }
            }
        }
       
        object ICollection.SyncRoot {
            get { 
                if(syncRoot == null) {
                    System.Threading.Interlocked.CompareExchange<object>(ref syncRoot, new object(), null);    
                }
                return syncRoot; 
            }
        }

        public BinomialHeap() : base(null) { }

        public BinomialHeap(IComparer<T> comparer) : base(comparer) { }

        public BinomialHeap(IEnumerable<T> collection) : this(collection, null) { }

        public BinomialHeap(IEnumerable<T> collection, IComparer<T> comparer) : base(comparer) {
            foreach (var item in collection) {
                Add(item);
            }
        }

        public void Add(BinomialHeapNode<T> node) {
            AddNode(node);
        }

        public void Add(T item) {
            AddNode(new BinomialHeapNode<T>(item, this));
        }
        
        public bool Contains(T item) {
            return FindNode(item) != null;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Value cannot be negative.");
            }

            if (array.Length - arrayIndex < this.count) {
                throw new ArgumentException(
                    "The number of elements in the source BinomialHeap<T> is greater than the available space from index to the end of the destination array."
                );
            }

            foreach (var item in this) {
                array[arrayIndex++] = item;
            }
        }

        void ICollection.CopyTo(Array array, int index) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }
            
            if (array.Rank != 1) {
                throw new ArgumentException("array is multidimensional.", nameof(array));
            }

            if( array.GetLowerBound(0) != 0 ) {
                throw new ArgumentException("Lower bound is non zero.", nameof(array));
            }
            
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Length - index < Count) {
                throw new ArgumentException(
                    "The number of elements in the source ICollection is greater than the available space from index to the end of the destination array."
                );
            }

            if (array is T[] arr) {
                CopyTo(arr, index);
            }
            else {
                if (array is object[] objects) {
                    try {
                        foreach (var item in this) {
                            objects[index++] = item;
                        }
                    }
                    catch (ArrayTypeMismatchException ex) {
                        throw new ArgumentException("The type of the source ICollection cannot be cast automatically to the type of the destination array.", ex);
                    }
                }
                else {
                    throw new ArgumentException("The type of the source ICollection cannot be cast automatically to the type of the destination array.");
                }
            }
        }

        public BinomialHeapNode<T> Find (T item) {
            return FindNode(item) as BinomialHeapNode<T>;
        }

        public List<BinomialHeapNode<T>> FindAll (Predicate<T> match) {
            return FindAllNodes<BinomialHeapNode<T>>(match);
        }

        public IEnumerator<T> GetEnumerator() {
            return new Enumerator(this);
        }

        public bool Remove(BinomialHeapNode<T> node){
            return RemoveNode(node);
        }

        public bool UpdateKey(BinomialHeapNode<T> node, T newKey) {
            return UpdateNode(node, newKey);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}