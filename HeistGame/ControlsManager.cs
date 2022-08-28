using System;
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
                        switch (State)
                        {
                            case ControlState.Interact:
                                game.Selector.Move(Directions.up);
                                break;
                            case ControlState.Peek:
                                game.PlayerCharacter.Peek(Directions.up, level);
                                break;
                            default:
                                State = ControlState.Move;
                                game.PlayerCharacter.ResetPeek(level);
                                game.PlayerCharacter.Move(Directions.up, level, game, deltaTimeMS);
                                
                                break;
                        }
                        game.CancelUnlocking();
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.NumPad2:
                        switch (State)
                        {
                            case ControlState.Interact:
                                game.Selector.Move(Directions.down);
                                break;
                            case ControlState.Peek:
                                game.PlayerCharacter.Peek(Directions.down, level);
                                break;
                            default:
                                State = ControlState.Move;
                                game.PlayerCharacter.ResetPeek(level);
                                game.PlayerCharacter.Move(Directions.down, level, game, deltaTimeMS);

                                break;
                        }
                        game.CancelUnlocking();
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    case ConsoleKey.NumPad4:
                        switch (State)
                        {
                            case ControlState.Interact:
                                game.Selector.Move(Directions.left);
                                break;
                            case ControlState.Peek:
                                game.PlayerCharacter.Peek(Directions.left, level);
                                break;
                            default:
                                State = ControlState.Move;
                                game.PlayerCharacter.ResetPeek(level);
                                game.PlayerCharacter.Move(Directions.left, level, game, deltaTimeMS);

                                break;
                        }
                        game.CancelUnlocking();
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.NumPad6:
                        switch (State)
                        {
                            case ControlState.Interact:
                                game.Selector.Move(Directions.right);
                                break;
                            case ControlState.Peek:
                                game.PlayerCharacter.Peek(Directions.right, level);
                                break;
                            default:
                                State = ControlState.Move;
                                game.PlayerCharacter.ResetPeek(level);
                                game.PlayerCharacter.Move(Directions.right, level, game, deltaTimeMS);

                                break;
                        }
                        game.CancelUnlocking();
                        break;

                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Add:
                        if (State != ControlState.Interact)
                        {
                            game.PlayerCharacter.ResetPeek(level);
                            game.CancelUnlocking();

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
                            game.PlayerCharacter.ResetPeek(level);
                            game.CancelUnlocking();
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

                    case ConsoleKey.R:
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.Insert:
                        if (State == ControlState.Peek)
                        {
                            game.PlayerCharacter.ResetPeek(level);
                            State = ControlState.Idle;
                        }
                        else
                        {
                            game.Selector.Deactivate();
                            game.CancelUnlocking();
                            State = ControlState.Peek;
                            game.PlayerCharacter.StartPeek();
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
                            game.Selector.Deactivate();
                            //NO INTERACTION! This just cancels it
                            game.CancelUnlocking();
                        }
                        break;
                    default:
                        game.Selector.Deactivate();
                        game.CancelUnlocking();
                        game.PlayerCharacter.ResetPeek(level);
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

    internal enum ControlState { Move, Interact, Yell, Peek, Escape, Idle }
}
