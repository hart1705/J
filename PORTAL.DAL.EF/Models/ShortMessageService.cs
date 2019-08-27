using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PORTAL.DAL.EF.Models
{
    public class ShortMessageService : IAuditable
    {
        public string Id { get; set; }
        public string ShortMessageService_Id { get; set; }
        [StringLength(12)]
        public string MobileNumber { get; set; }
        [StringLength(450)]
        public string MessageBody { get; set; }
        public string Modem_Id { get; set; }
        public Enums.SMSStatus SMSStatus { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
        [NotMapped]
        public bool IsAllocated { get; set; }
        [NotMapped]
        public bool IsFinished { get; set; }
    }
}
