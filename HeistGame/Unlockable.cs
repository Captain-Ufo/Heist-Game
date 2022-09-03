////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

namespace HeistGame
{
    internal abstract class Unlockable
    {
        public Lock LockProp { get; protected set; }

        public abstract void Unlock(int deltaTimeMS, Game game);

        public abstract void Unlock(Game game);

        public abstract void Reset();

        public abstract bool IsLocked();

        public string[] GetUnlockProgress()
        {
            string[] progressText = new string[2];

            progressText[0] = $"Lock level: {LockProp.GetCurrentLockLevel()} / {LockProp.GetLockLevel()}";
            progressText[1] = $"{LockProp.GetUnlockingProgress()}%";

            return progressText;
        }

        public void CancelUnlocking()
        {
            LockProp.CancelUnlocking();
        }
    }
}
