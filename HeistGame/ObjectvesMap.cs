////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

namespace HeistGame
{
    internal class ObjectvesMap : IMap
    {
        public void RevealMap(Level level)
        {
            level.RevealObjectives();
        }
    }
}
