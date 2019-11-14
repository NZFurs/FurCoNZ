using Microsoft.EntityFrameworkCore.Migrations;

namespace FurCoNZ.Web.DAL.Migrations
{
    public partial class TermsAndConditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptedTermsAndConditions",
                table: "Tickets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedTermsAndConditions",
                table: "Tickets");
        }
    }
}
