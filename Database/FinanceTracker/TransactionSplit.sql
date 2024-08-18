CREATE TABLE [dbo].[TransactionSplit]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
  [TransactionId] INT NOT NULL,
  CONSTRAINT [FK_TransactionSplit_ToTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction]([Id]),
  [Amount] DECIMAL(18, 2) NOT NULL,
  [Description] NVARCHAR(100) NOT NULL,
  [CategoryId] INT NOT NULL,
  CONSTRAINT [FK_TransactionSplit_ToTransactionCategory] FOREIGN KEY ([CategoryId]) REFERENCES [TransactionCategory]([Id])
)
