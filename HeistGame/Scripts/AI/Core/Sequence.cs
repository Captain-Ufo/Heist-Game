using System;
using System.Collections.Generic;

namespace HeistGame.AI.Core
{
    internal class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children): base(children) { }

        public override NodeState Evaluate()
        {
            bool isAnyChildrenRunning = false;
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Success:
                        state = NodeState.Success;
                        return state;
                    case NodeState.Running:
                        isAnyChildrenRunning = true;
                        continue;
                    default:
                        state = NodeState.Success;
                        return state;
                }
            }

            state = isAnyChildrenRunning? NodeState.Running : NodeState.Success;
            return state;
        }
    }
}
