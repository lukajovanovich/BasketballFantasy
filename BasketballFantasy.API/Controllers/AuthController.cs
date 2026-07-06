using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.Auth;
using BasketballFantasy.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [AllowAnonymous]
    [Route("auth")]
    public class AuthController : ApiControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            TokenService tokenService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var validationResult = _registerValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (await _userManager.FindByNameAsync(request.UserName) != null)
            {
                return BadRequest("Korisnicko ime je vec zauzeto.");
            }

            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return BadRequest("Korisnik sa tim email-om vec postoji.");
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors.Select(error => error.Description));
            }

            await _userManager.AddToRoleAsync(user, ApplicationRoles.User);

            var token = _tokenService.CreateToken(user, new[] { ApplicationRoles.User });
            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName ?? string.Empty,
                Role = ApplicationRoles.User
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var validationResult = _loginValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized("Pogresno korisnicko ime ili lozinka.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            });
        }
    }
}
