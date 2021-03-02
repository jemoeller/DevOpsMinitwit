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
        Latest, CurrentUser
    }

    [ApiController]
    [Route("/")]
    public class MiniTwitController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private User currentUser;
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
        public async Task<IEnumerable<TimelineDTO>> GetFeed(int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _userRepository.Timeline(no, (int) GetCurrentUser().UserId);
        }

        [HttpGet("msgs/{username}")]
        public async Task<IEnumerable<TimelineDTO>> GetUserMessages(string username, int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _messageRepository.GetUserMessages(username, no);
        }

        [HttpPost("msgs/{username}")]
        public async Task<ActionResult<long>> PostUserMessages([FromBody] MessageCreateDTO request, string username, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _messageRepository.AddMessage(request, username);
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
        public async Task<long?> Login([FromBody] LoginDTO dto, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var user = await _userRepository.Login(dto.username, dto.password);
            currentUser = user;
            //_memoryCache.Set(CacheFields.CurrentUser, user);
            if (user == null) return null;
            return user.UserId;
        }

        [HttpPost("register/")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterDTO registration, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var dto = new UserCreateDTO()
            {
                Username = registration.username,
                Password = registration.pwd,
                Email = registration.email
            };

            var user = await _userRepository.RegisterUser(dto);

            currentUser = user;
            //_memoryCache.Set(CacheFields.CurrentUser, user);

            return user.UserId;
        }

        [HttpPost("logout/")]
        public void Logout(int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            currentUser = null;
            //_memoryCache.Set<User>(CacheFields.CurrentUser, null);
        }

        public User GetCurrentUser()
        {
            //return _memoryCache.Get<User>(CacheFields.CurrentUser);
            return currentUser;
        }

        public async Task<bool> IsFollowing(string follower, string follows)
        {
            return await _userRepository.IsFollowing(follower, follows);
        }
    }
}
