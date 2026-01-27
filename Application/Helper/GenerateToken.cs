using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HotelDemo.Helper
{
    public class GenerateToken
    {
        private readonly JwtSettings jwtSettings;

        public GenerateToken(JwtSettings jwtSettings)
        {
            this.jwtSettings = jwtSettings;
        }
        public string GenerateJwtToken(string userId , string email , List<string> Roles )
        {
            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
           var creds = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256);

            var Claims = new List<Claim>    
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
            };

            foreach( string role in Roles )
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
