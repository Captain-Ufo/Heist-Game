using HeistGame.AI.Core;
using System;
using System.Collections.Generic;

namespace HeistGame.AI.GuardAI
{
    internal class GuardAITree : BehaviorTree
    {
        protected override Node SetupTree()
        {
            //Node root = new TaskPatrol();
            return new Node();
        }
    }
}
