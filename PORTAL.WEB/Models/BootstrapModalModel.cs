using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Models
{
    public class BootstrapModalModel
    {
        public string ModalID { get; set; }
        public bool ShouldClose { get; set; }
        public bool FetchData { get; set; }
        public string Destination { get; set; }
        public string Target { get; set; }
        public string OnSuccess { get; set; }
    }
}
