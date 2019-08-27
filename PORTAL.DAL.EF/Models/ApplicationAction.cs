using System;
using System.Collections.Generic;
using System.Text;

namespace PORTAL.DAL.EF.Models
{
    public class ApplicationAction
    {
        public string Id { get; set; }
        public string ApplicationAction_Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public bool IsHttpPOST { get; set; }
        public bool? AccessNeeded { get; set; }
        public ICollection<ApplicationAction_ApplicationPermission> ApplicationAction_ApplicationPermissions { get; set; }
    }
}
