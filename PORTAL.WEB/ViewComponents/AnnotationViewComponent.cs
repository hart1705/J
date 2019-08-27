using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Models;
using PORTAL.WEB.UserControls.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
namespace PORTAL.WEB.ViewComponents
{
    public class AnnotationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnnotationViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string entityName, string recordId)
        {
            ApplicationUser curUser = await Helpers.Common.GetCurrentUser(_userManager, HttpContext.User);
            curUser = curUser == null ? new ApplicationUser() : curUser;
            var annotations = await _context.Annotation.Select(n => new Annotation
            {
                Annotation_Id = n.Annotation_Id,
                ObjectName = n.ObjectName,
                ObjectId = n.ObjectId,
                CreatedBy = n.CreatedBy,
                CreatedOn = n.CreatedOn,
                FileName = n.FileName,
                FileSize = n.FileSize,
                Id = n.Id,
                IsDocument = n.IsDocument,
                IsPrivate = n.IsPrivate,
                MimeType = n.MimeType,
                ModifiedBy = n.ModifiedBy,
                ModifiedOn = n.ModifiedOn,
                NoteText = n.NoteText,
                Subject = n.Subject
            }).Where(n => n.ObjectName == entityName && n.ObjectId == recordId &&
            (!n.IsPrivate || (n.IsPrivate && n.CreatedBy == curUser.Id))).
            OrderByDescending(n => n.CreatedOn).ToListAsync();

            List<AnnotationViewModel> notesModel = new List<AnnotationViewModel>();
            foreach (var note in annotations)
            {
                notesModel.Add(new AnnotationViewModel
                {
                    Annotation_Id = note.Annotation_Id,
                    ObjectName = note.ObjectName,
                    ObjectId = note.ObjectId,
                    CreatedBy = Helpers.Common.ResolveUser(_context, note.CreatedBy),
                    CreatedOn = note.CreatedOn,
                    FileName = note.FileName,
                    FileSize = note.FileSize,
                    Id = note.Id,
                    IsDocument = note.IsDocument,
                    IsPrivate = note.IsPrivate,
                    MimeType = note.MimeType,
                    ModifiedBy = Helpers.Common.ResolveUser(_context, note.CreatedBy),
                    ModifiedOn = note.ModifiedOn,
                    NoteText = note.NoteText,
                    Subject = note.Subject
                });
            }

            AnnotationModel notes = new AnnotationModel
            {
                EntityName = entityName,
                RecordId = recordId,
                CurrentUserId = curUser.Id,
                Notes = notesModel
            };
            return View(notes);
        }
    }
}
