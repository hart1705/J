using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PORTAL.WEB.Models;

namespace PORTAL.WEB.UserControls.Annotation
{
    public class AnnotationModel
    {
        public string EntityName { get; set; }
        public string RecordId { get; set; }
        public string CurrentUserId { get; set; }
        public IEnumerable<AnnotationViewModel> Notes { get; set; }
    }
}
