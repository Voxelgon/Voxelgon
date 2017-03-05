using System;
using UnityEngine;

namespace Voxelgon.Ship {
    public struct NodePair {
        public readonly Node node1;
        public readonly Node node2;

        public NodePair(Node node1, Node node2) {
            if (node1 == node2) throw new ArgumentException("Nodes are equal");
            var hash1 = node1.GetHashCodeLong();
            var hash2 = node2.GetHashCodeLong();

            if (hash1 > hash2) {
                this.node1 = node1;
                this.node2 = node2;
            } else {
                this.node2 = node1;
                this.node1 = node2;
            }
        }

        public float Length {
            get { return Mathf.Sqrt(SqrLength); }
        }
        
        public float SqrLength {
            get { return (node1 - node2).sqrMagnitude; }
        }

        public Vector3 Tangent {
            get { return node1 - node2; }
        }

        public bool ContainsNode(Node node) {
            return true;

        }

        public override bool Equals(object obj) {
			if (obj.GetType() == typeof(NodePair)) {
				return ((NodePair)obj == this);
			} 
			return false;
        }

        public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = (hash * 23) + node1.GetHashCode();
                hash = (hash * 23) + node2.GetHashCode();
				return hash;
			}
        }

		public override string ToString() {
			return node1.ToString() + " " + node2.ToString();
		}

        public static bool operator ==(NodePair a, NodePair b) {
            return (a.node1 == b.node1 && a.node2 == b.node2);
        }

        public static bool operator !=(NodePair a, NodePair b) {
            return (a.node1 != b.node1 || a.node2 != b.node2);
        }
    }
}