using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.HomeViewModels
{
    public class DashboardModel
    {
        public int TotalRecords { get; set; }
        public int TotalOnQueue { get; set; }
        public int TotalSent { get; set; }
        public int TotalSendFailed { get; set; }
        public int TotalCredit { get; set; }
        public int TotalCreditsUsed { get; set; }
        public int RemainingCredits { get { return TotalCredit - TotalCreditsUsed; } }
        public StatPerMonth StatPerMonth { get; set; }
    }
}
