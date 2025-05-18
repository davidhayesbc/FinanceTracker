using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transaction",
                newName: "Quantity");

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalCost",
                table: "Transaction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SecurityId",
                table: "Transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_SecurityId",
                table: "Transaction",
                column: "SecurityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Security_SecurityId",
                table: "Transaction",
                column: "SecurityId",
                principalTable: "Security",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Security_SecurityId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_SecurityId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "OriginalCost",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "SecurityId",
                table: "Transaction");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Transaction",
                newName: "Amount");
        }
    }
}
