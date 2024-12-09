using back_end.Data;
using back_end.General;
using back_end.Models.Api;
using back_end.Models.Entities;
using back_end.Services;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;
using MongoDB.Driver.Linq;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        private readonly CloudinaryService _cloudinaryService;
        private readonly JwtService _jwtService;

        public UserController(MongoDbService mongoDbService, CloudinaryService cloudinaryService, JwtService jwtService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("users");
            _cloudinaryService = cloudinaryService;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> GetAll()
        {
            var users = await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
            var userViews = users.Select<User, UserViewModel>(u => new UserViewModel(u)).ToList();

            return userViews;
        }

        [HttpGet("new_users")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetAllNewUsers()
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

            var newUserFilter = Builders<User>.Filter.And(
                Builders<User>.Filter.Nin(x => x.Id, user.Likes.Select(u => u.ToString())),
                Builders<User>.Filter.Nin(x => x.Id, user.Dislikes.Select(u => u.ToString())),
                Builders<User>.Filter.Nin(x => x.Id, user.Matches.Select(u => u.ToString())),
                Builders<User>.Filter.Ne(x => x.Id, user.Id),
                Builders<User>.Filter.In(x => x.Gender, user.GenderPreferences)
            );
            //var newUserFilter = Builders<User>.Filter.Nin(x => x.Id, oldAndUnpreferredUsers.Select(u => u.ToString()));
            var newUsers = await _users.Find(newUserFilter).ToListAsync();
            var newUserViews = newUsers.Select(u => new UserViewModel(u));
            return Ok(newUserViews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel?>> GetById(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(x => x.Id, id);
                var user = _users.Find(filter).FirstOrDefault();
                if (user is null)
                {
                    return NotFound();
                }
                var userView = new UserViewModel(user);
                return Ok(userView);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpGet("{id}/likes")]
        public async Task<ActionResult<List<string?>>> GetUserLikesById(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(x => x.Id, id);
                var user = _users.Find(filter).FirstOrDefault();
                var likes = user.Likes.Select(i => i.ToString()).ToList();
                return user is not null ? Ok(likes) : NotFound();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpGet("{id}/matches")]
        public async Task<ActionResult<List<UserViewModel>>> GetUserMatchesById(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(x => x.Id, id);
                var user = _users.Find(filter).FirstOrDefault();
                if (user is null) 
                {
                    return NotFound();
                }
                var matches = new List<UserViewModel>();
                foreach (var match in user.Matches)
                {
                    var matchFilter = Builders<User>.Filter.Eq(x => x.Id, match.ToString());
                    var matchedUser = _users.Find(matchFilter).FirstOrDefault();
                    if (matchedUser is not null)
                    {
                        matches.Add(new UserViewModel(matchedUser));
                    }
                    else
                    {
                        return NotFound("Error loading matches");
                    }
                }
                return user is not null ? Ok(matches) : NotFound();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        //[HttpPost]
        //public async Task<ActionResult> Create(User user)
        //{
        //    user.Password = PasswordHasherTool.GenerateHash(user.Password);
        //    await _users.InsertOneAsync(user);
        //    return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        //}

        [HttpPut]
        public async Task<ActionResult> Update(UserUpdateModel user)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "name" && claim.Value != user.Email)
                    {
                        return BadRequest("You cannot change another users account!");
                    }
                    else if (claim.Type == "name" && claim.Value == user.Email)
                    {
                        break;
                    }
                }
            }
            
            var filterId = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            var userWithMatchingId = _users.Find(filterId).FirstOrDefault();
            if (userWithMatchingId is null)
            {
                return NotFound("Could not find user with given Id");
            }

            var filterEmail = Builders<User>.Filter.Eq(x => x.Email, user.Email);
            var existingUser = _users.Find(filterEmail).FirstOrDefault();
            if (existingUser is not null && existingUser.Id != user.Id)
            {
                return BadRequest("Provided email is already in use");
            }

            string image_url = "";
            if (user.Image != string.Empty)
            {
                if (user.Image!.StartsWith("data:image"))
                {
                    try
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(@$"{user.Image}"),
                            UseFilename = true,
                            UniqueFilename = false,
                            Overwrite = true
                        };

                        var uploadResponse = await _cloudinaryService.Cloudinary.UploadAsync(uploadParams);
                        image_url = uploadResponse.SecureUrl.ToString();
                        Console.WriteLine(uploadResponse.JsonObj);
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return BadRequest("Error uploading image. Profile upadte cancelled.");
                    }
                }
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            var update = Builders<User>.Update
                .Set(x => x.Name, user.Name)
                .Set(x => x.Age, user.Age)
                .Set(x => x.Gender, user.Gender)
                .Set(x => x.GenderPreferences, user.GenderPreferences)
                .Set(x => x.Bio, user.Bio)
                .Set(x => x.Image, image_url != string.Empty ? image_url : existingUser.Image);
            await _users.UpdateOneAsync(filter, update);

            //user.Password = PasswordHasherTool.GenerateHash(user.Password);
            //await _users.ReplaceOneAsync(filter, user);
            return Ok();
        }

        [HttpPut("like")]
        public async Task<ActionResult> AddLike(string userId, string likedUserId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var user = _users.Find(filter).FirstOrDefault();

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "name" && claim.Value != user.Email)
                    {
                        return BadRequest("You cannot perform likes for other users!");
                    }
                    else if (claim.Type == "name" && claim.Value == user.Email)
                    {
                        break;
                    }
                }
            }

            var likedfilter = Builders<User>.Filter.Eq(x => x.Id, likedUserId);
            var likedUser = _users.Find(likedfilter).FirstOrDefault();

            if (user is null || likedUser is null)
            {
                return NotFound();
            }
            var updatedLikes = user.Likes;
            updatedLikes.Add(ObjectId.Parse(likedUser.Id));

            bool newMatch = false;
            var matches = user.Matches;
            var likedUserMatches = likedUser.Matches;
            if (!user.Matches.Contains(ObjectId.Parse(likedUser.Id)) && likedUser.Likes.Contains(ObjectId.Parse(user.Id)))
            {
                newMatch = true;
                matches.Add(ObjectId.Parse(likedUser.Id));
                likedUserMatches.Add(ObjectId.Parse(user.Id));
            }
            var update = Builders<User>.Update
                .Set(x => x.Likes, updatedLikes)
                .Set(x => x.Matches, matches);
            await _users.UpdateOneAsync(filter, update);
            
            // TODO notify new match
            if (newMatch)
            {
                var likeUpdate = Builders<User>.Update
                    .Set(x => x.Matches, likedUserMatches);
                await _users.UpdateOneAsync(likedfilter, likeUpdate);
            }
            return Ok();
        }
        [HttpPut("dislike")]
        public async Task<ActionResult> AddDislike(string userId, string dislikedUserId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var user = _users.Find(filter).FirstOrDefault();

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "name" && claim.Value != user.Email)
                    {
                        return BadRequest("You cannot perform likes for other users!");
                    }
                    else if (claim.Type == "name" && claim.Value == user.Email)
                    {
                        break;
                    }
                }
            }

            var dislikedfilter = Builders<User>.Filter.Eq(x => x.Id, dislikedUserId);
            var likedUser = _users.Find(dislikedfilter).FirstOrDefault();

            if (user is not null && likedUser is not null)
            {
                var updatedDislikes = user.Dislikes;
                updatedDislikes.Add(ObjectId.Parse(likedUser.Id));
                var update = Builders<User>.Update
                    .Set(x => x.Dislikes, updatedDislikes);
                await _users.UpdateOneAsync(filter, update);
                return Ok();
            }
            else return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(x => x.Id, id);
                var user = _users.Find(filter).FirstOrDefault();
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    foreach (var claim in claims)
                    {
                        if (claim.Type == "name" && claim.Value != user.Email)
                        {
                            return BadRequest("You cannot change another users account!");
                        }
                        else if (claim.Type == "name" && claim.Value == user.Email)
                        {
                            break;
                        }
                    }
                }
                await _users.DeleteOneAsync(filter);
                return Ok();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }
    }
}
