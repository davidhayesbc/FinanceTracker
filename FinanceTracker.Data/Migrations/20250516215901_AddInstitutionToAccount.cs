using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutionToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Institution",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Institution",
                table: "Account");
        }
    }
}
