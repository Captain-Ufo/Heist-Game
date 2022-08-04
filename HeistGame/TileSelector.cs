using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Draw();
        }

        public void Deactivate()
        {
            IsActive = false;
            Clear();
        }


        public void Move(Directions direction)
        {
            Clear();

            switch (direction)
            {
                case Directions.up:
                    if (Y > anchorY - 1)
                    {
                        if (X == anchorX)
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X, Y - 2) == null) { return; }
                            Y -= 2;
                        }
                        else
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X, Y - 1) == null) { return; }
                            Y--;
                        }
                    }
                    break;
                case Directions.down:
                    if (Y < anchorY + 1 )
                    {
                        if (X == anchorX)
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X, Y + 2) == null) { return; }
                            Y += 2;
                        }
                        else
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X, Y + 1) == null) { return; }
                            Y++;
                        }
                    }
                    break;
                case Directions.left:
                    if (X > anchorX - 1 ) 
                    {
                        if (Y == anchorY)
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X - 2, Y) == null) { return; }
                            X -= 2;
                        }
                        else
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X - 1, Y) == null) { return; }
                            X--;
                        }
                    }
                    break;
                case Directions.right:
                    if (X < anchorX + 1) 
                    {
                        if (Y == anchorY)
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X + 2, Y) == null) { return; }
                            X += 2;
                        }
                        else
                        {
                            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X + 1, Y) == null) { return; }
                            X++;
                        }
                    }
                    break;
            }

            Draw();
        }

        private void SetPosition(int x, int y)
        {
            anchorX = x;
            anchorY = y;
            X = anchorX;
            Y = anchorY + 1;
            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X, Y) == null)
            {
                Y = anchorY - 1;
            }
        }

        public void Draw()
        {
            Level level = game.ActiveCampaign.Levels[game.CurrentRoom];
            level.DrawTile(X, Y, level.GetElementAt(X, Y), true);

        }

        private void Clear()
        {
            Level level = game.ActiveCampaign.Levels[game.CurrentRoom];
            level.DrawTile(X, Y, level.GetElementAt(X, Y), false);
        }
    }
}
