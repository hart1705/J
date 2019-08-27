using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Services
{
    public interface ICodeGenerator
    {
        string GeneratePINCode();
        string GenerateSecurityCode();
    }
}
