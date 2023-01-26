using System;
using System.Collections.Generic;
using HeistGame.AI.Core;

namespace HeistGame.AI.GuardAI
{
    internal class TaskPatrol : Node
    {
        NPC patroller;

        public TaskPatrol(NPC npc)
        {
            patroller = npc;
        }

        public override NodeState Evaluate()
        {
            return base.Evaluate();
        }
    }
}
