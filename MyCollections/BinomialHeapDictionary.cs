/*
Author: Marcin Pietrzykowski
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyCollections {
    //FIXME: Dictionary based on BinomialHeap. Key are unique sorted by value. Values are stored as BinomialHeap
    //Need implementation of heap/hashmap
    public sealed class BinomialHeapDictionary<TKey, TValue> : 
        BinomialHeapBase<TValue>, 
        IDictionary<TKey, TValue>, 
        IDictionary,
        IReadOnlyDictionary<TKey, TValue>, 
        ICollection<KeyValuePair<TKey, TValue>>, 
        ICollection {

        public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey> {

            public struct Enumerator : IEnumerator<TKey> {
                
                private BinomialHeapNodeBase<TValue> current;

                private readonly BinomialHeapDictionary<TKey, TValue> heap;

                private readonly int version;

                public TKey Current {
                    get {
                        if (this.current != null) {
                            return (this.current as BinomialHeapNode<TKey, TValue>).key;
                        }
                        else {
                            return default;
                        }
                    }
                }

                object IEnumerator.Current => this.Current;
                
                internal Enumerator(BinomialHeapDictionary<TKey, TValue> heap) {
                    this.heap = heap;
                    version = heap.version;
                    current = default;
                }

                public void Dispose() { }

                public bool MoveNext() {
                    if (this.version != this.heap.version) {
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                    }

                    if (this.current == null) {
                        this.current = this.heap.root;
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
           
            private readonly BinomialHeapDictionary<TKey, TValue> heap;

            public KeyCollection(BinomialHeapDictionary<TKey, TValue> heap) {
                this.heap = heap ?? throw new ArgumentNullException(nameof(heap));
            }

            public int Count => heap.count;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => (heap as ICollection).SyncRoot;

            bool ICollection<TKey>.IsReadOnly => true;

            public void CopyTo(TKey[] array, int arrayIndex) {
                if (array == null) {
                    throw new ArgumentNullException(nameof(array));
                }

                if (arrayIndex < 0) {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Value cannot be negative.");
                }

                if (array.Length - arrayIndex < heap.count) {
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

                if (array is TKey[] arr) {
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

            void ICollection<TKey>.Add(TKey item) {
                throw new NotSupportedException("Collection is read only.");
            }

            void ICollection<TKey>.Clear() {
                throw new NotSupportedException("Collection is read only.");
            }

            bool ICollection<TKey>.Contains(TKey item) {
                return heap.ContainsKey(item);
            }

            public IEnumerator<TKey> GetEnumerator() {
                return new Enumerator(heap);
            }
            bool ICollection<TKey>.Remove(TKey item) {
                throw new NotSupportedException("Collection is read only.");
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

        public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue> {
            public struct Enumerator : IEnumerator<TValue> {
                
                private BinomialHeapNodeBase<TValue> current;

                private readonly BinomialHeapDictionary<TKey, TValue> heap;

                private readonly int version;

                public TValue Current {
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
                
                internal Enumerator(BinomialHeapDictionary<TKey, TValue> heap) {
                    this.heap = heap;
                    version = heap.version;
                    current = default;
                }

                public void Dispose() { }

                public bool MoveNext() {
                    if (this.version != this.heap.version) {
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                    }

                    if (this.current == null) {
                        this.current = this.heap.root;
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
           
            private readonly BinomialHeapDictionary<TKey, TValue> heap;

            public ValueCollection(BinomialHeapDictionary<TKey, TValue> heap) {
                this.heap = heap ?? throw new ArgumentNullException(nameof(heap));
            }

            public int Count => heap.count;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => (heap as ICollection).SyncRoot;

            bool ICollection<TValue>.IsReadOnly => true;

            public void CopyTo(TValue[] array, int arrayIndex) {
                if (array == null) {
                    throw new ArgumentNullException(nameof(array));
                }

                if (arrayIndex < 0) {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Value cannot be negative.");
                }

                if (array.Length - arrayIndex < heap.count) {
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

                if (array is TValue[] arr) {
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

            void ICollection<TValue>.Add(TValue item) {
                throw new NotSupportedException("Collection is read only.");
            }

            void ICollection<TValue>.Clear() {
                throw new NotSupportedException("Collection is read only.");
            }

            bool ICollection<TValue>.Contains(TValue item) {
                return heap.ContainsValue(item);
            }

            public IEnumerator<TValue> GetEnumerator() {
                return new Enumerator(heap);
            }
            bool ICollection<TValue>.Remove(TValue item) {
                throw new NotSupportedException("Collection is read only.");
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator {
            private BinomialHeapNodeBase<TValue> current;

            private readonly BinomialHeapDictionary<TKey, TValue> heap;

            private readonly int version;

            public KeyValuePair<TKey, TValue> Current {
                get {
                    if (this.current != null) {
                        BinomialHeapNode<TKey, TValue> node = (BinomialHeapNode<TKey, TValue>)this.current;
                        return new KeyValuePair<TKey, TValue>(node.key, node.value);
                    }
                    else {
                        return new KeyValuePair<TKey, TValue>();
                    }
                }
            }

            object IEnumerator.Current => this.Current;

            internal Enumerator(BinomialHeapDictionary<TKey, TValue> heap) {
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
                    this.current = this.heap.root;
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

            DictionaryEntry IDictionaryEnumerator.Entry {
                get {
                    if (this.current != null) {
                        BinomialHeapNode<TKey, TValue> node = (BinomialHeapNode<TKey, TValue>)this.current;
                        return new DictionaryEntry(node.key, node.value);
                    }
                    else { 
                        return new DictionaryEntry(default(TKey), default(TValue));
                    }
                }
            }

            object IDictionaryEnumerator.Key {
                 get {
                    if (this.current != null) {
                        BinomialHeapNode<TKey, TValue> node = (BinomialHeapNode<TKey, TValue>)this.current;
                        return node.key;
                    }
                    else {
                        return default(TKey);
                    }
                }
            }

            object IDictionaryEnumerator.Value {
                 get {
                    if (this.current != null) {
                        return this.current.value;
                    }
                    else {
                        return default(TValue);
                    }
                }
            }
        }

        //TODO: need own implementioation.
        private readonly Dictionary<TKey, BinomialHeapNode<TKey, TValue>> dict;        

        private KeyCollection keys;

        private object syncRoot;

        private ValueCollection values;

        public KeyCollection Keys {
            get {
                if (keys == null) {
                    keys = new(this);
                }
                return keys;
            }
        }

        public KeyValuePair<TKey, TValue> Min {
            get { 
                if (this.count == 0) {
                    return new KeyValuePair<TKey, TValue>();
                } 
                else {
                    return (this.min as BinomialHeapNode<TKey, TValue>).CreateKeyValuePair();
                }
            }
        }

        public BinomialHeapNode<TKey, TValue> MinNode {
            get { 
                if (this.count == 0) {
                    return default;
                } 
                else {
                    return this.min as BinomialHeapNode<TKey, TValue>;
                }
            }
        }
    
        public ValueCollection Values {
            get {
                if (values == null) {
                    values = new(this);
                }
                return values;
            }
        }

        public TValue this[TKey key] { 
            get { return dict[key].value; }
            set {
                if (dict.TryGetValue(key, out BinomialHeapNode<TKey, TValue> node)) {
                    UpdateNode(node, value);
                } else {
                    Add(key, value);
                }
            }
        }
        
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot {
            get { 
                if(syncRoot == null) {
                    System.Threading.Interlocked.CompareExchange<object>(ref syncRoot, new object(), null);    
                }
                return syncRoot; 
            }
        }
    
        bool IDictionary.IsFixedSize => false;

        ICollection IDictionary.Keys => Keys;

        ICollection IDictionary.Values => Values;

        object IDictionary.this[object key] { 
            get {
                if (key is TKey k) {
                    return this[k];
                }
                return null;
            } 
            set {
                if (key == null) {
                    throw new ArgumentNullException(nameof(key));
                }

                try {
                    TKey tempKey = (TKey)key;
                    try {
                        this[tempKey] = (TValue)value; 
                    }
                    catch (InvalidCastException ex) { 
                        throw new ArgumentException("Parameter cannot be casted to TValue", nameof(value), ex);
                    }
                }
                catch (InvalidCastException ex) { 
                    throw new ArgumentException("Parameter cannot be casted to TKey", nameof(key), ex);
                }
            }
        }        

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public BinomialHeapDictionary() : base(null) { 
            dict = new Dictionary<TKey, BinomialHeapNode<TKey, TValue>>();
        }

        public BinomialHeapDictionary(IEnumerable<KeyValuePair<TKey,TValue>> collection) : this(collection, null, null) { }

        public BinomialHeapDictionary(IEqualityComparer<TKey> keyComparer, IComparer<TValue> valueComparer) : base(valueComparer) {
            dict = new Dictionary<TKey, BinomialHeapNode<TKey, TValue>>(keyComparer);
        }

        public BinomialHeapDictionary(
            IEnumerable<KeyValuePair<TKey,TValue>> collection, 
            IEqualityComparer<TKey> keyComparer, 
            IComparer<TValue> valueComparer
        ) : base(valueComparer) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection) {
                Add(item.Key, item.Value);
            }

            dict = new Dictionary<TKey, BinomialHeapNode<TKey, TValue>>(keyComparer);
        }

        public void Add(TKey key, TValue value) {
            BinomialHeapNode<TKey, TValue> node = new(key, value, this);
            dict.Add(key, node);
            AddNode(node);
        }

        public void Add(BinomialHeapNode<TKey, TValue>  node) {
            this.dict.Add(node.key, node);
            AddNode(node);
        }

        public bool ContainsKey(TKey key) {
            return dict.ContainsKey(key);
        }

        public bool ContainsValue(TValue value) {
            return FindNode(value) != null;
        }

        public BinomialHeapNode<TKey, TValue> Find(TValue value) {
            return FindNode(value) as BinomialHeapNode<TKey, TValue>;
        }

        public List<BinomialHeapNode<TKey, TValue>> FindAll(Predicate<TValue> match) {
            return FindAllNodes<BinomialHeapNode<TKey, TValue>>(match);
        }

        public bool Remove(TKey key) {
            if (dict.Remove(key, out BinomialHeapNode<TKey, TValue> node)) {
                return RemoveNode(node);
            } 

            return false;
        }

        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value){
            if (dict.Remove(key, out BinomialHeapNode<TKey, TValue> node)) {
                value = node.value;
                return RemoveNode(node);
            } 
            else {
                value = default;
                return false;
            }
        }

        public bool TryAdd(TKey key, TValue value) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.dict.ContainsKey(key)){
                return false;
            }

            Add(key, value);
            return true;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
            if (dict.TryGetValue(key, out BinomialHeapNode<TKey, TValue> node)) {
                value = node.value;
                return true;
            } 
            else {
                value = default;
                return false;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return new Enumerator(this);
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

            if (array is KeyValuePair<TKey, TValue>[] arr) {
                (this as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(arr, index);
            }
            else if (array is DictionaryEntry[] dictEntryArr){
                foreach (var item in this) {
                    dictEntryArr[index++] = new DictionaryEntry(item.Key, item.Value);
                }
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

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            return dict.TryGetValue(item.Key, out BinomialHeapNode<TKey, TValue> node) 
            && Comparer<TValue>.Default.Compare(node.value, item.Value) == 0;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
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

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            if (dict.TryGetValue(item.Key, out BinomialHeapNode<TKey, TValue> node) 
                && Comparer<TValue>.Default.Compare(node.value, item.Value) == 0) {
                dict.Remove(item.Key);
                return RemoveNode(node);
            } 

            return false;
        }

        void IDictionary.Add(object key, object value) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            try {
                TKey k = (TKey)key;
                try {
                    TValue v = (TValue)value;
                    Add(k, v);
                }
                catch (InvalidCastException ex) {
                    throw new ArgumentException(
                        "value is of a type that is not assignable to the value type TValue of the BinomialHeapDictionary<TKey,TValue>.",
                        nameof(value),
                        ex
                    );
                }
            }
            catch (InvalidCastException ex) {
                throw new ArgumentException(
                    "key is of a type that is not assignable to the key type TKey of the BinomialHeapDictionary<TKey,TValue>.",
                    nameof(key),
                    ex
                );
            }
        }

        bool IDictionary.Contains(object key) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            if (key is TKey k)  {
                return dict.ContainsKey(k);
            }

            return false;
        }

        void IDictionary.Remove(object key) {
            if (key is TKey k)  {
                Remove(k);
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}