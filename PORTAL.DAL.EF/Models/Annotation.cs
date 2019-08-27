using Microsoft.AspNetCore.Http;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PORTAL.DAL.EF.Models
{
    public class Annotation : IAuditable
    {
        public string Id { get; set; }
        public string Annotation_Id { get; set; }
        public string ObjectName { get; set; }
        public string ObjectId { get; set; }
        [StringLength(500)]
        [Required]
        public string Subject { get; set; }
        public bool IsDocument { get; set; }
        [Display(Name = "Note")]
        public string NoteText { get; set; }
        [StringLength(256)]
        public string MimeType { get; set; }
        public string DocumentBody { get; set; }
        public int? FileSize { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [Display(Name = "Is Private?")]
        public bool IsPrivate { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [NotMapped]
        //[FileExtensions(Extensions = "jpg,png,jpeg,bmp,svg")]
        public IFormFile Attachment { get; set; }
        [NotMapped]
        public bool? HasAttachment { get; set; }
        public Enums.Status Status { get; set; }
    }
}