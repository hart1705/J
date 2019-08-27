using PORTAL.DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Services
{
    public interface IRegister
    {
        bool CheckSecurityCode(string pin, string securityCode);
        void UpdateReferralCode(string securityCode, string id);
        string RegisterBayanihan(Bayanihan bayanihan);
    }
}
