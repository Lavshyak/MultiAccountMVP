using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiAccountMVP.Auth;
using MultiAccountMVP.Models;

namespace MultiAccountMVP;

[ApiController]
[Route("[controller]/[action]")]
public class TheControllerController : ControllerBase
{
    private readonly UserManager<DevAccount> _devUserManager;
    private readonly SignInManager<DevAccount> _devSignInManager;
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;

    public TheControllerController(UserManager<DevAccount> devUserManager, SignInManager<DevAccount> devSignInManager,
        UserManager<Account> userManager, SignInManager<Account> signInManager)
    {
        _devUserManager = devUserManager;
        _devSignInManager = devSignInManager;
        _devSignInManager.AuthenticationScheme = CustomAuthSchemes.CookieDevAccount;
        
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [NonAction]
    private bool IsDev(ClaimsPrincipal user)
    {
        return user.Claims.Any(claim =>
            claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod" &&
            claim.Value == CustomAuthSchemes.CookieDevAccount);
    }
    
    [Authorize]
    [HttpGet]
    public bool CheckAccount()
    {
        var user = User;
        return !IsDev(user);
    }
    
    [Authorize(CustomAuthPolicies.Account)]
    [HttpGet]
    public bool CheckAccountWithPolicy()
    {
        var user = User;
        return !IsDev(user);
    }

    [Authorize(AuthenticationSchemes = CustomAuthSchemes.CookieDevAccount)]
    [HttpGet]
    public bool CheckDevAccount()
    {
        var user = User;
        return IsDev(user);
    }
    
    [Authorize(CustomAuthPolicies.DevAccount)]
    [HttpGet]
    public bool CheckDevAccountWithPolicy()
    {
        var user = User;
        return IsDev(user);
    }
    
    public record RegisterParameters(string Email, string Password);

    [HttpPost]
    public async Task DevRegister()
    {
        var parameters = new RegisterParameters($"devAccount{Random.Shared.Next()%1000}@email.com", "password1234");
        
        var account = new DevAccount()
        {
            UserName = parameters.Email,
            Email = parameters.Email
        };
        var result = await _devUserManager.CreateAsync(account, parameters.Password);
        if (result.Succeeded)
        {
            await _devSignInManager.SignInAsync(account, false, CustomAuthSchemes.CookieDevAccount);
        }
    }
    
    [HttpPost]
    public async Task Register()
    {
        var parameters = new RegisterParameters($"account{Random.Shared.Next()%1000}@email.com", "password1234");
        
        var account = new Account()
        {
            UserName = parameters.Email,
            Email = parameters.Email
        };
        var result = await _userManager.CreateAsync(account, parameters.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(account, false);
        }
    }
}