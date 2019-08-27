using PORTAL.DAL.EF.Helper;
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
    public class SMSModel : IFormModel
    {
        public string Id { get; set; }
        [Display(Name = "SMS ID")]
        public string ShortMessageService_Id { get; set; }
        [Required]
        [StringLength(12)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Required]
        [StringLength(450)]
        [Display(Name = "Message Body (450 max)")]
        public string MessageBody { get; set; }
        [Display(Name = "Modem")]
        public string Modem_Id { get; set; }
        [Display(Name = "SMS Status")]
        public Enums.SMSStatus SMSStatus { get; set; }
        [Display(Name = "Error Message")]
        public string ErrorMessage { get; set; }
        [Display(Name = "Completed On")]
        public DateTime? CompletedOn { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "Modified On")]
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
        public IUserHandler UserHandler { get; set; }
        public Global.FormType FormType { get; set; }
        public FormBehavior FormBehavior { get; set; }
        public bool IsRecordOwner { get; set; }
        public bool Selected { get; set; }
    }
}
