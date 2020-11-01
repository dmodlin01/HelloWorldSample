using System;
using System.Collections.Generic;
using System.Text;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Repositories.Domain;

namespace Repositories
{
    public class AppDbContext : DbContext
    {
        //Create a db context with our db sets
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        private DbSet<UserEnt> Users { get; set; }
        private DbSet<MessageEnt> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<MessageEnt>().HasData(new MessageEnt
            {
                Message = "Hello World",
                MessageBody = "Hello World and all of its inhabitants!",
                MessageId = 1,
                UserId = 8775895

            });

            modelBuilder.Entity<MessageEnt>().HasData(new List<MessageEnt>
            {

                new MessageEnt{Message = "Greetings", MessageBody ="Greeting Jane", UserId = 8902550, MessageId = 2},
                new MessageEnt{Message = "Invitation", MessageBody ="Jane, you are cordially invited to the Halloween gala. Costumes are encouraged.", UserId =8902550, MessageId = 3},
                new MessageEnt{Message = "Invitation", MessageBody ="Bob, we are looking forward to seeing you this halloween. We ask that you please wear a fitting costume (preferably not the homeless look again).", UserId = 8775895, MessageId = 4},
                new MessageEnt{Message = "William, it is your mother. Call me!", MessageBody ="William Smith, for nine months I carried you in my belly.. Is calling your mother once a week too much to ask for?.", UserId = 8775895, MessageId = 5},
            });
            modelBuilder.Entity<UserEnt>().HasData(new List<UserEnt>
                {
                    new UserEnt {UserName = "jane", FullName = "Jane Smith", UserId = 8902550},
                    new UserEnt {UserName = "bob", FullName = "Bob Smith", UserId = 8775895},
                }
            );
        }
    }
}
