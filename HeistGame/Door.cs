using System;

namespace HeistGame
{
    internal class Door : Unlockable
    {
        public Door(int lockLevel)
        {
            LockProp = new Lock(lockLevel);
        }

        public override void Unlock(Game game)
        {
            if (LockProp.IsLocked())
            {
                LockProp.Unlock();
            }
        }
    }
}
