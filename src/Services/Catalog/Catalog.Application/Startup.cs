using System;
using Catalog.Domain.Models;
using Catalog.Infra;
using Catalog.Infra.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application
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
            //linked our CatalogSettings with Configuration object, provide strong type configuration
            services.Configure<CatalogSettings>(Configuration);

            #region EntityFramework Core
            var connection = Configuration["ConnectionString"];
            services.AddDbContext<CatalogContext>(options=>options.UseSqlServer(connection, b => b.MigrationsAssembly("Catalog.Application")));
            #endregion

            #region Database repository
            services.AddScoped<ICatalogRepository, CatalogRepository>();
            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "ProductCatalogAPI",
                        Version = "v1",
                        Description = "Through this API you can access product catalog functions",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "shawn.zhang@avanade.com",
                            Name = "Shawn Zhang",
                            Url = new Uri("https://www.linkedin.com/in/shawnzxx/")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //we add at back of UseHttpsRedirection, so that all link to swagger website using http will redirect to https site
            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            //Enable middleware to serve Swagger UI (HTML, CSS, JS etc.)
            //Specifying the Swagger JSON endpoint
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "ProductCatalogAPI V1");
                setupAction.RoutePrefix = "";
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
