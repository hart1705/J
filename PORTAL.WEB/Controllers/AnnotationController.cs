using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Models;

namespace PORTAL.WEB.Controllers
{
    public class AnnotationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnnotationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Form(string entityName, string recordId, string id = "")
        {
            Annotation note = new Annotation();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var tempNote = _context.Annotation.SingleOrDefault(n => n.Id == id);
                if (tempNote != null)
                {
                    note = tempNote;
                }
            }
            else if (!string.IsNullOrWhiteSpace(entityName) && !string.IsNullOrWhiteSpace(recordId))
            {
                note.ObjectName = entityName;
                note.ObjectId = recordId;
            }
            else
            {
                throw new Exception("Invalid action");
            }
            return PartialView("_AnnotationForm", note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Form(Annotation model)
        {
            Annotation note = new Annotation();
            if (model.Attachment != null)
            {
                if (model.Attachment.Length > 5242880)
                {
                    model.FileName = string.Empty;
                    ModelState.AddModelError(string.Empty, "File attachment too large. It should be less than 4mb.");
                }
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    note = _context.Annotation.Find(model.Id);
                }
                note.IsPrivate = model.IsPrivate;
                note.NoteText = model.NoteText;
                note.Subject = model.Subject;
                note.Annotation_Id = model.Subject;
                if (model.Attachment != null)
                {
                    string fileData = null;
                    using (var ms = new MemoryStream())
                    {
                        model.Attachment.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        fileData = Convert.ToBase64String(fileBytes);
                    }

                    note.IsDocument = true;
                    note.FileSize = (int)model.Attachment.Length;
                    note.FileName = model.Attachment.FileName;
                    note.DocumentBody = fileData;
                    note.MimeType = model.Attachment.ContentType;
                }
                else if (model.HasAttachment.HasValue && model.HasAttachment.Value)
                {
                    note.IsDocument = false;
                    note.FileSize = null;
                    note.FileName = null;
                    note.DocumentBody = null;
                    note.MimeType = null;
                }

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    _context.Update(note);
                    _context.SaveChanges();
                }
                else
                {
                    note.ObjectName = model.ObjectName;
                    note.ObjectId = model.ObjectId;
                    var result = _context.Add(note);
                    var affected = _context.SaveChanges();
                    if (affected > 0)
                    {
                        model.Id = result.Entity.Id;
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    return PartialView("_BootstrapModalAction", AnnotationModalAction(model.ObjectName, model.ObjectId));
                }
            }
            model.HasAttachment = false;
            return PartialView("_AnnotationForm", model);
        }

        public IActionResult LoadViewComponent(string entityName, string recordId)
        {
            return ViewComponent("Annotation", new { entityName, recordId });
        }

        private BootstrapModalModel AnnotationModalAction(string entityName, string recordId)
        {
            return new BootstrapModalModel
            {
                ModalID = $"#annotation-modal",
                ShouldClose = true,
                FetchData = true,
                Destination = Url.Action("LoadViewComponent", new
                {
                    entityName,
                    recordId
                }),
                Target = $"#annotation-list-container",
                OnSuccess = $"Annotation.ToSlimScroll"
            };
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Action");
            }

            var annotation = await _context.Annotation.Select(n=>new Annotation {Id = n.Id, FileName = n.FileName, Subject = n.Subject, NoteText = n.NoteText })
                .SingleOrDefaultAsync(m => m.Id == id);
            if (annotation == null)
            {
                ModelState.AddModelError(string.Empty, "Record not found.");
            }

            return PartialView("_AnnotationDelete", annotation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Annotation note = null;
            try
            {
                note = await _context.Annotation.Select(n => new Annotation { Id = n.Id, ObjectName = n.ObjectName, ObjectId = n.ObjectId }).SingleOrDefaultAsync(m => m.Id == id);
                _context.Annotation.Remove(note);
                await _context.SaveChangesAsync();
                return PartialView("_BootstrapModalAction", AnnotationModalAction(note.ObjectName, note.ObjectId));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting record: {ex.Message}.");
            }
            return PartialView("_DARDelete", note);
        }

        public FileResult Download(string id)
        {
            var note = _context.Annotation.SingleOrDefault(n => n.Id == id);
            if (note != null && note.IsDocument)
            {
                return File(Convert.FromBase64String(note.DocumentBody), note.MimeType, note.FileName);
            }
            return null;
        }
    }
}
