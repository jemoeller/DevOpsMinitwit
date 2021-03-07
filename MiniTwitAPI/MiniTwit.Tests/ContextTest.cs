using Microsoft.EntityFrameworkCore;
using MiniTwit.Entities;
using MiniTwit.Models;

namespace Models.Test
{
    public class ContextTest : MiniTwitContext
    {
        public ContextTest(DbContextOptions<MiniTwitContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "olduser1",
                    Email = "olduser1@mail.io",
                    PwHash = "CB10E796FAC82E4FE345DD4E30B6B6EF940AAD18CEEB6DD55A7C5A4553816159"
                },

                new User
                {
                    UserId = 2,
                    Username = "olduser2",
                    Email = "olduser2@mail.io",
                    PwHash = "76E2D06976544FB98DEBD8297BEE138A03CD5C5212F3D70797C07D75A476D4CE"
                },

                new User
                {
                    UserId = 3,
                    Username = "olduser3",
                    Email = "olduser3@mail.io",
                    PwHash = "B91F86D0CE688DF80AB84175AF163E9964CA4E80179F1D2668B4BB67B600D295"
                },

                new User
                {
                    UserId = 4,
                    Username = "olduser4",
                    Email = "olduser4@mail.io",
                    PwHash = "5CA833F9ED5BC47AEEECA98D6EC5AFE1D4741D4D7DA7C571E037D624BA141388"
                }
            );
        }
    }
}