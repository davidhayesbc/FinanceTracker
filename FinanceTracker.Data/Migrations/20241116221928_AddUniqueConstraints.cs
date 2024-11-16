using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_TransactionType_Type",
                table: "TransactionType",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TransactionCategory_Category",
                table: "TransactionCategory",
                column: "Category",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_AccountType_Type",
                table: "AccountType",
                column: "Type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_TransactionType_Type",
                table: "TransactionType");

            migrationBuilder.DropIndex(
                name: "UX_TransactionCategory_Category",
                table: "TransactionCategory");

            migrationBuilder.DropIndex(
                name: "UX_AccountType_Type",
                table: "AccountType");
        }
    }
}
