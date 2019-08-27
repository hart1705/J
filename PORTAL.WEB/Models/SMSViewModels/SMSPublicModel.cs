using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.SMSViewModels
{
    public class SMSPublicModel : IFormModel
    {
        [Required]
        [StringLength(15)]
        [Display(Name = "Your Name")]
        public string SenderName { get; set; }
        [Required]
        [StringLength(12)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Required]
        [StringLength(400)]
        [Display(Name = "Message Body")]
        public string MessageBody { get; set; }
        public IUserHandler UserHandler { get; set; }
        public Global.FormType FormType { get; set; }
        public FormBehavior FormBehavior { get; set; }
        public bool IsRecordOwner { get; set; }
    }
}
