using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.FibonacciHeap;
using System;

namespace Stratus
{
    public class StratusPriorityQueue<Element, Key>
        where Key : IComparable<Key>
    {
        private readonly FibonacciHeap<Element, Key> heap;

        /// <summary>
        /// Whether this queue is empty
        /// </summary>
        public bool Empty() => heap.IsEmpty();

        /// <summary>
        /// Whether this queue is not empty
        /// </summary>
        /// <returns></returns>
        public bool NotEmpty() => !Empty();

        /// <summary>
        /// Clears the priority queue
        /// </summary>
        public void Clear() => heap.Clear();


        public int Count => heap.Count;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="minPriority"></param>
        public StratusPriorityQueue(Key minPriority)
        {
            heap = new FibonacciHeap<Element, Key>(minPriority);
        }

        /// <summary>
        /// Inserts the element with the given priority
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public virtual void Insert(Element item, Key priority)
        {
            heap.Insert(new FibonacciHeapNode<Element, Key>(item, priority));
        }

        /// <summary>
        /// Returns the cheapest element
        /// </summary>
        /// <returns></returns>
        public Element Top()
        {
            return heap.Min().Data;
        }

        /// <summary>
        /// Removes and returns the cheapest element
        /// </summary>
        /// <returns></returns>
        public virtual Element Pop()
        {
            return heap.RemoveMin().Data;
        }
    }

}