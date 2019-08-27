using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;
namespace PORTAL.WEB.Services
{
    public class Register : IRegister
    {
        private readonly ApplicationDbContext _context;
        public Register(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CheckSecurityCode(string pin, string securityCode)
        {
            //string securityCode = string.Empty;
            //securityCode = _context.ReferralCode.Where(a => a.SecutiryCode == "2D#JHCTCGAoR").Select(a => a.SecutiryCode);
            bool code = _context.ReferralCode.Where(a => a.SecutiryCode == securityCode && a.PINCode == pin && a.ReferralCodeStatus == Enums.ReferralCodeStatus.Open).Any();
            return code;
        }
        
        public void UpdateReferralCode(string securityCode, string id)
        {
            //var refCode = _context.ReferralCode.Where(a => a.SecutiryCode == securityCode).Select(a => a.Id);
            
            var dbRecord = _context.ReferralCode.SingleOrDefault(a => a.SecutiryCode == securityCode);
            dbRecord.UserId = id;
            dbRecord.ReferralCodeStatus = Enums.ReferralCodeStatus.Redeemed;

            _context.ReferralCode.Update(dbRecord);
            _context.SaveChanges();
        }

        public string RegisterBayanihan(Bayanihan bayanihan)
        {
            int increment = 0;
            int totalCount = _context.Bayanihan.Count();
            if(totalCount != 0)
            {
                increment = 1;
                totalCount = totalCount - 1;
            }

            bayanihan.Count = totalCount + increment;
            _context.Bayanihan.Add(bayanihan);
            _context.SaveChanges();
            return bayanihan.Id;
        }
    }
}
