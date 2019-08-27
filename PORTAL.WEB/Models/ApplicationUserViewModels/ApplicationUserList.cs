using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.ApplicationUserViewModels
{
    public class ApplicationUserList
    {
        public IEnumerable<ApplicationUserModel> Users { get; set; }
        public IUserHandler UserHandler { get; set; }
    }
}
