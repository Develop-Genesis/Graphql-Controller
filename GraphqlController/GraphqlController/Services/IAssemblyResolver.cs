using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphqlController.Services
{
    public interface IAssemblyResolver
    {
        public IEnumerable<Assembly> GetAssemblies();
    }
}
