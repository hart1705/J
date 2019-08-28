using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Extensions
{
    public class AJMPActionPermission
    {
        private readonly ApplicationDbContext _context;

        public AJMPActionPermission(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Setup()
        {
            var permission = _context.ApplicationPermission.Where(x => x.ApplicationPermission_Id == "Administrator").FirstOrDefault();
            if (permission != null)
            {
                var actions = new string[] { "Account - Register", "Unilevel - Index", "BinaryTree - Index", "Income - Index",
                    "ReferralCode - ChangeStatus", "ReferralCode - Delete", "ReferralCode - Form", "ReferralCode - GenerateCode",
                    "ReferralCode - Index", "ReferralCode - New", "ReferralCode - Update",
                    "ApplicationUser - Form", "ApplicationUser - Index", "ApplicationUser - Update" };
                AssociateActionPermission(permission.Id, actions, true);
            }

            permission = _context.ApplicationPermission.Where(x => x.ApplicationPermission_Id == "User").FirstOrDefault();
            if (permission != null)
            {
                var actions = new string[] { "Account - Register", "Unilevel - Index", "BinaryTree - Index", "Income - Index" };
                AssociateActionPermission(permission.Id, actions);
            }

            //permission = _context.ApplicationPermission.Where(x => x.ApplicationPermission_Id == "User Bayanihan").FirstOrDefault();
            //if (permission != null)
            //{
            //    var actions = new string[] { "Account - Register", "Unilevel - Index", "Income - Index" };
            //    AssociateActionPermission(permission.Id, actions);
            //}
        }

        private void AssociateActionPermission(string permissionId, string[] actions, bool isAdmin = false)
        {
            foreach (var action in actions)
            {
                var ac = _context.ApplicationAction.Where(c => c.ApplicationAction_Id == action).FirstOrDefault();
                if (ac != null && !_context.ApplicationAction_ApplicationPermission.Where(a => a.ApplicationActionId == ac.Id && a.ApplicationPermissionId == permissionId).Any())
                {
                    _context.ApplicationAction_ApplicationPermission.Add(new ApplicationAction_ApplicationPermission
                    {
                        AccessType = (isAdmin ? DAL.EF.Helper.Enums.AccessType.Organization : DAL.EF.Helper.Enums.AccessType.User),
                        ApplicationActionId = ac.Id,
                        ApplicationPermissionId = permissionId
                    });
                }
            }
            _context.SaveChanges();
        }
    }
}
