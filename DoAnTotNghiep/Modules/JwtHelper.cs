using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DoAnTotNghiep.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DoAnTotNghiep.Modules
{
    public interface IJwtHelper
    {
        JwtSecurityToken GenerateToken(List<Claim> claims);
        JwtUserModel GetUserFromToken(string token);
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            string? signingKey = this._configuration.GetConnectionString(ConfigurationJWT.JwtBearerIssuerSigningKey());
            string? issuer = this._configuration.GetConnectionString(ConfigurationJWT.JwtBearerIssuer());
            string? audience = this._configuration.GetConnectionString(ConfigurationJWT.JwtBearerAudience());
            if (signingKey == null || issuer == null || audience == null)
            {
                throw new Exception("ConfigurationJWT Errors");
            }
            else
            {
                var token = GetJwtToken(claims, signingKey, issuer, audience);
                return token;
            }
        }

        private JwtSecurityToken GetJwtToken(List<Claim> claims, string signingKey, string issuer, string audience)
        {
            var key = Encoding.UTF8.GetBytes(signingKey);

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var expirationDate = DateTime.UtcNow.AddMinutes(15);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expirationDate,
                signingCredentials: credentials);
        }

        public JwtUserModel GetUserFromToken(string token)
        {
            var model = new JwtSecurityTokenHandler().ReadJwtToken(token);
            string claims = model.Claims.First(m => m.Type == JwtRegisteredClaimNames.Sub).Value;
            JwtUserModel? result = JsonSerializer.Deserialize<JwtUserModel>(claims);
            return result == null ? new JwtUserModel() : result;
        }
    }

    public class JwtUserModel
    {
        public string Email { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public string Role { get; set; } = DoAnTotNghiep.Enum.Role.MemberString();
    } 
}