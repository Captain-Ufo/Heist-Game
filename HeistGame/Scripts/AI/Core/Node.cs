using System.Collections;
using System.Collections.Generic;

namespace HeistGame.AI.Core
{
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public class Node
    {
        public Node Parent { get; set; }

        protected NodeState state;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public Node()
        {
            Parent = null;
        }

        public Node (List<Node> children)
        {

        }

        public virtual NodeState Evaluate() => NodeState.Failure;

        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;

            if (dataContext.TryGetValue(key, out value)) { return value; }

            Node node = Parent;
            while ( node != null)
            {
                value = node.GetData(key);
                if (value != null) { return value; }
                node = node.Parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            object value = null;

            if (dataContext.ContainsKey(key)) 
            { 
                dataContext.Remove(key);
                return true; 
            }

            Node node = Parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared) { return false; }
                node = node.Parent;
            }
            return false;
        }

        private void AttachChildren(Node node)
        {
            node.Parent = this;
            children.Add(node);
        }
    }
}
