using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MiniTwit.Entities;
using Xunit;
using Microsoft.EntityFrameworkCore.Sqlite;
using MiniTwit.Models;

namespace Models.Test
{
    public class UserRepositoryTests
    {
        private readonly MiniTwitContext context;
        private readonly UserRepository repo;

        public UserRepositoryTests()
        {
            var builder = new DbContextOptionsBuilder<MiniTwitContext>().UseSqlite("DataSource=:memory:");
            context = new ContextTest(builder.Options);
            context.Database.EnsureCreated();
            repo = new UserRepository(context);
        }

        [Fact]
        public async Task create_user_succesfully()
        {
            var result = await repo.RegisterUser(
                new UserCreateDTO
                {
                    Username = "userTest",
                    Email = "userTest@mail.io",
                    Password = "123"
                }
                );
            var userQuery = from u in context.Users where u.Username == "userTest" select u;
            var user = await userQuery.FirstOrDefaultAsync();

            Assert.NotNull(user);
            Assert.Equal("userTest", user.Username);
            Assert.Equal("userTest@mail.io", user.Email);
            Assert.NotEqual("123", user.PwHash);
        }

        //could be a nice test to have
        /*
        [Fact]
        public async Task create_user_taken_username()
        {
            var result = await repo.RegisterUser(
                new UserCreateDTO { 
                    Username = "olduser1", 
                    Email = "test@mail.com", 
                    Password = "123"}
            );

            Assert.Equal(-2, result);
        }
        */

        /*
        [Fact]
        public async Task create_user_invalid_email()
        {
            var result = await repo.RegisterUser(
                new UserCreateDTO { 
                    Username = "user5", 
                    Email = "usermail.io", 
                    Password = "123" }
                );

            Assert.Equal(-4, result);
        }
        */
        /*
        [Fact]
        public async Task create_user_taken_email()
        {
            var result = await repo.RegisterUser(
                new UserCreateDTO { 
                    Username = "user5", 
                    Email = "olduser1@mail.io", 
                    Password = "123" }
                );

            Assert.Equal(-3, result);
        }
        */

        /*
        [Fact]
        public async Task create_user_password_mismatch()
        {
            var result = await repo.RegisterUser(
                new UserCreateDTO { 
                    Username = "user5", 
                    Email = "user5@mail.io", 
                    Password = "123" }
                );

            Assert.Equal(-1, result);
        }
        */

        /*
        [Fact]
        public async Task delete_existing_user()
        {
            var userbefore = await repo.ReadAsync("olduser1");
            var result = await repo.DeleteAsync(userbefore.user_id);
            var userafter = await repo.ReadAsync("olduser1");

            Assert.NotNull(userbefore);
            Assert.Null(userafter);
            Assert.Equal(userbefore.user_id, result);
        }
        */

        /*
        [Fact]
        public async Task delete_nonexisting_user()
        {
            var result = await repo.DeleteAsync(1337);

            Assert.Equal(-1, result);
        }
        */
        /*
        [Fact]
        public async Task read_existing_user_by_name()
        {
            var result = await repo.ReadAsync(1);

            Assert.NotNull(result);
            Assert.Equal("olduser1", result.username);
            Assert.Equal(1, result.user_id);
        }
        */

        /*
        [Fact]
        public async Task read_nonexisting_user_by_id()
        {
            var result = await repo.ReadAsync(1337);

            Assert.Null(result);
        }
        */

        /*
        [Fact]
        public async Task read_existing_user_by_name()
        {
            var result = await repo.ReadAsync("olduser1");

            Assert.NotNull(result);
            Assert.Equal("olduser1", result.username);
            Assert.Equal(1, result.user_id);
        }
        */

        /*
        [Fact]
        public async Task read_nonexisting_user_by_name()
        {
            var result = await repo.ReadAsync("olduser1337");

            Assert.Null(result);
        }
        */

        /*
        [Fact]
        public async Task read_all_users()
        {
            var resultfirst = await repo.ReadAllAsync();
            await repo.user(new UserCreateDTO { Username = "user6", Email = "user6@mail.io", Password = "123"});
            var resultsecond = await repo.ReadAllAsync();

            Assert.Equal(5, resultfirst.Count);
            Assert.Equal(6, resultsecond.Count);
        }
        */

        //TODO lav denne test
        /*
        [Fact]
        public async Task read_pwhash_existing_user()
        {
            var result = await repo.GenerateHash("password");

            Assert.Equal("CB10E796FAC82E4FE345DD4E30B6B6EF940AAD18CEEB6DD55A7C5A4553816159", result);
        }
        */

        /*
        [Fact]
        public async Task read_pwhash_nonexisting_user()
        {
            var result = await repo.ReadPWHash("nonuser");

            Assert.Null(result);
        }
        */

        [Fact]
        public async Task follow_existing_user()
        {
            //var result = await repo.FollowUser("olduser1", "olduser2");
            var followeduser = await repo.FollowUser("olduser2", "olduser1");
            var followinguser = await repo.FollowUser("olduser1", "olduser2");

            //Assert.Equal(0, result);
            Assert.True(await repo.IsFollowing("olduser1", "olduser2"));
            Assert.True(await repo.IsFollowing("olduser2", "olduser1"));
        }

        /*
        [Fact]
        public async Task follow_from_nonexisting_user()
        {
            var result = await repo.FollowAsync("nonuser1", "olduser1");

            Assert.Equal(-3, result);
        }
        */

        /*
        [Fact]
        public async Task follow_nonexisting_user()
        {
            var result = await repo.FollowAsync("olduser1", "nonuser1");

            Assert.Equal(-2, result);
        }
        */

        /*
        [Fact]
        public async Task follow_self()
        {
            var result = await repo.FollowAsync("olduser1", "olduser1");

            Assert.Equal(-1, result);
        }
        */

        [Fact]
        public async Task unfollow_existing_user_being_followed()
        {
            var followeduser = await repo.UnfollowUser("olduser2", "olduser1");
            var followinguser = await repo.UnfollowUser("olduser1", "olduser2");

            //Assert.Equal(0, result);
            Assert.False(await repo.IsFollowing("olduser1", "olduser2"));
            Assert.False(await repo.IsFollowing("olduser2", "olduser1"));
        }

        /*
        [Fact]
        public async Task unfollow_self()
        {
            var result = await repo.UnfollowAsync("olduser1", "olduser1");

            Assert.Equal(-1, result);
        }
        */

        /*
        [Fact]
        public async Task unfollow_existing_user_not_bring_followed()
        {
            var result = await repo.UnfollowAsync("olduser3", "olduser4");

            Assert.Equal(-4, result);
        }
        */

        /*
        [Fact]
        public async Task unfollow_nonexisting_user()
        {
            var result = await repo.UnfollowAsync("olduser1", "nonuser");

            Assert.Equal(-2, result);
        }
        */

        /*
        [Fact]
        public async Task unfollow_from_nonexisting_user()
        {
            var result = await repo.UnfollowAsync("nonuser", "olduser1");

            Assert.Equal(-3, result);
        }
        */

        /*
        public void Dispose()
        {
            context.Dispose();
        }
        */
    }
}