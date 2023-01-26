﻿/////////////////////////////////
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

        private bool isTurned;
        private bool isBribed;
        private int bribeTick;
        private int bribeTimer;

        private bool firstSighted;
        private int searchPivotTimer;
        private int searchPivotDuration;

        private int walkingSpeed;
        private int searchingSpeed;
        private int runningSpeed;

        /// <summary>
        /// To be set depending on difficulty level. If true, it will prevent being bribed a second time
        /// </summary>
        public int TimesBribed { get; private set; }

        public int GuardArt { get; private set; }

        /// <summary>
        /// Instantiates a Guard Object and sets its parameters
        /// </summary>
        public Guard()
        {
            searchPivotDuration = 30;
            walkingSpeed = 200;
            searchingSpeed = 230;
            runningSpeed = 150;
            bribeTick = 60000; // 1 minute
            bribeTimer = 0;

            rng = new Random();
            nextPatrolPoint = 0;
            isBribed = false;
            isTurned = false;
            firstSighted = true;
            TimesBribed = 0;

            GuardArt = rng.Next(0, 100);

            sightDistance = 18;
            hearingDistance = 10;
            previouslyHeardNoiseLevel = 0;
            alertLevel = 0;
            alertRestingLevel = 0;
            alertTimer = 0;
            sightTick = 300;
            hearingTick = 300;

            behavior = AIBehaviors.Neutral;
            state = AIState.NotFoundPlayer;

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

            Health = 10;
            IsUnconscious = false;
            IsDead = false;
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
            if (IsUnconscious || IsDead) { return; }

            base.UpdateBehavior(level, game, deltaTimeMS);

            if (alertTimer > 0) 
            {
                alertTimer -= deltaTimeMS;

                if (alertTimer <= 0)
                {
                    if (game.PlayerCharacter.IsViolent && alertRestingLevel < 4 ) 
                    { 
                        alertRestingLevel++; 
                    }
                    alertLevel = alertRestingLevel;
                    alertTimer = 0;
                }
            }

            UpdateBribe(deltaTimeMS);

            CatchPlayer(game);

            int distance = GetDistanceFromTile(game.PlayerCharacter.X, game.PlayerCharacter.Y);

            int previousAlertLevel = alertLevel;
            int alarm = 0;

            if (!isBribed)
            {
                alarm = GetPerceptionsAlarm(game.PlayerCharacter, level, distance);
            }
            alertLevel += alarm;
            if (alertLevel > previousAlertLevel)
            {
                int timerLength = alertLevel * 3000;

                if (alertTimer < timerLength)
                {
                    alertTimer = timerLength;
                }
            }
            //IMPORTANT! This instruction must be placed here so that the previous block is executed every time the guard
            //registers any disturbance at alertLevel 15: if this is placed before, once at 15 the guard would ignore any
            //further alarm until the alertTimer ticks down.
            if (alertLevel > 15) { alertLevel = 15; }

            //TODO: if health is low, turn on flee behavior no matter what.
            //TODO: temporary. Add some way to recover health. Maybe walk towards a healing point if available?

            if (alertLevel == 15)
            {
                isTurned = false;
                if (alarm > 0)
                {
                    if (previousAlertLevel < 5)
                    {
                        lastPatrolPoint = new Vector2(X, Y);
                    }
                    searchTarget = new Vector2(game.PlayerCharacter.X, game.PlayerCharacter.Y);
                    state = AIState.FoundPlayer;
                    behavior = AIBehaviors.Chasing;
                }
                else if (state == AIState.FoundPlayer)
                {
                    if (X != searchTarget.X && Y != searchTarget.Y)
                    {
                        behavior = AIBehaviors.Chasing;
                    }
                    else
                    {
                        state = AIState.PreviouslyFoundPlayer;
                        behavior = AIBehaviors.Searching;
                        firstSighted = true;
                    }
                }
            }
            else if (alertLevel >= 10 && alertLevel < 15)
            {
                if (previousAlertLevel < 5)
                {
                    lastPatrolPoint = new Vector2(X, Y);
                }
                searchTarget = new Vector2(game.PlayerCharacter.X, game.PlayerCharacter.Y);
                isTurned = false;
                behavior = AIBehaviors.Searching;
            }
            else if (alertLevel >= 5 && alertLevel < 10)
            {
                if (alarm > 0)
                {
                    isTurned = false;
                }
                behavior = AIBehaviors.Suspicious;
            }
            else if (alertLevel < 5)
            {
                isTurned = false;
                NPCTileColor = ConsoleColor.DarkRed;
                if (state == AIState.FoundPlayer || state == AIState.PreviouslyFoundPlayer)
                {
                    behavior = AIBehaviors.Returning;
                }
                else
                {
                    behavior = AIBehaviors.Neutral;
                }
            }

            switch (behavior)
            {
                case AIBehaviors.Neutral:
                    NPCTileColor = ConsoleColor.DarkRed;
                    NormalBehavior(game);
                    break;
                case AIBehaviors.Suspicious:
                    NPCTileColor = ConsoleColor.Magenta;
                    SuspiciousBehavior(game);
                    break;
                case AIBehaviors.Chasing:
                    NPCTileColor = ConsoleColor.Red;
                    ChasePlayer(game);
                    break;
                case AIBehaviors.Searching:
                    NPCTileColor = ConsoleColor.Yellow;
                    Investigate(game);
                    break;
                case AIBehaviors.Returning:
                    NPCTileColor = ConsoleColor.DarkRed;
                    ReturnToPatrol(game);
                    break;
            }
        }

        /// <summary>
        /// Prevents a Game Over
        /// </summary>
        public void BribeGuard()
        {
            isBribed = true;
            behavior = AIBehaviors.Returning;
            state = AIState.NotFoundPlayer;
            alertLevel = 0;
            alertTimer = 0;
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

            if (range == 0) { range = hearingDistance; }

            int horizontalHearingRange = range;
            if (expectedTarget.X >= X - horizontalHearingRange && expectedTarget.X <= X + horizontalHearingRange &&
                expectedTarget.Y >= Y - range && expectedTarget.Y <= Y + range)
            {
                searchTarget = expectedTarget;
                timeBetweenMoves = runningSpeed;
            }
        }

        /// <summary>
        /// Resets the guard to start of level conditions.
        /// </summary>
        public override void Reset()
        {
            nextPatrolPoint = 0;
            isBribed = false;
            bribeTimer = 0;
            alertTimer = 0;
            alertLevel = 0;
            alertRestingLevel = 0;
            state = AIState.NotFoundPlayer;
            behavior = AIBehaviors.Neutral;
            pivotTimer = rng.Next(201);
            searchPivotTimer = 20;
            X = originPoint.X;
            Y = originPoint.Y;
            timeSinceLastMove = 0;
            TimesBribed = 0;
            ChoosePivotDirection();

            Health = 10;
            IsUnconscious = false;
            IsDead = false;
        }

        private void UpdateBribe(int deltaTimeMS)
        {
            bribeTimer += deltaTimeMS;
            if (bribeTimer < bribeTick) { return; }

            isBribed = false;
            TimesBribed++;
        }

        private void SuspiciousBehavior(Game game)
        {
            if (timeSinceLastMove >= timeBetweenMoves)
            {
                timeSinceLastMove -= timeBetweenMoves;
            }

            if (isTurned) { return; }

            bool horizontal = false;
            int xDistance = X - game.PlayerCharacter.X;
            int yDistance = Y - game.PlayerCharacter.Y;

            if (MathF.Abs(xDistance) > MathF.Abs(yDistance)) { horizontal = true; }
            if (horizontal)
            {
                if (xDistance > 0) { Direction = Directions.left; }
                else { Direction = Directions.right; }
            }
            else
            {
                if (yDistance > 0) { Direction = Directions.up; }
                else { Direction = Directions.down; }
            }
            NPCMarker = npcMarkersLUT[(int)Direction];
            isTurned = true;
        }

        private void NormalBehavior(Game game)
        {
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

        private void ChasePlayer(Game game)
        {
            if (firstSighted)
            {
                game.TunePlayer.PlaySFX(800, 600);
                game.TimesSpotted++;
            }

            firstSighted = false;
            timeBetweenMoves = runningSpeed;
            MoveTowards(searchTarget, game);
        }

        private void Investigate(Game game)
        {
            if (timeSinceLastMove < timeBetweenMoves)
            {
                return;
            }

            if (X != searchTarget.X | Y != searchTarget.Y)
            {
                if (MoveTowards(searchTarget, game))
                {
                    if (behavior != AIBehaviors.Searching)
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
            else if (behavior != AIBehaviors.Searching) 
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

                timeSinceLastMove -= timeBetweenMoves;
                return;
            }
            else { SearchPlayer(game.ActiveCampaign.Levels[game.CurrentLevel]); }

            alertTimer++;

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

                    //TODO: add check so that guards prefer dark tiles over fully lit ones.

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
            timeBetweenMoves = walkingSpeed;

            if (X != lastPatrolPoint.X || Y != lastPatrolPoint.Y)
            {
                MoveTowards(new Vector2(lastPatrolPoint.X, lastPatrolPoint.Y), game);
                return;
            }

            behavior = AIBehaviors.Neutral;
            state = AIState.NotFoundPlayer;
        }

        private void CatchPlayer(Game game)
        {
            if (isBribed) { return; }

            switch (Direction)
            {
                case Directions.up:
                    if (game.PlayerCharacter.X >= X - 1 && game.PlayerCharacter.X <= X + 1
                         && game.PlayerCharacter.Y >= Y - 1 && game.PlayerCharacter.Y <= Y)
                    {
                        game.CapturePlayer(this);
                    }
                    break;
                case Directions.right:
                    if (game.PlayerCharacter.X >= X && game.PlayerCharacter.X <= X + 1
                         && game.PlayerCharacter.Y >= Y - 1 && game.PlayerCharacter.Y <= Y + 1)
                    {
                        game.CapturePlayer(this);
                    }
                    break;
                case Directions.down:
                    if (game.PlayerCharacter.X >= X - 1 && game.PlayerCharacter.X <= X + 1
                         && game.PlayerCharacter.Y >= Y && game.PlayerCharacter.Y <= Y + 1)
                    {
                        game.CapturePlayer(this);
                    }
                    break;
                case Directions.left:
                    if (game.PlayerCharacter.X >= X - 1 && game.PlayerCharacter.X <= X
                         && game.PlayerCharacter.Y >= Y - 1 && game.PlayerCharacter.Y <= Y + 1)
                    {
                        game.CapturePlayer(this);
                    }
                    break;
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