using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Services
{
    public interface ISMSService
    {
        ShortMessageService RetrieveSMS(string id);
        string NewSMS(ShortMessageService sms, bool validateSMS = false);
        bool UpdateSMS(ShortMessageService sms);
        bool DeleteSMS(string id);
        bool UpdateStatus(string id, Enums.Status status);
    }
}
