using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Models;
using PORTAL.WEB.Models.ActionViewModels;
using PORTAL.WEB.Models.HelperViewModels;
using PORTAL.WEB.Models.PermissionViewModels;
using PORTAL.WEB.Services;
using PORTAL.WEB.UserControls.LookupControl;

namespace PORTAL.WEB.Controllers
{
    [Authorize]
    public class ApplicationPermissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserHandler _userHandler;

        public ApplicationPermissionController(ApplicationDbContext context, IMapper mapper, IUserHandler userHandler)
        {
            _context = context;
            _mapper = mapper;
            _userHandler = userHandler;
        }


        [AuthorizationService(true, "Permission List")]
        public async Task<IActionResult> Index()
        {
            return View(await GetRecords());
        }

        private async Task<PermissionList> GetRecords()
        {
            var dbRec = await _context.ApplicationPermission.ToListAsync();
            PermissionList permissions = new PermissionList
            {
                UserHandler = _userHandler,
                Permissions = _mapper.Map<List<ApplicationPermission>, List<Permission>>(dbRec)
            };
            return permissions;
        }

        public async Task<IActionResult> Records()
        {
            return PartialView("_RecordList", await GetRecords());
        }

        [AuthorizationService(true, "New Permission")]
        public IActionResult New()
        {
            Permission model = new Permission
            {
                FormType = Global.FormType.Create,
                UserHandler = _userHandler,
                IsRecordOwner = true
            };
            return View("Form", model);
        }

        [HttpPost]
        [AuthorizationService(true, "New Permission")]
        public async Task<IActionResult> New(Permission model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        [AuthorizationService(true, "Permission Form")]
        public async Task<IActionResult> Form(string id)
        {
            var dbRec = await _context.ApplicationPermission.SingleOrDefaultAsync(p => p.Id == id);
            var viewModel = _mapper.Map<ApplicationPermission, Permission>(dbRec);
            if (dbRec != null)
            {
                viewModel.FormType = _userHandler.HasRequiredPermission("ApplicationPermission-Update") ?
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
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizationService(true, "Update Permission")]
        public async Task<IActionResult> Update(Permission model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        private async Task<Permission> Transact(Permission model, bool changeStatus = false)
        {
            model.FormBehavior = new FormBehavior();
            model.UserHandler = _userHandler;

            try
            {
                var permission = _mapper.Map<Permission, ApplicationPermission>(model);
                if (model.FormType == Global.FormType.Create)
                {
                    if (ModelState.IsValid)
                    {
                        _context.ApplicationPermission.Add(permission);
                        await _context.SaveChangesAsync();
                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "ApplicationPermission", new { @id = permission.Id })
                            }
                        };
                        model.IsRecordOwner = true;
                    }
                }
                else if (model.FormType == Global.FormType.Update || model.FormType == Global.FormType.ReadOnly)
                {
                    var permissionUpdate = _context.ApplicationPermission.Find(permission.Id);
                    if (changeStatus)
                    {
                        ModelState.Clear();
                        var status = permissionUpdate.Status == Enums.Status.Active ?
                            Enums.Status.Inactive : Enums.Status.Active;
                        permissionUpdate.Status = status;
                        _context.ApplicationPermission.Update(permissionUpdate);
                        await _context.SaveChangesAsync();
                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "ApplicationPermission", new { @id = model.Id })
                            }
                        };
                    }
                    else if (model.FormType == Global.FormType.Update)
                    {
                        if (ModelState.IsValid)
                        {
                            permissionUpdate.ApplicationPermission_Id = permission.ApplicationPermission_Id;
                            permissionUpdate.Description = permission.Description;
                            _context.Update(permissionUpdate);
                            await _context.SaveChangesAsync();
                            model.FormBehavior.Notification = new Notification
                            {
                                IsError = false,
                                Message = "Changes successfuly saved.",
                                Title = "Permission"
                            };
                            model.IsRecordOwner = permissionUpdate.CreatedBy == _userHandler.User.Id;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.FormBehavior.Notification = new Notification
                {
                    IsError = true,
                    Message = ex.Message,
                    Title = "Permission"
                };
            }
            return model;
        }

        public IActionResult LookupActionRecord(string permissionId, string search = "")
        {
            var dbRecord = _context.ApplicationPermission.Include(p => p.ApplicationAction_ApplicationPermissions).SingleOrDefault(p => p.Id == permissionId);
            ActionPermission model = new ActionPermission { PermissinId = permissionId };
            if (dbRecord != null)
            {
                var listApplicationActions = dbRecord.ApplicationAction_ApplicationPermissions.Select(l => l.ApplicationActionId).ToList();
                var actions = _context.ApplicationAction.Where(p => p.ApplicationAction_Id.Contains(search) && !listApplicationActions.Contains(p.Id)).ToList();
                model.Actions = _mapper.Map<IList<ApplicationAction>, IList<ApplicationActionModel>>(actions).ToList();
            }
            return PartialView("_ActionRecord", model);
        }

        public IActionResult LoadActionItemViewComponent(string permissionId)
        {
            return ViewComponent("ApplicationPermission", new { permissionId });
        }

