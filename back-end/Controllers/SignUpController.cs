using back_end.Data;
using back_end.General;
using back_end.Models.Api;
using back_end.Models.Entities;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Security.Claims;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User>? _users;
        private readonly JwtService _jwtService;

        public SignUpController(IConfiguration configuration, MongoDbService mongoDbService, JwtService jwtService)
        {
            _configuration = configuration;
            _users = mongoDbService.Database?.GetCollection<User>("users");
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResponseModel?>> SignUp(SignUpModel signUpModel)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, signUpModel.Email);
            var existingUser = _users.Find(filter).FirstOrDefault();
            if (existingUser is not null)
            {
                return BadRequest("Provided email is already in use");
            }
            if (signUpModel.Age < 18)
            {
                return BadRequest("You must be at least 18 to create an account");
            }
            if (signUpModel.Password.Length < 6)
            {
                return BadRequest("Password length must be at least 6 characters");
            }

            var user = new User()
            {
                Name = signUpModel.Name,
                Email = signUpModel.Email,
                Password = PasswordHasherTool.GenerateHash(signUpModel.Password),
                Age = signUpModel.Age,
                Gender = signUpModel.Gender,
                GenderPreferences = signUpModel.GenderPreferences
            };

            // TODO, also some jwt or something so the site doesn't lose it's state on refresh
            await _users.InsertOneAsync(user);

            var tokenResponse = await _jwtService.GenerateTokenFromEmail(signUpModel.Email);


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

            return Ok(response);
        }
    }
}
