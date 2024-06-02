using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReklamationAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixedOutboxModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Customers_CustomerEmail",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessages_Complaints_ComplaintId",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ComplaintId",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_CustomerEmail",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "ComplaintId",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "OutboxMessages",
                newName: "To");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "OutboxMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "OutboxMessages",
                newName: "Message");

            migrationBuilder.AddColumn<int>(
                name: "ComplaintId",
                table: "OutboxMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ComplaintId",
                table: "OutboxMessages",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CustomerEmail",
                table: "Complaints",
                column: "CustomerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Customers_CustomerEmail",
                table: "Complaints",
                column: "CustomerEmail",
                principalTable: "Customers",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessages_Complaints_ComplaintId",
                table: "OutboxMessages",
                column: "ComplaintId",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
