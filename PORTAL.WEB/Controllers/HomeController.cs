using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Models;
using PORTAL.WEB.Models.HomeViewModels;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PORTAL.WEB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHandler _userHandler;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ApplicationDbContext context, IUserHandler userHandler, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userHandler = userHandler;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            //DashboardModel model = GenerateDashboardData();
            //return View(model);
            var user = string.Empty;
            user = _userManager.GetUserName(User);
            string hart = string.Empty;
            if(user == "hart17")
            {
                hart = Test();
            }

            return View((object)hart);
        }

        private string Test()
        {
            return "Test";
        }
        public IActionResult RefreshDashBoard()
        {
            DashboardModel model = GenerateDashboardData();
            return PartialView("_Dashboard", model);
        }

        private DashboardModel GenerateDashboardData()
        {
            DashboardModel model = new DashboardModel();
            model.TotalCredit = 0;
            model.TotalCreditsUsed = 0;
            
            model.TotalRecords = _context.ShortMessageService.Where(sms => sms.CreatedBy == _userHandler.User.Id).Count();
            model.TotalOnQueue = _context.ShortMessageService.Where(sms => sms.CreatedBy == _userHandler.User.Id && sms.SMSStatus == Enums.SMSStatus.Queue).Count();
            model.TotalSent = _context.ShortMessageService.Where(sms => sms.CreatedBy == _userHandler.User.Id && sms.SMSStatus == Enums.SMSStatus.Sent).Count();
            model.TotalSendFailed = _context.ShortMessageService.Where(sms => sms.CreatedBy == _userHandler.User.Id && sms.SMSStatus == Enums.SMSStatus.SendFailed).Count();

            model.StatPerMonth = new StatPerMonth();
            model.StatPerMonth.Labels = new List<string>();
            model.StatPerMonth.datasets = new List<Dataset>();
            model.StatPerMonth.datasets.Add(new Dataset { backgroundColor = "window.chartColors.blue", label = "Total On Queue" });
            model.StatPerMonth.datasets.Add(new Dataset { backgroundColor = "window.chartColors.green", label = "Total Sent" });
            model.StatPerMonth.datasets.Add(new Dataset { backgroundColor = "window.chartColors.red", label = "Total Send Failed" });

            var currDate = DateTime.Now;

            for (int i = 1; i <= currDate.Month; i++)
            {
                var startDate = new DateTime(currDate.Year, i, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                model.StatPerMonth.Labels.Add(DateTimeFormatInfo.CurrentInfo.GetMonthName(i));
            }
            for (int j = 0; j < 3; j++)
            {
                model.StatPerMonth.datasets[j].data = new List<int>();
                for (int i = 1; i <= currDate.Month; i++)
                {
                    var startDate = new DateTime(currDate.Year, i, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var total = _context.ShortMessageService.Where(sms => sms.CreatedBy == _userHandler.User.Id && sms.CreatedOn.HasValue &&
                    sms.CreatedOn.Value.Date >= startDate.Date && sms.CreatedOn.Value.Date <= endDate.Date && sms.SMSStatus == (Enums.SMSStatus)(j+1)).Count();
                    model.StatPerMonth.datasets[j].data.Add(total);
                }
            }
            return model;
        }

        public IActionResult GetUserBinaryTree()
        {

            return Json("");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
