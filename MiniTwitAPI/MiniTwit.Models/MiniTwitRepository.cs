using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTwit.Entities;

namespace MiniTwit.Models
{
    public class MiniTwitRepository : IMiniTwitRepository
    {
        private readonly IMiniTwitContext _context;

        public MiniTwitRepository(IMiniTwitContext context)
        {
            _context = context;
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

        public async Task<IEnumerable<Message>> GetAuthorMessages(int authorId)
        {
            var messages = Task.Run(() => from m in _context.Messages
                                          where m.AuthorId == authorId
                                          select new Message
                                          {
                                              AuthorId = m.AuthorId,
                                              MessageId = m.MessageId,
                                              PubDate = m.PubDate,
                                              Flagged = m.Flagged,
                                              Text = m.Text
                                          });
            return await messages;
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


        // User Methods TODO: Place in own repository
        public async Task<long> GetUserId(string username)
        {
            var user_id = await Task.Run(() => (from u in _context.Users
                                                where u.Username == username
                                                select u.UserId).FirstOrDefault());
            return user_id;
        }

        public async Task FollowUser(string username)
        {
            var WhomId = await GetUserId(username);

            var follower = new Follower
            {
                WhoId = 2,
                WhomId = WhomId
            };

            _context.Followers.Add(follower);
        }

        public async Task UnfollowUser(string username)
        {
            var WhomId = await GetUserId(username);

            var follower = (from f in _context.Followers
                            where f.WhoId == 2 && f.WhomId == WhomId
                            select f).FirstOrDefault();

            _context.Followers.Remove(follower);
        }

        //Displays the latest messages of all users, limited by per_page
        public async Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page)
        {
            var messages = await Task.Run(() => (from m in _context.Messages
                                                 join u in _context.Users on m.AuthorId equals u.UserId
                                                 where m.Flagged == 0
                                                 orderby m.PubDate descending
                                                 select new TimelineDTO
                                                 {
                                                     message = m,
                                                     user = u
                                                 }).Take(per_page).ToList());

            return messages;
        }

        //public async Task<IEnumerable<Message>> Timeline(int per_page, int currentUser)
        //{
        //    if(currentUser == null)
        //    {
        //        PublicTimeline(per_page);
        //    }
        //    else
        //    {
        //    var messages = (from m in _context.Messages
        //                    join u in _context.Users on m.AuthorId equals u.UserId
        //                    where m.Flagged == 0 && (u.UserId == currentUser
        //                                            || u.UserId in (from f in _context.Followers
        //                                                            where f.WhoId equals currentUser
        //                                                            select f.WhomId))
        //                    orderby m.PubDate descending
        //                    select new { Message = m,
        //                                 User = u}).Take(per_page);

        //        return messages.ToList();
        //    };
        //}
    }
}
