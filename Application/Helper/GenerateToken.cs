using Application.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelDemo.Helper
{
    public class GenerateToken
    {
        private readonly JwtSettings jwtSettings;

        public GenerateToken(IOptions<JwtSettings> jwtoptions)
        {
            this.jwtSettings = jwtoptions.Value;
        }
        public string GenerateJwtToken(string userId, string email, List<string> Roles)
        {
            var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
            };

            foreach (string role in Roles)
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(

                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                expires: DateTime.Now.AddMinutes(jwtSettings.DurationInMinutes),
                claims: Claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
