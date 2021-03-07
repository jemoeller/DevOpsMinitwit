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
        public async Task<IEnumerable<TimelineDTO>> GetMessages(int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _userRepository.PublicTimeline(no);
        }

        [HttpGet("feed/")]
        public async Task<IEnumerable<TimelineDTO>> GetFeed(long userId, int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _userRepository.Timeline(no, (int) userId);
        }

        [HttpGet("msgs/{username}")]
        public async Task<IEnumerable<TimelineDTO>> GetUserMessages(string username, int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _messageRepository.GetUserMessages(username, no);
        }

        [HttpPost("msgs/{username}")]
        public async Task<IActionResult> PostUserMessages([FromBody] MessageCreateDTO request, string username, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            await _messageRepository.AddMessage(request, username);
            return NoContent();
        }

        [HttpPost("fllws/{username}")]
        [ProducesResponseType(Status204NoContent)]
        public async Task<ActionResult> FollowUser([FromBody] FollowDTO request, string username, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var response = HttpStatusCode.InternalServerError;
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
        public async Task<IActionResult> RegisterEndpoint([FromBody] RegisterDTO registration, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);

            var user = await Register(registration);

            if (user == null) return BadRequest();
            else return NoContent();
        }
        public async Task<User> Register(RegisterDTO registration)
        {
            var dto = new UserCreateDTO()
            {
                Username = registration.username,
                Password = registration.pwd,
                Email = registration.email
            };

            var user = await _userRepository.RegisterUser(dto);

            return user;
        }

        public async Task<bool> IsFollowing(string follower, string follows)
        {
            return await _userRepository.IsFollowing(follower, follows);
        }
    }
}
