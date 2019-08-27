using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Filters
{
    public class AuthorizationActionFilter : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly bool _assignable;
        private readonly string _actiondescription;
        private readonly IUserHandler _userHandler;

        public AuthorizationActionFilter(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserHandler userHandler, bool assignable, string actiondescription)
        {
            _context = context;
            _userManager = userManager;
            _userHandler = userHandler;
            _assignable = assignable;
            _actiondescription = actiondescription;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_assignable)
            {
                string requiredPermission = String.Format("{0}-{1}", context.RouteData.Values["controller"], context.RouteData.Values["action"]);
                HttpRequest request = context.HttpContext.Request;
                //Create an instance of our custom user authorization object passing requesting user's 'Windows Username' into constructor
                var userName = context.HttpContext.User.Identity.Name;

                if (userName == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                }
                else
                {
                    if (!_userHandler.HasRequiredPermission(requiredPermission))
                    {
                        //User doesn't have the required permission and is not a SysAdmin, return our custom “401 Unauthorized” access error
                        //Since we are setting filterContext.Result to contain an ActionResult page, the controller's action will not be run.
                        //The custom “401 Unauthorized” access error will be returned to the browser in response to the initial request.
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "AuthorizationDenied" } });
                    }
                }
            }
        }
    }
}
