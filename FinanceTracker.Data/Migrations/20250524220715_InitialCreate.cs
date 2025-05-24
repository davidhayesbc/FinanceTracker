using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AccountType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DisplaySymbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountTypeId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_ToAccountType",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Account_ToCurrency",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
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
                name: "AccountPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: true),
                    PeriodCloseDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountPeriod_ToAccount",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                }); migrationBuilder.CreateTable(
                name: "CashAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OverdraftLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashAccount_Account_Id",
                        column: x => x.Id,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                }); migrationBuilder.CreateTable(
                name: "InvestmentAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    BrokerAccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsTaxAdvantaged = table.Column<bool>(type: "bit", nullable: false),
                    TaxAdvantageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentAccount_Account_Id",
                        column: x => x.Id,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    AccountPeriodId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_ToAccountPeriod",
                        column: x => x.AccountPeriodId,
                        principalTable: "AccountPeriod",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transaction_ToTransactionCategory",
                        column: x => x.CategoryId,
                        principalTable: "TransactionCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transaction_ToTransactionType",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionType",
                        principalColumn: "Id");
                });

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
                    CashAccountId = table.Column<int>(type: "int", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringTransaction_ToCashAccount",
                        column: x => x.CashAccountId,
                        principalTable: "CashAccount",
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

            migrationBuilder.CreateTable(
                name: "CashTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferToCashAccountId = table.Column<int>(type: "int", nullable: true),
                    CashAccountId = table.Column<int>(type: "int", nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_CashTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashTransaction_CashAccount_CashAccountId",
                        column: x => x.CashAccountId,
                        principalTable: "CashAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashTransaction_ToTransferAccount",
                        column: x => x.TransferToCashAccountId,
                        principalTable: "CashAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CashTransaction_Transaction_Id",
                        column: x => x.Id,
                        principalTable: "Transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Commission = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecurityId = table.Column<int>(type: "int", nullable: false),
                    InvestmentAccountId = table.Column<int>(type: "int", nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentTransaction_InvestmentAccount_InvestmentAccountId",
                        column: x => x.InvestmentAccountId,
                        principalTable: "InvestmentAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvestmentTransaction_ToSecurity",
                        column: x => x.SecurityId,
                        principalTable: "Security",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvestmentTransaction_Transaction_Id",
                        column: x => x.Id,
                        principalTable: "Transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionSplit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CashTransactionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionSplit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionSplit_ToCashTransaction",
                        column: x => x.CashTransactionId,
                        principalTable: "CashTransaction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionSplit_ToTransactionCategory",
                        column: x => x.CategoryId,
                        principalTable: "TransactionCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccountTypeId",
                table: "Account",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_CurrencyId",
                table: "Account",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPeriod_AccountId",
                table: "AccountPeriod",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "UX_AccountType_Type",
                table: "AccountType",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashTransaction_CashAccountId",
                table: "CashTransaction",
                column: "CashAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashTransaction_TransferToCashAccountId",
                table: "CashTransaction",
                column: "TransferToCashAccountId");

            migrationBuilder.CreateIndex(
                name: "UX_Currency_Symbol",
                table: "Currency",
                column: "Symbol",
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
                name: "IX_InvestmentTransaction_InvestmentAccountId",
                table: "InvestmentTransaction",
                column: "InvestmentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransaction_SecurityId",
                table: "InvestmentTransaction",
                column: "SecurityId");

            migrationBuilder.CreateIndex(
                name: "UX_Price_SecurityId_Date",
                table: "Price",
                columns: new[] { "SecurityId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransaction_CashAccountId",
                table: "RecurringTransaction",
                column: "CashAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransaction_CategoryId",
                table: "RecurringTransaction",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransaction_TransactionTypeId",
                table: "RecurringTransaction",
                column: "TransactionTypeId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AccountPeriodId",
                table: "Transaction",
                column: "AccountPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CategoryId",
                table: "Transaction",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TransactionTypeId",
                table: "Transaction",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_TransactionCategory_Category",
                table: "TransactionCategory",
                column: "Category",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionSplit_CashTransactionId",
                table: "TransactionSplit",
                column: "CashTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionSplit_CategoryId",
                table: "TransactionSplit",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "UX_TransactionType_Type",
                table: "TransactionType",
                column: "Type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FxRate");

            migrationBuilder.DropTable(
                name: "InvestmentTransaction");

            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropTable(
                name: "RecurringTransaction");

            migrationBuilder.DropTable(
                name: "TransactionSplit");

            migrationBuilder.DropTable(
                name: "InvestmentAccount");

            migrationBuilder.DropTable(
                name: "Security");

            migrationBuilder.DropTable(
                name: "CashTransaction");

            migrationBuilder.DropTable(
                name: "CashAccount");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "AccountPeriod");

            migrationBuilder.DropTable(
                name: "TransactionCategory");

            migrationBuilder.DropTable(
                name: "TransactionType");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "AccountType");

            migrationBuilder.DropTable(
                name: "Currency");
        }
    }
}
