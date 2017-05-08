using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Voxelgon.Collections {
    public class Deque<T> : ICollection<T> {

        #region Fields

        private const int DefaultCapacity = 8; // default capacity for the deque

        private T[] _buffer; // the array containing all the values
        private int _offset; // where the first item in the deque is in the buffer

        #endregion

        #region Constructors

        public Deque(int capacity = DefaultCapacity) {
            if (capacity < 0) capacity = DefaultCapacity;
            _buffer = new T[capacity];
            Capacity = capacity;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get or set.</param>
        public T this[int index] {
            get { return _buffer[ConvertIndex(index)]; }
            set {
                if (index < 0 || index >= Count) {
                    throw new ArgumentOutOfRangeException("index",
                        "Invalid existing index " + index + " for source length " + Count);
                } else {
                    _buffer[ConvertIndex(index)] = value;
                }
            }
        }

        /// <summary>
        /// Capacity of the Deque
        /// </summary>
        public int Capacity {
            get { return _buffer.Length; }
            set {
                if (value < Count)
                    throw new ArgumentOutOfRangeException("value",
                        "Capacity cannot be set to a value less than Count");

                if (value == _buffer.Length)
                    return;

                // Create the new _buffer and copy our existing range.
                T[] newBuffer = new T[value];
                CopyTo(newBuffer, 0);

                // Set up to use the new _buffer.
                _buffer = newBuffer;
                _offset = 0;
            }
        }

        /// <summary>
        /// Number of elements in the Deque
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Returns true if the Deque is full and needs to be expanded
        /// </summary>
        public bool IsFull {
            get { return Count == Capacity; }
        }

        /// <summary>
        /// Returns true if the Deque is readonly
        /// </summary>
        public bool IsReadOnly {
            get { return false; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the first element in the Deque, but dont remove it
        /// </summary>
        /// <returns>the first element in the Deque</returns>
        public T PeekFront() {
            return _buffer[ConvertIndex(0)];
        }

        /// <summary>
        /// Get the last element in the Deque, but dont remove it
        /// </summary>
        /// <returns>the last element in the Deque</returns>
        public T PeekBack() {
            return _buffer[ConvertIndex(Count - 1)];
        }

        /// <summary>
        /// Get the first element in the Deque and remove it
        /// </summary>
        /// <returns>the first element in the Deque</returns>
        public T PopFront() {
            var value = PeekFront();
            _offset = (_offset + 1) % Capacity;
            --Count;

            return value;
        }

        /// <summary>
        /// Get the last element in the Deque and remove it
        /// </summary>
        /// <returns>the last element in the Deque</returns>
        public T PopBack() {
            var value = PeekBack();
            --Count;
            return value;
        }

        /// <summary>
        /// Adds a new element to the front
        /// </summary>
        /// <param name="value">element to add</param>
        public void PushFront(T value) {
            ExpandIfNeeded();
            _offset = (_offset - 1 + Capacity) % Capacity;
            _buffer[ConvertIndex(0)] = value;
            ++Count;
        }

        /// <summary>
        /// Adds a new element to the back
        /// </summary>
        /// <param name="value">element to add</param>
        public void PushBack(T value) {
            ExpandIfNeeded();
            _buffer[ConvertIndex(Count)] = value;
            ++Count;
        }

        /// <summary>
        /// Adds a collection to the front of the deque
        /// </summary>
        /// <param name="values">collection to add</param>
        public void PushFront(ICollection<T> values) {
            ExpandIfNeeded(values.Count);
            foreach (var v in values.Reverse()) {
                _offset = (_offset - 1 + Capacity) % Capacity;
                _buffer[ConvertIndex(0)] = v;
                ++Count;
            }
        }

        /// <summary>
        /// Adds a collection to the back of the deque
        /// </summary>
        /// <param name="values">collection to add</param>
        public void PushBack(ICollection<T> values) {
            ExpandIfNeeded(values.Count);
            foreach (var v in values) {
                _buffer[ConvertIndex(Count)] = v;
                ++Count;
            }
        }

        /// <summary>
        /// clear the contents of the deque
        /// </summary>
        public void Clear() {
            Count = 0;
            _offset = 0;
        }

        /// <summary>
        /// finds the index of a given item
        /// </summary>
        /// <param name="value">value to find</param>
        /// <returns>the item's index in the deque</returns>
        public int IndexOf(T value) {
            for (var i = 0; i < Count; i++) {
                if (_buffer[ConvertIndex(i)].Equals(value)) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if a given item is in this deque
        /// </summary>
        /// <param name="value">value to find</param>
        /// <returns>if the item is present in the item</returns>
        public bool Contains(T value) {
            return IndexOf(value) != -1;
        }

        /// <summary>
        /// Finds and removes the given index
        /// </summary>
        /// <param name="value">item to remove</param>
        /// <returns>if the item was removed</returns>
        public bool Remove(T value) {
            var index = IndexOf(value);

            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public void Insert(int index, T value) {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException("index");

            if (index == 0) {
                PushFront(value);
            } else if (index == Count - 1) {
                PushBack(value);
            } else {
                ExpandIfNeeded();
                if (index < (Count / 2)) {
                    // first half of list
                    for (var j = 0; j < index; j++) {
                        _buffer[ConvertIndex(j - 1)] = _buffer[ConvertIndex(j)];
                    }
                    _offset = (_offset - 1 + Capacity) % Capacity;
                } else {
                    // second half of list
                    for (var j = Count; j > index; j--) {
                        _buffer[ConvertIndex(j + 1)] = _buffer[ConvertIndex(j)];
                    }
                }

                _buffer[ConvertIndex(index)] = value;

                Count++;
            }
        }

        /// <summary>
        /// Removes the item at a given index
        /// </summary>
        /// <param name="index">index to remove from</param>
        /// <exception cref="IndexOutOfRangeException">index is not in the deque</exception>
        public void RemoveAt(int index) {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException("index");

            if (index == 0) {
                PopFront();
            } else if (index == Count - 1) {
                PopBack();
            } else {
                if (index < (Count / 2)) {
                    // Removing from first half of list
                    for (var j = (index - 1); j >= 0; j--) {
                        _buffer[ConvertIndex(j + 1)] = _buffer[ConvertIndex(j)];
                    }
                    _offset = (_offset + 1) % Capacity;
                } else {
                    // Removing from second half of list
                    for (var j = (index + 1); j < Count; j++) {
                        _buffer[ConvertIndex(j - 1)] = _buffer[ConvertIndex(j)];
                    }
                }
                Count--;
            }
        }

        /// <summary>
        /// Copies the deque's contents to an array
        /// </summary>
        /// <param name="array">array to copy to</param>
        /// <param name="index">index in the array to start at</param>
        /// <exception cref="ArgumentException">not enough room in the array</exception>
        public void CopyTo(T[] array, int index) {
            if (array.Length - index < Count) throw new ArgumentException("not enough room in array");
            if (_offset > (Capacity - Count)) {
                // The existing buffer is split, so we have to copy it in parts
                int length = Capacity - _offset;
                Array.Copy(_buffer, _offset, array, 0, length);
                Array.Copy(_buffer, 0, array, length, Count - length);
            } else {
                // The existing buffer is whole
                Array.Copy(_buffer, _offset, array, 0, Count);
            }
        }

        /// <summary>
        /// Get the enumator for the deque
        /// </summary>
        /// <returns>Enumerator for the deque</returns>
        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < Count; i++) {
                yield return _buffer[ConvertIndex(i)];
            }
        }

        #endregion

        #region Interface Methods

        void ICollection<T>.Add(T value) {
            PushBack(value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region Private Methods

        private int ConvertIndex(int queueIndex) {
            return (queueIndex + _offset + Capacity) % Capacity;
        }

        private void ExpandIfNeeded() {
            if (IsFull) {
                Capacity = (Capacity == 0) ? 1 : Capacity * 2;
            }
        }

        private void ExpandIfNeeded(int amount) {
            while (Capacity < Count + amount) {
                Capacity = (Capacity == 0) ? 1 : Capacity * 2;
            }
        }

        #endregion
    }
}