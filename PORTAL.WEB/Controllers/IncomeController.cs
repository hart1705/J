using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Models.IncomeViewModels;

namespace PORTAL.WEB.Controllers
{
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IncomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AuthorizationService(true, "Income - List")]
        public async Task<IActionResult> Index()
        {
            
            return View(await GetIncomeList(_userManager.GetUserId(User)));
        }
        
        private async Task<IncomeList> GetIncomeList(string id)
        {
            
            List<IncomeModel> dbRec = new List<IncomeModel>();
            var record = await _context.Income.Where(a => a.UserId == id).SingleOrDefaultAsync();
            //var incomeBayanihan = new IncomeModel
            //{
            //    IncomeType = "Bayanihan",
            //    Description = "Bayanihan Income",
            //    TotalAmount = record.BayanihanIncome,
            //    NetIncome = record.NetIncome
                
            //};

            var incomeBinary = new IncomeModel
            {
                IncomeType = "Direct Referral",
                Description = "Direct Referral Income",
                TotalAmount = record.DirectReferralIncome,
                NetIncome = record.NetIncome
            };

            var incomeUnilevel = new IncomeModel
            {
                IncomeType = "Unilevel",
                Description = "Unilevel Income",
                TotalAmount = record.UnilevelIncome,
                NetIncome = record.NetIncome
            };

            var incomeGeneology = new IncomeModel
            {
                IncomeType = "Geneology",
                Description = "Geneology Income",
                TotalAmount = record.GeneologyIncome,
                NetIncome = record.NetIncome
            };


            //dbRec.Add(incomeBayanihan);
            dbRec.Add(incomeBinary);
            dbRec.Add(incomeUnilevel);
            dbRec.Add(incomeGeneology);

            IncomeList incomeList = new IncomeList
            {
                Records = dbRec
            };
            return incomeList;
        }
    }
}
