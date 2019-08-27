using System;
using System.Collections.Generic;
using System.Text;

namespace PORTAL.DAL.EF.Helper
{
    public class Enums
    {
        public enum Status
        {
            Active = 0,
            Inactive = 1
        }
        public enum AccessType
        {
            User = 0,
            Organization = 1
        }

        public enum SMSStatus
        {
            Hold = 0,
            Queue = 1,
            Sent = 2,
            SendFailed = 3
        }

        public enum GSMStatus
        {
            Active = 0,
            Close = 1,
            DebugMode = 2
        }

        public enum PlanType
        {
            Daily = 0,
            Weekly = 1,
            Monthly = 2
        }

        public enum ReferralCodeStatus
        {
            Open = 0,
            Redeemed = 1,
            Expired = 2
        }

        public enum BPosition
        {
            Left = 0,
            Right = 1
        }
        public enum RegistrationType
        {
            Bayanihan = 0,
            Geneology = 1
        }
    }
}
