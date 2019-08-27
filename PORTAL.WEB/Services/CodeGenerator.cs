using PORTAL.DAL.EF;
using PORTAL.WEB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PORTAL.WEB.Services
{
    public class CodeGenerator : ICodeGenerator
    {
        private readonly ApplicationDbContext _context;
        public CodeGenerator(ApplicationDbContext context)
        {
            _context = context;
        }
        public string GeneratePINCode()
        {
            string pin = string.Empty;
            using (var crypto = new RNGCryptoServiceProvider())
            {
                while (pin == string.Empty)
                {
                    byte[] buffer = new byte[sizeof(UInt64)];
                    crypto.GetBytes(buffer);
                    var num = BitConverter.ToUInt64(buffer, 0);
                    var tempPin = num % 100000;
                    pin = tempPin.ToString("D5");
                    pin = _context.ReferralCode.Where(p => p.PINCode == pin).Any() ? string.Empty : pin;
                }
            }
            return pin;
        }

        public string GenerateSecurityCode()
        {
            string securityCode = string.Empty;
            while (securityCode == string.Empty)
            {
                securityCode = PasswordGenerator.GeneratePassword(true, true, true, false, false, 12);
                securityCode = _context.ReferralCode.Where(s => s.SecutiryCode == securityCode).Any() ? string.Empty : securityCode;
            }
            return securityCode;
        }
    }
}
