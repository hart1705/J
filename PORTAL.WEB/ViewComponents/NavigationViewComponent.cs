using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PORTAL.WEB.Services;
using PORTAL.WEB.UserControls.NavigationControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IUserHandler _userHandler;
        private readonly IHostingEnvironment _env;

        public NavigationViewComponent(IHostingEnvironment env, IUserHandler userHandler)
        {
            _env = env;
            _userHandler = userHandler;
        }

        public  IViewComponentResult Invoke(string entityName, string recordId)
        {
            Navigation navigation = new Navigation { Item = new List<Item>(), NavItem = new List<NavItem>() };
            if (User.Identity.IsAuthenticated)
            {
                string navPath = Path.Combine(_env.WebRootPath, "sitemap.json");
                string rawNavItems = File.ReadAllText(navPath);
                navigation = JsonConvert.DeserializeObject<Navigation>(rawNavItems);
                
                foreach (var item in navigation.Item)
                {
                    foreach (var menuItem in item.MenuItem)
                    {
                        var askingPermission = $"{menuItem.Controller}-{menuItem.Action}";
                        if(!_userHandler.HasRequiredPermission(askingPermission))
                        {
                            menuItem.Visible = false;
                        }
                    }
                }

                foreach (var item in navigation.NavItem)
                {
                    var askingPermission = $"{item.MenuItem.Controller}-{item.MenuItem.Action}";
                    if (!_userHandler.HasRequiredPermission(askingPermission))
                    {
                        item.MenuItem.Visible = false;
                    }
                }
            }
            return View(navigation);
        }
    }
}
