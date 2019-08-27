using Microsoft.AspNetCore.Identity;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PORTAL.DAL.EF.Models
{
    public class ApplicationRole : IdentityRole, IAuditable
    {
        public string Description { get; set; }
        public bool IsSysAdmin { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
        public ICollection<ApplicationRole_ApplicationPermission> ApplicationRole_ApplicationPermissions { get; set; }
    }
}