        [HttpPost]
        public async Task<IActionResult> AssociateAction(ActionPermission model)
        {
            foreach (var item in model.Actions.Where(m => m.Selected))
            {
                _context.Add(new ApplicationAction_ApplicationPermission
                {
                    AccessType = Enums.AccessType.User,
                    ApplicationActionId = item.Id,
                    ApplicationPermissionId = model.PermissinId
                });
            }
            await _context.SaveChangesAsync();
            return PartialView("_BootstrapModalAction", ActionPermissionModalAction(model.PermissinId));
        }

        public async Task<IActionResult> DisassociateAction(string id, string permissionId)
        {
            ApplicationAction_ApplicationPermission actionPermission = await _context.ApplicationAction_ApplicationPermission.Include(i => i.ApplicationAction).Where(a => a.ApplicationActionId == id && a.ApplicationPermissionId == permissionId).FirstOrDefaultAsync();
            if (actionPermission == null)
            {
                return NotFound();
            }
            return PartialView("_DisassociateAction", actionPermission);
        }

        [HttpPost, ActionName("DisassociateAction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisassociateActionConfirmed(string id)
        {
            ApplicationAction_ApplicationPermission actionPermission = new ApplicationAction_ApplicationPermission();
            string permissionId = string.Empty;
            try
            {
                actionPermission = await _context.ApplicationAction_ApplicationPermission.Where(a => a.Id == id).SingleOrDefaultAsync();
                permissionId = actionPermission.ApplicationPermissionId;
                _context.ApplicationAction_ApplicationPermission.Remove(actionPermission);
                await _context.SaveChangesAsync();
                return PartialView("_BootstrapModalAction", ActionPermissionModalAction(permissionId));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting record: {ex.Message}.");
            }
            return PartialView("_DisassociateAction", actionPermission);
        }

        private BootstrapModalModel ActionPermissionModalAction(string permissionId)
        {
            return new BootstrapModalModel
            {
                ModalID = $"#applicationactionpermission-modal",
                ShouldClose = true,
                FetchData = true,
                Destination = Url.Action("LoadActionItemViewComponent", new
                {
                    permissionId
                }),
                Target = $"#applicationactionpermission-list-container",
                OnSuccess = $"Lookup.BindToDataTable('applicationactionpermission-table')"
            };
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAccessType(string actionId, string permissionId, int accessType)
        {

            var actionPermission = await _context.ApplicationAction_ApplicationPermission.Where(a => a.ApplicationActionId == actionId && a.ApplicationPermissionId == permissionId).SingleOrDefaultAsync();

            if (actionPermission == null)
            {
                throw new Exception("Error");
            }

            actionPermission.AccessType = (int)Enums.AccessType.User == accessType ? Enums.AccessType.User : Enums.AccessType.Organization;
            _context.Update(actionPermission);
            await _context.SaveChangesAsync();
            return PartialView("_BootstrapModalAction", ActionPermissionModalAction(actionPermission.ApplicationPermissionId));
        }

        [AuthorizationService(true, "Delete Permission")]
        public IActionResult Delete(string id)
        {
            ApplicationPermission permission;
            var deleteModel = DeleteModel(id, out permission);
            return PartialView("_RecordDelete", deleteModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationPermission permission;
            var deleteModel = DeleteModel(id, out permission);
            try
            {
                if (permission != null)
                {
                    _context.ApplicationPermission.Remove(permission);
                    await _context.SaveChangesAsync();
                }
                return PartialView("_BootstrapModalAction", PermissionDeleteModalAction());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting record: {ex.Message}.");
            }
            return PartialView("_RecordDelete", deleteModel);
        }

        private RecordDeleteViewModel DeleteModel(string id, out ApplicationPermission permission)
        {
            RecordDeleteViewModel deleteModel = new RecordDeleteViewModel();
            permission = null;
            deleteModel.ActionName = "Delete";
            deleteModel.ControllerName = "ApplicationPermission";
            deleteModel.Id = id;
            deleteModel.Title = "Delete Permission";
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Action");
                return deleteModel;
            }
            permission = _context.ApplicationPermission.SingleOrDefault(m => m.Id == id);
            if (permission == null)
            {
                ModelState.AddModelError(string.Empty, "Record not found.");
            }
            else
            {
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Name", Value = permission.ApplicationPermission_Id });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Description", Value = permission.Description });
            }
            return deleteModel;
        }

        private BootstrapModalModel PermissionDeleteModalAction()
        {
            return new BootstrapModalModel
            {
                ModalID = $"#record-delete-modal",
                ShouldClose = true,
                FetchData = true,
                Destination = Url.Action("Records"),
                Target = $"#record-list-container",
                OnSuccess = $"Helper.BindToDataTable('#permission-list-table')"
            };
        }


        [HttpPost]
        [AuthorizationService(true, "Change Status Application Permission")]
        public async Task<IActionResult> ChangeStatus(Permission model)
        {
            model = await Transact(model, true);
            return PartialView("_Form", model);
        }
    }
}
