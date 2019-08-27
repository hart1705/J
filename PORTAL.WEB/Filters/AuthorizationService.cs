using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Filters
{
    public class AuthorizationService : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        public bool Assignable { get; set; }
        public string ActionDescription { get; set; }
        public AuthorizationService(bool assignable, string actiondescription)
        {
            Assignable = assignable;
            ActionDescription = actiondescription;
        }
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AuthorizationActionFilter(
             serviceProvider.GetRequiredService<ApplicationDbContext>(),
             serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
             serviceProvider.GetRequiredService<IUserHandler>(),
             Assignable,
             ActionDescription);
        }

    }
}
