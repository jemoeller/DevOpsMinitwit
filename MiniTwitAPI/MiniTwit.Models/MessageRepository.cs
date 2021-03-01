using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MiniTwit.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Net.HttpStatusCode;

namespace MiniTwit.Models
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMiniTwitContext _context;

        public MessageRepository(IMiniTwitContext context)
        {
            _context = context;
        }

        public async Task<long> AddMessage(MessageCreateDTO message, string username)
        {
            var userId = await GetUserId(username);

            var newMessage = new Message
            {
                AuthorId = userId,
                Text = message.content,
                PubDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Flagged = 0
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return newMessage.MessageId;
        }

        public async Task<HttpStatusCode> DeleteMessage(long id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound;
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent;
        }

        public async Task<Message> GetMessage(int messageId)
        {
            var message = Task.Run(() => (from m in _context.Messages
                                          where m.MessageId == messageId
                                          select new Message
                                          {
                                              AuthorId = m.AuthorId,
                                              MessageId = m.MessageId,
                                              PubDate = m.PubDate,
                                              Flagged = m.Flagged,
                                              Text = m.Text
                                          }).FirstOrDefault());
            return await message;
        }

        public async Task<IEnumerable<TimelineDTO>> GetUserMessages(string username, int per_page)
        {
            var userId = await GetUserId(username);

            var messages = await (from m in _context.Messages
                                        join u in _context.Users on m.AuthorId equals u.UserId
                                        where m.AuthorId == userId
                                  select new TimelineDTO
                                  {
                                      message = m,
                                      user = u
                                  }).Take(per_page).ToListAsync();

            return messages;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync()
        {
            var messages = Task.Run(() => (from m in _context.Messages
                                           select new Message
                                           {
                                               AuthorId = m.AuthorId,
                                               MessageId = m.MessageId,
                                               PubDate = m.PubDate,
                                               Flagged = m.Flagged,
                                               Text = m.Text
                                           }).ToList());
            return await messages;
        }

        public async Task<long> GetUserId(string username)
        {
            var user_id = await Task.Run(() => (from u in _context.Users
                                                where u.Username == username
                                                select u.UserId).FirstOrDefault());
            return user_id;
        }
    }
}
