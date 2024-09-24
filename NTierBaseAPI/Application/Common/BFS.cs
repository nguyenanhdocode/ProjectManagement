using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class BFS<T> where T : class
    {
        public delegate void NodeAction(ref T node);

        public NodeAction Action { get; set; }
        public string NodeChidrenFieldName { get; set; }

        public BFS(NodeAction nodeAction, string nodeChidrenFieldName)
        {
            if (nodeAction == null)
                throw new ArgumentNullException(nameof(nodeAction));

            if (nodeChidrenFieldName == null)
                throw new ArgumentNullException(nameof(nodeChidrenFieldName));

            if (typeof(T).GetProperty(nodeChidrenFieldName) == null)
                throw new ArgumentException($"{typeof(T).FullName} is not contain {nameof(nodeChidrenFieldName)}");

            Action = nodeAction;
            NodeChidrenFieldName = nodeChidrenFieldName;
        }

        public void TrevelTree(T root)
        {
            Queue<T> queue = new Queue<T>();
            List<T> visited = new List<T>();

            queue.Enqueue(root);
            visited.Add(root);

            while (queue.Count > 0)
            {
                var top = queue.Dequeue();

                Action(ref top);

                var children = (IEnumerable<T>)(typeof(T).GetProperty(NodeChidrenFieldName)?.GetValue(top));

                if (children == null)
                    continue;

                foreach ( var child in children )
                {
                    if (!visited.Contains(child))
                    {
                        queue.Enqueue(child);
                        visited.Add(child);
                    }
                }
            }
        }
    }
}
