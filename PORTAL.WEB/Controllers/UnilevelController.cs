using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Models.UnilevelViewModels;

namespace PORTAL.WEB.Controllers
{
    public class UnilevelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UnilevelController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [AuthorizationService(true, "Unilevel - list")]
        public async Task<IActionResult> Index()
        {
            
            return View(await GetUnilevelList(_userManager.GetUserId(User)));
        }

        private async Task<UnilevelList> GetUnilevelList(string id)
        {
            List<UnilevelModel> dbRec = new List<UnilevelModel>();
            decimal overridingCom = 5;
            int lvlCount = 0;
            decimal totalGrossEarning = _context.Income.Where(a => a.UserId == id).Select(a => a.UnilevelIncome).SingleOrDefault();
            int recordL1 = _context.Users.Where(a => a.CreatedBy == id).Count();
            int?[] levels = GetCountPerLevel(id);

            foreach (var level in levels.Where(a=>a.HasValue))
            {
                lvlCount = lvlCount + 1;
                var unilevellevel = new UnilevelModel
                {
                    Levels = "L" + lvlCount.ToString(),
                    Referrals = level.Value,
                    UnilevelPoints = UnilevelPoints(level.Value),
                    OverridingCom = overridingCom,
                    GrossEarning = GrossEarning(level.Value, overridingCom),
                    TotalGrossEarning = totalGrossEarning
                };
                dbRec.Add(unilevellevel);

            }
 
            UnilevelList unilevelList = new UnilevelList
            {
                Records = dbRec
            };
            return unilevelList;
        }

        private int?[] GetCountPerLevel(string id)
        {
            int?[] levelsCount = new int?[8];
            int totalPerLevelCount = 0;
            string[] ids = new string[20];
            int lvlCount = 0;
            levelsCount[0] = _context.Users.Where(a => a.CreatedBy == id).Count();
            string[] perlevelIds = GetIds(id);
            string[] allIds = new string[25];
            for (int i = 1; i <= 7; i++)
            {
                lvlCount = 0;
                totalPerLevelCount = 0;
                foreach (var item in perlevelIds)                    
                {
                    if (item == null) continue;

                    totalPerLevelCount = totalPerLevelCount + GetCount(item);
                    ids = GetIds(item);
                    foreach(var itemId in ids)
                    {
                        if (itemId == null) continue;
                        allIds[lvlCount] = itemId;
                        lvlCount = lvlCount + 1;
                    }
                }
                //if (lvlCount == 0) continue;
                perlevelIds = allIds;
                levelsCount[i] = totalPerLevelCount;
                //Array.Clear(allIds, 0, allIds.Length);
            }
            return levelsCount;
        }

        private int GetCount(string id)
        {
            int recordCount = _context.Users.Where(a => a.CreatedBy == id).Count();
            //var records = _context.Users.Where(a => a.CreatedBy == id).Count();
            //if (records.Count == 0) return 0;
            //foreach (var item in records)
            //{
            //    recordCount = recordCount + _context.Users.Where(a => a.CreatedBy == item.Id).Count();

            //}
            return recordCount;
        }
        private string[] GetIds(string id)
        {
            int lvlCount = 0;
            int?[] levelsCount = new int?[8];
            string[] lvlsIds = new string[20];
            var records = _context.Users.Where(a => a.CreatedBy == id).ToList();

            foreach (var item in records)
            {
                lvlsIds[lvlCount] = item.Id;
                lvlCount = lvlCount + 1;

            }
            return lvlsIds;
        }
        private List<Tuple<int, string[]>> GetPerLevelCount(string id)
        {
            int recordCount = 0;
            int lvlCount = 0;
            int?[] levelsCount = new int?[8];
            string[] lvlsIds = new string[8];
            List<Tuple<int, string[]>> CountAndIds = new List<Tuple<int, string[]>>();
            CountAndIds.Clear();
            var records = _context.Users.Where(a => a.CreatedBy == id).ToList();

            foreach (var item in records)
            {
                recordCount = recordCount + _context.Users.Where(a => a.CreatedBy == item.Id).Count();
                lvlsIds[lvlCount] = item.Id;
                lvlCount = lvlCount + 1;
                
            }
            CountAndIds.Add(new Tuple<int, string[]>(recordCount, lvlsIds));

            return CountAndIds;
        }


        private decimal UnilevelPoints(int? Count)
        {
            var total = 100 * Count;
            
            return Convert.ToDecimal(total);
        }
        private decimal GrossEarning(int? count, decimal overridingCom)
        {
            var total = overridingCom * count;
            return Convert.ToDecimal(total);
        }
    } 
}