using System;
using Voxelgon.Util;
using UnityEngine;

namespace Voxelgon.Ship {

    public class Deck {

        // FIELDS

        private readonly Hull _hull;

        private readonly int _level;
        private BVH<Wall> _walls;


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
            _walls = new BVH<Wall>();
        }


        // METHODS

        public bool AddWall(Wall wall) {
            if (wall.DeckLevel == _level) {
                _walls.Add(wall);
            }
            return false;
        }
    }
}