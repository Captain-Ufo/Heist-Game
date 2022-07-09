using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame
{
    internal class Lock
    {
        private int lockLevel;
        private int currentLockLevel;
        private int timeToUnlockLevel;
        private int unlockingProgress;

        public Lock (int level)
        {
            lockLevel = level;
            currentLockLevel = level;
        }

        public bool IsLocked()
        {
            return currentLockLevel <= 0;
        }

        public void Unlock()
        {

        }
    }
}
