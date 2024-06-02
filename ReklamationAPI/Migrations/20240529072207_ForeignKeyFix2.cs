using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReklamationAPI.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Customer_CustomerEmail",
                table: "Complaint");

            migrationBuilder.DropIndex(
                name: "IX_Complaint_CustomerEmail",
                table: "Complaint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Complaint_CustomerEmail",
                table: "Complaint",
                column: "CustomerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_Customer_CustomerEmail",
                table: "Complaint",
                column: "CustomerEmail",
                principalTable: "Customer",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
