using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Models;
using PORTAL.WEB.Models.GSMModemViewModels;
using PORTAL.WEB.Models.HelperViewModels;
using PORTAL.WEB.Services;

namespace PORTAL.WEB.Controllers
{
    public class ModemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserHandler _userHandler;

        public ModemController(ApplicationDbContext context, IMapper mapper, IUserHandler userHandler)
        {
            _context = context;
            _mapper = mapper;
            _userHandler = userHandler;
        }

        [AuthorizationService(true, "Modem - List")]

        public async Task<IActionResult> Index()
        {
            return View(await GetRecords());
        }

        private async Task<ModemList> GetRecords(DateTime? startDate = null, DateTime? endDate = null, bool? allRecord = false)
        {
            List<GSMModem> dbRec = new List<GSMModem>();
            if (!startDate.HasValue || !endDate.HasValue)
            {
                dbRec = await _context.GSMModem.Where(d => (allRecord.Value || (!allRecord.Value && d.CreatedBy == _userHandler.User.Id))).OrderByDescending(m => m.ModifiedOn).Take(1000).ToListAsync();
            }
            else
            {
                dbRec = await _context.GSMModem.Where(d => (allRecord.Value || (!allRecord.Value && d.CreatedBy == _userHandler.User.Id)) &&
                d.CreatedOn.HasValue && d.CreatedOn.Value.Date >= startDate.Value.Date && d.CreatedOn.Value.Date <= endDate.Value.Date).OrderByDescending(m => m.ModifiedOn).ToListAsync();
            }
            ModemList recList = new ModemList
            {
                UserHandler = _userHandler,
                Records = _mapper.Map<List<GSMModem>, List<ModemModel>>(dbRec)
            };
            return recList;
        }

        public async Task<IActionResult> Records(DateTime? startDate = null, DateTime? endDate = null, bool? allRecord = false)
        {
            return PartialView("_RecordList", await GetRecords(startDate, endDate, allRecord));
        }

        [AuthorizationService(true, "New Modem")]
        public IActionResult New()
        {
            ModemModel model = new ModemModel
            {
                FormType = Global.FormType.Create,
                UserHandler = _userHandler,
                IsRecordOwner = true
            };

            return View("Form", model);
        }

        [HttpPost]
        [AuthorizationService(true, "New Modem")]
        public async Task<IActionResult> New(ModemModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        [AuthorizationService(true, "Modem Form")]
        public async Task<IActionResult> Form(string id)
        {
            var dbRec = await _context.GSMModem.SingleOrDefaultAsync(p => p.Id == id);
            var viewModel = _mapper.Map<GSMModem, ModemModel>(dbRec);
            if (dbRec != null)
            {
                viewModel.FormType = _userHandler.HasRequiredPermission("Modem-Update") ?
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
        [AuthorizationService(true, "Modem Update")]
        public async Task<IActionResult> Update(ModemModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        private async Task<ModemModel> Transact(ModemModel model, bool changeStatus = false)
        {
            model.FormBehavior = new FormBehavior();
            model.UserHandler = _userHandler;
            try
            {
                var record = _mapper.Map<ModemModel, GSMModem>(model);
                if (model.FormType == Global.FormType.Create)
                {
                    if (ModelState.IsValid)
                    {
                        _context.GSMModem.Add(record);
                        await _context.SaveChangesAsync();
                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "Modem", new { @id = record.Id })
                            }
                        };
                    }
                    model.IsRecordOwner = true;
                }
                else if (model.FormType == Global.FormType.Update || model.FormType == Global.FormType.ReadOnly)
                {
                    var dbRecord = _context.GSMModem.Find(model.Id);

                    if (changeStatus)
                    {
                        ModelState.Clear();
                        var status = dbRecord.Status == Enums.Status.Active ?
                            Enums.Status.Inactive : Enums.Status.Active;
                        dbRecord.Status = status;
                        _context.GSMModem.Update(dbRecord);
                        await _context.SaveChangesAsync();
                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "Modem", new { @id = model.Id })
                            }
                        };
                    }
                    else if (model.FormType == Global.FormType.Update)
                    {
                        if (ModelState.IsValid)
                        {
                            dbRecord.PortName = model.PortName;
                            dbRecord.BaudRate = model.BaudRate;
                            dbRecord.ReadTimeout = model.ReadTimeout;
                            dbRecord.WriteTimeout = model.WriteTimeout;
                            dbRecord.GSMStatus = model.GSMStatus;
                            _context.GSMModem.Update(dbRecord);
                            await _context.SaveChangesAsync();
                            model.FormBehavior.Notification = new Notification
                            {
                                IsError = false,
                                Message = "Changes successfuly saved.",
                                Title = "Modem"
                            };
                        }
                    }

                    model.IsRecordOwner = dbRecord.CreatedBy == _userHandler.User.Id;
                }
            }
            catch (Exception ex)
            {
                model.FormBehavior.Notification = new Notification
                {
                    IsError = true,
                    Message = ex.Message,
                    Title = "Modem"
                };
            }
            return model;
        }

        [AuthorizationService(true, "Modem Delete")]
        public IActionResult Delete(string id)
        {
            GSMModem record;
            var deleteModel = DeleteModel(id, out record);
            return PartialView("_RecordDelete", deleteModel);
        }

        [AuthorizationService(true, "Modem Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            GSMModem record;
            var deleteModel = DeleteModel(id, out record);
            try
            {
                if (record != null)
                {
                    _context.GSMModem.Remove(record);
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

        private RecordDeleteViewModel DeleteModel(string id, out GSMModem record)
        {
            RecordDeleteViewModel deleteModel = new RecordDeleteViewModel();
            record = null;
            deleteModel.ActionName = "Delete";
            deleteModel.ControllerName = "Modem";
            deleteModel.Id = id;
            deleteModel.Title = "Delete Modem";
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Action");
                return deleteModel;
            }
            record = _context.GSMModem.SingleOrDefault(m => m.Id == id);
            if (record == null)
            {
                ModelState.AddModelError(string.Empty, "Record not found.");
            }
            else
            {
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Modem Name", Value = record.GSMModem_Id });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Port", Value = record.PortName });
            }
            return deleteModel;
        }

        private BootstrapModalModel DeleteModalAction()
        {
            return new BootstrapModalModel
            {
                ModalID = $"#record-delete-modal",
                ShouldClose = true,
                FetchData = true,
                Destination = Url.Action("Records"),
                Target = $"#record-list-container",
                OnSuccess = $"Helper.BindToDataTable('#record-list-table')"
            };
        }

        [HttpPost]
        [AuthorizationService(true, "Modem Change Status")]
        public async Task<IActionResult> ChangeStatus(ModemModel model)
        {
            model = await Transact(model, true);
            return PartialView("_Form", model);
        }
    }
}