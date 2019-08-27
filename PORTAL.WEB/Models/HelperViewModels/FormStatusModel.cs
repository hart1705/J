using PORTAL.DAL.EF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.HelperViewModels
{
    public class FormStatusModel
    {
        public string CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Enums.Status Status { get; set; }
    }
}
