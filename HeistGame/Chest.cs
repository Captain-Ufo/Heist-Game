using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame
{
    internal class Chest
    {
        private int treasure = 0;

        public Lock ChestLock { get; private set; }

        public Chest(int lockLevel, int treasure)
        {
            ChestLock = new Lock(lockLevel);
            this.treasure = treasure;
        }

        public void Open(Game game)
        {
            if (ChestLock.IsLocked())
            {
                ChestLock.Unlock();
            }
            
            if (treasure > 0)
            {
                game.PlayerCharacter.ChangeLoot(treasure);
                treasure = 0;
            }
            else
            {
                game.DisplayMessage("The chest is empty.");
            }
        }
    }
}
