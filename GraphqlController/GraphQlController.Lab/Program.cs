 using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphqlController.GraphQl;
using GraphqlController.Services;
using GraphQlController.Lab.TestTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GraphQlController.Lab
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var services = new ServiceCollection();            

            // add the services
            services.AddGraphQlController();

            // create provider
            var provider = services.BuildServiceProvider();

            var pool = provider.GetService<IGraphQlTypePool>();

            //var schema = new Schema { 
            //    Query = pool.GetRootGraphType( new Person() ) as IObjectGraphType,
                
            //};

            //var json = await schema.ExecuteAsync(_ =>
            //{
            //    _.Query = "{ name, lastName, motherName(jojoto: null), father { name } }";
            //});

            //Console.WriteLine(json);

        }
    }
    
}
