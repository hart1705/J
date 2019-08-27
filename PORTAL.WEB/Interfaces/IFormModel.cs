using PORTAL.WEB.Helpers;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Interfaces
{
    public interface IFormModel
    {
        IUserHandler UserHandler { get; set; }
        Global.FormType FormType { get; set; }
        FormBehavior FormBehavior { get; set; }
        bool IsRecordOwner { get; set; }
    }

    public class FormBehavior
    {
        public PageRedirect PageRedirect { get; set; }
        public Notification Notification { get; set; }
    }

    public class PageRedirect
    {
        public bool Reload { get; set; }
        public string URL { get; set; }
    }

    public class Notification
    {
        public bool IsError { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
