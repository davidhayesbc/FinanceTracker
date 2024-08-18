CREATE TABLE [dbo].[Transaction]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
  [TransactionDate] DATE NOT NULL,
  [Amount] DECIMAL(18, 2) NOT NULL,
  [Description] NVARCHAR(100) NOT NULL,
  [AccountId] INT NOT NULL,
  CONSTRAINT [FK_Transaction_ToAccount] FOREIGN KEY ([AccountId]) REFERENCES [Account]([Id]),
  [TransactionTypeId] INT NOT NULL,
  CONSTRAINT [FK_Transaction_ToTransactionType] FOREIGN KEY ([TransactionTypeId]) REFERENCES [TransactionType]([Id]),
  [CategoryId] INT NOT NULL,
  CONSTRAINT [FK_Transaction_ToTransactionCategory] FOREIGN KEY ([CategoryId]) REFERENCES [TransactionCategory]([Id])
)
