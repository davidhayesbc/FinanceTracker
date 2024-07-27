CREATE TABLE [dbo].[Account]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  [Name] NVARCHAR(50) NOT NULL,
  [OpeningBalance] DECIMAL(18, 2) NOT NULL,
  [OpenDate] DATE NOT NULL,
  [AccountTypeId] INT NOT NULL, 
    CONSTRAINT [FK_Account_ToAccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [AccountType]([Id])
)
