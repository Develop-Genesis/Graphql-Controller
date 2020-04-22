using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNet
{
    public class SchemaRouteService : ISchemaRouteService
    {
        Dictionary<string, Type> _routes = new Dictionary<string, Type>();

        public void AddRoute(string path, Type rootType)
        {
            _routes.Add(path, rootType);
        }

        public void AddRoute<T>(string path)
          => AddRoute(path, typeof(T));


        public Type GetType(string path)
          => _routes[path];
    }
}
