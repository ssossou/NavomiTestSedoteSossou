using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NavomiApi.Models;
using NavomiApi.Interfaces;
using NavomiApi.Enumerators;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace NavomiApi.Services
{
    public class LoginService : ILoginService
    {
        IConfiguration _config;
        ILogService _logger;
        IHostingEnvironment _environment;

        public LoginService(IConfiguration config, ILogService logger, IHostingEnvironment environment)
        {
            _config = config;
            _logger = logger;
            _environment = environment;
        }

     
        public JwtToken BuildJwtToken(string accessToken, DateTime expires, int timezoneOffset, string serverCode)
        {
            var expiresForToken = expires;
            //only evaluation the TokenReductionMinutes setting in non-production environments
            if (!_environment.IsProduction())
            {
                var tokenReductionMinutesSetting = _config["Testing:TokenReductionMinutes"];
                if (!string.IsNullOrEmpty(tokenReductionMinutesSetting))
                {
                    int tokenReduction = 0;

                    if (int.TryParse(tokenReductionMinutesSetting, out tokenReduction) && tokenReduction > 0)
                    {
                        _logger.LogInformation($"Testing:TokenReductionMinutes has been set. The token duration will be limited by {tokenReduction} minutes.");
                        expiresForToken = expiresForToken.AddMinutes(-tokenReduction);
                    }
                }
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,accessToken),
               
                new Claim(ClaimKeys.ACCESS_TOKEN.ToString(), accessToken),
                new Claim(ClaimKeys.SERVERTIMEZONEOFFSET.ToString(), timezoneOffset.ToString()),
                new Claim(ClaimKeys.SERVER_CODE.ToString(), serverCode),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimKeys.VALID_TO.ToString(), expiresForToken.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: expiresForToken,
              signingCredentials: creds);

            return new JwtToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo
            };
        }

        public JwtToken BuildJwtToken(string tokenString)
        {
            var token = DecodeTokenString(tokenString);

            return new JwtToken()
            {
                Token = tokenString,
                ValidTo = DateTime.Parse(GetClaimValue(ClaimKeys.VALID_TO, token.Claims))
            };
        }

        public Object SpliceJwtTokenIntoTarget(JwtToken token, object target)
        {
            // Create generic JSON objects from the refreshed JwtToken and the JSON response from the caller
            var jwtJson = JObject.FromObject(token);
            var resultJson = JObject.FromObject(target);

            // Splice the refreshed JwtToken into the JSON response
            resultJson["jwtToken"] = jwtJson;

            return resultJson;
        }

        /// <summary>
        /// Returns a claim value from the raw JwtToken using the ClaimsKey enumerator
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jwtToken"></param>
        /// <returns>String value of claim for a given key</returns>
        public string GetClaimValue(ClaimKeys key, string jwtToken)
        {
            var claims = this.DecodeTokenString(jwtToken).Claims;
            var claim = this.GetClaimValue(key, claims);

            return claim;
        }

        /// <summary>
        /// Returns a claim value from the claims parameter using the ClaimsKey enumerator
        /// </summary>
        /// <param name="key"></param>
        /// <param name="claims"></param>
        /// <returns>String value of claim for given key</returns>
        public string GetClaimValue(ClaimKeys key, IEnumerable<Claim> claims)
        {
            var claim = claims.Where(c => c.Type == key.ToString()).FirstOrDefault();
            return claim?.Value;
        }

        /// <summary>
        /// Instantiates a JwtSecurityToken object from the jwtToken string parameter
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns>JwtSecurityToken object</returns>
        public JwtSecurityToken DecodeTokenString(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(jwtToken) as JwtSecurityToken;
            return token;
        }
    }
}
