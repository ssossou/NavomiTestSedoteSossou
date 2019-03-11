using NavomiApi.Enumerators;
using NavomiApi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System;


namespace NavomiApi.Interfaces
{
    public interface ILoginService
    {
        
  
        Object SpliceJwtTokenIntoTarget(JwtToken token, Object target);
        string GetClaimValue(ClaimKeys key, string jwtToken);
        string GetClaimValue(ClaimKeys key, IEnumerable<Claim> claims);
        JwtSecurityToken DecodeTokenString(string jwtToken);
    }
}
