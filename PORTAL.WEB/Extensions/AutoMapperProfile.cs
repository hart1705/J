using AutoMapper;
using PORTAL.DAL.EF.Models;
using PORTAL.WEB.Models.ActionViewModels;
using PORTAL.WEB.Models.ApplicationRoleViewModels;
using PORTAL.WEB.Models.ApplicationUserViewModels;
using PORTAL.WEB.Models.GSMModemViewModels;
using PORTAL.WEB.Models.PermissionViewModels;
using PORTAL.WEB.Models.ReferralCodeViewModels;
using PORTAL.WEB.Models.SMSViewModels;

namespace PORTAL.WEB.Extensions
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationPermission, Permission>().ReverseMap();
            CreateMap<ApplicationAction, ApplicationActionModel>().ReverseMap();
            CreateMap<ApplicationRole, ApplicationRoleModel>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserModel>().ReverseMap();
            CreateMap<ShortMessageService, SMSModel>().ReverseMap();
            CreateMap<GSMModem, ModemModel>().ReverseMap();
            CreateMap<ReferralCode, ReferralCodeModel>().ReverseMap();
        }
    }
}
