using Newtonsoft.Json;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace PORTAL.WEB.Services
{
    public class BinaryTree : IBinaryTree
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHandler _userHandler;
        public BinaryTree(ApplicationDbContext context, IUserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        private BinaryTreeModel GenerateUserBinary(string userId)
        {
            var user = _context.ApplicationUser.Where(u => u.Id == userId).Select(r => new ApplicationUser
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                CreatedOn = r.CreatedOn,
                UserName = r.UserName,
                ParentId = r.ParentId,
                ChildPosition = r.ChildPosition,
                ReferralsCount = _context.ApplicationUser.Where(u => u.CreatedBy == r.Id).Count()
            }).SingleOrDefault();

            BinaryTreeModel binaryTree = new BinaryTreeModel(userId, user, _context, _userHandler);
            return binaryTree;
        }

        public BinaryTreeModel GetUserTree(string userId)
        {
            return GenerateUserBinary(userId);
        }

        public string GetUserTreeAsJson(string userId, bool isLoggedInUser)
        {
            var binaries = GenerateUserBinary(userId);
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new JsonOrgChartConverter(isLoggedInUser) },
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(binaries, settings);
        }
    }

    public class Dummy
    {
        public string name = "Register New";
        public string userid;
        public string isdummy = "1";
        public string position;

        public Dummy(string userid, Enums.BPosition bPosition)
        {
            position = bPosition.ToString();
            this.userid = userid;
        }
    }

    public class BinaryTreeModel
    {
        public string userid;
        public string name;
        public string title;
        public string dateregistered;
        public string referrals;
        public string position;
        public BinaryTreeModel left;
        public BinaryTreeModel right;

        public BinaryTreeModel(string userId, ApplicationUser user, ApplicationDbContext context, IUserHandler userHandler)
        {
            Load(userId, this, user, context, userHandler);
        }

        void Load(string userId, BinaryTreeModel tree, ApplicationUser user, ApplicationDbContext context, IUserHandler userHandler)
        {
            var current = user;
            this.userid = current.Id;
            this.name = $"{current.FirstName} {current.LastName}";
            this.title = $"{current.UserName}";
            this.dateregistered = current.CreatedOn.Value.ToString("MM/dd/yyyy");
            this.referrals = current.ReferralsCount.ToString();
            this.position = current.ChildPosition.ToString();

            var childs = context.ApplicationUser.Where(c => c.ParentId == current.Id && c.CreatedBy == userId).
                Select(r => new ApplicationUser
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    CreatedOn = r.CreatedOn,
                    UserName = r.UserName,
                    ParentId = r.ParentId,
                    ChildPosition = r.ChildPosition,
                    ReferralsCount = context.ApplicationUser.Where(u => u.CreatedBy == r.Id).Count()
                }).ToList();

            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    if (child.ChildPosition == Enums.BPosition.Left)
                    {
                        this.left = new BinaryTreeModel(userId, child, context, userHandler);
                    }
                    else if (child.ChildPosition == Enums.BPosition.Right)
                    {
                        this.right = new BinaryTreeModel(userId, child, context, userHandler);
                    }
                }
            }
        }


        public void Traverse(BinaryTreeModel root)
        {
            if (root == null)
            {
                return;
            }
            Traverse(root.left);
            Traverse(root.right);
        }
    }
}
