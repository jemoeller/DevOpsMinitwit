using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MiniTwit.Entities;
using MiniTwit.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace MiniTwit.API.Controllers
{
    public enum CacheFields
    {
        Latest
    }

    [ApiController]
    [Route("/")]
    public class MiniTwitController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private IMemoryCache _memoryCache;

        public MiniTwitController(
            IUserRepository userRepository, 
            IMessageRepository messageRepository,
            IMemoryCache memoryCache)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public dynamic GetStatus()
        {
            return new { go_to = "/swagger/index.html" };
        }

        [HttpGet("latest/")]
        public dynamic GetLatest()
        {
            return new { latest = _memoryCache.Get<int>(CacheFields.Latest) };
        }

        [HttpGet("msgs/")]
        public async Task<IActionResult<IEnumerable<TimelineDTO>>> GetMessages(int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var result = await _userRepository.PublicTimeline(no);
            return this.Ok(result);
        }

        [HttpGet("feed/")]
        public async Task<IEnumerable<TimelineDTO>> GetFeed(long userId, int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _userRepository.Timeline(no, (int) userId);
        }

        [HttpGet("msgs/{username}")]
        public async Task<IActionResult<IEnumerable<TimelineDTO>>> GetUserMessages(string username, int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var result = await _messageRepository.GetUserMessages(username, no);
            
            if (result == null) return this.BadRequest();
            else return this.NoContent();
        }

        [HttpPost("msgs/{username}")]
        public async Task<IActionResult<ActionResult<long>>> PostUserMessages([FromBody] MessageCreateDTO request, string username, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var result = await _messageRepository.AddMessage(request, username);
            return this.NoContent();
        }

        [HttpPost("fllws/{username}")]
        public async Task<ActionResult> FollowUser([FromBody] FollowDTO request, string username, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var response = HttpStatusCode.BadRequest;
            if (request.follow != null)
            {
                response = await _userRepository.FollowUser(username, request.follow);
            }
            else if(request.unfollow != null)
            {
                response = await _userRepository.UnfollowUser(username, request.unfollow);
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPost("login/")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<User> Login([FromBody] LoginDTO dto, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var user = await _userRepository.Login(dto.username, dto.password);
            if (user == null) return null;
            return user;
        }

        [HttpPost("register/")]
        public async Task<IActionResult<User>> Register([FromBody] RegisterDTO registration, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var dto = new UserCreateDTO()
            {
                Username = registration.username,
                Password = registration.pwd,
                Email = registration.email
            };

            var user = await _userRepository.RegisterUser(dto);

            if (user == null) return this.BadRequest()
            else return this.NoContent();
        }


        public async Task<bool> IsFollowing(string follower, string follows)
        {
            return await _userRepository.IsFollowing(follower, follows);
        }
    }
}
