using System;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Unity.AspNet.WebApi;

namespace InoxicoIdentity.App_Start
{
    public static class WebApiConfig
    {
        public static void Configure(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Adds "Z" to the end of serialized DateTime, so that clients are aware that the received time is UTC
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Unity
            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

            app.UseWebApi(config);
        }
    }
}