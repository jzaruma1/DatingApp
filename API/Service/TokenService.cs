using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text;
using API.Entities;
using API.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace API.Service
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser appUser)
        {
           var claims = new List<Claim>
           {
               new Claim(JwtRegisteredClaimNames.NameId, appUser.UserName)
           };

           var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

           var tokenDescriptor = new SecurityTokenDescriptor
           {
               Subject = new ClaimsIdentity(claims),
               Expires = DateTime.Now.AddDays(7),
               SigningCredentials = creds
           };

           var tokenHandlers = new JwtSecurityTokenHandler();

           var token = tokenHandlers.CreateToken(tokenDescriptor);

           return tokenHandlers.WriteToken(token);
        }
    }
}