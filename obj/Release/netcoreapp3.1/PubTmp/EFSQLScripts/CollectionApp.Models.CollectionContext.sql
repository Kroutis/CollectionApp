IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200412001633_AddProperty')
BEGIN
    CREATE TABLE [Collections] (
        [Id] int NOT NULL IDENTITY,
        [UserName] nvarchar(max) NULL,
        [CollectionName] nvarchar(max) NOT NULL,
        [CreationDate] nvarchar(max) NULL,
        [Likes] int NOT NULL,
        [Image] varbinary(max) NULL,
        [ItemCount] int NOT NULL,
        [Text] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Collections] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200412001633_AddProperty')
BEGIN
    CREATE TABLE [ItemComments] (
        [Id] int NOT NULL IDENTITY,
        [ItemId] int NOT NULL,
        [Text] nvarchar(max) NULL,
        [UserName] nvarchar(max) NULL,
        CONSTRAINT [PK_ItemComments] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200412001633_AddProperty')
BEGIN
    CREATE TABLE [Items] (
        [Id] int NOT NULL IDENTITY,
        [CollectionId] int NOT NULL,
        [ItemName] nvarchar(max) NOT NULL,
        [Likes] int NOT NULL,
        [Image] varbinary(max) NULL,
        [UserName] nvarchar(max) NULL,
        [Text] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Items] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200412001633_AddProperty')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200412001633_AddProperty', N'3.1.3');
END;

GO

