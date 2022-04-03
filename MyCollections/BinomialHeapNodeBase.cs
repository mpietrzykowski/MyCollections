namespace MyCollections {
    public abstract class BinomialHeapNodeBase<T> {
        
        protected internal BinomialHeapNodeBase<T> child;

        protected internal int degree; 

        protected internal BinomialHeapBase<T> heap;

        protected internal T value;

        protected internal BinomialHeapNodeBase<T> parent;

        protected internal BinomialHeapNodeBase<T> sibling;

        public int Degree { get { return this.degree; } }

        public T Value { get { return this.value; } }

        internal BinomialHeapNodeBase(T value) {
            this.value = value;
        }   

        public override string ToString() {
            return this.value.ToString();
        }
    }
}