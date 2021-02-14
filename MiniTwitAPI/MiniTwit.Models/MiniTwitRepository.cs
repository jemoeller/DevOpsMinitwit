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

        public int Count()
        {
            var count = _context.Messages.Count();
            return count;
        }

        public async Task<Message> getCountAsync()
        {
            var val = Task.Run(() => (from m in _context.Messages
                                     select new Message
                                     {
                                         AuthorId = m.AuthorId,
                                         MessageId = m.MessageId,
                                         PubDate = m.PubDate,
                                         Flagged = m.Flagged,
                                         Text = m.Text
                                     }).FirstOrDefault());
            return await val;
        }
    }
}
