using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FurCoNZ.Web.DAL.Migrations
{
    public partial class Chicken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CabinAssignment",
                table: "Tickets",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CheckInTime",
                table: "Tickets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CabinAssignment",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Tickets");
        }
    }
}
