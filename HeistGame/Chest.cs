using System;

namespace HeistGame
{
    internal class Chest : Unlockable
    {
        private int treasure = 0;
        private int treasureValue = 0;
        private int x;
        private int y;


        public Chest(int lockLevel, int treasure, int x, int y, ScreenDisplayer sc) : base(sc)
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
                screenDisplayer.DisplayMessageOnLable(new string[] { "The chest is empty." }, true);
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

            game.ActiveCampaign.Levels[game.CurrentRoom].ChangeElementAt(x, y, SymbolsConfig.ChestOpened.ToString(), false);


            if (treasure > 0)
            {
                game.PlayerCharacter.ChangeLoot(treasure);
                screenDisplayer.DeleteLable(game);
                screenDisplayer.DisplayMessageOnLable(new string[] { $"The chest contains $ {treasure} in loot." }, true);
                treasure = 0;
            }
            else
            {
                screenDisplayer.DeleteLable(game);
                screenDisplayer.DisplayMessageOnLable(new string[] { "The chest doesn't contain anything of value." }, true);
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
