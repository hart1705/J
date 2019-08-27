using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PORTAL.DAL.EF;
using PORTAL.WEB.Extensions;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Services;

namespace PORTAL.WEB.Controllers
{
    public class BinaryTreeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHandler _userHandler;
        private readonly IBinaryTree _binaryTree;

        public BinaryTreeController(ApplicationDbContext context, IUserHandler userHandler, IBinaryTree binaryTree)
        {
            _context = context;
            _userHandler = userHandler;
            _binaryTree = binaryTree;
        }

        [AuthorizationService(true, "Binary Tree - View")]
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetUserBinaryTree(string userId = "")
        {
            var resolvedUserId = string.IsNullOrWhiteSpace(userId) ? _userHandler.User.Id : userId;
            var binaries = _binaryTree.GetUserTree(resolvedUserId);
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new JsonOrgChartConverter(resolvedUserId == _userHandler.User.Id) },
                Formatting = Formatting.Indented
            };
            return Json(binaries, settings);
        }
    }
}