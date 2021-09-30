using System;
using System.Collections.Generic;

namespace HeistGame
{
    /// <summary>
    /// A helper class that contains methods which return the coordinates of each point on a rasterized line.
    /// </summary>
    class Rasterizer
    {
        /// <summary>
        /// A method that calculates the coordinates of the points on a rasterized line.
        /// </summary>
        /// <param name="startX">X coordinate of the beginning point of the line.</param>
        /// <param name="startY">Y coordinate of the beginning point of the line.</param>
        /// <param name="finishX">X coordinates of the end point of the line.</param>
        /// <param name="finishY">Y coordinates of the end point of the line.</param>
        /// <returns>The integer coordinates of the points on the line.</returns>
        public static Vector2[] GetCellsAlongLine(int startX, int startY, int finishX, int finishY)
        {
            bool isLineSteep = Math.Abs(finishY - startY) > Math.Abs(finishX - startX);

            if (isLineSteep)
            {
                int temp = startX;
                startX = startY;
                startY = temp;
                temp = finishX;
                finishX = finishY;
                finishY = temp;
            }

            if (startX > finishX)
            {
                int temp = startX;
                startX = finishX;
                finishX = temp;
                temp = startY;
                startY = finishY;
                finishY = temp;
            }

            int deltaX = finishX - startX;
            int deltaY = Math.Abs(finishY - startY);
            int error = deltaX / 2;
            int yStep = (startY < finishY) ? 1 : -1;
            int y = startY;

            List<Vector2> tilesBetweenGuardAndPlayer = new List<Vector2>();

            for (int x = startX; x <= finishX; x++)
            {
                tilesBetweenGuardAndPlayer.Add(new Vector2((isLineSteep ? y : x), (isLineSteep ? x : y)));
                error = error - deltaY;
                if (error < 0)
                {
                    y += yStep;
                    error += deltaX;
                }
            }

            return tilesBetweenGuardAndPlayer.ToArray();
        }

        /// <summary>
        /// A method that calculates all the points on a rasterized circumference.
        /// </summary>
        /// <param name="centerX">The X coordinate of the center point of the circle</param>
        /// <param name="centerY">The Y coordinate of the center point of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <returns>The integer coordinates of each point on the circumference.</returns>
        public static Vector2[] GetCellsAlongCircumference(int centerX, int centerY, int radius)
        {
            List<Vector2> circumference = new List<Vector2>();
            int circumferencePointX = radius;
            int circumferencePointY = 0;

            circumference.Add(new Vector2(radius + centerX, centerY));

            if (radius > 0)
            {
                circumference.Add(new Vector2(-radius + centerX, centerY));
                circumference.Add(new Vector2(centerX, radius + centerY));
                circumference.Add(new Vector2(centerX, -radius + centerY));
            }

            int medianPoint = 1 - radius;

            while (circumferencePointX > circumferencePointY)
            {
                circumferencePointY++;

                if (medianPoint <= 0)
                {
                    medianPoint = medianPoint + 2 * circumferencePointY + 1;
                }
                else
                {
                    circumferencePointX--;
                    medianPoint = medianPoint + 2 * circumferencePointY - 2 * circumferencePointX + 1;
                }

                if (circumferencePointX < circumferencePointY)
                break;

                circumference.Add(new Vector2(circumferencePointX + centerX, circumferencePointY + centerY));
                circumference.Add(new Vector2(-circumferencePointX + centerX, circumferencePointY + centerY));
                circumference.Add(new Vector2(circumferencePointX + centerX, -circumferencePointY + centerY));
                circumference.Add(new Vector2(-circumferencePointX + centerX, -circumferencePointY + centerY));

                if (circumferencePointX != circumferencePointY)
                {
                    circumference.Add(new Vector2(circumferencePointY + centerX, circumferencePointX + centerY));
                    circumference.Add(new Vector2(-circumferencePointY + centerX, circumferencePointX + centerY));
                    circumference.Add(new Vector2(circumferencePointY + centerX, -circumferencePointX + centerY));
                    circumference.Add(new Vector2(-circumferencePointY + centerX, -circumferencePointX + centerY));
                }
            }

            return circumference.ToArray();
        }
    }
}
