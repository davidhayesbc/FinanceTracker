using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_ToTransactionType",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionSplit_ToTransactionCategory",
                table: "TransactionSplit");
            
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_ToTransactionCategory",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionSplit_ToTransaction",
                table: "TransactionSplit");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ToAccountType",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_ToAccount",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Transact__3214EC07D26F23FD",
                table: "TransactionType");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Transact__3214EC078C80138B",
                table: "TransactionSplit");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Transact__3214EC0788389A10",
                table: "TransactionCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK__tmp_ms_x__3214EC07E906A3AC",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AccountT__3214EC0785814C1D",
                table: "AccountType");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Account__3214EC072E8BD840",
                table: "Account");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionType",
                table: "TransactionType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionSplit",
                table: "TransactionSplit",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionCategory",
                table: "TransactionCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AccountType",
                table: "AccountType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Account",
                table: "Account",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_ToTransactionType",
                table: "Transaction",
                column: "TransactionTypeId",
                principalTable: "TransactionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionSplit_ToTransactionCategory",
                table: "TransactionSplit",
                column: "CategoryId",
                principalTable: "TransactionCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_ToTransactionCategory",
                table: "Transaction",
                column: "CategoryId",
                principalTable: "TransactionCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionSplit_ToTransaction",
                table: "TransactionSplit",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ToAccountType",
                table: "Account",
                column: "AccountTypeId",
                principalTable: "AccountType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_ToAccount",
                table: "Transaction",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            
            migrationBuilder.CreateTable(
                name: "RecurringTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountVariancePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecurrenceCronExpression = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringTransaction_ToAccount",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecurringTransaction_ToTransactionCategory",
                        column: x => x.CategoryId,
                        principalTable: "TransactionCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecurringTransaction_ToTransactionType",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransaction_AccountId",
                table: "RecurringTransaction",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransaction_CategoryId",
                table: "RecurringTransaction",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransaction_TransactionTypeId",
                table: "RecurringTransaction",
                column: "TransactionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionType",
                table: "TransactionType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionSplit",
                table: "TransactionSplit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionCategory",
                table: "TransactionCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AccountType",
                table: "AccountType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Account",
                table: "Account");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Transact__3214EC07D26F23FD",
                table: "TransactionType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Transact__3214EC078C80138B",
                table: "TransactionSplit",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Transact__3214EC0788389A10",
                table: "TransactionCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__tmp_ms_x__3214EC07E906A3AC",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AccountT__3214EC0785814C1D",
                table: "AccountType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Account__3214EC072E8BD840",
                table: "Account",
                column: "Id");
        }
    }
}
