using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame.AI.Core
{
    public abstract class BehaviorTree
    {
        private Node root = null;

        protected void Initialize()
        {
            root = SetupTree();
        }

        private void Update()
        {
            if (root != null) { root.Evaluate(); }
        }

        protected abstract Node SetupTree();
    }
}
