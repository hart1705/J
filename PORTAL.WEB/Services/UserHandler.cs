using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;

namespace PORTAL.WEB.Services
{
    public class UserHandler : IUserHandler
    {
        public string UserName { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsSystemAdmin { get; set; }
        public List<ApplicationRole> Roles { get; set; }
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null &&
                _httpContextAccessor.HttpContext.User != null &&
                _httpContextAccessor.HttpContext.User.Identity != null &&
                _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                ResolveUserIdentity(_httpContextAccessor.HttpContext.User.Identity.Name).Wait();
            }
        }
        
        private async Task ResolveUserIdentity(string userName)
        {
            UserName = userName;
            User = await _userManager.FindByNameAsync(UserName);
            Roles = new List<ApplicationRole>();
            if (User != null)
            {
                var roles = await _userManager.GetRolesAsync(User);
                foreach (var roleName in roles)
                {
                    var role = _context.ApplicationRole.Include(r => r.ApplicationRole_ApplicationPermissions).
                        Where(r => r.Name == roleName && r.Status == Enums.Status.Active).FirstOrDefault();
                    if (role != null)
                    {
                        foreach (var rolePermission in role.ApplicationRole_ApplicationPermissions)
                        {
                            var permission = _context.ApplicationPermission.Include(p => p.ApplicationAction_ApplicationPermissions).
                                Where(p => p.Id == rolePermission.ApplicationPermissionId && p.Status == Enums.Status.Active).FirstOrDefault();
                            if (permission == null)
                                continue;
                            if (permission.ApplicationAction_ApplicationPermissions == null)
                                continue;

                            foreach (var actionPermission in permission.ApplicationAction_ApplicationPermissions)
                            {
                                var action = _context.ApplicationAction.Find(actionPermission.ApplicationActionId);
                                actionPermission.ApplicationAction = action;
                            }
                            rolePermission.ApplicationPermission = permission;
                        }
                        Roles.Add(role);
                        if (!IsSystemAdmin)
                        {
                            IsSystemAdmin = role.IsSysAdmin;
                        }
                    }
                }
            }
        }

        public bool HasRequiredPermission(string requiredPermission, bool? isOwner = null)
        {
            bool bFound = false;
            if (IsSystemAdmin)
            {
                return true;
            }
            foreach (var role in Roles)
            {
                if (role.ApplicationRole_ApplicationPermissions == null)
                    continue;
                bFound = role.ApplicationRole_ApplicationPermissions.Where(p => p.ApplicationPermission != null && 
                p.ApplicationPermission.ApplicationAction_ApplicationPermissions != null &&
                p.ApplicationPermission.ApplicationAction_ApplicationPermissions.
                Where(a => $"{a.ApplicationAction.ControllerName}-{a.ApplicationAction.ActionName}".
                ToLower() == requiredPermission.ToLower() &&
                (!isOwner.HasValue || (a.AccessType == DAL.EF.Helper.Enums.AccessType.Organization ||
                (isOwner.HasValue && isOwner.Value && a.AccessType == DAL.EF.Helper.Enums.AccessType.User)))).
                ToList().Count > 0).Any();
                if (bFound)
                    break;
            }
            return bFound;
        }

        public bool HasRole(string role)
        {
            return Roles.Where(p => p.Name == role).ToList().Any();
        }

        public bool HasRoles(string roles)
        {
            bool bFound = false;
            string[] _roles = roles.ToLower().Split(';');
            foreach (var role in Roles)
            {
                try
                {
                    bFound = _roles.Contains(role.Name.ToLower());
                    if (bFound)
                        return bFound;
                }
                catch (Exception)
                {
                }
            }
            return bFound;
        }

        public async Task<ApplicationUser> GetUserIdentity(string userId)
        {
            ApplicationUser user = null;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                user = await _userManager.FindByIdAsync(userId);
            }
            return user;
        }

        public async Task<string> GetUserFullName(string userId)
        {
            var name = string.Empty;
            var user =  await GetUserIdentity(userId);
            if(user != null)
            {
                name = $"{user.FirstName} {user.LastName}";
            }
            return name;
        }

        public void SetUser(string userId)
        {
            ResolveUserIdentity(userId).Wait();
        }
    }
}
