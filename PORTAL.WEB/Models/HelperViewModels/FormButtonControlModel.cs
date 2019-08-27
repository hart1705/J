using PORTAL.DAL.EF.Helper;
using PORTAL.WEB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.HelperViewModels
{
    public class FormButtonControlModel
    {
        public Global.FormType FormType { get; set; }
        public bool IsRecordOwner { get; set; }
        public string ControllerName { get; set; }
        public Enums.Status? Status { get; set; }
    }
}
