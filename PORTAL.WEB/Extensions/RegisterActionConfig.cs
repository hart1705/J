using Microsoft.AspNetCore.Mvc;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PORTAL.WEB.Extensions
{
    public class RegisterActionConfig
    {
        public static void RegisterActions(ApplicationDbContext context)
        {
            var q = from type in Assembly.GetExecutingAssembly().GetTypes()
                    where type.IsClass && type.Namespace != null && type.Namespace.Contains("Controller")
                    select type;
            List<string> activeActions = new List<string>();
            foreach (Type controller in q)
            {
                var actions = controller.GetMethods().ToList();
                foreach (MethodInfo mi in actions)
                {
                    AuthorizationService attribute = mi.GetCustomAttribute(typeof(AuthorizationService), false) as AuthorizationService;
                    if (attribute != null)
                    {
                        var actionName = mi.Name;
                        ActionNameAttribute customAction = mi.GetCustomAttribute(typeof(ActionNameAttribute), false) as ActionNameAttribute;
                        if (customAction != null)
                        {
                            actionName = customAction.Name;
                        }
                        string controllerName = controller.Name.Substring(0, controller.Name.Length - 10);
                        bool isHttpPost = mi.GetCustomAttributes(typeof(HttpPostAttribute)).Count() > 0;
                        bool isBackGroundProcess = mi.ReturnType != typeof(ActionResult) && mi.ReturnType != typeof(IActionResult);
                        var action = context.ApplicationAction.Where(p => p.ActionName == actionName && p.ControllerName == controllerName).FirstOrDefault();
                        // && p.IsHttpPOST == isHttpPost)
                        var affected = 0;
                        if (action == null)
                        {
                            action = new ApplicationAction
                            {
                                ApplicationAction_Id = $"{controllerName} - {actionName}",
                                ActionName = actionName,
                                ControllerName = controllerName,
                                AccessNeeded = attribute.Assignable,
                                IsHttpPOST = isHttpPost,
                                Description = attribute.ActionDescription
                            };
                            context.ApplicationAction.Add(action);
                        }
                        else
                        {
                            action.ApplicationAction_Id = $"{controllerName} - {actionName}";
                            action.ActionName = actionName;
                            action.ControllerName = controllerName;
                            action.AccessNeeded = attribute.Assignable;
                            action.IsHttpPOST = isHttpPost;
                            action.Description = attribute.ActionDescription;
                            context.ApplicationAction.Update(action);
                        }
                        affected = context.SaveChanges();
                        if (affected > 0)
                        {
                            activeActions.Add(action.Id);
                        }
                    }
                }
            }

            if (activeActions.Any())
            {
                var actionsToRemove = context.ApplicationAction.Where(a => !activeActions.Contains(a.Id)).ToList();
                if (actionsToRemove.Count > 0)
                {
                    context.RemoveRange(actionsToRemove);
                    context.SaveChanges();
                }

                AJMPActionPermission aJMPActionPermission = new AJMPActionPermission(context);
                aJMPActionPermission.Setup();
            }
            else
            {
                if (context.ApplicationAction.Any())
                {
                    context.ApplicationAction.RemoveRange(context.ApplicationAction.ToList());
                    context.SaveChanges();
                }
            }
        }
    }
}
