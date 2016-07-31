using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FileWebsite.Security
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                          System.AttributeTargets.Struct)
   ]
    public class CustomAuthorization : FilterAttribute , IAuthorizationFilter    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session["Username"] == null)
            { 
                filterContext.Result = new RedirectResult("~/Account/Login");
                filterContext.Result.ExecuteResult(filterContext);                
            }


        }
    }
}