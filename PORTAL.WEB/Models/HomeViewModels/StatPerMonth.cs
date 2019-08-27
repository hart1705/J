using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models.HomeViewModels
{
    public class Dataset
    {
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public List<int> data { get; set; }
    }

    public class StatPerMonth
    {
        public List<string> Labels { get; set; }
        public List<Dataset> datasets { get; set; }
    }

}
