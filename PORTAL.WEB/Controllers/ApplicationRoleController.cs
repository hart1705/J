using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Models;
using PORTAL.WEB.Models.ApplicationRoleViewModels;
using PORTAL.WEB.Models.ApplicationUserViewModels;
using PORTAL.WEB.Models.HelperViewModels;
using PORTAL.WEB.Services;
using PORTAL.WEB.UserControls.LookupControl;

namespace PORTAL.WEB.Controllers
{
    [Authorize]
    public class ApplicationRoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IUserHandler _userHandler;

        public ApplicationRoleController(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, IMapper mapper, IUserHandler userHandler)
        {
            _context = context;
            _roleManager = roleManager;
            _mapper = mapper;
            _userHandler = userHandler;
        }

        [AuthorizationService(true, "Application Role - List")]

        public async Task<IActionResult> Index()
        {
            return View(await GetRecords());
        }

        private async Task<ApplicationRoleList> GetRecords()
        {
            var dbRec = await _context.ApplicationRole.ToListAsync();
            ApplicationRoleList rolesList = new ApplicationRoleList
            {
                UserHandler = _userHandler,
                Roles = _mapper.Map<List<ApplicationRole>, List<ApplicationRoleModel>>(dbRec)
            };
            return rolesList;
        }

        public async Task<IActionResult> Records()
        {
            return PartialView("_RecordList", await GetRecords());
        }

        [AuthorizationService(true, "New Application Role")]
        public IActionResult New()
        {
            ApplicationRoleModel model = new ApplicationRoleModel
            {
                FormType = Global.FormType.Create,
                UserHandler = _userHandler,
                IsRecordOwner = true
            };

            return View("Form", model);
        }

