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
        /// A method that calculates all the coordinates of the points on a rasterized line.
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

            List<Vector2> tilesOnTheLine = new List<Vector2>();

            for (int x = startX; x <= finishX; x++)
            {
                tilesOnTheLine.Add(new Vector2((isLineSteep ? y : x), (isLineSteep ? x : y)));
                error = error - deltaY;
                if (error < 0)
                {
                    y += yStep;
                    error += deltaX;
                }
            }

            return tilesOnTheLine.ToArray();
        }

        public static Vector2[] PlotRasterizedLine(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x0 - x1);
            int dy = Math.Abs(y0 - y1);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = dx - dy;

            List<Vector2> tilesOnTheLine = new List<Vector2>();

            for (int i = 0; true; i++)
            {
                tilesOnTheLine.Add(new Vector2(x0, y0));

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = err * 2;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                else if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return tilesOnTheLine.ToArray();
        }

        /// <summary>
        /// A method that calculates the coordinates of all the points on a rasterized circumference.
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

        /// <summary>
        /// A method that returns all the coordinates of the points on a rasterized ellipse.
        /// </summary>
        /// <param name="centerX">The X coordinate of the center point of the ellipse.</param>
        /// <param name="centerY">The Y coordinate of the center point of the ellipse.</param>
        /// <param name="semiAxisX">The horizontal semiaxis of the ellipse</param>
        /// <param name="semiAxisY">The vertical semiaxis of the ellipse</param>
        /// <returns></returns>
        public static Vector2[] GetCellsAlongEllipse(int centerX, int centerY, int semiAxisX, int semiAxisY)
        {

            double dx, dy, d1, d2, x, y;
            x = 0;
            y = semiAxisY;

            // Initial decision parameter of region 1
            d1 = (semiAxisY * semiAxisY) - (semiAxisX * semiAxisX * semiAxisY) +
                            (0.25f * semiAxisX * semiAxisX);
            dx = 2 * semiAxisY * semiAxisY * x;
            dy = 2 * semiAxisX * semiAxisX * y;

            List<Vector2> ellipse = new List<Vector2>();

            // For region 1
            while (dx < dy)
            {

                // Print points based on 4-way symmetry
                ellipse.Add(new Vector2((int)(x + centerX), (int)(y + centerY)));
                ellipse.Add(new Vector2((int)(-x + centerX), (int)(y + centerY)));
                ellipse.Add(new Vector2((int)(x + centerX), (int)(-y + centerY)));
                ellipse.Add(new Vector2((int)(-x + centerX), (int)(-y + centerY)));

                // Checking and updating value of
                // decision parameter based on algorithm
                if (d1 < 0)
                {
                    x++;
                    dx = dx + (2 * semiAxisY * semiAxisY);
                    d1 = d1 + dx + (semiAxisY * semiAxisY);
                }
                else
                {
                    x++;
                    y--;
                    dx = dx + (2 * semiAxisY * semiAxisY);
                    dy = dy - (2 * semiAxisX * semiAxisX);
                    d1 = d1 + dx - dy + (semiAxisY * semiAxisY);
                }
            }

            // Decision parameter of region 2
            d2 = ((semiAxisY * semiAxisY) * ((x + 0.5f) * (x + 0.5f)))
                + ((semiAxisX * semiAxisX) * ((y - 1) * (y - 1)))
                - (semiAxisX * semiAxisX * semiAxisY * semiAxisY);

            // Plotting points of region 2
            while (y >= 0)
            {
                ellipse.Add(new Vector2((int)(x + centerX), (int)(y + centerY)));
                ellipse.Add(new Vector2((int)(-x + centerX), (int)(y + centerY)));
                ellipse.Add(new Vector2((int)(x + centerX), (int)(-y + centerY)));
                ellipse.Add(new Vector2((int)(-x + centerX), (int)(-y + centerY)));

                // Checking and updating parameter
                // value based on algorithm
                if (d2 > 0)
                {
                    y--;
                    dy = dy - (2 * semiAxisX * semiAxisX);
                    d2 = d2 + (semiAxisX * semiAxisX) - dy;
                }
                else
                {
                    y--;
                    x++;
                    dx = dx + (2 * semiAxisY * semiAxisY);
                    dy = dy - (2 * semiAxisX * semiAxisX);
                    d2 = d2 + dx - dy + (semiAxisX * semiAxisX);
                }
            }

            return ellipse.ToArray();
        }
    }
}
