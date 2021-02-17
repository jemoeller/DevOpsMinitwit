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
        private readonly IMiniTwitRepository _repository;

        public MiniTwitController(IMiniTwitRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("messages/{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            return await _repository.GetMessage(id);
        }

        [HttpGet("messages/")]
        public async Task<IEnumerable<Message>> GetMessages()
        {
            return await _repository.GetMessagesAsync();
        }

        [HttpGet("messages/user/{userid}")]
        public async Task<IEnumerable<Message>> GetAuthorMessages(int userid)
        {
            return await _repository.GetAuthorMessages(userid);
        }

        [HttpGet("users/{username}")]
        public async Task<ActionResult<long>> GetUser(string username)
        {
            return await _repository.GetUserId(username);
        }

        [HttpGet("timeline/{userid}")]
        public async Task<IEnumerable<TimelineDTO>> GetTimeline(int? userid)
        {
            return await _repository.Timeline(30, userid);
        }

        [HttpGet("login/username={username}+password={password}")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<long?> Login(string username, string password)
        {
            var user = await _repository.Login(username, password);
            return user;
        }

        [HttpPost("register/u={usern}+pw={pw}+em={email}")]
        public async Task<ActionResult<long>> Register(string usern, string pw, string email)
        {
            var dto = new UserCreateDTO()
            {
                Username = usern,
                Password = pw,
                Email = email
            };

            var userid = await _repository.RegisterUser(dto);
            return new StatusCodeResult((int) userid);
        }
    }
}
