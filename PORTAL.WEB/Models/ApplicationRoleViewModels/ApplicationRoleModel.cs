using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Services;
using PORTAL.WEB.UserControls.LookupControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.ApplicationRoleViewModels
{
    public class ApplicationRoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Is System Admin")]
        public bool IsSysAdmin { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }
        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }
        [DisplayName("Modified On")]
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
        public ICollection<ApplicationRole_ApplicationPermission> ApplicationRole_ApplicationPermissions { get; set; }
        public LookupControlModel PermissionLookup { get; set; }
        public IUserHandler UserHandler { get; set; }
        public Global.FormType FormType { get; set; }
        public FormBehavior FormBehavior { get; set; }
        public bool IsRecordOwner { get; set; }
    }
}
