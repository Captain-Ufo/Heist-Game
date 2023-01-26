/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

namespace HeistGame
{
    internal class WallsMap : IMap
    {
        public void RevealMap(Level level)
        {
            level.RevealAllAWalls();
        }
    }
}
