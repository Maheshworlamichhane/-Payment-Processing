using Application.ITokenService;
using Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthController(UserManager<IdentityUser> userManager, ITokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = await _userManager.FindByNameAsync(request.Email);
            if (userExists != null)
            {
                return BadRequest(new { message = "User already exists." });
            }

            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Extract error descriptions to string list
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { errors });
            }

            return Ok(new { message = "User created successfully" });
        }
    


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var token = _tokenGenerator.GenerateToken(user);
            HttpContext.Session.SetString("UserId", user.Id);
            var userId = HttpContext.Session.GetString("UserId");

            return Ok(new { token });
            //return Ok(new
            //{
            //    token = token,
            //    userId = user.Id
            //});

        }

        return Unauthorized("Invalid credentials");
    }
}
