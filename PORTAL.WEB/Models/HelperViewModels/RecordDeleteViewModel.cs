using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.HelperViewModels
{
    public class RecordDeleteViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string DeleteMsg { get; set; }
        public List<RecordDetail> RecordDetail { get; set; } = new List<RecordDetail>();
    }

    public class RecordDetail
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
