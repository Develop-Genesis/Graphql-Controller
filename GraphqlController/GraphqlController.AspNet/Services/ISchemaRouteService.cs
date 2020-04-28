using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore
{
    public interface ISchemaRouteService
    {
        public void AddRoute(string path, Type rootType);
        public void AddRoute<T>(string path);
        public Type GetType(string path);
    }
}
