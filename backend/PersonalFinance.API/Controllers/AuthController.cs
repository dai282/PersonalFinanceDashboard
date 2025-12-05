//not needed now using Auth0

// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using PersonalFinance.API.DTOs;
// using PersonalFinance.API.Services;
// using PersonalFinance.Core.Entities;
// using PersonalFinance.Infrastructure.Data;

// namespace PersonalFinance.API.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class AuthController : ControllerBase
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly AuthService _authService;

//         public AuthController(ApplicationDbContext context, AuthService authService)
//         {
//             _context = context;
//             _authService = authService;
//         }

//         [HttpPost("register")]
//         public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
//         {
//             // Check if user exists
//             if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
//             {
//                 return BadRequest("User already exists");
//             }

//             // Create new user
//             var user = new User
//             {
//                 Email = dto.Email,
//                 PasswordHash = _authService.HashPassword(dto.Password),
//                 FirstName = dto.FirstName,
//                 LastName = dto.LastName,
//                 CreatedAt = DateTime.UtcNow
//             };

//             _context.Users.Add(user);
//             await _context.SaveChangesAsync();

//             // Generate token
//             var token = _authService.GenerateJwtToken(user);

//             return Ok(new AuthResponseDto
//             {
//                 Token = token,
//                 Email = user.Email,
//                 FirstName = user.FirstName,
//                 LastName = user.LastName
//             });
//         }

//         [HttpPost("login")]
//         public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
//         {
//             // Find user
//             var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
//             if (user == null)
//             {
//                 return Unauthorized("Invalid credentials");
//             }

//             // Verify password
//             if (!_authService.VerifyPassword(dto.Password, user.PasswordHash))
//             {
//                 return Unauthorized("Invalid credentials");
//             }

//             // Generate token
//             var token = _authService.GenerateJwtToken(user);

//             return Ok(new AuthResponseDto
//             {
//                 Token = token,
//                 Email = user.Email,
//                 FirstName = user.FirstName,
//                 LastName = user.LastName
//             });
//         }
//     }
// }