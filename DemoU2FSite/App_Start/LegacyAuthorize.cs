using System.Web;
using System.Web.Mvc;

namespace DemoU2FSite
{
    public class LegacyAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext actionContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                base.HandleUnauthorizedRequest(actionContext);
        }
    }
}