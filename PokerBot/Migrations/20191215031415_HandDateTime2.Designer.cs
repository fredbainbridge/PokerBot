﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokerBot.Models;

namespace PokerBot.Migrations
{
    [DbContext(typeof(PokerDBContext))]
    [Migration("20191215031415_HandDateTime2")]
    partial class HandDateTime2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PokerBot.Models.Hand", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Data")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("WinnerID")
                        .HasColumnType("int");

                    b.Property<int>("WinningAmount")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("WinnerID");

                    b.ToTable("Hand");
                });

            modelBuilder.Entity("PokerBot.Models.Payment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Chips")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Confirmed")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateRequested")
                        .HasColumnType("datetime2");

                    b.Property<int>("PayeeID")
                        .HasColumnType("int");

                    b.Property<int>("PayerID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Sent")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("PayeeID");

                    b.HasIndex("PayerID");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("PokerBot.Models.Session", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Chips")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("PokerBot.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RealName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SlackID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("PokerBot.Models.Hand", b =>
                {
                    b.HasOne("PokerBot.Models.User", "Winner")
                        .WithMany("Winners")
                        .HasForeignKey("WinnerID")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("PokerBot.Models.Payment", b =>
                {
                    b.HasOne("PokerBot.Models.User", "Payee")
                        .WithMany("Payees")
                        .HasForeignKey("PayeeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PokerBot.Models.User", "Payer")
                        .WithMany("Payers")
                        .HasForeignKey("PayerID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("PokerBot.Models.Session", b =>
                {
                    b.HasOne("PokerBot.Models.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
