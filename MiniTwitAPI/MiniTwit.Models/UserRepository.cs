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
    public class UserRepository : IUserRepository
    {
        private readonly IMiniTwitContext _context;
        private User _currentUser;

        public UserRepository(IMiniTwitContext context)
        {
            _context = context;
        }

        public async Task<long> GetUserId(string username)
        {
            var user_id = await Task.Run(() => (from u in _context.Users
                                                where u.Username == username
                                                select u.UserId).FirstOrDefault());
            return user_id;
        }

        public async Task<HttpStatusCode> FollowUser(string username, string followName)
        {
            var WhoId = await GetUserId(username);
            var WhomId = await GetUserId(followName);

            var follower = new Follower
            {
                WhoId = WhoId,
                WhomId = WhomId
            };

            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();

            return OK;
        }

        public async Task<HttpStatusCode> UnfollowUser(string username, string unfollowName)
        {
            var WhoId = await GetUserId(username);
            var WhomId = await GetUserId(unfollowName);

            var follower = (from f in _context.Followers
                            where f.WhoId == WhoId && f.WhomId == WhomId
                            select f).FirstOrDefault();

            _context.Followers.Remove(follower);
            await _context.SaveChangesAsync();

            return OK;
        }

        public async Task<IEnumerable<string>> GetFollowers()
        {
            var followers = await (from f in _context.Followers
                                   join u in _context.Users on f.WhomId equals u.UserId
                                   where f.WhoId == _currentUser.UserId
                                   select u.Username).ToListAsync();
            return followers;
        }

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

        public async Task<IEnumerable<TimelineDTO>> Timeline(int per_page)
        {
            if (_currentUser == null)
            {
                return await PublicTimeline(per_page);
            }

            var messages = await Task.Run(() => (from m in _context.Messages
                                                 join u in _context.Users on m.AuthorId equals u.UserId
                                                 where m.Flagged == 0 && (
                                                     u.UserId == _currentUser.UserId || _context.Followers
                                                                                         .Where(f => f.WhoId == _currentUser.UserId)
                                                                                         .Select(f => f.WhomId)
                                                                                         .Contains(u.UserId)
                                                 )
                                                 orderby m.PubDate descending
                                                 select new TimelineDTO
                                                 {
                                                     message = m,
                                                     user = u
                                                 }).Take(per_page).ToList());

            return messages;
        }

        public string GenerateHash(string password)
        {
            using var sha256 = SHA256.Create();

            var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));

            return Encoding.ASCII.GetString(hash, 0, hash.Length);
        }

        public async Task<long> RegisterUser(UserCreateDTO user)
        {
            //FIX: This creates an error everytime you try to register
            //if (await _context.Users.Select(u => u.Username == user.Username).AnyAsync()) return 0; //username taken

            //FIX: ID increments by 2
            var entity = new User
            {
                Username = user.Username,
                PwHash = GenerateHash(user.Password),
                Email = user.Email
            };

            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.UserId;
        }

        public async Task<long?> Login(string username, string password)
        {
            var user = await (from u in _context.Users
                              where u.Username == username
                              select u).FirstOrDefaultAsync();

            if (user == null) return null; //wrong username

            if (GenerateHash(password) != user.PwHash) return null; //wrong password
            _currentUser = user;
            return _currentUser.UserId;

        }
        public void Logout()
        {
            _currentUser = null;
        }
    }
}
