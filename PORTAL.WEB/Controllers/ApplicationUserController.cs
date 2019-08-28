using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Extensions;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Models;
using PORTAL.WEB.Models.ApplicationUserViewModels;
using PORTAL.WEB.Models.HelperViewModels;
using PORTAL.WEB.Services;
using PORTAL.WEB.UserControls.LookupControl;
using PORTAL.WEB.UserControls.ViewPaginationControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PORTAL.WEB.Controllers
{
    [Authorize]
    public class ApplicationUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserHandler _userHandler;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ApplicationUserController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, IUserHandler userHandler, IEmailSender emailSender)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _userHandler = userHandler;
            _emailSender = emailSender;
        }

        [AuthorizationService(true, "Application User - List")]

        public IActionResult Index()
        {
            return View();
        }

        private async Task<ApplicationUserList> GetRecords()
        {
            var dbRec = await _context.ApplicationUser.Where(u => u.UserName != User.Identity.Name && u.UserName != "admin@portal.com").ToListAsync();
            ApplicationUserList records = new ApplicationUserList
            {
                UserHandler = _userHandler,
                Users = _mapper.Map<List<ApplicationUser>, List<ApplicationUserModel>>(dbRec)
            };


            return records;
        }

        public JsonResult CustomServerSideSearchAction(DataTableAjaxModel model)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var whereClause = BuildDynamicWhereClause(_context, searchBy);

            string sortBy = "";
            bool sortDir = true;

            if (String.IsNullOrEmpty(searchBy))
            {
                // if we have an empty search then just order the results by Id ascending
                sortBy = "Id";
                sortDir = true;
            }

            if (model.order != null)
            {
                // in this example we just default sort on the 1st column
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }

            var res = _context.ApplicationUser.Where(u => u.UserName != User.Identity.Name && u.UserName != "admin@portal.com").Where(whereClause).OrderByDyn(sortBy, sortDir).GetPaged<ApplicationUser, ApplicationUserModel>(model.start, model.length);
            for (int i = 0; i < res.Results.Count; i++)
            {
                var cur = res.Results[i];
                if (string.IsNullOrWhiteSpace(cur.CreatedBy))
                    continue;

                var user = Common.ResolveUser(_context, cur.CreatedBy);
                if (user == null)
                    continue;

                res.Results[i].CreatedByName = $"{user.FirstName} {user.LastName}";

                var isOwner = res.Results[i].CreatedBy == _userHandler.User.Id;
                res.Results[i].HasUpdatePermission = _userHandler.HasRequiredPermission("ApplicationUser-Update", isOwner);
                res.Results[i].HasDeletePermission = _userHandler.HasRequiredPermission("ApplicationUser-Delete", isOwner);
                res.Results[i].HasViewPermission = _userHandler.HasRequiredPermission("ApplicationUser-Form", isOwner);
            }

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = _context.ApplicationUser.Count(),
                recordsFiltered = res.RowCount,
                data = res.Results
            });
        }

        private Expression<Func<ApplicationUser, bool>> BuildDynamicWhereClause(ApplicationDbContext entities, string searchValue)
        {
            // simple method to dynamically plugin a where clause
            var predicate = PredicateBuilder.New<ApplicationUser>(true); // true -where(true) return all
            if (String.IsNullOrWhiteSpace(searchValue) == false)
            {
                // as we only have 2 cols allow the user type in name 'firstname lastname' then use the list to search the first and last name of dbase
                var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());

                predicate = predicate.Or(s => searchTerms.Any(srch => s.UserName.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.FirstName.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.LastName.ToLower().Contains(srch)));
            }
            return predicate;
        }

        public async Task<IActionResult> Records()
        {
            return PartialView("_RecordList", await GetRecords());
        }

        [AuthorizationService(true, "New Application User")]
        public IActionResult New()
        {
            RegisterUserModel model = new RegisterUserModel
            {
                FormType = Global.FormType.Create,
                UserHandler = _userHandler,
                IsRecordOwner = true
            };

            return View("RegisterForm", model);
        }

        [HttpPost]
        [AuthorizationService(true, "New Application User")]
        public async Task<IActionResult> New(RegisterUserModel model)
        {
            model.FormBehavior = new FormBehavior();
            model.UserHandler = _userHandler;
            model.IsRecordOwner = true;
            if (ModelState.IsValid)
            {
                var userCheck = await _userManager.FindByEmailAsync(model.Email);
                if (userCheck != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered in the system.");
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        MiddleName = model.MiddleName,
                        PhoneNumber = model.PhoneNumber,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user, "P@ssword1");
                    if (result.Succeeded)
                    {
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        //_logger.LogInformation("User created a new account with password.");

                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "ApplicationUser", new { @id = user.Id })
                            }
                        };
                    }
                    AddErrors(result);
                }
            }
            return PartialView("_RegisterForm", model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [AuthorizationService(true, "Application User Form")]
        public async Task<IActionResult> Form(string id)
        {
            var dbRec = await _context.ApplicationUser.SingleOrDefaultAsync(p => p.Id == id);
            var viewModel = _mapper.Map<ApplicationUser, ApplicationUserModel>(dbRec);
            if (dbRec != null)
            {
                viewModel.FormType = _userHandler.HasRequiredPermission("ApplicationUser-Update") ?
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
            lookupModel.Title = "Application Roles";
            lookupModel.LookupControlId = "rolelookup";
            lookupModel.PrimaryRecordId = id;
            lookupModel.RelationshipName = "AspNetUserRoles";

            viewModel.RoleLookup = lookupModel;

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizationService(true, "Update Application User")]
        public async Task<IActionResult> Update(ApplicationUserModel model)
        {
            if(ModelState.IsValid)
            {
                model = await Transact(model);
            }
            return PartialView("_Form", model);
        }

        private async Task<ApplicationUserModel> Transact(ApplicationUserModel model, bool changeStatus = false)
        {
            model.FormBehavior = new FormBehavior();
            model.UserHandler = _userHandler;
            try
            {
                var recordUpdate = _context.ApplicationUser.Find(model.Id);
                if (changeStatus)
                {
                    ModelState.Clear();
                    var status = recordUpdate.Status == Enums.Status.Active ?
                        Enums.Status.Inactive : Enums.Status.Active;
                    recordUpdate.Status = status;
                    _context.ApplicationUser.Update(recordUpdate);
                    await _context.SaveChangesAsync();
                    model.FormBehavior = new FormBehavior
                    {
                        PageRedirect = new PageRedirect
                        {
                            Reload = true,
                            URL = Url.Action("Form", "ApplicationUser", new { @id = model.Id })
                        }
                    };
                }
                else if (model.FormType == Global.FormType.Update)
                {
                    if (ModelState.IsValid)
                    {
                        var userCheck = await _userManager.FindByEmailAsync(model.Email);
                        if (recordUpdate.Email != model.Email && userCheck != null)
                        {
                            ModelState.AddModelError("Email", "Email is already registered in another user.");
                        }
                        else
                        {
                            recordUpdate.Email = model.Email;
                            recordUpdate.PhoneNumber = model.PhoneNumber;
                            recordUpdate.MiddleName = model.MiddleName;
                            recordUpdate.LastName = model.LastName;
                            recordUpdate.FirstName = model.FirstName;
                            _context.ApplicationUser.Update(recordUpdate);
                            await _context.SaveChangesAsync();

                            model.FormBehavior.Notification = new Notification
                            {
                                IsError = false,
                                Message = "Changes successfuly saved.",
                                Title = "Application User"
                            };
                        }
                    }
                }
                model.IsRecordOwner = recordUpdate.CreatedBy == _userHandler.User.Id;
            }
            catch (Exception ex)
            {
                model.FormBehavior.Notification = new Notification
                {
                    IsError = true,
                    Message = ex.Message,
                    Title = "Application User"
                };
            }

            LookupControlModel lookupModel = new LookupControlModel();
            lookupModel.Title = "Application Roles";
            lookupModel.LookupControlId = "rolelookup";
            lookupModel.PrimaryRecordId = model.Id;
            lookupModel.RelationshipName = "AspNetUserRoles";

            model.RoleLookup = lookupModel;
            return model;
        }

        [AuthorizationService(true, "Delete User")]
        public IActionResult Delete(string id)
        {
            ApplicationUser user;
            var deleteModel = DeleteModel(id, out user);
            return PartialView("_RecordDelete", deleteModel);
        }

        [AuthorizationService(true, "Delete User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser user;
            var deleteModel = DeleteModel(id, out user);
            try
            {
                if (user != null)
                {
                    _context.ApplicationUser.Remove(user);
                    await _context.SaveChangesAsync();
                }
                return PartialView("_BootstrapModalAction", DeleteModalAction());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting record: {ex.Message}.");
            }
            return PartialView("_RecordDelete", deleteModel);
        }

        private RecordDeleteViewModel DeleteModel(string id, out ApplicationUser user)
        {
            RecordDeleteViewModel deleteModel = new RecordDeleteViewModel();
            user = null;
            deleteModel.ActionName = "Delete";
            deleteModel.ControllerName = "ApplicationUser";
            deleteModel.Id = id;
            deleteModel.Title = "Delete Application User";
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Action");
                return deleteModel;
            }
            user = _context.ApplicationUser.SingleOrDefault(m => m.Id == id);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Record not found.");
            }
            else
            {
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Username", Value = user.UserName });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Full Name", Value = $"{user.FirstName} {user.LastName}" });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Email Address", Value = user.Email });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Mobile Number", Value = user.PhoneNumber });
            }
            return deleteModel;
        }

        private BootstrapModalModel DeleteModalAction()
        {
            return new BootstrapModalModel
            {
                ModalID = $"#record-delete-modal",
                ShouldClose = true,
                FetchData = false,
                Destination = string.Empty,
                Target = $"#record-list-container",
                OnSuccess = $"table.ajax.reload();"
            };
        }

        [HttpPost]
        [AuthorizationService(true, "Change Status Application User")]
        public async Task<IActionResult> ChangeStatus(ApplicationUserModel model)
        {
            model = await Transact(model, true);
            return PartialView("_Form", model);
        }
    }
}
