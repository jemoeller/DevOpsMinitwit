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
            if (!await IsFollowing(username, followName))
            {
                var WhoId = await GetUserId(username);
                var WhomId = await GetUserId(followName);

                var follower = new Follower
                {
                    WhoId = (int) WhoId,
                    WhomId = (int) WhomId
                };

                _context.Followers.Add(follower);
                await _context.SaveChangesAsync();

                return NoContent;
            }
            return BadRequest;
        }

        public async Task<HttpStatusCode> UnfollowUser(string username, string unfollowName)
        {
            if(await IsFollowing(username, unfollowName))
            {
                var WhoId = await GetUserId(username);
                var WhomId = await GetUserId(unfollowName);

                var follower = (from f in _context.Followers
                                where f.WhoId == WhoId && f.WhomId == WhomId
                                select f).FirstOrDefault();

                _context.Followers.Remove(follower);
                await _context.SaveChangesAsync();

                return NoContent;
            }
            return BadRequest;
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

        public async Task<IEnumerable<TimelineDTO>> Timeline(int per_page, int userid)
        {
            
            var messages = await Task.Run(() => (from m in _context.Messages
                                                 join u in _context.Users on m.AuthorId equals u.UserId
                                                 where m.Flagged == 0 && (
                                                     u.UserId == userid || _context.Followers
                                                                                         .Where(f => f.WhoId == userid)
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

        public async Task<User> RegisterUser(UserCreateDTO user)
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
            return entity;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await (from u in _context.Users
                              where u.Username == username
                              select u).FirstOrDefaultAsync();

            if (user == null) return null; //wrong username

            if (GenerateHash(password) != user.PwHash) return null; //wrong password
            return user;
        }

        public async Task<bool> IsFollowing(string follower, string follows)
        {
            var followerId = await GetUserId(follower);
            var followsId = await GetUserId(follows);

            var following = await _context.Followers
                                        .Where(f => f.WhoId == followerId)
                                        .AnyAsync(f => f.WhomId == followsId);

            return following;
        }
    }
}
