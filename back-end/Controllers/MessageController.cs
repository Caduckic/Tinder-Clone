using back_end.Data;
using back_end.Models.Entities;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMongoCollection<Message>? _messages;
        private readonly IMongoCollection<User>? _users;
        private readonly JwtService _jwtService;

        public MessageController(MongoDbService mongoDbService, JwtService jwtService)
        {
            _messages = mongoDbService.Database?.GetCollection<Message>("messages");
            _users = mongoDbService.Database?.GetCollection<User>("users");
            _jwtService = jwtService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetConversation(string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = "";
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "name")
                    {
                        userEmail = claim.Value;
                    }
                }
            }
            if (userEmail == string.Empty)
            {
                return BadRequest("Could not verify current user token");
            }

            var filter = Builders<User>.Filter.Eq(x => x.Email, userEmail);
            var user = _users.Find(filter).FirstOrDefault();
            if (user is null)
            {
                return NotFound("Could not verify current user");
            }

            var receiverFilter = Builders<User>.Filter.Eq(x => x.Id, id);
            var receiverUser = _users.Find(receiverFilter).FirstOrDefault();
            if (receiverUser is null)
            {
                return NotFound("Could not find messaged user");
            }

            var messageFilter = Builders<Message>.Filter.Or(
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(x => x.SenderID, user.Id),
                    Builders<Message>.Filter.Eq(x => x.ReceiverId, receiverUser.Id)
                ),
                 Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(x => x.SenderID, receiverUser.Id),
                    Builders<Message>.Filter.Eq(x => x.ReceiverId, user.Id)
                )
            );

            var conversation = await _messages.Find(messageFilter).ToListAsync();
            var conversationSorted = conversation.OrderBy(x => x.CreatedAt);
            return Ok(conversationSorted);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(string content, string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = "";
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "name")
                    {
                        userEmail = claim.Value;
                    }
                }
            }
            if (userEmail == string.Empty)
            {
                return BadRequest("Could not verify current user token");
            }

            var filter = Builders<User>.Filter.Eq(x => x.Email, userEmail);
            var user = _users.Find(filter).FirstOrDefault();
            if (user is null)
            {
                return NotFound("Could not verify current user");
            }

            var receiverFilter = Builders<User>.Filter.Eq(x => x.Id, id);
            var receiverUser = _users.Find(receiverFilter).FirstOrDefault();
            if (receiverUser is null)
            {
                return NotFound("Could not find messaged user");
            }

            var message = new Message()
            {
                SenderID = user.Id,
                ReceiverId = receiverUser.Id,
                Content = content
            };

            await _messages.InsertOneAsync(message);
            return Ok(message);
        }
    }
}
