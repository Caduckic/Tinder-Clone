using back_end.Data;
using back_end.General;
using back_end.Models.Api;
using back_end.Models.Entities;
using back_end.Services;
using DnsClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Security.Claims;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User>? _users;
        private readonly JwtService _jwtService;

        public LoginController(IConfiguration configuration, MongoDbService mongoDbService, JwtService jwtService)
        {
            _configuration = configuration;
            _users = mongoDbService.Database?.GetCollection<User>("users");
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResponseModel?>> Login(LoginRequestModel request)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, request.Email);
            var user = _users.Find(filter).FirstOrDefault();
            if (user is null || !PasswordHasherTool.Verify(request.Password, user.Password))
            {
                return BadRequest("Wrong email or passowrd");
            }

            var tokenResponse = await _jwtService.GenerateTokenFromEmail(request.Email);

            var tokenValidiyMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidiyMins);
            HttpContext.Response.Cookies.Append("jwt", tokenResponse.AccessToken,
                new CookieOptions
                {
                    Expires = tokenExpiryTimeStamp,
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });

            var response = new LoginResponseModel()
            {
                Name = user.Name,
                Email = user.Email,
                AccessToken = tokenResponse.AccessToken,
                ExpiresIn = tokenResponse.ExpiresIn,
            };

            // TODO, also some jwt or something so the site doesn't lose it's state on refresh
            return Ok(response);
        }
    }
}
