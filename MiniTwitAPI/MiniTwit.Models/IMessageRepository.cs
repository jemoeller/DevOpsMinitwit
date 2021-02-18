using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MiniTwit.Entities;

namespace MiniTwit.Models
{
    public interface IMessageRepository
    {
        Task<Message> GetMessage(int id);
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<long> AddMessage(MessageCreateDTO message, string username);
        Task<HttpStatusCode> DeleteMessage(long id);
        Task<IEnumerable<Message>> GetUserMessages(string username, int per_page);
    }
}
