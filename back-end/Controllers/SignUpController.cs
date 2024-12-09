using back_end.Data;
using back_end.General;
using back_end.Models.Api;
using back_end.Models.Entities;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        private readonly JwtService _jwtService;

        public SignUpController(MongoDbService mongoDbService, JwtService jwtService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("users");
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResponseModel?>> SignUp(string name, string email, string password, int age, Gender gender, List<Gender> genderPreferences)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            var existingUser = _users.Find(filter).FirstOrDefault();
            if (existingUser is not null)
            {
                return BadRequest("Provided email is already in use");
            }
            if (age < 18)
            {
                return BadRequest("You must be at least 18 to create an account");
            }
            if (password.Length < 6)
            {
                return BadRequest("Password length must be at least 6 characters");
            }

            var user = new User()
            {
                Name = name,
                Email = email,
                Password = PasswordHasherTool.GenerateHash(password),
                Age = age,
                Gender = gender,
                GenderPreferences = genderPreferences
            };

            // TODO, also some jwt or something so the site doesn't lose it's state on refresh
            await _users.InsertOneAsync(user);

            var tokenResponse = await _jwtService.GenerateTokenFromEmail(email);

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
