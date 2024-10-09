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
        public delegate void NodeAction(T node);
        public delegate Task NodeActionAsync(T node);

        public void TrevelTree(NodeAction action, T root, string nodeChidrenFieldName)
        {
            Queue<T> queue = new Queue<T>();
            List<T> visited = new List<T>();

            queue.Enqueue(root);
            visited.Add(root);

            while (queue.Count > 0)
            {
                var top = queue.Dequeue();

                action(top);

                var children = (IEnumerable<T>)(typeof(T).GetProperty(nodeChidrenFieldName)?.GetValue(top));

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

        public async void TrevelTreeAsync(NodeActionAsync action, T root, string nodeChidrenFieldName)
        {
            Queue<T> queue = new Queue<T>();
            List<T> visited = new List<T>();

            queue.Enqueue(root);
            visited.Add(root);

            while (queue.Count > 0)
            {
                var top = queue.Dequeue();

                await action.Invoke(top);

                var children = (IEnumerable<T>)(typeof(T).GetProperty(nodeChidrenFieldName)?.GetValue(top));

                if (children == null)
                    continue;

                foreach (var child in children)
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
