using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame
{
    internal class Door
    {
        public Lock DoorLock { get; private set; }

        public Door(int lockLevel)
        {
            DoorLock = new Lock(lockLevel);
        }

        public void Open()
        {
            if (DoorLock.IsLocked())
            {
                DoorLock.Unlock();
            }
        }
    }
}
