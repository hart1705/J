using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PORTAL.DAL.EF.Models;
using PORTAL.DAL.EF;

namespace PORTAL.DAL.EF.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {

        }

        public static async Task InitializeDb(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            //context.Database.Migrate();
            //context.Database.EnsureCreated();
            // Add roles
            if (!await context.Roles.AnyAsync(r => string.Equals(r.Name, "System Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "System Administrator", IsSysAdmin = true });
            }
            // Add Admin user
            var adminUser = userManager.Users.FirstOrDefault(u => string.Equals(u.UserName, "admin@portal.com", StringComparison.OrdinalIgnoreCase));
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@portal.com",
                    Email = "admin@portal.com",
                    FirstName = "Super",
                    LastName = "Admin"
                };
                var result = await userManager.CreateAsync(adminUser, "P@ssword1");
                if (result != IdentityResult.Success)
                {
                    throw new Exception($"Unable to create '{adminUser.UserName}' account: {result}");
                }
            }
            await userManager.SetLockoutEnabledAsync(adminUser, false);
            
            // Check AdminUserRoles
            var adminRoles = await userManager.GetRolesAsync(adminUser);
            if (!adminRoles.Any(r => string.Equals(r, "System Administrator")))
            {
                await userManager.AddToRoleAsync(adminUser, "System Administrator");
            }


            #region [AJMP Seed]

            var AJMPUserRolePermission = new string[] { "Administrator", "User", "User Bayanihan" };

            foreach (var rolePermission in AJMPUserRolePermission)
            {
                if (!await context.Roles.AnyAsync(r => string.Equals(r.Name, rolePermission, StringComparison.OrdinalIgnoreCase)))
                {
                    var userRole = new ApplicationRole { Name = rolePermission, IsSysAdmin = false, CreatedBy = adminUser.Id };
                    await roleManager.CreateAsync(userRole);
                    if (!await context.ApplicationPermission.AnyAsync(r => string.Equals(r.ApplicationPermission_Id, rolePermission, StringComparison.OrdinalIgnoreCase)))
                    {
                        var role = context.ApplicationRole.Where(x => x.Name == userRole.Name).FirstOrDefault();
                        if (role != null)
                        {
                            var permission = new ApplicationPermission { ApplicationPermission_Id = rolePermission, CreatedBy = adminUser.Id };
                            context.ApplicationPermission.Add(permission);
                            await context.SaveChangesAsync();
                            if (!context.ApplicationRole_ApplicationPermission.Where(x => x.RoleId == userRole.Id && x.ApplicationPermissionId == permission.Id).Any())
                            {
                                context.ApplicationRole_ApplicationPermission.Add(new ApplicationRole_ApplicationPermission { ApplicationPermissionId = permission.Id, RoleId = userRole.Id });
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            // Add Income to Admin
            var adminIncome = context.Income.Where(a => a.UserId == adminUser.Id).SingleOrDefault();
            if (adminIncome == null)
            {
                Income income = new Income
                {
                    UserId = adminUser.Id
                };
                context.Income.Add(income);
                context.SaveChanges();
            }

            #endregion
        }
    }
}
