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
            timeToUnlockLevel = 160 * 5;
            unlockingProgress = timeToUnlockLevel;
        }

        public bool IsLocked()
        {
            return currentLockLevel > 0;
        }

        public int GetLockLevel()
        {
            return currentLockLevel;
        }

        public void Unlock(int deltaTimeMS, Game game)
        {
            unlockingProgress -= deltaTimeMS;
            if (unlockingProgress <= 0)
            {
                currentLockLevel--;

                if (currentLockLevel <= 0)
                {
                    return;
                }

                unlockingProgress = timeToUnlockLevel;
            }
        }

        public void Reset()
        {
            currentLockLevel = lockLevel;
            unlockingProgress = timeToUnlockLevel;
        }
    }
}
