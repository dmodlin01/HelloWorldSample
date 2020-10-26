using System;
using System.Collections.Generic;
using System.Text;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Repositories.Domain;

namespace Repositories
{
    public class AppDbContext:DbContext
    {
        //Create a db context with our db sets
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<MessageEnt> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MessageEnt>().HasData(new MessageDTO {Message = "Hello World", MessageBody = "Hello World and all of its inhabitants!", MessageId = 1});
            modelBuilder.Entity<MessageEnt>().HasData(new List<MessageEnt>
            {
                new MessageEnt{Message = "Greetings", MessageBody ="Greeting Friends", MessageId = 2},
                new MessageEnt{Message = "Welcome", MessageId = 3}
            });
        }
    }
}
