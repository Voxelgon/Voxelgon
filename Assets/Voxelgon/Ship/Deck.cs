using System;
using Voxelgon.Util;
using UnityEngine;

namespace Voxelgon.Ship {

    public class Deck {

        // FIELDS

        private readonly Hull _hull;

        private readonly int _level;
        private BVH<Panel> _panels;


        // PROPERTIES

        public Hull Hull {
            get { return _hull; }
        }

        public int Level {
            get { return _level; }
        }

        // CONSTRUCTORS

        public Deck(Hull hull, int level) {
            _level = level;
            _hull = hull;
            _panels = new BVH<Panel>();
        }


        // METHODS

        public bool AddPanel(Panel panel) {
            if (panel.DeckLevel == _level) {
                _panels.Add(panel);
            }
            return false;
        }
    }
}