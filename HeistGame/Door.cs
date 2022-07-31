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
            if (!IsLocked())
            {
                game.UserInterface.DisplayMessageOnLable(new string[] { "The door is already open." }, true);
                game.ActiveUnlockable = null;
                game.UserInterface.DeleteLable();
                return;
            }

            game.ActiveUnlockable = this;
            game.UserInterface.DisplayMessageOnLable(base.GetUnlockProgress(), true);
        }

        public override void Unlock(int deltaTimeMS, Game game)
        {
            if (LockProp.IsLocked())
            {
                LockProp.Unlock(deltaTimeMS, game);
                game.UserInterface.DisplayMessageOnLable(base.GetUnlockProgress(), false);
                return;
            }

            game.ActiveUnlockable = null;
        }

        public override bool IsLocked()
        {
            return LockProp.IsLocked();
        }

        public override void Reset()
        {
            LockProp.Reset();
        }
    }
}
