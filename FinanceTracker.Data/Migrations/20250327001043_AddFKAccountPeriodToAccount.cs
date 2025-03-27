using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFKAccountPeriodToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccountPeriod_AccountId",
                table: "AccountPeriod",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPeriod_ToAccount",
                table: "AccountPeriod",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountPeriod_ToAccount",
                table: "AccountPeriod");

            migrationBuilder.DropIndex(
                name: "IX_AccountPeriod_AccountId",
                table: "AccountPeriod");
        }
    }
}
