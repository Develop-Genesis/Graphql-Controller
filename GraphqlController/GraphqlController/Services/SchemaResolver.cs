using GraphQL.Types;
using GraphqlController.GraphQl;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Services
{
    public class SchemaResolver : ISchemaResolver
    {
        Dictionary<Type, DynamicSchema> _rootTypeSchema = new Dictionary<Type, DynamicSchema>();
        IAssemblyResolver _assemblyResolver;
        IGraphQlTypePool _graphQlTypePool;
        
        List<Action<Schema>> _intializers = new List<Action<Schema>>();

        public SchemaResolver(IAssemblyResolver assemblyResolver, IGraphQlTypePool graphQlTypePool)
        {
            _assemblyResolver = assemblyResolver;
            _graphQlTypePool = graphQlTypePool;
        }

        public void BuildSchemas()
        {
            var rootTypes = _assemblyResolver.GetRootTypes();

            foreach(var rootType in rootTypes)
            {
                var mutationTypes = _assemblyResolver.GetMutationTypes(rootType);
                var subscriptionTypes = _assemblyResolver.GetSubscriptionTypes(rootType);

                var schema = new DynamicSchema(_graphQlTypePool, rootType, mutationTypes, subscriptionTypes, _intializers);                
                _rootTypeSchema.Add(rootType, schema);
            }
        }

        public void AddIntializer(Action<Schema> initializer)
        {
            _intializers.Add(initializer);
        }

        public ISchema GetSchema(Type rootType)
        {
            return _rootTypeSchema[rootType];
        }

    }
}
