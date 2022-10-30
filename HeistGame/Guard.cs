/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// A gameplay element that patrols the level or pivots in place, and spots, chases and catches the player if within range.
    /// </summary>
    class Guard : NPC
    {
        private Vector2[] patrolPath;
        private int nextPatrolPoint;
        private Vector2 lastPatrolPoint;
        private Vector2 originPoint;
        private Vector2 searchTarget;

        private bool isBribed;
        private int bribeTick;
        private int bribeTimer;

        private int alertTimer;
        private bool isAlerted;
        private bool isSearching;
        private bool firstSighted;
        private bool isReturning;
        private int searchPivotTimer;
        private int searchPivotDuration;

        private int walkingSpeed;
        private int searchingSpeed;
        private int runningSpeed;

        private int hearingRange;

        /// <summary>
        /// To be set depending on difficulty level. If true, it will prevent being bribed a second time
        /// </summary>
        public int TimesBribed { get; private set; }

        /// <summary>
        /// Instantiates a Guard Object and sets its parameters
        /// </summary>
        public Guard()
        {
            searchPivotDuration = 30;
            walkingSpeed = 200;
            searchingSpeed = 230;
            runningSpeed = 100;
            hearingRange = 9;
            bribeTick = 60000; // 1 minute
            bribeTimer = 0;

            rng = new Random();
            nextPatrolPoint = 0;
            isBribed = false;
            isAlerted = false;
            isSearching = false;
            firstSighted = true;
            isReturning = false;
            TimesBribed = 0;

            alertTimer = 0;
            pivotTimer = rng.Next(201);
            durationTimer = searchPivotDuration;
            minTimeBetweenPivots = 0;
            timeBetweenMoves = walkingSpeed;
            Direction = (Directions)rng.Next(0, 4);
            NPCMarker = npcMarkersLUT[(int)Direction];
            NPCSymbolColor = ConsoleColor.Black;
            NPCTileColor = ConsoleColor.DarkRed;
            ChoosePivotDirection();
        }

        /// <summary>
        /// Assigns the Guard's initial position when the level is first loaded. To be used only when parsing the level file.
        /// </summary>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public void AssignOriginPoint(int x, int y)
        {
            X = x;
            Y = y;

            originPoint = new Vector2(X, Y);
            lastPatrolPoint = originPoint;
        }

        /// <summary>
        /// Assigns the patrol path
        /// </summary>
        /// <param name="path">An array of patrol path points in the form of a Coordinates objects</param>
        public void AssignPatrol(Vector2[] path)
        {
            patrolPath = path;
        }

        public bool IsStationary()
        {
            return patrolPath.Length > 0;
        }

        public override void UpdateBehavior(Level level, Game game, int deltaTimeMS)
        {
            timeSinceLastMove += deltaTimeMS;

            CatchPlayer(game);


            UpdateBribe(deltaTimeMS);

            if (CanSeePlayer(game.PlayerCharacter, level))
            {
                if (firstSighted)
                {
                    game.TunePlayer.PlaySFX(800, 600);
                    game.TimesSpotted++;
                }

                NPCTileColor = ConsoleColor.Red;
                searchTarget = new Vector2(game.PlayerCharacter.X, game.PlayerCharacter.Y);
                isAlerted = true;
                firstSighted = false;
                isReturning = false;
                timeBetweenMoves = runningSpeed;
                MoveTowards(searchTarget, game);
            }
            else if (isAlerted)
            {
                NPCTileColor = ConsoleColor.Magenta;
                firstSighted = true;
                AlertedBehavior(game);
            }
            else if (isReturning)
            {
                NPCTileColor = ConsoleColor.DarkRed;
                ReturnToPatrol(game);
            }
            else
            {
                NPCTileColor = ConsoleColor.DarkRed;
                timeBetweenMoves = walkingSpeed;
                if (patrolPath.Length > 0)
                {
                    MoveTowards(Patrol(), game);
                }
                else
                {
                    Pivot(pivotTimer, 20, 30);

                    if (pivotTimer == int.MaxValue) { pivotTimer = 0; }
                    else { pivotTimer += rng.Next(1, 5); }
                }
                lastPatrolPoint.X = X;
                lastPatrolPoint.Y = Y;
            }
        }

        /// <summary>
        /// Prevents a Game Over
        /// </summary>
        public void BribeGuard()
        {
            isBribed = true;
            if (isAlerted)
            {
                isAlerted = false;
                isReturning = true;
            }
        }

        /// <summary>
        /// Used to alert the guard while out of their line of sight
        /// </summary>
        /// <param name="expectedTarget">The place they'll investigate while alerted</param>
        public void AlertGuard(Vector2 expectedTarget, int range = 0)
        {
            if (isBribed)
            {
                return;
            }

            if (range == 0) { range = hearingRange; }

            int horizontalHearingRange = range;
            if (expectedTarget.X >= X - horizontalHearingRange && expectedTarget.X <= X + horizontalHearingRange &&
                expectedTarget.Y >= Y - range && expectedTarget.Y <= Y + range)
            {
                searchTarget = expectedTarget;
                isAlerted = true;
                timeBetweenMoves = runningSpeed;
            }
        }

        public override void Reset()
        {
            nextPatrolPoint = 0;
            isBribed = false;
            bribeTimer = 0;
            alertTimer = 0;
            isAlerted = false;
            isSearching = false;
            isReturning = false;
            pivotTimer = rng.Next(201);
            searchPivotTimer = 20;
            X = originPoint.X;
            Y = originPoint.Y;
            timeSinceLastMove = 0;
            TimesBribed = 0;
            ChoosePivotDirection();
        }

        private void UpdateBribe(int deltaTimeMS)
        {
            bribeTimer += deltaTimeMS;
            if (bribeTimer < bribeTick) { return; }

            isBribed = false;
            TimesBribed++;
        }

        /// <summary>
        /// Wraps the base class method and adds a check f
        /// </summary>
        /// <param name="player"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool CanSeePlayer(Player player, Level level)
        {
            if (isBribed)
            {
                return false;
            }

            return SpotPlayer(player, level);
        }

        private void AlertedBehavior(Game game)
        {
            if (timeSinceLastMove < timeBetweenMoves)
            {
                return;
            }

            if (X != searchTarget.X | Y != searchTarget.Y)
            {
                if (MoveTowards(searchTarget, game))
                {
                    if (!isSearching)
                    {
                        alertTimer = 0;
                        timeSinceLastMove -= timeBetweenMoves;
                        return;
                    }
                }
                else
                {
                    searchTarget.X = X;
                    searchTarget.Y = Y;
                }
            }
            else if (!isSearching) 
            { 
                timeBetweenMoves = searchingSpeed;
                int x = X;
                int y = Y;

                switch (Direction)
                {
                    case Directions.up:
                        y -= 10;
                        break;
                    case Directions.right:
                        x += 10;
                        break;
                    case Directions.down:
                        y += 10;
                        break;
                    case Directions.left:
                        x -= 10;
                        break;
                }

                Vector2[] tilesToTarget = Rasterizer.GetCellsAlongLine(this.X, this.Y, x, y);
                foreach (Vector2 tile in tilesToTarget)
                {
                    if (!game.ActiveCampaign.Levels[game.CurrentLevel].IsTileWalkable(tile.X, tile.Y))
                    {
                        break;
                    }

                    x = tile.X;
                    y = tile.Y;
                }

                searchTarget.X = x;
                searchTarget.Y = y;

                isSearching = true;
                timeSinceLastMove -= timeBetweenMoves;
                return;
            }
            else { SearchPlayer(game.ActiveCampaign.Levels[game.CurrentLevel]); }

            alertTimer++;

            if (alertTimer > 350)
            {
                alertTimer = 0;
                isAlerted = false;
                isSearching = false;
                NPCTileColor = ConsoleColor.DarkRed;
                timeBetweenMoves = walkingSpeed;
                isReturning = true;
            }
            timeSinceLastMove -= timeBetweenMoves;
        }

        private void SearchPlayer(Level level)
        {
            ChoosePivotDirection();
            if (!Pivot(alertTimer, 10, 0, searchPivotTimer, true))
            {
                bool hasValidTarget;
                do
                {
                    hasValidTarget = true;
                    searchTarget.X = rng.Next(X - 8, X + 9);
                    searchTarget.Y = rng.Next(Y - 8, Y + 9);

                    Vector2[] tilesToTarget = Rasterizer.GetCellsAlongLine(this.X, this.Y, searchTarget.X, searchTarget.Y);
                    foreach(Vector2 tile in tilesToTarget)
                    {
                        if (!level.IsTileWalkable(tile.X, tile.Y))
                        {
                            hasValidTarget = false;
                            break;
                        }
                    }
                }
                while (!hasValidTarget);
            }
        }

        private void ReturnToPatrol(Game game)
        {
            if (X != lastPatrolPoint.X || Y != lastPatrolPoint.Y)
            {
                MoveTowards(new Vector2(lastPatrolPoint.X, lastPatrolPoint.Y), game);
                return;
            }

            isReturning = false;
        }

        private void CatchPlayer(Game game)
        {
            if (isBribed)
            {
                return;
            }

            if (game.PlayerCharacter.X >= X - 1 && game.PlayerCharacter.X <= X + 1
                && game.PlayerCharacter.Y >= Y - 1 && game.PlayerCharacter.Y <= Y + 1)
            {
                game.CapturePlayer(this);
            }
        }

        private Vector2 Patrol()
        {
            if (X == patrolPath[nextPatrolPoint].X && Y == patrolPath[nextPatrolPoint].Y)
            {
                if (nextPatrolPoint < patrolPath.Length - 1)
                {
                    nextPatrolPoint++;
                }
                else
                {
                    nextPatrolPoint = 0;
                }
            }

            return new Vector2(patrolPath[nextPatrolPoint].X, patrolPath[nextPatrolPoint].Y);
        }
    }
}
