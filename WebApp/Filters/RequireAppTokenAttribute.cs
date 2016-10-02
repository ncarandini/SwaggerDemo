using System.Linq;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SwaggerDemo.WebApp.Filters
{
    public class RequireAppTokenAttribute : AuthorizationFilterAttribute
    {

#if DEBUG
        bool isDebug = true;
#else
        bool isDebug = false;
#endif

        private bool mobileAppTokenRequired = true;
        private string mobileAppToken;

        public RequireAppTokenAttribute()
        {
            mobileAppTokenRequired = !isDebug &&
                            (string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings["MobileAppTokenNotRequired"])
                            || (WebConfigurationManager.AppSettings["MobileAppTokenNotRequired"].ToLower() == "false")
                            || (WebConfigurationManager.AppSettings["MobileAppTokenNotRequired"] == "0"));

            mobileAppToken = WebConfigurationManager.AppSettings["MobileAppToken"];
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Because we are in the register.it hosting we cannot use the general check
            // if (httpsRequired && actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)

            string requestMobileAppToken = actionContext.Request.Headers.Contains("MobileAppToken") ?
                    actionContext.Request.Headers.GetValues("MobileAppToken").FirstOrDefault() : null;

            if ((requestMobileAppToken != null && requestMobileAppToken == mobileAppToken) || !mobileAppTokenRequired)
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