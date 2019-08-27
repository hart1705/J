using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.UnilevelViewModels
{
    public class UnilevelModel
    {
        public string Levels { get; set; }
        public int Referrals { get; set; }
        public decimal UnilevelPoints { get; set; }
        public decimal OverridingCom { get; set; }
        public decimal GrossEarning { get; set; }
        public decimal TotalGrossEarning { get; set; }
    }
}
