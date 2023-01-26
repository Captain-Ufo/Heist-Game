/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

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
                ScreenDisplayer.DisplayMessageOnLabel(new string[] { "The door is already open." });
                game.ActiveUnlockable = null;
                return;
            }

            game.ActiveUnlockable = this;
            ScreenDisplayer.DisplayMessageOnLabel(base.GetUnlockProgress());
        }

        public override void Unlock(int deltaTimeMS, Game game)
        {
            if (LockProp.IsLocked())
            {
                LockProp.Unlock(deltaTimeMS, game);
                ScreenDisplayer.DisplayMessageOnLabel(base.GetUnlockProgress());
                return;
            }
            ScreenDisplayer.DeleteLabel();
            ScreenDisplayer.DisplayMessageOnLabel(new string[] { "Unlocked." });
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
