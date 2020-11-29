using GraphqlController.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Relay
{    
    public class Connection<T>
    {
        public Connection(IEnumerable<Edge<T>> edges, PageInfo pageInfo)
        {
            Edges = edges;
            PageInfo = pageInfo;
        }

        public static string GenerateName(Type type)
        {
            var genericType = type.GetGenericArguments()[0];
            return $"{genericType.Name}Connection";            
        }

        public IEnumerable<Edge<T>> Edges { get; }
        public PageInfo PageInfo { get; }
    }

    public class PageInfo
    {
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public string StartCursor { get; set; }
        public string EndCursor { get; set; }
    }

    public class Edge<T>
    {
        public Edge(string cursor, T node)
        {
            Cursor = cursor;
            Node = node;
        }

        public static string GenerateName(Type type)
        {
            var genericType = type.GetGenericArguments()[0];
            return $"{genericType.Name}Edge";
        }

        public string Cursor { get; }
        public T Node { get; }
    }
}
