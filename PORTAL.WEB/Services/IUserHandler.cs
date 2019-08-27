using Microsoft.AspNetCore.Identity;
using PORTAL.DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Services
{
    public interface IUserHandler
    {
        string UserName { get; set; }
        ApplicationUser User { get; set; }
        bool IsSystemAdmin { get; set; }
        List<ApplicationRole> Roles { get; set; }
        bool HasRequiredPermission(string requiredPermission, bool? isOwner = null);
        bool HasRole(string role);
        bool HasRoles(string roles);
        Task<ApplicationUser> GetUserIdentity(string userId);
        Task<string> GetUserFullName(string userId);
        void SetUser(string userId);
    }
}
