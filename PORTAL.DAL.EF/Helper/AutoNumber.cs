using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PORTAL.DAL.EF.Helper
{
    public class AutoNumber
    {
        public static string GenerateSMSID(ApplicationDbContext context)
        {
            var smsID = string.Empty;
            const int length = 4;
            var now = DateTime.Now;
            do
            {
                smsID = $"SMS{now.Month.ToString("00")}{now.Day.ToString("00")}{now.Year.ToString()}-";
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var random = new Random();
                smsID += new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            while (context.ShortMessageService.Where(m => m.ShortMessageService_Id == smsID).Any());
            return smsID;
        }
    }
}
