using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Models;
using PORTAL.WEB.Models.HelperViewModels;
using PORTAL.WEB.Models.ReferralCodeViewModels;
using PORTAL.WEB.Services;

namespace PORTAL.WEB.Controllers
{
    public class ReferralCodeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserHandler _userHandler;
        private readonly ICodeGenerator _codeGenerator;

        public ReferralCodeController(ApplicationDbContext context, IMapper mapper, IUserHandler userHandler, ICodeGenerator codeGenerator)
        {
            _context = context;
            _mapper = mapper;
            _userHandler = userHandler;
            _codeGenerator = codeGenerator;
        }

        [AuthorizationService(true, "Referral Code - List")]

        public async Task<IActionResult> Index()
        {
            return View(await GetRecords());
        }

        private async Task<ReferralCodeList> GetRecords(DateTime? startDate = null, DateTime? endDate = null, bool? allRecord = false, int? status = null)
        {
            List<ReferralCode> dbRec = new List<ReferralCode>();
            if (!startDate.HasValue || !endDate.HasValue)
            {
                dbRec = await _context.ReferralCode.Where(d => (allRecord.Value || (!allRecord.Value && d.CreatedBy == _userHandler.User.Id)) && 
                (status == null || (status != null && (status.Value == -1 || status.Value == (int)d.ReferralCodeStatus)))).Include(s=>s.User).OrderByDescending(m => m.ModifiedOn).Take(1000).ToListAsync();
            }
            else
            {
                dbRec = await _context.ReferralCode.Where(d => (allRecord.Value || (!allRecord.Value && d.CreatedBy == _userHandler.User.Id)) &&
                d.CreatedOn.HasValue && d.CreatedOn.Value.Date >= startDate.Value.Date && d.CreatedOn.Value.Date <= endDate.Value.Date && 
                (status == null || (status != null && (status.Value == -1 || status.Value == (int)d.ReferralCodeStatus)))).Include(s => s.User).OrderByDescending(m => m.ModifiedOn).ToListAsync();
            }
            ReferralCodeList recList = new ReferralCodeList
            {
                UserHandler = _userHandler,
                Records = _mapper.Map<List<ReferralCode>, List<ReferralCodeModel>>(dbRec)
            };
            return recList;
        }

        public async Task<IActionResult> Records(DateTime? startDate = null, DateTime? endDate = null, bool? allRecord = false, int? status = null)
        {
            return PartialView("_RecordList", await GetRecords(startDate, endDate, allRecord, status));
        }

        [AuthorizationService(true, "New Referral Code")]
        public IActionResult New()
        {
            ReferralCodeModel model = new ReferralCodeModel
            {
                FormType = Global.FormType.Create,
                UserHandler = _userHandler,
                IsRecordOwner = true
            };

            return View("Form", model);
        }

        [HttpPost]
        [AuthorizationService(true, "New Referral Code")]
        public async Task<IActionResult> New(ReferralCodeModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }


        [AuthorizationService(true, "Generate Referral Code")]
        public IActionResult GenerateCode()
        {
            return View();
        }

        [HttpPost]
        [AuthorizationService(true, "Generate Referral Code")]
        public IActionResult GenerateCode(int quantity)
        {
            ReferralCodeList codeList = new ReferralCodeList();
            List<ReferralCodeModel> records = new List<ReferralCodeModel>();
            for (int i = 0; i < quantity; i++)
            {
                var code = new ReferralCode
                {
                    PINCode = _codeGenerator.GeneratePINCode(),
                    SecutiryCode = _codeGenerator.GenerateSecurityCode(),
                    ReferralCodeStatus = Enums.ReferralCodeStatus.Open
                };
                code.ReferralCode_Id = $"{code.PINCode}-{code.SecutiryCode}";
                _context.ReferralCode.Add(code);
                var affected = _context.SaveChanges();
                if (affected != 0)
                {
                    records.Add(_mapper.Map<ReferralCodeModel>(code));
                }
            }
            codeList.Records = records;
            return PartialView("_GenerateCode", codeList);
        }

