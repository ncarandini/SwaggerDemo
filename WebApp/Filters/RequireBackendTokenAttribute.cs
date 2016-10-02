using System.Linq;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SwaggerDemo.WebApp.Filters
{
    public class RequireBackendTokenAttribute : AuthorizationFilterAttribute
    {

#if DEBUG
        bool isDebug = true;
#else
   bool isDebug = false;
#endif

        private bool backendTokenRequired = true;
        private string backendToken;

        public RequireBackendTokenAttribute()
        {
            backendTokenRequired = !isDebug &&
                                   (string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings["BackendTokenNotRequired"])
                                   || (WebConfigurationManager.AppSettings["BackendTokenNotRequired"].ToLower() == "false")
                                   || (WebConfigurationManager.AppSettings["BackendTokenNotRequired"] == "0"));

            backendToken = WebConfigurationManager.AppSettings["BackendToken"];
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Get Tokens
            string requestBackendToken = actionContext.Request.Headers.Contains("BackendToken") ?
                actionContext.Request.Headers.GetValues("BackendToken").FirstOrDefault() : null;

            if ((requestBackendToken != null && requestBackendToken == backendToken) || !backendTokenRequired)
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}