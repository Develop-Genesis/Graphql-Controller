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
        IServiceCollection _services;

        AssemblyResolver _assemblyResolver;
        CustomTypesResolver _customTypesResolver;

        internal GraphqlControllerServiceBuilder(IServiceCollection services)
        {
            _services = services;
            _assemblyResolver = new AssemblyResolver();
            _customTypesResolver = new CustomTypesResolver();
            services.AddSingleton<ICustomTypesResolver>(_customTypesResolver);
            services.AddSingleton<IAssemblyResolver>(_assemblyResolver);
        }

        /// <summary>
        /// Add assembly to find for graph types
        /// </summary>
        /// <param name="assembly">The assembly</param>
        public void AddAssembly(Assembly assembly)
        {
            // Add the assembly to the resolver
            _assemblyResolver.AddAssembly(assembly);

            // Add all type nodes to the container as transient
            var nodeTypes = assembly.GetTypes().Where(x => typeof(IGraphNodeType).IsAssignableFrom(x));

            foreach (var type in nodeTypes)
            {
                _services.AddTransient(type);
            }
        }

        /// <summary>
        /// Add a custom type
        /// </summary>
        /// <param name="graphType"></param>
        public void RegisterType(IGraphType graphType)
            => _customTypesResolver.AddCustomType(graphType);


        /// <summary>
        /// Add the assembly where the curreent code is running to find graph types
        /// </summary>
        public GraphqlControllerServiceBuilder AddCurrentAssembly()
        {
            AddAssembly(Assembly.GetCallingAssembly());
            return this;
        }



    }
}
