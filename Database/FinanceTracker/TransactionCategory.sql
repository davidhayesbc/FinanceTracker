CREATE TABLE [dbo].[TransactionCategory]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
  [Category] NVARCHAR(50) NOT NULL,
  [Description] NVARCHAR(100) NOT NULL
)
