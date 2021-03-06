Build started...
Build succeeded.
[14:59:38 INF] Wired Logging for HelloWorldWeb
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [dbo].[Users] (
    [UserId] int NOT NULL,
    [UserName] nvarchar(50) NOT NULL,
    [FullName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);

GO

CREATE TABLE [dbo].[Messages] (
    [MessageId] int NOT NULL IDENTITY,
    [Message] nvarchar(100) NOT NULL,
    [MessageBody] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([MessageId]),
    CONSTRAINT [FK_Messages_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_Messages_UserId] ON [dbo].[Messages] ([UserId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201031195214_tables', N'3.1.9');

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'UserName') AND [object_id] = OBJECT_ID(N'[dbo].[Users]'))
    SET IDENTITY_INSERT [dbo].[Users] ON;
INSERT INTO [dbo].[Users] ([UserId], [FullName], [UserName])
VALUES (8902550, N'Jane Smith', N'jane');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'UserName') AND [object_id] = OBJECT_ID(N'[dbo].[Users]'))
    SET IDENTITY_INSERT [dbo].[Users] OFF;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'UserName') AND [object_id] = OBJECT_ID(N'[dbo].[Users]'))
    SET IDENTITY_INSERT [dbo].[Users] ON;
INSERT INTO [dbo].[Users] ([UserId], [FullName], [UserName])
VALUES (8775895, N'Bob Smith', N'bob');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'UserName') AND [object_id] = OBJECT_ID(N'[dbo].[Users]'))
    SET IDENTITY_INSERT [dbo].[Users] OFF;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MessageId', N'Message', N'MessageBody', N'UserId') AND [object_id] = OBJECT_ID(N'[dbo].[Messages]'))
    SET IDENTITY_INSERT [dbo].[Messages] ON;
INSERT INTO [dbo].[Messages] ([MessageId], [Message], [MessageBody], [UserId])
VALUES (1, N'Hello World', N'Hello World and all of its inhabitants!', 8902550),
(2, N'Greetings', N'Greeting Jane', 8902550),
(3, N'Invitation', N'Jane, you are cordially invited to the Halloween gala. Costumes are encouraged.', 8902550),
(4, N'Invitation', N'Bob, we are looking forward to seeing you this halloween. We ask that you please wear a fitting costume (preferably not the homeless look again).', 8775895),
(5, N'William, it is your mother. Call me!', N'William Smith, for nine months I carried you in my belly.. Is calling your mother once a week too much to ask for?.', 8775895);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MessageId', N'Message', N'MessageBody', N'UserId') AND [object_id] = OBJECT_ID(N'[dbo].[Messages]'))
    SET IDENTITY_INSERT [dbo].[Messages] OFF;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201031195808_seed-data', N'3.1.9');

GO