        [HttpPost]
        [AuthorizationService(true, "New Application Role")]
        public async Task<IActionResult> New(ApplicationRoleModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        [AuthorizationService(true, "Application Role Form")]
        public async Task<IActionResult> Form(string id)
        {
            var dbRec = await _context.ApplicationRole.SingleOrDefaultAsync(p => p.Id == id);
            var viewModel = _mapper.Map<ApplicationRole, ApplicationRoleModel>(dbRec);
            if (dbRec != null)
            {
                viewModel.FormType = _userHandler.HasRequiredPermission("ApplicationRole-Update") ?
                    Global.FormType.Update : Global.FormType.ReadOnly;

                if (dbRec.Status == Enums.Status.Inactive)
                {
                    viewModel.FormType = Global.FormType.ReadOnly;
                }

                viewModel.UserHandler = _userHandler;
                viewModel.IsRecordOwner = dbRec.CreatedBy == _userHandler.User.Id;
            }
            else
            {
                return NotFound();
            }
            LookupControlModel lookupModel = new LookupControlModel();
            lookupModel.Title = "Application Permissions";
            lookupModel.LookupControlId = "permissionlookup";
            lookupModel.PrimaryRecordId = id;
            lookupModel.RelationshipName = "RolePermission";

            viewModel.PermissionLookup = lookupModel;

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizationService(true, "Update Application Role")]
        public async Task<IActionResult> Update(ApplicationRoleModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        private async Task<ApplicationRoleModel> Transact(ApplicationRoleModel model, bool changeStatus = false)
        {
            model.FormBehavior = new FormBehavior();
            model.UserHandler = _userHandler;

            if (ModelState.IsValid)
            {
                try
                {
                    var role = _mapper.Map<ApplicationRoleModel, ApplicationRole>(model);
                    if (model.FormType == Global.FormType.Create)
                    {
                        IdentityResult result = await _roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                        }
                        else
                        {
                            model.FormBehavior = new FormBehavior
                            {
                                PageRedirect = new PageRedirect
                                {
                                    Reload = true,
                                    URL = Url.Action("Form", "ApplicationRole", new { @id = role.Id })
                                }
                            };
                        }
                        model.IsRecordOwner = true;
                    }
                    else if (model.FormType == Global.FormType.Update || model.FormType == Global.FormType.ReadOnly)
                    {
                        var tempRole = _context.ApplicationRole.Find(model.Id);

                        if ((tempRole.IsSysAdmin && !model.IsSysAdmin) || 
                            (tempRole.IsSysAdmin && changeStatus && tempRole.Status == Enums.Status.Active))
                        {
                            var adminCount = _context.ApplicationRole.Where(r => r.IsSysAdmin).Count();
                            if(adminCount <= 1)
                            {
                                throw new Exception("There should be one existing System Administrator.");
                            }
                        }

                        if (changeStatus)
                        {
                            var status = tempRole.Status == Enums.Status.Active ?
                                Enums.Status.Inactive : Enums.Status.Active;
                            tempRole.Status = status;
                            _context.ApplicationRole.Update(tempRole);
                            await _context.SaveChangesAsync();
                            model.FormBehavior = new FormBehavior
                            {
                                PageRedirect = new PageRedirect
                                {
                                    Reload = true,
                                    URL = Url.Action("Form", "ApplicationRole", new { @id = model.Id })
                                }
                            };
                        }
                        else if (model.FormType == Global.FormType.Update)
                        {
                            tempRole.Description = model.Description;
                            tempRole.IsSysAdmin = model.IsSysAdmin;
                            _context.ApplicationRole.Update(tempRole);
                            await _context.SaveChangesAsync();
                        }
                        model.FormBehavior.Notification = new Notification
                        {
                            IsError = false,
                            Message = "Changes successfuly saved.",
                            Title = "Permission"
                        };
                        model.IsRecordOwner = tempRole.CreatedBy == _userHandler.User.Id;
                    }
                }
                catch (Exception ex)
                {
                    model.FormBehavior.Notification = new Notification
                    {
                        IsError = true,
                        Message = ex.Message,
                        Title = "Application Role"
                    };
                }
            }

            LookupControlModel lookupModel = new LookupControlModel();
            lookupModel.Title = "Application Permissions";
            lookupModel.LookupControlId = "permissionlookup";
            lookupModel.PrimaryRecordId = model.Id;
            lookupModel.RelationshipName = "RolePermission";

            model.PermissionLookup = lookupModel;

            return model;
        }

        [AuthorizationService(true, "Delete Role")]
        public IActionResult Delete(string id)
        {
            ApplicationRole role;
            var deleteModel = DeleteModel(id, out role);
            return PartialView("_RecordDelete", deleteModel);
        }

        [AuthorizationService(true, "Delete Role")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationRole role;
            var deleteModel = DeleteModel(id, out role);
            try
            {
                if (role != null)
                {
                    _context.ApplicationRole.Remove(role);
                    await _context.SaveChangesAsync();
                }
                return PartialView("_BootstrapModalAction", RoleDeleteModalAction());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting record: {ex.Message}.");
            }
            return PartialView("_RecordDelete", deleteModel);
        }

        private RecordDeleteViewModel DeleteModel(string id, out ApplicationRole role)
        {
            RecordDeleteViewModel deleteModel = new RecordDeleteViewModel();
            role = null;
            deleteModel.ActionName = "Delete";
            deleteModel.ControllerName = "ApplicationRole";
            deleteModel.Id = id;
            deleteModel.Title = "Delete Application Role";
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Action");
                return deleteModel;
            }
            role = _context.ApplicationRole.SingleOrDefault(m => m.Id == id);
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Record not found.");
            }
            else
            {
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Name", Value = role.Name });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Description", Value = role.Description });
            }
            return deleteModel;
        }

        private BootstrapModalModel RoleDeleteModalAction()
        {
            return new BootstrapModalModel
            {
                ModalID = $"#record-delete-modal",
                ShouldClose = true,
                FetchData = true,
                Destination = Url.Action("Records"),
                Target = $"#record-list-container",
                OnSuccess = $"Helper.BindToDataTable('#role-list-table')"
            };
        }
        
        [HttpPost]
        [AuthorizationService(true, "Change Status Application User")]
        public async Task<IActionResult> ChangeStatus(ApplicationRoleModel model)
        {
            model = await Transact(model, true);
            return PartialView("_Form", model);
        }
    }
}
