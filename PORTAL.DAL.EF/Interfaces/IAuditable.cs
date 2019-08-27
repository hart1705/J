using PORTAL.DAL.EF.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace PORTAL.DAL.EF.Interfaces
{
    public interface IAuditable
    {
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        DateTime? ModifiedOn { get; set; }
        Enums.Status Status { get; set; }
    }
}
