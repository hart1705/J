using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Models.ActionViewModels;
using PORTAL.WEB.Models.PermissionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.ViewComponents
{
    public class ApplicationPermissionViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ApplicationPermissionViewComponent(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IViewComponentResult Invoke(string permissionId, bool enabled = true)
        {
            var dbRecord = _context.ApplicationAction_ApplicationPermission.Include(i => i.ApplicationAction).
                Where(p => p.ApplicationPermissionId == permissionId).
                Select(o=> new ApplicationAction_ApplicationPermission
                {
                    ApplicationAction = o.ApplicationAction,
                    AccessType = o.AccessType,
                    ApplicationActionId = o.ApplicationActionId
                }).ToList();
            
            var actionModels = _mapper.Map<List<ApplicationAction>, List<ApplicationActionModel>>( dbRecord.Select(p => p.ApplicationAction).ToList());

            foreach (var item in actionModels)
            {
                item.AccessType = dbRecord.Where(c => c.ApplicationActionId == item.Id).Select(s => s.AccessType).FirstOrDefault();
            }

            ActionPermission actionPermission = new ActionPermission
            {
                PermissinId = permissionId,
                Actions =  actionModels
            };
            ViewData["enabled"] = enabled;
            return View(actionPermission);
        }
    }
}
