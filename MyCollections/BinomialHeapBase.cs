using System;
using System.Collections;
using System.Collections.Generic;

namespace MyCollections {
    public abstract class BinomialHeapBase<T>  {

        protected int count;

        protected BinomialHeapNodeBase<T> min;

        protected BinomialHeapNodeBase<T> root;

        protected readonly IComparer<T> valueComparer;

        protected int version = 0;

        public int Count => this.count;

        public bool IsReadOnly => false;

        private static BinomialHeapNodeBase<T> Merge(BinomialHeapNodeBase<T> node1, BinomialHeapNodeBase<T> node2) {
            if (node1 == null) {
                return node2;
            }

            if (node2 == null) {
                return node1;
            }

            BinomialHeapNodeBase<T> result, tmp;

            if (node1.degree <= node2.degree){
                result = node1;
                node1 = node1.sibling;
            }
            else {
                result = node2;
                node2 = node2.sibling;
            }

            tmp = result;

            while (node1 != null && node2 != null) {
                if (node1.degree <= node2.degree) {
                    tmp.sibling = node1;
                    node1 = node1.sibling;
                }
                else {
                    tmp.sibling = node2;
                    node2 = node2.sibling;
                }

                tmp = tmp.sibling;
            }

            if (node1 != null) {
                tmp.sibling = node1;
            }
            else if (node2 != null) {
                tmp.sibling = node2;
            }

            return result;
        }
 
        internal BinomialHeapBase(IComparer<T> comparer) {
            valueComparer = comparer ?? Comparer<T>.Default;
        }

        protected void AddNode(BinomialHeapNodeBase<T> node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.heap != this){
                throw new InvalidOperationException("node belongs to another BinomialHeap<T>");
            }

            if (min == null || valueComparer.Compare(min.value, node.value) > 0){
                min = node;
            }

            node.heap = this;
            
            this.root = Union(node, root);
            this.count++;
            this.version++;
        }

        public void Clear() {
            this.count = 0;
            this.min = null;
            this.root = null;
            this.version++;
        }

