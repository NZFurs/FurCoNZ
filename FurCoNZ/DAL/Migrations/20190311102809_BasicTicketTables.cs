﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FurCoNZ.DAL.Migrations
{
    public partial class BasicTicketTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderedById = table.Column<int>(nullable: false),
                    OrderedById1 = table.Column<string>(nullable: true),
                    AmountPaidCents = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_OrderedById1",
                        column: x => x.OrderedById1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PriceCents = table.Column<int>(nullable: false),
                    TotalAvailable = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttendeeAccountId = table.Column<int>(nullable: false),
                    AttendeeAccountId1 = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    TicketTypeId = table.Column<int>(nullable: false),
                    TicketName = table.Column<string>(nullable: true),
                    PreferredName = table.Column<string>(nullable: true),
                    PreferredPronouns = table.Column<string>(nullable: true),
                    LegalName = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Suburb = table.Column<string>(nullable: true),
                    CityTown = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    MealRequirements = table.Column<int>(nullable: false),
                    KnownAllergens = table.Column<string>(nullable: true),
                    CabinGrouping = table.Column<string>(nullable: true),
                    CabinNoisePreference = table.Column<int>(nullable: false),
                    AdditionalNotes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_AttendeeAccountId1",
                        column: x => x.AttendeeAccountId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderedById1",
                table: "Orders",
                column: "OrderedById1");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AttendeeAccountId1",
                table: "Tickets",
                column: "AttendeeAccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OrderId",
                table: "Tickets",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "TicketTypes");
        }
    }
}
