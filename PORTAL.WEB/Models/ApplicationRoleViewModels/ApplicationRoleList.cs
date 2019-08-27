using PORTAL.WEB.Models.ApplicationUserViewModels;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.ApplicationRoleViewModels
{
    public class ApplicationRoleList
    {
        public IEnumerable<ApplicationRoleModel> Roles { get; set; }
        public IUserHandler UserHandler { get; set; }
    }
}
