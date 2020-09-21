using System;
using System.Collections.Generic;
using System.Text;

namespace JantarDosFilosofos.Classes
{
    class Fork
    {
        private int id;
        private bool beingUsed;

        /// <summary>
        /// Creates a new Fork class
        /// </summary>
        /// <param name="id">An identity to the fork</param>
        public Fork(int id)
        {
            this.id = id;
            beingUsed = false;
        }

        /// <summary>
        /// Gets the fork identity
        /// </summary>
        /// <returns>Fork identity number</returns>
        public int GetId()
        {
            return id;
        }

        /// <summary>
        /// Lifts the fork
        /// </summary>
        /// <returns>True if it was possible to pick the fork up, false if the fork is already being used</returns>
        public bool BeLifted()
        {
            if (beingUsed)
                return false;

            beingUsed = true;
            return true;
        }
        
        /// <summary>
        /// Places the fork down
        /// </summary>
        /// <returns>True if the fork was placed down, false if the fork was not even lifted in the fist place</returns>
        public bool BePlacedDown()
        {
            // If its not being used, there is no way to be placed down
            if (!beingUsed)
                return false;

            beingUsed = false;
            return true;
        }
    }
}
