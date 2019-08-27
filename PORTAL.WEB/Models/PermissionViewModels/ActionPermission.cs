using PORTAL.DAL.EF.Helper;
using PORTAL.WEB.Models.ActionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.PermissionViewModels
{
    public class ActionPermission
    {
        public Enums.AccessType AccessType { get; set; }
        public string PermissinId { get; set; }
        public List<ApplicationActionModel> Actions { get; set; }
    }
}
