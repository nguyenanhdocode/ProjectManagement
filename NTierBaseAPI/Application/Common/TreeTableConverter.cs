using Core.Entities;
using System.Data;

namespace Application.Common
{
    public class TreeTableConverter<S, D> where S : class where D : class
    {
        public DataTable ToDataTable(List<S> source, string sourceChidrenFieldName, List<string> columnNames)
        {
            var dt = new DataTable();

            if (columnNames == null) throw new ArgumentNullException("columnNames");

            if (source == null) throw new ArgumentNullException("source");

            if (sourceChidrenFieldName == null) throw new ArgumentNullException("sourceChidrenFieldName");

            if (source.GetType().GetProperty(sourceChidrenFieldName) == null)
                throw new Exception($"Source has no property named \"{sourceChidrenFieldName}\"");

            if (source.GetType() != typeof(ICollection<S>))
                throw new Exception($"Source children type is not an ICollection");

            foreach (var columnName in columnNames)
            {
                dt.Columns.Add(columnName);
            }

            // BFS
            Queue<S> queue = new Queue<S>();
            List<S> visited = new List<S>();

            foreach (var item in source)
            {
                queue.Enqueue(item);
                visited.Add(item);

                while (queue.Count > 0)
                {
                    var top = queue.Dequeue();

                    int row = 0, col = 0, colSpan = 1;


                    var children = top.GetType().GetProperty(sourceChidrenFieldName)?.GetValue(top) as IList<S>;

                    if (children != null)
                    {
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

            return dt;
        }
    }
}
