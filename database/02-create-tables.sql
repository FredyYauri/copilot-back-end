-- =============================================================================
-- CRM Database - Tables Creation Script
-- =============================================================================

USE [CRM_DB];
GO

-- -----------------------------------------------------------------------------
-- Table: Users
-- Description: Stores user accounts for authentication and authorization
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [Id]              UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [FirstName]       NVARCHAR(100)     NOT NULL,
        [LastName]        NVARCHAR(100)     NOT NULL,
        [Email]           NVARCHAR(256)     NOT NULL,
        [PasswordHash]    NVARCHAR(500)     NOT NULL,
        [Role]            NVARCHAR(50)      NOT NULL DEFAULT 'User',
        [IsActive]        BIT               NOT NULL DEFAULT 1,
        [IsDeleted]       BIT               NOT NULL DEFAULT 0,
        [CreatedAt]       DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy]       NVARCHAR(256)     NULL,
        [LastModifiedAt]  DATETIME2(7)      NULL,
        [LastModifiedBy]  NVARCHAR(256)     NULL,

        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email])
    );

    CREATE NONCLUSTERED INDEX [IX_Users_Email]
        ON [dbo].[Users] ([Email])
        WHERE [IsDeleted] = 0;

    CREATE NONCLUSTERED INDEX [IX_Users_IsActive]
        ON [dbo].[Users] ([IsActive])
        WHERE [IsDeleted] = 0;
END
GO

-- -----------------------------------------------------------------------------
-- Table: RefreshTokens
-- Description: Stores refresh tokens for JWT session management
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshTokens]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[RefreshTokens]
    (
        [Id]          UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [UserId]      UNIQUEIDENTIFIER  NOT NULL,
        [Token]       NVARCHAR(500)     NOT NULL,
        [ExpiresAt]   DATETIME2(7)      NOT NULL,
        [CreatedAt]   DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [RevokedAt]   DATETIME2(7)      NULL,
        [IsRevoked]   BIT               NOT NULL DEFAULT 0,

        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId]
        ON [dbo].[RefreshTokens] ([UserId]);

    CREATE NONCLUSTERED INDEX [IX_RefreshTokens_Token]
        ON [dbo].[RefreshTokens] ([Token])
        WHERE [IsRevoked] = 0;
END
GO

-- -----------------------------------------------------------------------------
-- Seed: Default admin user
-- Password: Admin@123 (BCrypt hash)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = N'admin@crm.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [PasswordHash], [Role], [IsActive], [CreatedAt], [CreatedBy])
    VALUES (
        NEWID(),
        N'Admin',
        N'Sistema',
        N'admin@crm.com',
        N'$2a$12$ypLVHu80x8k4EFZbhV/aue6BYF/jSEgBNtbPwSQNb57iiptL3o4VS',  -- Admin@123
        N'Admin',
        1,
        SYSUTCDATETIME(),
        N'system'
    );

    PRINT 'Default admin user seeded: admin@crm.com / Admin@123';
END
GO

PRINT 'Tables created successfully.';
GO
