using System;
using System.Collections.Generic;
using System.Text;

namespace JantarDosFilosofos.Classes
{
    class Fork
    {
        private int id;
        private bool beingUsed;

        public Fork(int id)
        {
            this.id = id;
            beingUsed = false;
        }

        public int GetId()
        {
            return id;
        }
        public bool BeLifted()
        {
            if (beingUsed)
                return false;

            beingUsed = true;
            return true;
        }

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
