////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System;

namespace HeistGame
{
    internal class Door : Unlockable
    {
        public Door(int lockLevel, ScreenDisplayer sc) : base(sc)
        {
            LockProp = new Lock(lockLevel);
        }

        public override void Unlock(Game game)
        {
            if (!IsLocked())
            {
                screenDisplayer.DisplayMessageOnLable(new string[] { "The door is already open." }, true);
                game.ActiveUnlockable = null;
                return;
            }

            game.ActiveUnlockable = this;
            screenDisplayer.DisplayMessageOnLable(base.GetUnlockProgress(), true);
        }

        public override void Unlock(int deltaTimeMS, Game game)
        {
            if (LockProp.IsLocked())
            {
                LockProp.Unlock(deltaTimeMS, game);
                screenDisplayer.DisplayMessageOnLable(base.GetUnlockProgress(), false);
                return;
            }
            screenDisplayer.DeleteLable(game);
            screenDisplayer.DisplayMessageOnLable(new string[] { "Unlocked." }, true);
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
