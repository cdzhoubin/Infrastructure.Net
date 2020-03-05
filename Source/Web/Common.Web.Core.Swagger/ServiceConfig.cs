using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Web.Swagger
{
    public static class ServiceConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="version">版本</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="urlFriendlyName">A URI-friendly name that uniquely identifies the document</param>
        public static void AddSwagger(this IServiceCollection services, string version, string title, List<string> xmlFiles, string description = "", string urlFriendlyName = "v1")
        {
            var info = new OpenApiInfo
            {
                Version = version,
                Title = title,
                Description = description,
                //TermsOfService = new Uri(""),
                Contact = new OpenApiContact { Name = "", Email = "" }
            };
            
            AddSwagger(services, urlFriendlyName, info, xmlFiles);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="version">版本</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="urlFriendlyName">A URI-friendly name that uniquely identifies the document</param>
        public static void AddSwagger(this IServiceCollection services, string version, string title, string description = "", string urlFriendlyName = "v1")
        {
            List<string> xmlFiles = new List<string>();
            foreach (var p in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
            {
                xmlFiles.Add(p.Substring(AppContext.BaseDirectory.Length));
            }
            AddSwagger(services, version,title,xmlFiles,description, urlFriendlyName);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name">A URI-friendly name that uniquely identifies the document</param>
        /// <param name="info">Global meta data to be included in the Swagger output</param>
        public static void AddSwagger(this IServiceCollection services, string name, OpenApiInfo info, List<string> xmlFiles = null)
        {
            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name, info);
                if (xmlFiles == null || !xmlFiles.Any())
                {
                    return;
                }
                var basePath = AppContext.BaseDirectory;
                xmlFiles.ForEach(p =>
                {
                    var xmlPath = Path.Combine(basePath, p);
                    if (XmlFilePathCache.Contains(xmlPath))
                    {
                        return;
                    }
                    c.IncludeXmlComments(xmlPath);
                    XmlFilePathCache.Add(xmlPath);
                });
            });

            #endregion
        }
        private static List<string> XmlFilePathCache = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <param name="urlFriendlyName"></param>
        public static void AddSwaggerUI(this IApplicationBuilder app, string name, string urlFriendlyName, string urlFormat = "/swagger/{0}/swagger.json")
        {
            AddSwaggerUI(app, c => { c.SwaggerEndpoint(string.Format(urlFormat, urlFriendlyName), name); });
        }
        private static bool isAddUseSwagger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setupAction"></param>
        public static void AddSwaggerUI(this IApplicationBuilder app, Action<SwaggerUIOptions> setupAction = null)
        {
            if (!isAddUseSwagger)
            {
                lock (typeof(ServiceConfig))
                {
                    if (!isAddUseSwagger)
                    {

                        app.UseSwagger();
                        isAddUseSwagger = true;
                    }
                }
            }
            app.UseSwaggerUI(setupAction);
        }
    }
}
