using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ProyectoV1.App_Start;

namespace ProyectoV1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services 
            config.EnableCors(new AccesPolicyCors());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
