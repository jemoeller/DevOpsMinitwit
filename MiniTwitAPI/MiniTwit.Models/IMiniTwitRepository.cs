using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using MiniTwit.Entities;

namespace MiniTwit.Models
{
    public interface IMiniTwitRepository
    {
        Task<Message> GetMessage(int id);
        Task<long> AddMessage(MessageCreateDTO message);
        Task<HttpStatusCode> DeleteMessage(long id);
        Task<IEnumerable<Message>> GetAuthorMessages(int authorId);
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page);
        Task<IEnumerable<TimelineDTO>> Timeline(int per_page);
        Task<long> GetUserId(string username);
        Task FollowUser(string username);
        Task UnfollowUser(string username);
        Task<IEnumerable<string>> GetFollowers();
        Task Login(string username, string password);
    }
}
