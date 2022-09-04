////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

namespace HeistGame
{
    internal class TileSelector
    {
        private int anchorX;
        private int anchorY;
        private Game game;

        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IsActive { get; private set; }

        public TileSelector(Game game)
        {
            anchorX = 0;
            anchorY = 0;
            X = 0;
            Y = 0;
            this.game = game;
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
            SetPosition(game.PlayerCharacter.X, game.PlayerCharacter.Y);
        }

        public void Deactivate()
        {
            IsActive = false;
        }


        public void Move(Directions direction)
        {
            switch (direction)
            {
                case Directions.up:
                    if (Y > anchorY - 1)
                    {
                        if (X == anchorX)
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X, Y - 2)) { return; }
                            Y -= 2;
                        }
                        else
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X, Y - 1)) { return; }
                            Y--;
                        }
                    }
                    break;
                case Directions.down:
                    if (Y < anchorY + 1 )
                    {
                        if (X == anchorX)
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X, Y + 2)) { return; }
                            Y += 2;
                        }
                        else
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X, Y + 1)) { return; }
                            Y++;
                        }
                    }
                    break;
                case Directions.left:
                    if (X > anchorX - 1 ) 
                    {
                        if (Y == anchorY)
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X - 2, Y)) { return; }
                            X -= 2;
                        }
                        else
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X - 1, Y)) { return; }
                            X--;
                        }
                    }
                    break;
                case Directions.right:
                    if (X < anchorX + 1) 
                    {
                        if (Y == anchorY)
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X + 2, Y)) { return; }
                            X += 2;
                        }
                        else
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X + 1, Y)) { return; }
                            X++;
                        }
                    }
                    break;
            }
        }

        private void SetPosition(int x, int y)
        {
            anchorX = x;
            anchorY = y;
            X = anchorX;
            Y = anchorY + 1;
            if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileInsideBounds(X, Y))
            {
                Y = anchorY - 1;
            }
        }
    }
}
