using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.HomeViewModels
{
    public class DashboardModel
    {
        public string Name { get; set; }
        public decimal TotalEarning { get; set; }
        public decimal WalletBalance { get; set; }
        public decimal DirectReferalBonus { get; set; }
        public decimal UnilevelBonus { get; set; }
        public decimal MatchingBonus { get; set; }
        public decimal TransactionRebates { get; set; }
        public int CouponPoints { get; set; }
        public decimal UnpairedBalance { get; set; }
        public int LeftPoints { get; set; }
        public int RightPoints { get; set; }
        public int LeftLeg { get; set; }
        public int RightLeg { get; set; }
    }
}
