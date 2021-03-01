using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniTwit.Entities;
using MiniTwit.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace MiniTwit.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class MiniTwitController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public MiniTwitController(
            IUserRepository userRepository, 
            IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [HttpGet]
        public dynamic GetStatus()
        {
            return new { go_to = "/swagger/index.html" };
        }

        [HttpGet("latest/")]
        public dynamic GetLatest()
        {
            return new { latest = HttpContext.Session.GetInt32("latest") };
        }

        [HttpGet("msgs/")]
        public async Task<IEnumerable<TimelineDTO>> GetMessages(int no = 30, int latest = 0)
        {
            HttpContext.Session.SetInt32("latest", latest);
            return await _userRepository.Timeline(no, (int) GetCurrentUser().UserId);
        }

        [HttpGet("msgs/{username}")]
        public async Task<IEnumerable<TimelineDTO>> GetUserMessages(string username, int no = 30, int latest = 0)
        {
            HttpContext.Session.SetInt32("latest", latest);
            return await _messageRepository.GetUserMessages(username, no);
        }

        [HttpPost("msgs/{username}")]
        public async Task<ActionResult<long>> PostUserMessages([FromBody] MessageCreateDTO request, string username, int latest = 0)
        {
            HttpContext.Session.SetInt32("latest", latest);
            return await _messageRepository.AddMessage(request, username);
        }

        [HttpPost("fllws/{username}")]
        public async Task<ActionResult> FollowUser([FromBody] FollowDTO request, string username, int latest = 0)
        {
            HttpContext.Session.SetInt32("latest", latest);
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
            HttpContext.Session.SetInt32("latest", latest);
            var user = await _userRepository.Login(dto.username, dto.password);
            //currentuser = user;
            return user.UserId;
        }

        [HttpPost("register/")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterDTO registration, int latest = 0)
        {
            HttpContext.Session.SetInt32("latest", latest);
            var dto = new UserCreateDTO()
            {
                Username = registration.username,
                Password = registration.pwd,
                Email = registration.email
            };

            var userid = await _userRepository.RegisterUser(dto);

            return userid;
        }

        [HttpPost("logout/")]
        public void Logout(int latest = 0)
        {
            HttpContext.Session.SetInt32("latest", latest);
            //TODO PLS FIX
        }

        public User GetCurrentUser()
        {
            //return currentuser;
            return null;
        }
    }
}
