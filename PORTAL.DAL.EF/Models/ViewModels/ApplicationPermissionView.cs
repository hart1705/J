using PORTAL.DAL.EF.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace PORTAL.DAL.EF.Models.ViewModels
{
    public class ApplicationPermissionView
    {
        public string Id { get; set; }
        public string ApplicationPermission_Id { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
        public string StatusName { get; set; }
    }
}
