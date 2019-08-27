using PORTAL.DAL.EF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.ActionViewModels
{
    public class ApplicationActionModel
    {
        public string Id { get; set; }
        public string ApplicationAction_Id { get; set; }
        public string Description { get; set; }
        public bool IsHttpPOST { get; set; }
        public Enums.AccessType AccessType { get; set; }
        public bool Selected { get; set; } = false;
    }
}