        [AuthorizationService(true, "Referral Code Form")]
        public async Task<IActionResult> Form(string id)
        {
            var dbRec = await _context.ReferralCode.SingleOrDefaultAsync(p => p.Id == id);
            var viewModel = _mapper.Map<ReferralCode, ReferralCodeModel>(dbRec);
            if (dbRec != null)
            {
                viewModel.FormType = _userHandler.HasRequiredPermission("ReferralCode-Update") ?
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
        [AuthorizationService(true, "Referral Code Update")]
        public async Task<IActionResult> Update(ReferralCodeModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        private async Task<ReferralCodeModel> Transact(ReferralCodeModel model, bool changeStatus = false)
        {
            model.FormBehavior = new FormBehavior();
            model.UserHandler = _userHandler;
            try
            {
                var record = _mapper.Map<ReferralCodeModel, ReferralCode>(model);
                if (model.FormType == Global.FormType.Create)
                {
                    if (ModelState.IsValid)
                    {
                        _context.ReferralCode.Add(record);
                        await _context.SaveChangesAsync();
                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "ReferralCode", new { @id = record.Id })
                            }
                        };
                    }
                    model.IsRecordOwner = true;
                }
                else if (model.FormType == Global.FormType.Update || model.FormType == Global.FormType.ReadOnly)
                {
                    var dbRecord = _context.ReferralCode.Find(model.Id);

                    if (changeStatus)
                    {
                        ModelState.Clear();
                        var status = dbRecord.Status == Enums.Status.Active ?
                            Enums.Status.Inactive : Enums.Status.Active;
                        dbRecord.Status = status;
                        _context.ReferralCode.Update(dbRecord);
                        await _context.SaveChangesAsync();
                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "ReferralCode", new { @id = model.Id })
                            }
                        };
                    }
                    else if (model.FormType == Global.FormType.Update)
                    {
                        if (ModelState.IsValid)
                        {
                            dbRecord.ReferralCode_Id = model.ReferralCode_Id;
                            dbRecord.PINCode = model.PINCode;
                            dbRecord.SecutiryCode = model.SecutiryCode;
                            dbRecord.ExpirationDate = model.ExpirationDate;
                            dbRecord.UserId = model.UserId;
                            dbRecord.ReferralCodeStatus = model.ReferralCodeStatus;
                            _context.ReferralCode.Update(dbRecord);
                            await _context.SaveChangesAsync();
                            model.FormBehavior.Notification = new Notification
                            {
                                IsError = false,
                                Message = "Changes successfuly saved.",
                                Title = "Referral Code"
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
                    Title = "Referral Code"
                };
            }
            return model;
        }

        [AuthorizationService(true, "Refferal Code Delete")]
        public IActionResult Delete(string id)
        {
            ReferralCode record;
            var deleteModel = DeleteModel(id, out record);
            return PartialView("_RecordDelete", deleteModel);
        }

        [AuthorizationService(true, "Referral Code Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ReferralCode record;
            var deleteModel = DeleteModel(id, out record);
            try
            {
                if (record != null)
                {
                    _context.ReferralCode.Remove(record);
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

        private RecordDeleteViewModel DeleteModel(string id, out ReferralCode record)
        {
            RecordDeleteViewModel deleteModel = new RecordDeleteViewModel();
            record = null;
            deleteModel.ActionName = "Delete";
            deleteModel.ControllerName = "ReferralCode";
            deleteModel.Id = id;
            deleteModel.Title = "Delete Referral Code";
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Action");
                return deleteModel;
            }
            record = _context.ReferralCode.SingleOrDefault(m => m.Id == id);
            if (record == null)
            {
                ModelState.AddModelError(string.Empty, "Record not found.");
            }
            else
            {
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "PIN Code", Value = record.PINCode });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Secutiry Code", Value = record.SecutiryCode });
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
        [AuthorizationService(true, "Referral Code Change Status")]
        public async Task<IActionResult> ChangeStatus(ReferralCodeModel model)
        {
            model = await Transact(model, true);
            return PartialView("_Form", model);
        }
    }
}