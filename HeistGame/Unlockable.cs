////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

namespace HeistGame
{
    internal abstract class Unlockable
    {
        private Lock lockProp;
        protected ScreenDisplayer screenDisplayer;

        public Lock LockProp { get => lockProp; protected set => lockProp = value; }

        public Unlockable(ScreenDisplayer sc)
        {
            screenDisplayer = sc;
        }

        public abstract void Unlock(int deltaTimeMS, Game game);

        public abstract void Unlock(Game game);

        public abstract void Reset();

        public abstract bool IsLocked();

        public string[] GetUnlockProgress()
        {
            string[] progressText = new string[2];

            progressText[0] = $"Lock level: {lockProp.GetCurrentLockLevel()} / {lockProp.GetLockLevel()}";
            progressText[1] = $"{lockProp.GetUnlockingProgress()}%";

            return progressText;
        }

        public void CancelUnlocking()
        {
            lockProp.CancelUnlocking();
        }
    }
}
