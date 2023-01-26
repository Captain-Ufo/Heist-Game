/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

namespace HeistGame
{
    internal class FullMap : IMap
    {
        public void RevealMap(Level level)
        {
            level.RevealAllMap();
        }
    }
}
