using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LoginAndRegister.Data;
using LoginAndRegister.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginAndRegister.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(DataContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registration)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registration.Email))
            {
                return BadRequest("Email already exists.");
            }

            var user = new User
            {
                Name = registration.Name,
                NickName = registration.NickName,
                Email = registration.Email,
                PasswordHash = _passwordHasher.HashPassword(null, registration.Password),
                Status = true,
                ShortDescription = registration.ShortDescription,
                LongDescription = registration.LongDescription
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { UserId = user.UserId, Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == login.Email);

            if (user == null || _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, login.Password) == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid login attempt.");
            }

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }




        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                // ניתן להוסיף עוד טענות כאן
            }),
                Expires = DateTime.Now.AddHours(2), // זמן תוקף הטוקן
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