        protected List<TNode> FindAllNodes<TNode>(Predicate<T> match) where TNode : BinomialHeapNodeBase<T> {
            if (match == null) {
                throw new ArgumentNullException(nameof(match));
            }

            List<TNode> result = new();

            BinomialHeapNodeBase<T> current = this.root;

            while (current != null) {
                if (match(current.Value)) {
                    result.Add(current as TNode);
                }
                
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

            return result;
        }

        protected BinomialHeapNodeBase<T> FindNode (T value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            BinomialHeapNodeBase<T> current = this.root;

            while (current != null) {
                if (valueComparer.Compare(current.value, value) == 0){
                    return current;
                }
                else if (current.child != null && valueComparer.Compare(current.value, value) <= 0) {
                    current = current.child;
                }
                else if (current.sibling != null) {
                    current = current.sibling;
                }
                else {
                    current = current.parent.sibling;
                }
            }

            return null;
        }

        public void Print(int length = 2) {
            string minusPlus = new string('-', length) + '+';
            string minuses = new('-', length + 1);
            string spaces = new string(' ', length - 1) + "|";
            string format = "{0," + length + "}";

            if (this.count == 0){
                Console.WriteLine(minusPlus + "\n  ∅\n");
                return;
            }
            BinomialHeapNodeBase<T> curr = this.root;            
            int degree = 0;

            //Rysowanie lini z której odchodzą kopce i znalezienie kopca o maksymalnej głębkokości
            Console.WriteLine();
            while (curr != null) {
                if (curr.degree == 0){
                    Console.Write(minusPlus);
                }
                else {
                    int stop = (int)Math.Pow(2, curr.degree - 1) - 1;
                    for (int i = 0; i < stop; i++) {
                        Console.Write(minuses);
                    }

                    Console.Write(minusPlus);
                }
                
                if (degree < curr.degree) {
                    degree = curr.degree;
                }
                curr = curr.sibling;
            }

            //stworzenie buffora
            for (int i = 0; i < degree * 2 + 1; i++) {
                Console.WriteLine();
            }

            int top = Console.CursorTop - degree * 2;
            int lastLine = Console.CursorTop;
            int left = 1;

            curr = this.root;
            degree = 0;

            while (curr != null) {
                if (curr.parent == null && curr.degree == 0) {    
                    Console.SetCursorPosition(left, top);
                    Console.Write(format, curr.value);
                    curr = curr.sibling;
                    left += length + 1;
                }
                else {
                    while (curr.child != null){
                        curr = curr.child;
                        degree++;
                    }
                    
                    Console.SetCursorPosition(left, top + degree * 2);
                    Console.Write(format, curr.value);
                    Console.SetCursorPosition(left, top + (degree * 2) - 1);
                    Console.Write(spaces);
                    Console.SetCursorPosition(left, top + (degree * 2) - 2);
                    Console.Write(format, curr.parent.value);
                    
                    curr = curr.parent.sibling;
                    degree--;
                    left += length + 1;

                    if (curr?.parent != null) {
                        Console.SetCursorPosition(left-1, top + (degree * 2) - 1);
                        Console.Write("/");
                    }
                }
            }
            Console.SetCursorPosition(0, lastLine);
            Console.WriteLine();
            
            if (this.count == 1) {
                Console.WriteLine();
            }
        }

        public bool Remove(T value) {
            BinomialHeapNodeBase<T> node = FindNode(value);
            if (node == null) {
                return false;
            }
            else {
                return RemoveNode(node);
            }
        }

        public bool RemoveMin() {
            if (this.count == 0) {
                return false;
            }

            RemoveNode(this.min);

            //find new min na głownej gałęzi
            BinomialHeapNodeBase<T> current = this.min = this.root;
            while (current != null) {
                if (valueComparer.Compare(this.min.value, current.value) > 0){
                    this.min = current;
                }

                current = current.sibling;
            }

            return true;
        }

        protected bool RemoveNode(BinomialHeapNodeBase<T> node){
             if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.heap != this){
                throw new InvalidOperationException("node belongs to another BinomialHeap<T>");
            }

            this.count--;
            this.version++;

            //put node to main branch
            while (node.parent != null) {
                SwapWithParent(node);
            }

            BinomialHeapNodeBase<T> current = this.root;
            BinomialHeapNodeBase<T> previous = null;

            while (current != node) {
                previous = current;
                current = current.sibling;
            }

            //ommit node from main branach.
            if (previous == null) {
                //first in the branch
                this.root = current.sibling;
            }
            else {
                previous.sibling = current.sibling;
            }

            //If node had children
            if (current.degree > 0) {
                current = current.child;
                BinomialHeapNodeBase<T> next = current.sibling;
                previous = null;

                //connect chilldren in reverse order 
                while (next != null) {
                    current.sibling = previous;
                    current.parent = null;
                    previous = current;
                    current = next;
                    next = current.sibling;
                }

                current.sibling = previous;
                current.parent = null;

                this.root = Union(this.root, current);
            }

            //made node orhpaned
            node.child = null;
            node.degree = 0;
            node.heap = null;
            node.parent = null;
            node.sibling = null;
            
            return true;
        }
      
