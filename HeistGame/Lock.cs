using System;

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
            return currentLockLevel > 0;
        }

        public int GetLockLevel()
        {
            return currentLockLevel;
        }

        public void Unlock()
        {
            Console.WriteLine("Unlocked!");
        }
    }
}
