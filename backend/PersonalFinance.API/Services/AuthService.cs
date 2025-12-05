//not needed now using Auth0




// using Microsoft.IdentityModel.Tokens;
// using PersonalFinance.Core.Entities;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Security.Cryptography;
// using System.Text;

// namespace PersonalFinance.API.Services
// {
//     public class AuthService
//     {
//         private readonly IConfiguration _configuration;

//         public AuthService(IConfiguration configuration)
//         {
//             _configuration = configuration;
//         }

//         public string HashPassword(string password)
//         {
//             return BCrypt.Net.BCrypt.HashPassword(password);
//         }

//         public bool VerifyPassword(string password, string hash)
//         {
//             return BCrypt.Net.BCrypt.Verify(password, hash);
//         }

//         public string GenerateJwtToken(User user)
//         {
//             var jwtSettings = _configuration.GetSection("JwtSettings");
//             var secret = jwtSettings["Secret"]!;
//             var issuer = jwtSettings["Issuer"]!;
//             var audience = jwtSettings["Audience"]!;
//             var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]!);

//             var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
//             var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//             var claims = new[]
//             {
//                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
//                 new Claim(JwtRegisteredClaimNames.Email, user.Email),
//                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//             };

//             var token = new JwtSecurityToken(
//                 issuer: issuer,
//                 audience: audience,
//                 claims: claims,
//                 expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
//                 signingCredentials: credentials
//             );

//             return new JwtSecurityTokenHandler().WriteToken(token);
//         }
//     }
// }