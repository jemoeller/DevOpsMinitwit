using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniTwit.Entities;
using MiniTwit.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace MiniTwit.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MiniTwitController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private int _latest;

        public MiniTwitController(IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [HttpGet("latest/")]
        public dynamic GetLatest()
        {
            return new { latest = _latest };
        }

        [HttpGet("msgs/")]
        public async Task<IEnumerable<TimelineDTO>> GetMessages(int? no, int latest)
        {
            _latest = latest;
            if (no == null) no = 30;
            return await _userRepository.Timeline(no.Value);
        }

        [HttpGet("msgs/{username}")]
        public async Task<IEnumerable<Message>> GetUserMessages(string username, int? no, int latest)
        {
            _latest = latest;
            if (no == null) no = 30;
            return await _messageRepository.GetUserMessages(username, no.Value);
        }

        [HttpPost("msgs/{username}")]
        public async Task<ActionResult<long>> PostUserMessages([FromBody] MessageCreateDTO request, string username, int latest)
        {
            _latest = latest;
            return await _messageRepository.AddMessage(request, username);
        }

        [HttpPost("fllws/{username}")]
        public async Task<ActionResult> FollowUser([FromBody] FollowDTO request)
        {
            var response = HttpStatusCode.BadRequest;
            if (request.follow != null)
            {
                response = await _userRepository.FollowUser(request.follow);
            }
            else if(request.unfollow != null)
            {
                response = await _userRepository.FollowUser(request.follow);
            }

            return new StatusCodeResult((int)response);
        }

        [HttpGet("login/")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<long?> Login([FromBody] LoginDTO dto)
        {
            var user = await _userRepository.Login(dto.username, dto.password);
            return user;
        }

        [HttpPost("register/")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterDTO registration)
        {
            var dto = new UserCreateDTO()
            {
                Username = registration.username,
                Password = registration.pwd,
                Email = registration.email
            };

            var userid = await _userRepository.RegisterUser(dto);
            return new StatusCodeResult((int) userid);
        }

        [HttpPost("logout/")]
        public void Logout()
        {
            _userRepository.Logout();
        }
    }
}
