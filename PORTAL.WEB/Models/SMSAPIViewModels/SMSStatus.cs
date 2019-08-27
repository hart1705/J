using PORTAL.DAL.EF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.SMSAPIViewModels
{
    public class SMSStatusUpdate
    {
        public int status { get; set; }
        public string errormsg { get; set; }
    }

    public class SMSStatusResponse
    {
        public string sms_id { get; set; }
        public Enums.SMSStatus status { get; set; }
        public DateTime? completedon { get; set; }
    }
}
