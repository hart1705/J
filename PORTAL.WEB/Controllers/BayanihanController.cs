using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Models.BayanihanViewModels;

namespace PORTAL.WEB.Controllers
{
    public class BayanihanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BayanihanController(ApplicationDbContext context, UserManager<ApplicationUser> userManger)
        {
            _context = context;
            _userManager = userManger;
        }

        [AuthorizationService(true, "Bayanihan - List")]
        public IActionResult Index()
        {
            BayanihanLevelsModel model = Bayanihan();
        
            return View(model);
            //return Json(new { level1 = 5, level2 = 25 });

        }

        private BayanihanLevelsModel Bayanihan()
        {
            BayanihanLevelsModel model = new BayanihanLevelsModel();
            int[] level = new int[8];
             level = getPerLevels();
            model.Level1 = level[0];
            model.Level2 = level[1];
            model.Level3 = level[2];
            model.Level4 = level[3];
            model.Level5 = level[4];
            model.Level6 = level[5];
            model.Level7 = level[6];
            model.Level8 = level[7];

            return model;
        }

        private int[] getPerLevels()
        {
            var id = _userManager.GetUserId(User);
            int[] countFinal = new int[8];
            int bayanihanUserCount = 0;
            //var UserData = _context.Users.Where(a => a.Id == id).FirstOrDefault();
            //if(UserData != null)
            //{
            //    var UserCreatedOn = UserData.CreatedOn;
            //    count = _context.Users.Where(a => a.CreatedOn > UserCreatedOn).Count();
            //}
            var bayanihanTotalCount = _context.Bayanihan.Count() - 1;
            var bayanihanData = _context.Bayanihan.Where(a => a.UserId == id).FirstOrDefault();
            if(bayanihanData != null)
            {
                bayanihanUserCount = bayanihanData.Count;
            }

            countFinal = FetchLevel(bayanihanUserCount, bayanihanTotalCount);

            return countFinal;
        }

        private int[] FetchLevel(int userCount, int totalCount)
        {
            int count = 0;
            (int, int) counts = (0, 0);
            int[] levels = new int[8];
            bool continueGenerate = true;
            int depth = 0;
            while (continueGenerate)
            {
                counts = Count(depth, userCount, totalCount, out continueGenerate);
                userCount = counts.Item1;
                count = counts.Item2;
                depth += 1;
                if (depth == 1)
                {
                    levels[0] = count;
                }
                else if (depth == 2)
                {
                    levels[1] = count;
                }
                else if (depth == 3)
                {
                    levels[2] = count;
                }
                else if (depth == 4)
                {
                    levels[3] = count;
                }
                else if (depth == 5)
                {
                    levels[4] = count;
                }
                else if (depth == 6)
                {
                    levels[5] = count;
                }
                else if (depth == 7)
                {
                    levels[6] = count;
                }
                else if (depth == 8)
                {
                    levels[7] = count;
                }
            }
            return levels;
        }

        private (int, int) Count(int depth, int UserCount, int totalCount, out bool stop)
        {
            int count = 0;
            stop = false;
            int nextLevelCount = 0;
            int nextLevel = (UserCount * 5) + 1;
            int cDepth = ComputeDepth(depth);
            for(int j = 0; j < cDepth; j++)
            {
                
                nextLevelCount = (UserCount * 5) + (j + 1);
                if (nextLevelCount > totalCount) break;
                count += 1;
            }
            stop = !(nextLevelCount >= totalCount);
            return (nextLevel, count);
        }

        private int ComputeDepth(int depth)
        {
            int count = 5;
            for(int i = 0; i < depth; i++)
            {
                count = count * 5;
            }
            return count;
        }

    }
}