using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using NavomiApi.Models;
using NavomiApi.Interfaces;
using NavomiApi.Filters;
using NavomiApi.Services;
using Microsoft.AspNetCore.Http;

namespace NavomiApi
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
            
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var sconfig = new SalesForceConfiguration();
            Configuration.Bind("SalesForce", sconfig);      
            services.AddSingleton(sconfig);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Configuration["Swagger:Version"], new Info
                {
                    Title = Configuration["Swagger:Title"],
                    Version = Configuration["Swagger:Version"]
                });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example - Authorization: Bearer {token}",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.DocumentFilter<SecurityRequirementsDocumentFilter>();
            });


            services.AddMvc(
               config =>
               {
                   config.Filters.Add(typeof(AddVersionHeaderFilter));
                   config.Filters.Add(typeof(CustomExceptionFilter));
               }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
         
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ISalesForceService, SalesForceService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration["Swagger:Endpoint"], Configuration["Swagger:Title"]);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
    public class SecurityRequirementsDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument document, DocumentFilterContext context)
        {
            document.Security = new List<IDictionary<string, IEnumerable<string>>>()
            {
                new Dictionary<string, IEnumerable<string>>()
                {
                    { "Bearer", new string[]{ } }
                }
            };
        }
    }
}
