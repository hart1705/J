using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.TagHelpers
{
    [HtmlTargetElement("rbcontainer", Attributes = "controller-name, action-name, is-owner")]
    public class RBCTagHelper : TagHelper
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsOwner { get; set; }
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserHandler _userHandler;

        public RBCTagHelper(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IUserHandler userHandler)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userHandler = userHandler;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var userName = ViewContext.HttpContext.User.Identity.Name;
            string requiredPermission = String.Format("{0}-{1}", ControllerName, ActionName);
            output.TagName = "text";
            if (!_userHandler.HasRequiredPermission(requiredPermission, IsOwner))
            {
                output.Content.SetHtmlContent("");
            }
        }
    }
}
