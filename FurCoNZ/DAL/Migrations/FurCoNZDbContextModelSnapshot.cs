﻿// <auto-generated />
using System;
using FurCoNZ.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FurCoNZ.DAL.Migrations
{
    [DbContext(typeof(FurCoNZDbContext))]
    partial class FurCoNZDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity("FurCoNZ.Models.LinkedAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Issuer");

                    b.Property<string>("Subject");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("Issuer", "Subject");

                    b.ToTable("LinkedAccounts");
                });

            modelBuilder.Entity("FurCoNZ.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AmountPaidCents");

                    b.Property<int>("OrderedById");

                    b.HasKey("Id");

                    b.HasIndex("OrderedById");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("FurCoNZ.Models.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalNotes");

                    b.Property<string>("Address");

                    b.Property<int>("AttendeeAccountId");

                    b.Property<string>("CabinGrouping");

                    b.Property<int>("CabinNoisePreference");

                    b.Property<string>("CityTown");

                    b.Property<string>("Country");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("KnownAllergens");

                    b.Property<string>("LegalName");

                    b.Property<int>("MealRequirements");

                    b.Property<int>("OrderId");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("PreferredName");

                    b.Property<string>("PreferredPronouns");

                    b.Property<string>("Suburb");

                    b.Property<string>("TicketName");

                    b.Property<int>("TicketTypeId");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeAccountId");

                    b.HasIndex("OrderId");

                    b.HasIndex("TicketTypeId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("FurCoNZ.Models.TicketType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<int>("PriceCents");

                    b.Property<int>("TotalAvailable");

                    b.HasKey("Id");

                    b.ToTable("TicketTypes");
                });

            modelBuilder.Entity("FurCoNZ.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FurCoNZ.Models.LinkedAccount", b =>
                {
                    b.HasOne("FurCoNZ.Models.User", "User")
                        .WithMany("LinkedAccounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FurCoNZ.Models.Order", b =>
                {
                    b.HasOne("FurCoNZ.Models.User", "OrderedBy")
                        .WithMany()
                        .HasForeignKey("OrderedById")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FurCoNZ.Models.Ticket", b =>
                {
                    b.HasOne("FurCoNZ.Models.User", "AttendeeAccount")
                        .WithMany()
                        .HasForeignKey("AttendeeAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FurCoNZ.Models.Order", "Order")
                        .WithMany("TicketsPurchased")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FurCoNZ.Models.TicketType", "TicketType")
                        .WithMany()
                        .HasForeignKey("TicketTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
