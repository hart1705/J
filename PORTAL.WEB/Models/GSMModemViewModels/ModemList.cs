using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.GSMModemViewModels
{
    public class ModemList
    {
        public IEnumerable<ModemModel> Records { get; set; }
        public IUserHandler UserHandler { get; set; }
    }
}
