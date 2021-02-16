using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiniTwit.Entities;
using System.Threading;

namespace MiniTwit.Models
{
    public interface IMiniTwitRepository
    {
        Task<Message> GetMessage(int id);
        Task<IEnumerable<Message>> GetAuthorMessages(int authorId);
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page);
        Task<IEnumerable<TimelineDTO>> Timeline(int per_page, int? currentuser);
        Task<long> GetUserId(string username);
        Task FollowUser(string username);
        Task UnfollowUser(string username);

        Task Login(string username, string password);

        Task<long> RegisterUser(UserCreateDTO user);

        string GenerateHashPassword(string password, string salt);

        string GenerateHashPassword(string password, int saltSize);

    }
}
