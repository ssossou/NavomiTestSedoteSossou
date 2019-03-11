using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using NavomiApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NavomiApi.Filters
{
    public class AddVersionHeaderFilter : ActionFilterAttribute
    {
        static string _assemblyVersion = "";
        IConfiguration _config = null;

        public AddVersionHeaderFilter (IConfiguration config)
        {
            _config = config;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //A configurabale delay for each API call.  Used for testing loading spinners in the UI.
            var apiDelaySeconds = _config["Testing:ApiDelaySeconds"];
            if (apiDelaySeconds != null)
            {
                int delay = 0;
                if (int.TryParse(apiDelaySeconds, out delay))
                {
                    Thread.Sleep(delay*1000);
                }
            }


            base.OnActionExecuted(context);

            if (string.IsNullOrEmpty(_assemblyVersion))
            {
                var attributes = typeof(RecordController).Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute));
                var assemblyTitleAttribute = attributes.SingleOrDefault() as AssemblyTitleAttribute;
                _assemblyVersion = assemblyTitleAttribute.Title;
            }
            
            context.HttpContext.Response.Headers.Add("X-API-Title", _assemblyVersion);
        }
    }
}
