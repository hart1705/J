using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.UserControls.NavigationControl
{
    public class MenuItem
    {
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool Visible { get; set; } = true;
    }

    public class Item
    {
        public string Category { get; set; }
        public IList<MenuItem> MenuItem { get; set; }
    }

    public class NavItem
    {
        public MenuItem MenuItem { get; set; }
    }

    public class Navigation
    {
        public IList<Item> Item { get; set; }
        public IList<NavItem> NavItem { get; set; }
    }
}
