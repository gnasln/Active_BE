//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using OpenIddict.Server.AspNetCore;
//using Bff.Infrastructure.Data;
//using Bff.Identity;

//namespace Bff.Endpoints;

//public class TenantController(UserManager<ApplicationUser> userManager) : Controller
//{
//    private readonly UserManager<ApplicationUser> _userManager = userManager;

//    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
//    [Authorize(Policy = "admin")]
//    [HttpPost("~/tenant/newuser"), Produces("application/json")]
//    public async Task<IActionResult> CreateNewUser(NewUser newUser)
//    {
//        if (!newUser.Password.Equals(newUser.Repassword)) return BadRequest("500|Password and Repassword not same");
//        if (_userManager.FindByNameAsync(newUser.UserName) is not null) return BadRequest("501|User existed");
//        var user = new ApplicationUser { UserName = newUser.UserName, Email = $"{newUser.UserName}@localhost" };
//        await _userManager.CreateAsync(user, newUser.Password);
//        return Ok("200|User created");
//    }
//}
