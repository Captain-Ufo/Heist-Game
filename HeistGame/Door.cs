////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

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
                ScreenDisplayer.DisplayMessageOnLable(new string[] { "The door is already open." });
                game.ActiveUnlockable = null;
                return;
            }

            game.ActiveUnlockable = this;
            ScreenDisplayer.DisplayMessageOnLable(base.GetUnlockProgress());
        }

        public override void Unlock(int deltaTimeMS, Game game)
        {
            if (LockProp.IsLocked())
            {
                LockProp.Unlock(deltaTimeMS, game);
                ScreenDisplayer.DisplayMessageOnLable(base.GetUnlockProgress());
                return;
            }
            ScreenDisplayer.DeleteLable(game);
            ScreenDisplayer.DisplayMessageOnLable(new string[] { "Unlocked." });
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
