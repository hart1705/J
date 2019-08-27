using Microsoft.AspNetCore.Http;
using PORTAL.WEB.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.SMSViewModels
{
    public class SMSImportModel
    {
        public string FileName { get; set; }
        public IFormFile FileAttachment { get; set; }
        public FormBehavior FormBehavior { get; set; }
        public List<SMSImportList> Records { get; set; } = new List<SMSImportList>();
    }
}
