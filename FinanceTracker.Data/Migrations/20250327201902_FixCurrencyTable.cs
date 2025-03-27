using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCurrencyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Currency_Code",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Currency");

            migrationBuilder.CreateIndex(
                name: "UX_Currency_Symbol",
                table: "Currency",
                column: "Symbol",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Currency_Symbol",
                table: "Currency");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Currency",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UX_Currency_Code",
                table: "Currency",
                column: "Code",
                unique: true);
        }
    }
}
