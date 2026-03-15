-- =============================================================================
-- CRM Database - Stored Procedures
-- =============================================================================

USE [CRM_DB];
GO

-- =============================================================================
-- SP: sp_Users_GetById
-- Description: Gets a user by their unique identifier
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_GetById]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetById];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [FirstName],
        [LastName],
        [Email],
        [PasswordHash],
        [Role],
        [IsActive],
        [IsDeleted],
        [CreatedAt],
        [CreatedBy],
        [LastModifiedAt],
        [LastModifiedBy]
    FROM [dbo].[Users]
    WHERE [Id] = @Id
      AND [IsDeleted] = 0;
END
GO

-- =============================================================================
-- SP: sp_Users_GetByEmail
-- Description: Gets a user by their email address
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_GetByEmail]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetByEmail];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetByEmail]
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [FirstName],
        [LastName],
        [Email],
        [PasswordHash],
        [Role],
        [IsActive],
        [IsDeleted],
        [CreatedAt],
        [CreatedBy],
        [LastModifiedAt],
        [LastModifiedBy]
    FROM [dbo].[Users]
    WHERE [Email] = @Email
      AND [IsDeleted] = 0;
END
GO

-- =============================================================================
-- SP: sp_Users_ExistsByEmail
-- Description: Checks if a user with the given email exists
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_ExistsByEmail]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_ExistsByEmail];
GO

CREATE PROCEDURE [dbo].[sp_Users_ExistsByEmail]
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM [dbo].[Users]
    WHERE [Email] = @Email
      AND [IsDeleted] = 0;
END
GO

-- =============================================================================
-- SP: sp_Users_Insert
-- Description: Inserts a new user and returns the generated ID
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_Insert]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_Insert];
GO

CREATE PROCEDURE [dbo].[sp_Users_Insert]
    @Id             UNIQUEIDENTIFIER,
    @FirstName      NVARCHAR(100),
    @LastName       NVARCHAR(100),
    @Email          NVARCHAR(256),
    @PasswordHash   NVARCHAR(500),
    @Role           NVARCHAR(50),
    @IsActive       BIT,
    @CreatedAt      DATETIME2(7),
    @CreatedBy      NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [PasswordHash], [Role], [IsActive], [CreatedAt], [CreatedBy])
    VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @Role, @IsActive, @CreatedAt, @CreatedBy);

    SELECT @Id;
END
GO

-- =============================================================================
-- SP: sp_Users_Update
-- Description: Updates an existing user's profile information
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_Update]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_Update];
GO

CREATE PROCEDURE [dbo].[sp_Users_Update]
    @Id             UNIQUEIDENTIFIER,
    @FirstName      NVARCHAR(100),
    @LastName       NVARCHAR(100),
    @Email          NVARCHAR(256),
    @Role           NVARCHAR(50),
    @IsActive       BIT,
    @LastModifiedAt  DATETIME2(7),
    @LastModifiedBy  NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Users]
    SET [FirstName]      = @FirstName,
        [LastName]       = @LastName,
        [Email]          = @Email,
        [Role]           = @Role,
        [IsActive]       = @IsActive,
        [LastModifiedAt] = @LastModifiedAt,
        [LastModifiedBy] = @LastModifiedBy
    WHERE [Id] = @Id
      AND [IsDeleted] = 0;
END
GO

-- =============================================================================
-- SP: sp_Users_UpdatePasswordHash
-- Description: Updates a user's password hash
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_UpdatePasswordHash]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_UpdatePasswordHash];
GO

CREATE PROCEDURE [dbo].[sp_Users_UpdatePasswordHash]
    @Id             UNIQUEIDENTIFIER,
    @PasswordHash   NVARCHAR(500),
    @LastModifiedAt  DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Users]
    SET [PasswordHash]   = @PasswordHash,
        [LastModifiedAt] = @LastModifiedAt
    WHERE [Id] = @Id
      AND [IsDeleted] = 0;
END
GO

-- =============================================================================
-- SP: sp_Users_SoftDelete
-- Description: Performs a soft delete on a user
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_SoftDelete]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_SoftDelete];
GO

CREATE PROCEDURE [dbo].[sp_Users_SoftDelete]
    @Id              UNIQUEIDENTIFIER,
    @LastModifiedAt  DATETIME2(7),
    @LastModifiedBy  NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Users]
    SET [IsDeleted]      = 1,
        [IsActive]       = 0,
        [LastModifiedAt] = @LastModifiedAt,
        [LastModifiedBy] = @LastModifiedBy
    WHERE [Id] = @Id
      AND [IsDeleted] = 0;
END
GO

PRINT 'Stored procedures created successfully.';
GO
