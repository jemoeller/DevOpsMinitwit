using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MiniTwit.Entities;

namespace MiniTwit.Models
{
    public interface IMiniTwitRepository
    {
        Task<Message> GetMessage(int id);
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<long> AddMessage(MessageCreateDTO message, string username);
        Task<HttpStatusCode> DeleteMessage(long id);
        Task<IEnumerable<Message>> GetUserMessages(string username, int per_page);
        Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page);
        Task<IEnumerable<TimelineDTO>> Timeline(int per_page);
        Task<long> GetUserId(string username);
        Task<HttpStatusCode> FollowUser(string username);
        Task<HttpStatusCode> UnfollowUser(string username);
        Task<long?> Login(string username, string password);
        Task<long> RegisterUser(UserCreateDTO user);
        string GenerateHash(string password);
        Task<IEnumerable<string>> GetFollowers();
        void Logout();
    }
}
