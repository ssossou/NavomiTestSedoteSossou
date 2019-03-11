using Microsoft.AspNetCore.Mvc;
using NavomiApi.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NavomiApi.Controllers
{
    public class ControllerBase : Controller
    {
        protected string GetClaimValue(ClaimKeys key)
        {
            if (((ClaimsIdentity)HttpContext.User.Identity).Claims.Count() > 0)
            {
                return ((ClaimsIdentity)HttpContext.User.Identity).FindFirst(key.ToString()).Value;
            }
            else
            {
                return null;
            }
        }

    }
}
