using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PORTAL.DAL.EF.Models
{
    public class ApplicationRole_ApplicationPermission
    {
        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public ApplicationRole ApplicationRole { get; set; }
        public string ApplicationPermissionId { get; set; }
        [ForeignKey("ApplicationPermissionId")]
        public ApplicationPermission ApplicationPermission { get; set; }
    }
}
