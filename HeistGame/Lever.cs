using System;
using System.Collections.Generic;
using System.Text;

namespace HeistGame
{
    /// <summary>
    /// A gameplay element that can switch some impassable blocks on and off
    /// </summary>
    class Lever
    {
        private int x;
        private int y;
        private Vector2[] connectedGates;

        /// <summary>
        /// Whether the lever is on or off
        /// </summary>
        public bool IsOn { get; set; } = false;


        /// <summary>
        /// Toggles the lever between on and off, and updates the gates
        /// </summary>
        /// <param name="level">The level the lever and the connected gates are in</param>
        /// <param name="xOffset">Horizontal offset to account for the centering of the World map on the screen</param>
        /// <param name="yOffset">Vertical offset to account for the centering of the World map on the screen</param>
        public void Toggle(Level level, int xOffset, int yOffset, bool redraw = true)
        {
            IsOn = !IsOn;

            string leverSymbol = SymbolsConfig.LeverOn.ToString();

            if (!IsOn)
            {
                leverSymbol = SymbolsConfig.LeverOff.ToString();
            }

            foreach (Vector2 coordinates in connectedGates)
            {
                if (level.GetElementAt(coordinates.X + xOffset, coordinates.Y + yOffset) == SymbolsConfig.Gate.ToString())
                {
                    level.ChangeElementAt(coordinates.X + xOffset, coordinates.Y + yOffset, SymbolsConfig.Empty.ToString(), true, redraw);
                }
                else
                {
                    level.ChangeElementAt(coordinates.X + xOffset, coordinates.Y + yOffset, SymbolsConfig.Gate.ToString(), true, redraw);
                }
            }

            level.ChangeElementAt(x + xOffset, y + yOffset, leverSymbol, true, redraw);
        }

        /// <summary>
        /// Sets the lever position on the World Map. To be used only when parsing the level file
        /// </summary>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public void SetLeverCoordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Assigns the gates that are swtiched by this lever. To be used only when parsing the level file
        /// </summary>
        /// <param name="gates">The array of gates (in the form of Vector2 objects)</param>
        public void AssignGates(Vector2[] gates)
        {
            connectedGates = gates;
        }
    }
}
