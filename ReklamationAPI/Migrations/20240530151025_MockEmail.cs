using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReklamationAPI.Migrations
{
    /// <inheritdoc />
    public partial class MockEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "OutboxMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "OutboxMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "OutboxMessages");
        }
    }
}
