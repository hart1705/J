using PORTAL.DAL.EF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.SMSViewModels
{
    public class SMSFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool AllRecord { get; set; } = false;
        public string ApplicationUserId { get; set; }
        public int SMSStatus { get; set; } = -1;
        public string Action { get; set; }
    }

    public class SMSBulkTransact
    {
        public bool Selected { get; set; }
        public string Id { get; set; }
    }
}
