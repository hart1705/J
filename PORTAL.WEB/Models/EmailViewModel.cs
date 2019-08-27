using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models
{
    public class EmailViewModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
