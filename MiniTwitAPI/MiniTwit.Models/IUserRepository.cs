using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MiniTwit.Entities;

namespace MiniTwit.Models
{
    public interface IUserRepository
    {
        Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page);
        Task<IEnumerable<TimelineDTO>> Timeline(int per_page, int userid);
        Task<long> GetUserId(string username);
        Task<HttpStatusCode> FollowUser(string username, string followName);
        Task<HttpStatusCode> UnfollowUser(string username, string unfollowName);
        Task<User> Login(string username, string password);
        Task<User> RegisterUser(UserCreateDTO user);
        string GenerateHash(string password);
        Task<bool> IsFollowing(string follower, string follows);
    }
}
