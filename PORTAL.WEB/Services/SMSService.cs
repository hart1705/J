using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Models;

namespace PORTAL.WEB.Services
{
    public class SMSService : ISMSService
    {
        private readonly ApplicationDbContext _context;

        public SMSService(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool DeleteSMS(string id)
        {
            var record = RetrieveSMS(id);
            if(record == null)
            {
                return false;
            }
            _context.ShortMessageService.Remove(record);
            int affected = _context.SaveChanges();
            return affected > 0;
        }

        public string NewSMS(ShortMessageService sms, bool validateSMS = false)
        {
            sms.MobileNumber = FixMobileNumber(sms.MobileNumber);
            if (validateSMS)
            {
                if (sms.MobileNumber.Length != 11)
                {
                    throw new Exception("Invalid format of mobile number");
                }
                var hasUnicode = ContainsUnicodeCharacter(sms.MessageBody);
                if (hasUnicode)
                {
                    throw new Exception("Message body should not contain unicode characters (ex. emojis)");
                }
            }
            sms.ShortMessageService_Id = AutoNumber.GenerateSMSID(_context);
            _context.ShortMessageService.Add(sms);
            _context.SaveChanges();
            return sms.Id;
        }

        public ShortMessageService RetrieveSMS(string id)
        {
            var record = _context.ShortMessageService.SingleOrDefault(m => m.Id == id);
            return record;
        }

        public bool UpdateSMS(ShortMessageService sms)
        {
            _context.ShortMessageService.Update(sms);
            var affected = _context.SaveChanges();
            return affected > 0;
        }

        public bool UpdateStatus(string id, Enums.Status status)
        {
            var record = RetrieveSMS(id);
            if(record == null)
            {
                return false;
            }
            record.Status = status;
            _context.ShortMessageService.Update(record);
            var affected = _context.SaveChanges();
            return affected > 0;
        }

        private string FixMobileNumber(string mobileNumber)
        {
            mobileNumber = mobileNumber.Trim(' ');
            if (!string.IsNullOrWhiteSpace(mobileNumber))
            {
                if (mobileNumber[0] == '9' && mobileNumber.Length == 10)
                    mobileNumber = $"0{mobileNumber}";
                else if (mobileNumber.Length == 12 && mobileNumber[0] == '6' && mobileNumber[1] == '3')
                    mobileNumber = $"0{mobileNumber.Substring(2)}";
            }
            return mobileNumber;
        }

        private bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }
    }
}
