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
            timeToUnlockLevel = 2000;
            unlockingProgress = timeToUnlockLevel;
        }

        public bool IsLocked()
        {
            return currentLockLevel > 0;
        }

        public int GetCurrentLockLevel()
        {
            return currentLockLevel;
        }

        public int GetLockLevel()
        {
            return lockLevel;
        }

        public int GetUnlockingProgress()
        {
            if (unlockingProgress == 0) { return 0; }

            return (timeToUnlockLevel * 100) / unlockingProgress; //gets the percentage of progress
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
