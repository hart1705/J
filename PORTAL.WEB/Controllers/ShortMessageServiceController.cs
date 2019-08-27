using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Extensions;
using PORTAL.WEB.Filters;
using PORTAL.WEB.Helpers;
using PORTAL.WEB.Interfaces;
using PORTAL.WEB.Models;
using PORTAL.WEB.Models.HelperViewModels;
using PORTAL.WEB.Models.SMSViewModels;
using PORTAL.WEB.Services;

namespace PORTAL.WEB.Controllers
{
    public class ShortMessageServiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserHandler _userHandler;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ISMSService _smsService;

        public ShortMessageServiceController(ApplicationDbContext context, IMapper mapper, IUserHandler userHandler, IHostingEnvironment hostingEnvironment, IConfiguration configuration, ISMSService smsService)
        {
            _context = context;
            _mapper = mapper;
            _userHandler = userHandler;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _smsService = smsService;
        }

        [AuthorizationService(true, "SMS - List")]

        public async Task<IActionResult> Index()
        {
            return View(await GetRecords());
        }

        private async Task<SMSList> GetRecords(SMSFilter filter = null)
        {
            filter = filter == null ? new SMSFilter() : filter;
            List<ShortMessageService> dbRec = new List<ShortMessageService>();

            if (!filter.StartDate.HasValue || !filter.EndDate.HasValue)
            {
                dbRec = await _context.ShortMessageService.Where(d => ((!string.IsNullOrWhiteSpace(filter.ApplicationUserId) && d.CreatedBy == filter.ApplicationUserId) ||
                (string.IsNullOrWhiteSpace(filter.ApplicationUserId) && (filter.AllRecord || (!filter.AllRecord && d.CreatedBy == _userHandler.User.Id)))) &&
                (filter.SMSStatus == -1 || (filter.SMSStatus != -1 && filter.SMSStatus == (int)d.SMSStatus))).OrderByDescending(m => m.ModifiedOn).Take(1000).ToListAsync();
            }
            else
            {
                dbRec = await _context.ShortMessageService.Where(d => ((!string.IsNullOrWhiteSpace(filter.ApplicationUserId) && d.CreatedBy == filter.ApplicationUserId) ||
                (string.IsNullOrWhiteSpace(filter.ApplicationUserId) && (filter.AllRecord || (!filter.AllRecord && d.CreatedBy == _userHandler.User.Id)))) &&
                d.CreatedOn.HasValue && d.CreatedOn.Value.Date >= filter.StartDate.Value.Date && d.CreatedOn.Value.Date <= filter.EndDate.Value.Date &&
                (filter.SMSStatus == -1 || (filter.SMSStatus != -1 && filter.SMSStatus == (int)d.SMSStatus))).OrderByDescending(m => m.ModifiedOn).ToListAsync();
            }
            SMSList recList = new SMSList
            {
                UserHandler = _userHandler,
                Records = _mapper.Map<List<ShortMessageService>, List<SMSModel>>(dbRec)
            };
            return recList;
        }

