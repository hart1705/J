using PORTAL.DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models
{
    public class AnnotationViewModel
    {
        public string Id { get; set; }
        public string Annotation_Id { get; set; }
        [StringLength(255)]
        public string ObjectName { get; set; }
        public string ObjectId { get; set; }
        public string Subject { get; set; }
        public bool IsDocument { get; set; }
        [Display(Name = "Note")]
        public string NoteText { get; set; }
        [StringLength(256)]
        public string MimeType { get; set; }
        public int? FileSize { get; set; }
        public string FileName { get; set; }
        [Display(Name = "Is Private")]
        public bool IsPrivate { get; set; }
        [Display(Name = "Created By")]
        public ApplicationUser CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public ApplicationUser ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
