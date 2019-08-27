using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PORTAL.DAL.EF.Models
{
    public class GSMModem : IAuditable
    {
        public string Id { get; set; }
        public string GSMModem_Id { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int? ReadTimeout { get; set; }
        public int? WriteTimeout { get; set; }
        public Enums.GSMStatus GSMStatus { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
    }
}
