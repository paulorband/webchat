using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebChat
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}",
				defaults: new { }
			);

			config.Routes.MapHttpRoute(
				name: "DefaultApi2",
				routeTemplate: "api/{controller}/{action}",
				defaults: new { }
			);
		}
	}
}
