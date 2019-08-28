using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using System.Linq;

namespace PORTAL.WEB.Services
{
    public class IncomeCompute : IIncomeCompute
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHandler _userHandler;

        public IncomeCompute(ApplicationDbContext context, IUserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        public void BayanihanIncomeCompute(string id)
        {
            if (_context.Bayanihan.Count() == 1)
            {
                return;
            }

            int?[] counts = new int?[8];
            var bayanihan = _context.Bayanihan.SingleOrDefault(a => a.UserId == id);
            int? UpperConnectedUser = bayanihan.Count;
            for (int i = 0; i <= 7; i++)
            {
                UpperConnectedUser = GetUpperNumberConnectedUser(UpperConnectedUser.Value);
                counts[i] = UpperConnectedUser;
                if (UpperConnectedUser == 0)
                {
                    break;
                }
            }

            AddBayanihanIncome(counts);

        }

        private int GetUpperNumberConnectedUser(double lastUpperNumber)
        {
            double connectNumber = (lastUpperNumber / 5) - 0.2;
            string numWithdecimal = connectNumber.ToString();
            string[] number = numWithdecimal.Split(".");
            int num = int.Parse(number[0]);

            return num;
        }

        private void AddBayanihanIncome(int?[] counts)
        {
            foreach (var count in counts.Where(X => X.HasValue))
            {
                var bayanihanUpperUser = _context.Bayanihan.Where(a => a.Count == count.Value).SingleOrDefault();
                if (bayanihanUpperUser == null)
                {
                    continue;
                }

                var incomeRecord = _context.Income.Where(a => a.UserId == bayanihanUpperUser.UserId).SingleOrDefault();
                if (incomeRecord == null)
                {
                    continue;
                }

                incomeRecord.BayanihanIncome = incomeRecord.BayanihanIncome + 5;
                _context.Income.Update(incomeRecord);
                _context.SaveChanges();
                ComputeNetIncome(bayanihanUpperUser.UserId);
                //enrollToBinary(bayanihanUpperUser.UserId);
            }
        }

        private void EnrollToBinary(string id)
        {
            var recorUser = _context.Users.SingleOrDefault(a => a.Id == id);
            if (recorUser.RegistrationType == Enums.RegistrationType.Geneology)
            {
                return;
            }

            var recordIncome = _context.Income.SingleOrDefault(a => a.UserId == id);
            var netIncome = recordIncome.NetIncome;
            if (netIncome >= 25)
            {
                recorUser.RegistrationType = Enums.RegistrationType.Geneology;
                _context.Users.Update(recorUser);
                _context.SaveChanges();
            }
        }

        public void DirectReferralCompute(string id)
        {
            var incomeRecord = _context.Income.SingleOrDefault(a => a.UserId == id);
            incomeRecord.DirectReferralIncome = incomeRecord.DirectReferralIncome + 50;
            _context.Income.Update(incomeRecord);
            _context.SaveChanges();
            ComputeNetIncome(id);

        }

        public void UnilevelCompute(string id)
        {
            string[] allUpperUnilevel = GetAllUpperUnilevel(id);
            foreach (var item in allUpperUnilevel)
            {
                if (item == null)
                {
                    continue;
                }

                var incomeRecord = _context.Income.SingleOrDefault(a => a.UserId == item.ToString());
                incomeRecord.UnilevelIncome = incomeRecord.UnilevelIncome + 5;
                _context.Income.Update(incomeRecord);
                _context.SaveChanges();
                ComputeNetIncome(item.ToString());
            }
        }

        private string[] GetAllUpperUnilevel(string id)
        {
            string[] ids = new string[8];
            var userRecord = _context.Users.SingleOrDefault(a => a.Id == id);
            string perLevelIds = id;
            for (int i = 0; i <= 7; i++)
            {
                perLevelIds = GetUserParentId(perLevelIds);
                if (perLevelIds == string.Empty)
                {
                    break;
                }

                ids[i] = perLevelIds;

            }
            return ids;
        }

        private string GetUserParentId(string id)
        {
            var userRecord = _context.Users.SingleOrDefault(a => a.Id == id);
            var userParentRecord = _context.Users.SingleOrDefault(a => a.Id == userRecord.CreatedBy);
            if (userParentRecord.CreatedBy == null)
            {
                return string.Empty;
            }

            string parentId = userRecord.CreatedBy.ToString();
            return parentId;
        }

        public void ComputeNetIncome(string id)
        {
            var record = _context.Income.SingleOrDefault(a => a.UserId == id);
            var directReferal = record.DirectReferralIncome;
            //var amountBayanihan = record.BayanihanIncome;
            var salesMatchBonus = record.SalesMatchBonusIncome;
            var amountUnilevel = record.UnilevelIncome;
            var geneology = record.GeneologyIncome;
            record.NetIncome = directReferal + amountUnilevel + geneology;
            _context.Income.Update(record);
            _context.SaveChanges();
        }

        public void GeneologyIncomeCompute(string id)
        {
            var user = _context.ApplicationUser.Find(id);
            if (user == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(user.ParentId))
            {
                return;
            }
            ComputeGeneologyIncome(user.ParentId);

        }

        private void ComputeGeneologyIncome(string parentId)
        {
            int level = 0;
            string tempParentId = parentId;
            while (!string.IsNullOrWhiteSpace(tempParentId))
            {
                var userId = IsPerfect(0, level += 1, tempParentId);
                if (userId != string.Empty)
                {
                    var income = _context.Income.Where(x => x.UserId == tempParentId).FirstOrDefault();
                    if (income != null)
                    {
                        income.GeneologyIncome += (15 * level);
                        _context.Update(income);
                        _context.SaveChanges();
                        ComputeNetIncome(tempParentId);
                    }
                    var user = _context.ApplicationUser.Find(tempParentId);
                    tempParentId = user != null && !string.IsNullOrWhiteSpace(user.ParentId) && user.ParentId != tempParentId ? user.ParentId : string.Empty;
                }
                else { break; }
            }
        }

        private string IsPerfect(int curLevel, int level, string parentId)
        {
            if (curLevel != level)
            {
                var childs = _context.ApplicationUser.Where(c => c.ParentId == parentId && c.CreatedBy == _userHandler.User.Id).ToList();
                if (childs.Count() < 2)
                {
                    return string.Empty;
                }

                foreach (var child in childs)
                {
                    var isValid = "";
                    if (child.ChildPosition == Enums.BPosition.Left)
                    {
                        isValid = IsPerfect(curLevel + 1, level, child.Id);
                    }
                    else if (child.ChildPosition == Enums.BPosition.Right)
                    {
                        isValid = IsPerfect(curLevel + 1, level, child.Id);
                    }

                    if (string.IsNullOrWhiteSpace(isValid))
                    {
                        return string.Empty;
                    }
                }
            }
            return parentId;
        }
    }
}
