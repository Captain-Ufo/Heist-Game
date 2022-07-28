using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame
{
    internal abstract class Unlockable
    {
        private Lock lockProp;

        public Lock LockProp { get => lockProp; protected set => lockProp = value; }

        public abstract void Unlock(int deltaTimeMS, Game game);

        public abstract void Unlock(Game game);

        public abstract void Reset();

        public abstract bool IsLocked();
    }
}
