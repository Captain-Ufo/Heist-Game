using System;

namespace HeistGame
{
    internal class Chest : Unlockable
    {
        private int treasure = 0;
        private int treasureValue = 0;
        private int x;
        private int y;


        public Chest(int lockLevel, int treasure, int x, int y)
        {
            LockProp = new Lock(lockLevel);
            this.treasure = treasure;
            treasureValue = treasure;
            this.x = x;
            this.y = y;
        }

        public override void Unlock(Game game)
        {
            if (!IsLocked())
            {
                ScreenDisplayer.DisplayMessageOnLable(new string[] { "The chest is empty." });
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

            game.ActiveCampaign.Levels[game.CurrentRoom].ChangeElementAt(x, y, SymbolsConfig.ChestOpened.ToString());


            if (treasure > 0)
            {
                game.PlayerCharacter.ChangeLoot(treasure);
                ScreenDisplayer.DeleteLable(game);
                ScreenDisplayer.DisplayMessageOnLable(new string[] { $"The chest contains $ {treasure} in loot." });
                treasure = 0;
            }
            else
            {
                ScreenDisplayer.DeleteLable(game);
                ScreenDisplayer.DisplayMessageOnLable(new string[] { "The chest doesn't contain anything of value." });
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
