////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System;

namespace HeistGame
{
    internal class Lock
    {
        private int lockLevel;
        private int currentLockLevel;
        private int timeBetweenTicks;
        private int timeSinceLastTick;
        private int unlockingProgress;

        public Lock (int level)
        {
            lockLevel = level;
            currentLockLevel = level;
            timeBetweenTicks = 50;
            timeSinceLastTick = 0;
            unlockingProgress = 0;
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

        public float GetUnlockingProgress()
        {
            return unlockingProgress;
        }

        public void Unlock(int deltaTimeMS, Game game)
        {
            if (currentLockLevel <= 0)
            {
                return;
            }

            timeSinceLastTick += deltaTimeMS;

            if (timeSinceLastTick > timeBetweenTicks)
            {
                unlockingProgress++;
                timeSinceLastTick = 0;
                if (unlockingProgress % 20 == 0)
                {
                    game.TunePlayer.PlaySFX(150, 50);
                }
            }

            if (unlockingProgress >= 100)
            { 
                game.TunePlayer.PlaySFX(100, 100);
                currentLockLevel--;
                unlockingProgress = 0;
                timeSinceLastTick = 0;
            }
        }

        public void CancelUnlocking()
        {
            unlockingProgress = 0;
        }

        public void Reset()
        {
            currentLockLevel = lockLevel;
            timeSinceLastTick = 0;
            unlockingProgress = 0;
        }
    }
}
