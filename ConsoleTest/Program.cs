/*
Author: Marcin Pietrzykowski
*/

using System;
using System.Collections.Generic;

using MyCollections;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args) {
            BinomialHeapDictionary<string, int> binomialHeap = new();
            binomialHeap.Add("ala", 4);
            binomialHeap.Add("ola", 5);

            BinomialHeap<int> heap = new();

            for (int i = 0; i <= 40; i++) {
                heap.Add(i);
                // heap.Print();
            }
            var v2 = heap.FindAll(i => i <= 4);

            heap.Print(4);
            foreach (var item in heap) {
                // Console.Write(item + " ");
            }

            heap.Print();
            foreach (var item in heap) {
                Console.Write(item + " ");
            }

            heap.Remove(3);
            heap.RemoveMin();
            heap.Print();

            heap.Clear();
            for (int i = 80; i >= 0; i--) {
                heap.Add(i);
                //heap.Print();
            }
            heap.Print();
            foreach (var item in heap) {
                Console.Write(item + " ");
            }

            heap.Clear();
            Random rnd = new(33);
            for (int i = 0; i < 80; i++) {
                heap.Add(rnd.Next(100));
                // heap.Print();
            }
            heap.Print();
            heap.Find(24);
            //heap.UpdateKey(0, 28);
            heap.Remove(20);
            heap.Print();
            foreach (var item in heap) {
                // Console.Write(item + " ");
            }

            for (int i = 0; i < 80; i++) {
                heap.RemoveMin();
                heap.Print();
            }
            heap.Print();
        }
    }
}
