using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Interfaces;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Services;
using PORTAL.WEB.UserControls.LookupControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.ApplicationUserViewModels
{
    public class ApplicationUserModel : IFormModel, IAuditable
    {
        public string Id { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }
        [DisplayName("Email Address")]
        public string Email { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [DisplayName("Mobile Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }
        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }
        [DisplayName("Modified On")]
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
        public LookupControlModel RoleLookup { get; set; }
        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool HasUpdatePermission { get; set; } = false;
        public bool HasDeletePermission { get; set; } = false;
        public bool HasViewPermission { get; set; } = false;

        public IUserHandler UserHandler { get; set; }
        public Global.FormType FormType { get; set; }
        public FormBehavior FormBehavior { get; set; }
        public bool IsRecordOwner { get; set; }

        [Display(Name = "Registration Type")]
        public Enums.RegistrationType? RegistrationType { get; set; }
    }
}
