using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.PermissionViewModels
{
    public class PermissionList
    {
        public List<Permission> Permissions { get; set; } = new List<Permission>();
        public IUserHandler UserHandler { get; set; }
    }
}