        private BinomialHeapNodeBase<T> SwapWithParent(BinomialHeapNodeBase<T> node) {
            BinomialHeapNodeBase<T> parent = node.parent;
            BinomialHeapNodeBase<T> tmp, prev, leftNodeSibling = null;

            int degree = node.degree;
            node.degree = parent.degree;
            parent.degree = degree;

            tmp = node.child;

            while (tmp != null) {
                //update parent of node children
                tmp.parent = parent;
                tmp = tmp.sibling;
            }
            
            tmp = parent.child;
            prev = null;
            while (tmp != null) {
                if (tmp == node) {
                    //get left node sibling
                    leftNodeSibling = prev;
                }
                else {
                    //update parent of node siblings
                    tmp.parent = node;
                }
                prev = tmp;
                tmp = tmp.sibling;
            }

            tmp = parent.parent?.child;
            prev = null;
            while (tmp != null) {
                if (tmp == parent) {
                    if (prev != null) {
                        //get left parent sibling and swap it with node
                        prev.sibling = node;
                    }
                    break;
                }
                prev = tmp;
                tmp = tmp.sibling;
            }

            if (parent.parent == null) {
                //If it is main branch
                if (this.root == parent) {
                    this.root = node;
                }
                else {
                    tmp = this.root;
                    while (tmp.sibling != parent) {
                        tmp = tmp.sibling;
                    }
                    tmp.sibling = node;
                }
            }
            else {
                //if not update children
                if (parent.parent.child == parent) {
                    parent.parent.child = node;
                }
            }

            //update parents
            node.parent = parent.parent;
            parent.parent = node;

            //update child
            tmp = parent.child == node ? parent : parent.child;
            parent.child = node.child;
            node.child = tmp;

            //update siblings
            if (leftNodeSibling != null) {
                leftNodeSibling.sibling = parent;
            }
            tmp = parent.sibling;
            parent.sibling = node.sibling;
            node.sibling = tmp;

            return parent;
        }

        private BinomialHeapNodeBase<T> Union(BinomialHeapNodeBase<T> node1, BinomialHeapNodeBase<T> node2) {
            BinomialHeapNodeBase<T> prev = null;
            BinomialHeapNodeBase<T> curr = Merge(node1, node2);
            BinomialHeapNodeBase<T> next = curr?.sibling;
            
            BinomialHeapNodeBase<T> result = curr;

            while (next != null) {
                if (curr.degree == next.degree) {
                    if (next.degree == next.sibling?.degree){
                        prev = curr;
                        curr = next;
                        next = next.sibling;
                    }
                    else {
                        if (valueComparer.Compare(curr.value, next.value) <= 0) {
                            curr.sibling = next.sibling;
                            next.sibling = curr.child;
                            next.parent = curr;
                            curr.child = next;
                            curr.degree++;

                            next = curr.sibling;
                        }
                        else {
                            if (prev != null) {
                                prev.sibling = next;
                            }
                            else {
                                result = next;
                            }
                            curr.sibling = next.child;
                            curr.parent = next;
                            next.child = curr;
                            next.degree++;

                            curr = next;
                            next = curr.sibling;
                        }
                    }
                }
                else {
                    prev = curr;
                    curr = next;
                    next = next.sibling;
                }
            }

            return result;
        }

        public bool UpdateKey(T oldKey, T newKey) {
            BinomialHeapNodeBase<T> current = FindNode(oldKey);
            
            if (current == null) {
                return false;
            }
            else {
                return UpdateNode(current, newKey);
            }
        }

        protected bool UpdateNode(BinomialHeapNodeBase<T> node, T newKey) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.heap != this){
                throw new InvalidOperationException("node belongs to another BinomialHeap<T>");
            }

            if (valueComparer.Compare(node.value, newKey) < 0) {
                node.value = newKey;
                while (node.degree > 0) {
                    //Find smallest child and swap it with parent
                    BinomialHeapNodeBase<T> child = node.child;
                    BinomialHeapNodeBase<T> smallestChild = null;
                    while (child != null) {
                        if (valueComparer.Compare(child.value, node.value) < 0
                            && (smallestChild == null
                                || valueComparer.Compare(child.value, smallestChild.value) < 0)) {
                            smallestChild = child;
                        }
                        child = child.sibling;
                    }
                    
                    if (smallestChild == null) {
                        break;
                    }

                    SwapWithParent(smallestChild);
                }
            }
            else {
                node.value = newKey;
                while (node.parent != null && valueComparer.Compare(node.value, node.parent.value) < 0) {
                    SwapWithParent(node);
                }
            }

            this.version++;

            return true;
        }
    }
}