using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Skii.Models;
using Skii.Services;

namespace Skii.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController(IJwtService jwtService,IAuthService authService) :ControllerBase
{
    [HttpGet("signin-google")]
    public Task<IActionResult> GoogleSignin()
    {
        var redirectUrl = Url.Action("GoogleCallback", "Auth");
        var prop = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Task.FromResult<IActionResult>(Challenge(prop, GoogleDefaults.AuthenticationScheme));
    }
    
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var res = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!res.Succeeded)
            return BadRequest();
        var claims = res.Principal.Claims.ToArray();
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (googleId is null)
            return StatusCode(StatusCodes.Status500InternalServerError);
        var user = new User
        {
            GoogleId = googleId,
            Name = name ?? string.Empty,
            Email = email ?? string.Empty
        };
        var token = jwtService.GenerateJwtToken(user.Email,user.GoogleId,user.Id);
       
        await authService.AddUser(user);
        return Ok(new { token, email, name });
    }

    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        if (await authService.GetUsers() is { } users)
            return Ok(users);
        return NotFound();
    }
}