        [HttpPost]
        [RequestFormSizeLimit(100000000, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
        public async Task<IActionResult> BulkTransact(List<SMSBulkTransact> items, int action)
        {
            var records = items.Where(i => i.Selected);
            switch (action)
            {
                case 0: //RESEND
                    foreach (var item in records)
                    {
                        var smsU = _context.ShortMessageService.SingleOrDefault(sms => sms.Id == item.Id);
                        if (smsU != null)
                        {
                            if (smsU.SMSStatus == Enums.SMSStatus.Queue) continue;
                            smsU.SMSStatus = Enums.SMSStatus.Queue;
                            _context.ShortMessageService.Update(smsU);
                        }
                    }
                    await _context.SaveChangesAsync();
                    break;
                case 1: //DELETE
                    var ids = records.Select(x => x.Id).ToList();
                    _context.ShortMessageService.RemoveRange(_context.ShortMessageService.Where(i => ids.Contains(i.Id)).ToList());
                    await _context.SaveChangesAsync();
                    break;
                case 2: //ACTIVATE
                case 3: //DEACTIVATE
                    var status = action == 2 ? Enums.Status.Active : Enums.Status.Inactive;
                    foreach (var item in records)
                    {
                        var smsU = _context.ShortMessageService.SingleOrDefault(sms => sms.Id == item.Id);
                        if (smsU != null)
                        {
                            if (smsU.Status == status) continue;
                            smsU.Status = status;
                            _context.ShortMessageService.Update(smsU);
                        }
                    }
                    await _context.SaveChangesAsync();
                    break;
                default:
                    break;
            }
            return RedirectToActionPermanent(nameof(Index));
        }

        public async Task<IActionResult> Records(SMSFilter filter)
        {
            return PartialView("_RecordList", await GetRecords(filter));
        }

        [AuthorizationService(true, "New SMS")]
        public IActionResult New()
        {
            SMSModel model = new SMSModel
            {
                FormType = Global.FormType.Create,
                UserHandler = _userHandler,
                IsRecordOwner = true
            };
            return View("Form", model);
        }

        [HttpPost]
        [AuthorizationService(true, "New SMS")]
        public async Task<IActionResult> New(SMSModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        [AuthorizationService(true, "SMS Form")]
        public async Task<IActionResult> Form(string id)
        {
            var dbRec = await _context.ShortMessageService.SingleOrDefaultAsync(p => p.Id == id);
            var viewModel = _mapper.Map<ShortMessageService, SMSModel>(dbRec);
            if (dbRec != null)
            {
                viewModel.FormType = _userHandler.HasRequiredPermission("ShortMessageService-Update") ?
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
        [AuthorizationService(true, "SMS Update")]
        public async Task<IActionResult> Update(SMSModel model)
        {
            model = await Transact(model);
            return PartialView("_Form", model);
        }

        private async Task<SMSModel> Transact(SMSModel model, bool changeStatus = false)
        {
            model.FormBehavior = new FormBehavior();
            model.UserHandler = _userHandler;
            try
            {
                var record = _mapper.Map<SMSModel, ShortMessageService>(model);
                if (model.FormType == Global.FormType.Create)
                {
                    if (ModelState.IsValid)
                    {
                        _smsService.NewSMS(record);
                        model.FormBehavior = new FormBehavior
                        {
                            PageRedirect = new PageRedirect
                            {
                                Reload = true,
                                URL = Url.Action("Form", "ShortMessageService", new { @id = record.Id })
                            }
                        };
                    }
                    model.IsRecordOwner = true;
                }
                else if (model.FormType == Global.FormType.Update || model.FormType == Global.FormType.ReadOnly)
                {
                    var dbRecord = _context.ShortMessageService.Find(model.Id);

                    if (changeStatus)
                    {
                        ModelState.Clear();
                        var status = dbRecord.Status == Enums.Status.Active ?
                            Enums.Status.Inactive : Enums.Status.Active;
                        var success = _smsService.UpdateStatus(model.Id, status);
                        if (success)
                        {
                            model.FormBehavior = new FormBehavior
                            {
                                PageRedirect = new PageRedirect
                                {
                                    Reload = true,
                                    URL = Url.Action("Form", "ShortMessageService", new { @id = model.Id })
                                }
                            };
                        }
                        else
                        {
                            throw new Exception("Status update failed.");
                        }
                    }
                    else if (model.FormType == Global.FormType.Update)
                    {
                        if (ModelState.IsValid)
                        {
                            dbRecord.ShortMessageService_Id = model.ShortMessageService_Id;
                            dbRecord.MobileNumber = model.MobileNumber;
                            dbRecord.MessageBody = model.MessageBody;
                            dbRecord.SMSStatus = model.SMSStatus;
                            _smsService.UpdateSMS(dbRecord);
                            model.FormBehavior.Notification = new Notification
                            {
                                IsError = false,
                                Message = "Changes successfuly saved.",
                                Title = "Short Message Service"
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
                    Title = "Short Message Service"
                };
            }
            return model;
        }

        [AuthorizationService(true, "SMS Delete")]
        public IActionResult Delete(string id)
        {
            ShortMessageService record;
            var deleteModel = DeleteModel(id);
            return PartialView("_RecordDelete", deleteModel);
        }

        [AuthorizationService(true, "SMS Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var deleteModel = DeleteModel(id);
            try
            {
                var success = _smsService.DeleteSMS(id);
                return PartialView("_BootstrapModalAction", DeleteModalAction());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting record: {ex.Message}.");
            }
            return PartialView("_RecordDelete", deleteModel);
        }

        private RecordDeleteViewModel DeleteModel(string id)
        {
            RecordDeleteViewModel deleteModel = new RecordDeleteViewModel();
            ShortMessageService record = null;
            deleteModel.ActionName = "Delete";
            deleteModel.ControllerName = "ShortMessageService";
            deleteModel.Id = id;
            deleteModel.Title = "Delete SMS";
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Action");
                return deleteModel;
            }
            record = _context.ShortMessageService.SingleOrDefault(m => m.Id == id);
            if (record == null)
            {
                ModelState.AddModelError(string.Empty, "Record not found.");
            }
            else
            {
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Mobile Number", Value = record.MobileNumber });
                deleteModel.RecordDetail.Add(new RecordDetail { Label = "Message Body", Value = record.MessageBody });
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
        [AuthorizationService(true, "SMS Change Status")]
        public async Task<IActionResult> ChangeStatus(SMSModel model)
        {
            model = await Transact(model, true);
            return PartialView("_Form", model);
        }

        public IActionResult SendSMS()
        {
            SMSPublicModel model = new SMSPublicModel
            {
                FormType = Global.FormType.Create,
                UserHandler = _userHandler,
                IsRecordOwner = true
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SendSMS(SMSPublicModel model)
        {
            model.FormBehavior = new FormBehavior();
            CaptchaResponse response = ValidateCaptcha(HttpContext.Request.Form["g-recaptcha-response"]);
            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, "Please click captcha for security purposes.");
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    var sms = new SMSModel
                    {
                        MobileNumber = model.MobileNumber,
                        MessageBody = $"From: {model.SenderName}\n{model.MessageBody}\n\nDon't reply to this number."
                    };
                    var record = _mapper.Map<SMSModel, ShortMessageService>(sms);
                    record.SMSStatus = Enums.SMSStatus.Queue;
                    _smsService.NewSMS(record, true);
                    model.FormBehavior.Notification = new Notification
                    {
                        IsError = false,
                        Message = "Message successfully queued.",
                        Title = "SMS"
                    };
                    model.MobileNumber = string.Empty;
                    model.MessageBody = string.Empty;
                    model.SenderName = string.Empty;
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("mobile number"))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid format of mobile number.");
                    }
                    else if (ex.Message.Contains("unicode"))
                    {
                        ModelState.AddModelError(string.Empty, "Message body should not contain unicode characters (ex. emojis)");
                    }
                    else
                    {
                        model.FormBehavior.Notification = new Notification
                        {
                            IsError = true,
                            Message = ex.Message,
                            Title = "SMS"
                        };
                    }
                }
            }
            return PartialView("_SendSMS", model);
        }

        [AuthorizationService(true, "Import SMS")]
        public IActionResult Import()
        {
            ViewData["importeddata"] = 0;
            ViewData["hasimport"] = false;
            SMSImportModel model = new SMSImportModel();
            return View(model); ;
        }

        [HttpPost]
        [AuthorizationService(true, "Import SMS")]
        public async Task<IActionResult> Import(SMSImportModel model, string processType, int smsStatus)
        {
            model.FormBehavior = new FormBehavior();
            model.FormBehavior.Notification = null;
            int importcount = 0;
            ViewData["importeddata"] = importcount;
            ViewData["hasimport"] = false;

            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string uploadPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            if (processType == "CommitImport")
            {
                if (!string.IsNullOrWhiteSpace(model.FileName))
                {
                    try
                    {
                        var records = ExcelToSMSList(uploadPath, model.FileName, true);
                        foreach (var item in records)
                        {
                            try
                            {
                                _smsService.NewSMS(
                                    new ShortMessageService
                                    {
                                        SMSStatus = (Enums.SMSStatus)smsStatus,
                                        MobileNumber = item.MobileNumber,
                                        MessageBody = item.MessageBody
                                    }, true
                                    );
                                importcount += 1;
                            }
                            catch {}
                        }
                        model.FormBehavior.Notification = new Notification
                        {
                            IsError = false,
                            Message = "Records successfully imported.",
                            Title = "Short Message Service"
                        };
                        ViewData["importeddata"] = importcount;
                        model.Records = new List<SMSImportList>();
                    }
                    catch (Exception ex)
                    {
                        model.FormBehavior.Notification = new Notification
                        {
                            IsError = false,
                            Message = ex.Message,
                            Title = "Short Message Service"
                        };
                    }
                    finally
                    {
                        model.FileName = string.Empty;
                    }
                }
            }
            else if (processType == "ProcessImport")
            {
                model.Records = new List<SMSImportList>();
                var file = model.FileAttachment;

                if (file != null && file.Length > 0)
                {
                    string fullPath = Path.Combine(uploadPath, file.FileName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    try
                    {
                        model.FileName = file.FileName;
                        model.Records = ExcelToSMSList(uploadPath, file.FileName);
                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {
                        model.FormBehavior.Notification = new Notification
                        {
                            IsError = true,
                            Message = ex.Message,
                            Title = "Short Message Service"
                        };
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please attach a file.");
                }
            }
            return PartialView("_Import", model);
        }

        private List<SMSImportList> ExcelToSMSList(string uploadPath, string fileName, bool removeFile = false)
        {
            List<SMSImportList> records = new List<SMSImportList>();
            ISheet sheet;
            string sFileExtension = Path.GetExtension(fileName).ToLower();
            string fullPath = Path.Combine(uploadPath, fileName);

            try
            {
                using (var stream = System.IO.File.OpenRead(fullPath))
                {
                    stream.Position = 0;
                    //This will read the Excel 97-2000 formats  
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream);
                        sheet = hssfwb.GetSheetAt(0);
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream);
                        sheet = hssfwb.GetSheetAt(0);
                    }
                    IRow headerRow = sheet.GetRow(0);
                    if (headerRow == null || (headerRow != null && headerRow.LastCellNum != 2))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid format of template.");
                    }
                    else
                    {

                        bool isValid = !(headerRow.GetCell(0) == null || headerRow.GetCell(1) == null);

                        if (isValid)
                        {
                            isValid = headerRow.GetCell(0).ToString().ToLower().Trim(' ') == "mobile number" &&
                                headerRow.GetCell(1).ToString().ToLower().Trim(' ') == "message body";
                        }
                        if (!isValid)
                        {
                            ModelState.AddModelError(string.Empty, "Invalid format of template.");
                        }
                        else
                        {
                            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                            {
                                IRow row = sheet.GetRow(i);
                                if (row == null) continue;
                                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                                if (row.GetCell(0) == null) continue;
                                SMSImportList list = new SMSImportList { MobileNumber = string.Empty };

                                if (row.GetCell(0) != null)
                                {
                                    list.MobileNumber = row.GetCell(0).ToString();
                                }
                                if (row.GetCell(1) != null)
                                {
                                    list.MessageBody = row.GetCell(1).ToString();
                                }
                                records.Add(list);
                            }
                            ViewData["hasimport"] = true;
                        }
                    }
                }
            }
            finally
            {
                if (removeFile && System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            return records;
        }

        /// <summary>  
        /// Validate Captcha  
        /// </summary>  
        /// <param name="response"></param>  
        /// <returns></returns>  
        private CaptchaResponse ValidateCaptcha(string response)
        {
            var recaptchaConfig = _configuration.GetSection("reCAPTCHA");
            string secret = recaptchaConfig["PrivateKey"].ToString();
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }

    }
}
