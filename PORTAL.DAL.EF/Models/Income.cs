using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PORTAL.DAL.EF.Models
{
    public class Income : IAuditable
    {
        public string Id { get; set; }
        public Decimal NetIncome { get; set; }
        public Decimal DirectReferralIncome { get; set; }
        public Decimal SalesMatchBonusIncome { get; set; }
        public Decimal ProductRewardBonusIncome { get; set; }
        public Decimal UnilevelIncome { get; set; }
        public Decimal GeneologyIncome { get; set; }
        public Decimal BayanihanIncome { get; set; }
        public Decimal Tax { get; set; }
        public Decimal TotalCashOut { get; set; }
        public Decimal TotalBalance { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Enums.Status Status { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
