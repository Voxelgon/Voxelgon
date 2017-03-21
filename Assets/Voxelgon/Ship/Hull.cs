using System;
using System.Collections.Generic;
using Voxelgon.Util.Geometry;
using UnityEngine;

namespace Voxelgon.Ship {

    [Serializable]
    public class Hull {

        // FIELDS

        private Dictionary<short, Deck> _decks;


        // CONSTRUCTORS

        public Hull() {
            _decks = new Dictionary<short, Deck>();
            AddDeck(0);
        }

        
        // METHODS

        public Deck GetDeck(short level) {
            if (!_decks.ContainsKey(level)) {
                return AddDeck(level);
            }
            return _decks[level];
        }

        private Deck AddDeck(short level) {
            var newDeck = new Deck(this, level);
            _decks.Add(level, newDeck); 
            return newDeck;
        }
    }
}