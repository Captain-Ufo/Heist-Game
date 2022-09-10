////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

namespace HeistGame
{
    internal class ExitMap : IMap
    {
        public void RevealMap(Level level)
        {
            level.RevealExitTile();
        }
    }
}
