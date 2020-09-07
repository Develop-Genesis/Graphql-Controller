using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphqlController.Services
{
    public class GraphqlControllerServiceBuilder
    {
        public IServiceCollection Services { get; }

        public AssemblyResolver AssemblyResolver;
        CustomScalarTypesResolver _customScalarTypesResolver;

        internal GraphqlControllerServiceBuilder(IServiceCollection services)
        {
            Services = services;
            AssemblyResolver = new AssemblyResolver();
            _customScalarTypesResolver = new CustomScalarTypesResolver();
            services.AddSingleton<IAssemblyResolver>(AssemblyResolver);
            services.AddSingleton<ICustomScalarTypesResolver>(_customScalarTypesResolver);
        }

        /// <summary>
        /// Add the assembly where the curreent code is running to find graph types
        /// </summary>
        public void AddCurrentAssembly()
        {
            AddAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Add assembly to find for graph types
        /// </summary>
        /// <param name="assembly">The assembly</param>
        public void AddAssembly(Assembly assembly)
        {
            // Add the assembly to the resolver
            AssemblyResolver.AddAssembly(assembly);

            // Add all type nodes to the container as transient
            var nodeTypes = assembly.GetTypes().Where(x => typeof(IGraphNodeType).IsAssignableFrom(x));

            foreach (var type in nodeTypes)
            {
                Services.AddTransient(type);
            }
        }

        public void AddCustomScalarType(Type type, IGraphType graphType)
        {
            _customScalarTypesResolver.AddScalarType(type, graphType);
        }
    }
}
