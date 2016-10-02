using System;
using System.Linq;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SwaggerDemo.WebApp.Filters
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {

#if DEBUG
        bool isDebug = true;
#else
   bool isDebug = false;
#endif

        private bool httpsRequired = true;

        public RequireHttpsAttribute()
        {
            httpsRequired = !isDebug &&
                            (string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings["HttpsNotRequired"])
                            || (WebConfigurationManager.AppSettings["HttpsNotRequired"].ToLower() == "false")
                            || (WebConfigurationManager.AppSettings["HttpsNotRequired"] == "0"));
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (httpsRequired && actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "HTTPS Required for Pod Web API"
                };
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}