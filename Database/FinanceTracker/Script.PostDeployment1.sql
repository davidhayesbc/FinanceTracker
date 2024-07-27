--Transaction Types
IF NOT EXISTS (SELECT 1
FROM TransactionType
WHERE Type = 'Change')
BEGIN
    INSERT INTO TransactionType
        (Type)
    VALUES
        ('Change');
END
IF NOT EXISTS (SELECT 1
FROM TransactionType
WHERE Type = 'BalanceUpdate')
BEGIN
    INSERT INTO TransactionType
        (Type)
    VALUES
        ('BalanceUpdate');
END
--Account Types
IF NOT EXISTS (SELECT 1
FROM AccountType
WHERE Type = 'Savings')
BEGIN
    INSERT INTO AccountType
        (Type)
    VALUES
        ('Savings');
END
IF NOT EXISTS (SELECT 1
FROM AccountType
WHERE Type = 'Checking')
BEGIN
    INSERT INTO AccountType
        (Type)
    VALUES
        ('Checking');
END
IF NOT EXISTS (SELECT 1
FROM AccountType
WHERE Type = 'Investment')
BEGIN
    INSERT INTO AccountType
        (Type)
    VALUES
        ('Investment');
END
