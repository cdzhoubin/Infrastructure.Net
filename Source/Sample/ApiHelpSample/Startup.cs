using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Zhoubin.Infrastructure.Common.Web.Swagger;

namespace ApiHelpSample
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
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v0.1.0",
            //        Title = "Blog.Core API",
            //        Description = "框架说明文档",
            //        //TermsOfService = "None",
            //        Contact = new OpenApiContact { Name = "Blog.Core", Email = "Blog.Core@xxx.com" }
            //    });
            //});

            services.AddControllers();
            services.AddSwagger("1.0.0.0", "WebApi示例程序",new List<string> { "ApiHelpSample.xml" },"","v1");
            services.AddSwagger("2.0.0.0", "WebApi示例程序", new List<string> { },"", "v2");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //#region Swagger
                //app.UseSwagger();
                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                //});
                //#endregion
            }
            app.AddSwaggerUI("WebApi示例程序-UI-1", "v1");
            app.AddSwaggerUI("WebApi示例程序-UI-2", "v2");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
