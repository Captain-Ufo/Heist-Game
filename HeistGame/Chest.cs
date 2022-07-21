using System;

namespace HeistGame
{
    internal class Chest : Unlockable
    {
        private int treasure = 0;


        public Chest(int lockLevel, int treasure)
        {
            LockProp = new Lock(lockLevel);
            this.treasure = treasure;
        }

        public override void Unlock(Game game)
        {
            if (LockProp.IsLocked())
            {
                LockProp.Unlock();
                return;
            }
            
            if (treasure > 0)
            {
                game.PlayerCharacter.ChangeLoot(treasure);
                treasure = 0;
            }
            else
            {
                game.DisplayMessage(new string[] { "The chest doesn't contain anything of value." });
            }
        }
    }
}
