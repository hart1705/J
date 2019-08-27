using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.ReferralCodeViewModels
{
    public class ReferralCodeModel : IFormModel
    {
        public string Id { get; set; }
        [Display(Name = "Referral Code ID")]
        public string ReferralCode_Id { get; set; }
        [Required]
        [Display(Name = "PIN Code")]
        public string PINCode { get; set; }
        [Required]
        [Display(Name = "Security Code")]
        public string SecutiryCode { get; set; }
        [Required]
        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }
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
        [Display(Name = "Status Reason")]
        public Enums.ReferralCodeStatus ReferralCodeStatus { get; set; }
        [Display(Name = "Redeemed By")]
        public string UserId { get; set; }
        [Display(Name = "Redeemed By")]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public IUserHandler UserHandler { get; set; }
        public Global.FormType FormType { get; set; }
        public FormBehavior FormBehavior { get; set; }
        public bool IsRecordOwner { get; set; }
    }
}
