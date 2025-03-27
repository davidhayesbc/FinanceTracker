using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FxRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyId = table.Column<int>(type: "int", nullable: false),
                    ToCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FxRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FxRate_ToFromCurrency",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FxRate_ToToCurrency",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISIN = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    SecurityType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Security", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Security_ToCurrency",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Price",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecurityId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Price_ToSecurity",
                        column: x => x.SecurityId,
                        principalTable: "Security",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_CurrencyId",
                table: "Account",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "UX_Currency_Code",
                table: "Currency",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FxRate_ToCurrencyId",
                table: "FxRate",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "UX_FxRate_Currencies_Date",
                table: "FxRate",
                columns: new[] { "FromCurrencyId", "ToCurrencyId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Price_SecurityId_Date",
                table: "Price",
                columns: new[] { "SecurityId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Security_CurrencyId",
                table: "Security",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Security_ISIN",
                table: "Security",
                column: "ISIN");

            migrationBuilder.CreateIndex(
                name: "UX_Security_Symbol",
                table: "Security",
                column: "Symbol",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ToCurrency",
                table: "Account",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ToCurrency",
                table: "Account");

            migrationBuilder.DropTable(
                name: "FxRate");

            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropTable(
                name: "Security");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropIndex(
                name: "IX_Account_CurrencyId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Account");
        }
    }
}
