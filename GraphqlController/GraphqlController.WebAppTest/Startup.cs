using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GraphqlController.Services;
using GraphiQl;
using GraphqlController.WebAppTest.Repositories;
using GraphqlController.AspNetCore;
using GraphqlController.WebAppTest.Types;
using GraphqlController.AspNetCore.Cache;
using GraphqlController.AspNetCore.PersistedQuery;
using GraphqlController.AspNetCore.Subscriptions.WebSockets;

namespace GraphqlController.WebAppTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDistributedMemoryCache();

            services.AddGraphQlController()
                    .AddCurrentAssembly();

            services.AddGraphQlEndpoint()                    
                    .AddGraphqlCache(new CacheConfiguration()
                    {
                        DefaultMaxAge = 5,
                        ResponseCache = ResponseCacheType.Distributed,
                        UseHttpCaching = true,
                        IncludeETag = true
                    });

            services.AddPersistedQuery()
                    .AddDistributedPersistedQuery();

            services.AddScoped<TeacherRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            }

            app.UseWebSockets();

            app.UseGraphiQl("/graphi", "/graphql/root");

            app.UseHttpsRedirection();

            app.UseRouting();
                        
            app.UseAuthorization();

            app.UseGraphQLController()
               .UseGraphQlExecutionFor<Root>()
               .UsePersistedQuery()
               .UseCache();

            app.UseGraphqlWebSocketProtocol<Root>("/graphql");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQLEnpoint<Root>("/graphql/root");
                endpoints.MapGraphQLEnpoint<Root2>("/graphql/root2");
                endpoints.MapControllers();
            });
        }
    }
}
