﻿using System;
using static System.Console;

namespace HeistGame
{
    internal static class ControlsManager
    {
        public static ControlState State { get; set; }

        public static ControlState HandleInputs(Level level, Game game, int deltaTimeMS)
        {
            if (KeyAvailable)
            {
                ConsoleKey key;
                do
                {
                    ConsoleKeyInfo keyInfo = ReadKey(true);
                    key = keyInfo.Key;
                }
                while (KeyAvailable);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.NumPad8:
                        if (State != ControlState.Interact)
                        {
                            game.PlayerCharacter.Move(Directions.up, level, game, deltaTimeMS);
                            State = ControlState.Move;
                        }
                        else
                        {
                            game.Selector.Move(Directions.up);
                        }
                        game.ActiveUnlockable = null;
                        game.UserInterface.DeleteLable();
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.NumPad2:
                        if (State != ControlState.Interact)
                        {
                            game.PlayerCharacter.Move(Directions.down, level, game, deltaTimeMS);
                            State = ControlState.Move;
                        }
                        else
                        {
                            game.Selector.Move(Directions.down);
                        }
                        game.ActiveUnlockable = null;
                        game.UserInterface.DeleteLable();
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    case ConsoleKey.NumPad4:
                        if (State != ControlState.Interact)
                        {
                            game.PlayerCharacter.Move(Directions.left, level, game, deltaTimeMS);
                            State = ControlState.Move;
                        }
                        else
                        {
                            game.Selector.Move(Directions.left);
                        }
                        game.ActiveUnlockable = null;
                        game.UserInterface.DeleteLable();
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.NumPad6:
                        if (State != ControlState.Interact)
                        {
                            game.PlayerCharacter.Move(Directions.right, level, game, deltaTimeMS);
                            State = ControlState.Move;
                        }
                        else
                        {
                            game.Selector.Move(Directions.right);
                        }
                        game.ActiveUnlockable = null;
                        game.UserInterface.DeleteLable();
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Add:
                        if (State != ControlState.Interact)
                        {
                            game.ActiveUnlockable = null;
                            game.UserInterface.DeleteLable();

                            game.PlayerCharacter.MakeNoise(level, game);
                            State = ControlState.Yell;
                        }
                        else
                        {
                            if (!game.ActiveCampaign.Levels[game.CurrentRoom].InteractWithElementAt(game.Selector.X, game.Selector.Y, game))
                            {
                                State = ControlState.Idle;
                                game.Selector.Deactivate();
                            }

                            State = ControlState.Idle;
                        }
                        break;
                    case ConsoleKey.E:
                    case ConsoleKey.Enter:
                        if (State != ControlState.Interact)
                        {
                            game.ActiveUnlockable = null;
                            game.UserInterface.DeleteLable();
                            State = ControlState.Interact;
                            game.Selector.Activate();
                        }
                        else
                        {

                            if (!game.ActiveCampaign.Levels[game.CurrentRoom].InteractWithElementAt(game.Selector.X, game.Selector.Y, game))
                            { 
                                State = ControlState.Idle;
                                game.Selector.Deactivate(); 
                            }
                        }
                        break;
                    case ConsoleKey.Escape:
                        if (State != ControlState.Interact)
                        {
                            State = ControlState.Escape;
                        }
                        else
                        {
                            State = ControlState.Idle;
                            //NO INTERACTION! This just cancels it
                            game.Selector.Deactivate();
                            game.ActiveUnlockable = null;
                            game.UserInterface.DeleteLable();
                        }
                        break;
                    default:
                        game.Selector.Deactivate();
                        game.ActiveUnlockable = null;
                        game.UserInterface.DeleteLable();
                        State = ControlState.Idle;
                        break;
                }
            }
            return State;
        }

        public static void ResetControlState(Game game)
        {
            game.Selector.Deactivate();
            State = ControlState.Idle;
        }
    }

    internal enum ControlState { Move, Interact, Yell, Escape, Idle }
}
