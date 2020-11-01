﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repositories;

namespace HelloWorldWeb.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Repositories.Domain.MessageEnt", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("MessageBody")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages","dbo");

                    b.HasData(
                        new
                        {
                            MessageId = 1,
                            Message = "Hello World",
                            MessageBody = "Hello World and all of its inhabitants!",
                            UserId = 8775895
                        },
                        new
                        {
                            MessageId = 2,
                            Message = "Greetings",
                            MessageBody = "Greeting Jane",
                            UserId = 8902550
                        },
                        new
                        {
                            MessageId = 3,
                            Message = "Invitation",
                            MessageBody = "Jane, you are cordially invited to the Halloween gala. Costumes are encouraged.",
                            UserId = 8902550
                        },
                        new
                        {
                            MessageId = 4,
                            Message = "Invitation",
                            MessageBody = "Bob, we are looking forward to seeing you this halloween. We ask that you please wear a fitting costume (preferably not the homeless look again).",
                            UserId = 8775895
                        },
                        new
                        {
                            MessageId = 5,
                            Message = "William, it is your mother. Call me!",
                            MessageBody = "William Smith, for nine months I carried you in my belly.. Is calling your mother once a week too much to ask for?.",
                            UserId = 8775895
                        });
                });

            modelBuilder.Entity("Repositories.Domain.UserEnt", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("UserId");

                    b.ToTable("Users","dbo");

                    b.HasData(
                        new
                        {
                            UserId = 8902550,
                            FullName = "Jane Smith",
                            UserName = "jane"
                        },
                        new
                        {
                            UserId = 8775895,
                            FullName = "Bob Smith",
                            UserName = "bob"
                        });
                });

            modelBuilder.Entity("Repositories.Domain.MessageEnt", b =>
                {
                    b.HasOne("Repositories.Domain.UserEnt", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
