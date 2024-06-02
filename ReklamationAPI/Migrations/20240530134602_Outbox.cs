using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReklamationAPI.Migrations
{
    /// <inheritdoc />
    public partial class Outbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Customer_CustomerEmail",
                table: "Complaint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaint",
                table: "Complaint");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "Complaint",
                newName: "Complaints");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_CustomerEmail",
                table: "Complaints",
                newName: "IX_Complaints_CustomerEmail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComplaintId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboxMessages_Complaints_ComplaintId",
                        column: x => x.ComplaintId,
                        principalTable: "Complaints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ComplaintId",
                table: "OutboxMessages",
                column: "ComplaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Customers_CustomerEmail",
                table: "Complaints",
                column: "CustomerEmail",
                principalTable: "Customers",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Customers_CustomerEmail",
                table: "Complaints");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customer");

            migrationBuilder.RenameTable(
                name: "Complaints",
                newName: "Complaint");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_CustomerEmail",
                table: "Complaint",
                newName: "IX_Complaint_CustomerEmail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaint",
                table: "Complaint",
                column: "Id");

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
