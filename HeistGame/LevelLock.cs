﻿using System.Collections.Generic;

namespace HeistGame
{
    /// <summary>
    /// Handles the keys that unlock the level exit
    /// </summary>
    class LevelLock
    {
        private int revealedKeyPieces;
        private int hiddenKeyPieces;
        private int hiddenKeyGroup;

        private Dictionary<int, List<Vector2>> levelKeys;
        private List<Vector2> keysGroup1;
        private List<Vector2> keysGroup2;
        private List<Vector2> keysGroup3;
        private List<Vector2> keysGroup4;


        public LevelLock()
        {
            hiddenKeyGroup = 2;
            revealedKeyPieces = 0;

            keysGroup1 = new List<Vector2>();
            keysGroup2 = new List<Vector2>();
            keysGroup3 = new List<Vector2>();
            keysGroup4 = new List<Vector2>();

            levelKeys = new Dictionary<int, List<Vector2>>
            {
                [1] = keysGroup1,
                [2] = keysGroup2,
                [3] = keysGroup3,
                [4] = keysGroup4
            };
        }

        /// <summary>
        /// Collects the key piece, by removing it from the map and, if necessary, spawning the next key piece
        /// </summary>
        /// <param name="level">The current level</param>
        /// <param name="x">The X coordinate of the piece the player is collecting</param>
        /// <param name="y">The X coordinate of the piece the player is collecting</param>
        /// <returns>returns whether the level is still locked or not (so true if there are other pieces to collect, false if there are none)</returns>
        public bool CollectKeyPiece(Level level, int x, int y)
        {
            level.ChangeElementAt(x, y, SymbolsConfig.EmptySpace.ToString());

            revealedKeyPieces--;

            if (revealedKeyPieces <= 0)
            {
                if (hiddenKeyPieces > 0)
                {
                    RevealKeys(level);
                    return true;
                }
                return false;
            }

            return true;
        }

        public bool IsLocked()
        {
            return revealedKeyPieces > 0;
        }

        /// <summary>
        /// Adds the key to the correct group, and increases the relevant counter (revealedKeyPieces for the first group, hiddenKeyPieces for any other)
        /// </summary>
        /// <param name="x">The x coordinate of the key</param>
        /// <param name="y">The y coordinate of the key</param>
        /// <param name="group">The group where the key belong (used to distinguish between revealed and hidden keys)</param>
        public void AddKey(int x, int y, int group)
        {
            levelKeys[group].Add(new Vector2(x, y));

            if (group == 1)
            {
                revealedKeyPieces++;
            }
            else if (group > 1)
            {
                hiddenKeyPieces++;
            }
        }

        /// <summary>
        /// Resets the lock and all the keys to the state they are at the beginning of the level (uncollects keys, hides hidden keys, re-locks the exit, etc)
        /// </summary>
        /// <param name="level">The level the lock is in</param>
        public void ResetKeys(Level level)
        {
            revealedKeyPieces = 0;
            hiddenKeyPieces = 0;
            hiddenKeyGroup = 2;

            for (int i = 1; i <= levelKeys.Count; i++)
            {
                foreach (Vector2 key in levelKeys[i])
                {
                    if (i == 1)
                    {
                        revealedKeyPieces++;
                        level.ChangeElementAt(key.X, key.Y, SymbolsConfig.KeyChar.ToString(), false, false);
                    }
                    else
                    {
                        hiddenKeyPieces++;
                        level.ChangeElementAt(key.X, key.Y, SymbolsConfig.EmptySpace.ToString(), false, false);
                    }
                }
            }
        }

        private void RevealKeys(Level level)
        {
            foreach (Vector2 key in levelKeys[hiddenKeyGroup])
            {
                revealedKeyPieces++;
                hiddenKeyPieces--;
                level.ChangeElementAt(key.X, key.Y, SymbolsConfig.KeyChar.ToString(), false);
            }
            hiddenKeyGroup++;
        }
    }
}
