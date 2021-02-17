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
        private int _latest;

        public MiniTwitController(IMiniTwitRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("latest/")]
        public dynamic GetLatest()
        {
            return new { latest = _latest };
        }

        [HttpGet("msgs/")]
        public async Task<IEnumerable<TimelineDTO>> GetMessages(int no, int latest)
        {
            _latest = latest;
            return await _repository.Timeline(no);
        }

        [HttpGet("msgs/{username}")]
        public async Task<IEnumerable<Message>> GetUserMessages(string username, int no, int latest)
        {
            _latest = latest;
            return await _repository.GetUserMessages(username, no);
        }

        [HttpPost("msgs/{username}")]
        public async Task<ActionResult<long>> PostUserMessages([FromBody] MessageCreateDTO request, string username, int latest)
        {
            _latest = latest;
            return await _repository.AddMessage(request, username);
        }

        [HttpPost("fllws/{username}")]
        public async Task<ActionResult> FollowUser([FromBody] FollowDTO request)
        {
            var response = await _repository.FollowUser(request.follow);

            return new StatusCodeResult((int)response);
        }

        //[HttpPost("fllws/{username}")]
        //public async Task<ActionResult> UnfollowUser([FromBody] UnfollowDTO request)
        //{
        //    var response = await _repository.UnfollowUser(request.follow);

        //    return new StatusCodeResult((int)response);
        //}

        [HttpGet("login/username={username}+password={password}")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<HttpStatusCode> Login(string username, string password)
        {
            try
            {
                await _repository.Login(username, password);
                return HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                return HttpStatusCode.NotFound;
            }
        }

        [HttpPost("logout/")]
        public void Logout()
        {
            _repository.Logout();
        }
    }
}
