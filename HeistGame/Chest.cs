using System;

namespace HeistGame
{
    internal class Chest : Unlockable
    {
        private int treasure = 0;
        private int treasureValue = 0;


        public Chest(int lockLevel, int treasure)
        {
            LockProp = new Lock(lockLevel);
            this.treasure = treasure;
            treasureValue = treasure;
        }

        public override void Unlock(Game game)
        {
            game.ActiveUnlockable = this;
        }

        public override void Unlock(int deltaTimeMS, Game game)
        {
            if (LockProp.IsLocked())
            {
                LockProp.Unlock(deltaTimeMS, game);
                return;
            }

            if (treasure > 0)
            {
                game.PlayerCharacter.ChangeLoot(treasure);
                treasure = 0;
            }
            else
            {
                game.UserInterface.DisplayMessageOnLable(new string[] { "The chest doesn't contain anything of value." });
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
            treasure = treasureValue;
        }
    }
}
