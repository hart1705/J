using PORTAL.DAL.EF.Helper;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.GSMModemViewModels
{
    public class ModemModel : IFormModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Modem Name")]
        public string GSMModem_Id { get; set; }
        [Required]
        [Display(Name = "Port Name")]
        public string PortName { get; set; }
        [Required]
        [Display(Name = "Baudrate")]
        public int BaudRate { get; set; }
        [Required]
        [Display(Name = "Read Timeout")]
        public int? ReadTimeout { get; set; }
        [Required]
        [Display(Name = "Write Timeout")]
        public int? WriteTimeout { get; set; }
        [Display(Name = "Modem Status")]
        public Enums.GSMStatus GSMStatus { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "Modified On")]
        public DateTime? ModifiedOn { get; set; }
        [Display(Name = "Status")]
        public Enums.Status Status { get; set; }
        public IUserHandler UserHandler { get; set; }
        public Global.FormType FormType { get; set; }
        public FormBehavior FormBehavior { get; set; }
        public bool IsRecordOwner { get; set; }
    }
}
