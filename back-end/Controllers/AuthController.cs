using back_end.Data;
using back_end.Models.Api;
using back_end.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        public AuthController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("users");
        }

        [HttpGet]
        public ActionResult<CurrentUserViewModel> VerifyAuth()
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Console.WriteLine(userId);

            var emailClaim = User.Claims.FirstOrDefault(i => i.Type == JwtRegisteredClaimNames.Name);
            if (emailClaim != null)
            {
                Console.WriteLine(emailClaim.Value);
                var filter = Builders<User>.Filter.Eq(x => x.Email, emailClaim.Value);
                var user = _users.Find(filter).FirstOrDefault();
                if (user is null)
                {
                    return NotFound();
                }
                var userView = new CurrentUserViewModel(user);
                return Ok(userView);
            }
            return BadRequest("User is not authed");
        }
    }
}
