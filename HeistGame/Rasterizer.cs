using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame
{
    class Rasterizer
    {
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

        public static Vector2[] GetCellsAlongCircle(int centerX, int centerY, int radius)
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
