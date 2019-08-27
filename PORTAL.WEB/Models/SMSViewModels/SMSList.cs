using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.SMSViewModels
{
    public class SMSList
    {
        public IEnumerable<SMSModel> Records { get; set; }
        public IUserHandler UserHandler { get; set; }
    }
}
