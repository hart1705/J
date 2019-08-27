using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.SMSViewModels
{
    public class SMSImportList
    {
        [Required]
        [StringLength(12)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Required]
        [StringLength(450)]
        [Display(Name = "Message Body (450 max)")]
        public string MessageBody { get; set; }
        public bool Include { get; set; } = true;
    }
}
