using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OcrSystemApi.Models;
using OcrSystemApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace OcrSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Tạo token xác nhận email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = HttpUtility.UrlEncode(token); // Mã hóa token để an toàn trong URL

                    // Tạo liên kết xác nhận
                    var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = encodedToken }, Request.Scheme);
                    var emailBody = $"<h2>Welcome to OcrSystem!</h2><p>Please confirm your email by clicking the link below:</p><p><a href='{confirmationLink}'>Confirm Email</a></p>";

                    // Gửi email
                    await _emailService.SendEmailAsync(user.Email, "Confirm Your Email - OcrSystem", emailBody);

                    return Ok(new { Message = "User registered successfully. Please check your email to confirm your account." });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting to login with username: {model.Username}");
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    _logger.LogWarning($"User '{model.Username}' not found.");
                    return Unauthorized("Invalid username or password");
                }

                _logger.LogInformation($"User '{model.Username}' found. Checking password...");
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordValid)
                {
                    _logger.LogWarning($"Invalid password for user '{model.Username}'.");
                    return Unauthorized("Invalid username or password");
                }

                _logger.LogInformation($"Password valid for user '{model.Username}'. Generating token...");
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for user {model.Username}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }