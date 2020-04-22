using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphqlController.Services
{
    public class AssemblyResolver : IAssemblyResolver
    {
        List<Assembly> _assemblies;

        public AssemblyResolver()
        {
            _assemblies = new List<Assembly>();
        }

        internal void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }

        public IEnumerable<Assembly> GetAssemblies() => _assemblies;
        
    }
}
