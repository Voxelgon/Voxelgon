using System;
using System.Collections.Generic;

namespace Voxelgon.Collections {
    public static class CollectionExtensions {
        public static T Next<T>(IList<T> target, int index) {
            return target[NextIndex(target.Count, index)];
        }

        public static T Next<T>(T[] target, int index) {
            return target[NextIndex(target.Length, index)];
        }

        public static T Prev<T>(IList<T> target, int index) {
            return target[PrevIndex(target.Count, index)];
        }

        public static T Prev<T>(T[] target, int index) {
            return target[PrevIndex(target.Length, index)];
        }

        public static T Offset<T>(IList<T> target, int index, int offset) {
            return target[OffsetIndex(target.Count, index, offset)];
        }

        public static T Offset<T>(T[] target, int index, int offset) {
            return target[OffsetIndex(target.Length, index, offset)];
        }

        public static int NextIndex(int length, int index) {
            return (index % length);
        }

        public static int PrevIndex(int length, int index) {
            return (index + length) % length;
        }

        public static int OffsetIndex(int length, int index, int offset) {
            index += offset;
            while (index < 0) {
                index += length;
            }
            return index % length;
        }
    }
}