using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphqlController.Services
{
    public class AssemblyResolver : IAssemblyResolver
    {
        Assembly[] _assemblies;

        public AssemblyResolver(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies.ToArray();
        }

        public IEnumerable<Assembly> GetAssemblies() => _assemblies;
        
    }
}
