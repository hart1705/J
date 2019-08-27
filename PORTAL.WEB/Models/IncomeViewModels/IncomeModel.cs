using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.IncomeViewModels
{
    public class IncomeModel
    {
        public string IncomeType { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetIncome { get; set; }
    }
}
