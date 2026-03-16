-- =============================================================================
-- CRM Database - Migration: Users.Role (NVARCHAR) → Users.RoleId (FK → Roles)
-- =============================================================================

USE [CRM_DB];
GO

-- =============================================================================
-- STEP 1: Add RoleId column to Users table
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'RoleId')
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [RoleId] UNIQUEIDENTIFIER NULL;

    PRINT 'Column [RoleId] added to [Users] table.';
END
GO

-- =============================================================================
-- STEP 2: Migrate existing data - map Role name → Roles.Id
-- =============================================================================

-- Map 'Admin' → 'Administrador'
UPDATE u
SET u.[RoleId] = r.[Id]
FROM [dbo].[Users] u
INNER JOIN [dbo].[Roles] r ON r.[Name] = N'Administrador'
WHERE u.[Role] = N'Admin' AND u.[RoleId] IS NULL;

-- Map 'Manager' → 'Gerente'
UPDATE u
SET u.[RoleId] = r.[Id]
FROM [dbo].[Users] u
INNER JOIN [dbo].[Roles] r ON r.[Name] = N'Gerente'
WHERE u.[Role] = N'Manager' AND u.[RoleId] IS NULL;

-- Map 'User' → 'Vendedor' (default role for regular users)
UPDATE u
SET u.[RoleId] = r.[Id]
FROM [dbo].[Users] u
INNER JOIN [dbo].[Roles] r ON r.[Name] = N'Vendedor'
WHERE u.[Role] = N'User' AND u.[RoleId] IS NULL;

-- Catch any remaining unmapped users → assign 'Vendedor' as default
UPDATE u
SET u.[RoleId] = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = N'Vendedor')
FROM [dbo].[Users] u
WHERE u.[RoleId] IS NULL;
GO

PRINT 'Data migration completed: Role names mapped to RoleIds.';
GO

-- =============================================================================
-- STEP 3: Make RoleId NOT NULL and add FK constraint
-- =============================================================================
ALTER TABLE [dbo].[Users]
ALTER COLUMN [RoleId] UNIQUEIDENTIFIER NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Users_Roles')
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]);

    PRINT 'Foreign key [FK_Users_Roles] added.';
END
GO

-- Create index on RoleId
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_RoleId' AND object_id = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_RoleId]
        ON [dbo].[Users] ([RoleId])
        WHERE [IsDeleted] = 0;

    PRINT 'Index [IX_Users_RoleId] created.';
END
GO

-- =============================================================================
-- STEP 4: Drop old Role column
-- =============================================================================
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Role')
BEGIN
    -- Drop default constraint on Role column first
    DECLARE @constraintName NVARCHAR(256);
    SELECT @constraintName = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.object_id = OBJECT_ID(N'[dbo].[Users]') AND c.name = 'Role';

    IF @constraintName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [dbo].[Users] DROP CONSTRAINT [' + @constraintName + ']');
        PRINT 'Default constraint on [Role] dropped.';
    END

    ALTER TABLE [dbo].[Users] DROP COLUMN [Role];
    PRINT 'Column [Role] dropped from [Users] table.';
END
GO

-- =============================================================================
-- STEP 5: Update stored procedures to use RoleId instead of Role
-- =============================================================================

-- SP: sp_Users_GetById
IF OBJECT_ID(N'[dbo].[sp_Users_GetById]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetById];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[PasswordHash],
        u.[RoleId],
        r.[Name] AS [RoleName],
        u.[IsActive],
        u.[IsDeleted],
        u.[CreatedAt],
        u.[CreatedBy],
        u.[LastModifiedAt],
        u.[LastModifiedBy]
    FROM [dbo].[Users] u
    INNER JOIN [dbo].[Roles] r ON r.[Id] = u.[RoleId]
    WHERE u.[Id] = @Id
      AND u.[IsDeleted] = 0;
END
GO

-- SP: sp_Users_GetByEmail
IF OBJECT_ID(N'[dbo].[sp_Users_GetByEmail]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetByEmail];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetByEmail]
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[PasswordHash],
        u.[RoleId],
        r.[Name] AS [RoleName],
        u.[IsActive],
        u.[IsDeleted],
        u.[CreatedAt],
        u.[CreatedBy],
        u.[LastModifiedAt],
        u.[LastModifiedBy]
    FROM [dbo].[Users] u
    INNER JOIN [dbo].[Roles] r ON r.[Id] = u.[RoleId]
    WHERE u.[Email] = @Email
      AND u.[IsDeleted] = 0;
END
GO

-- SP: sp_Users_Insert (now uses RoleId)
IF OBJECT_ID(N'[dbo].[sp_Users_Insert]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_Insert];
GO

CREATE PROCEDURE [dbo].[sp_Users_Insert]
    @Id             UNIQUEIDENTIFIER,
    @FirstName      NVARCHAR(100),
    @LastName       NVARCHAR(100),
    @Email          NVARCHAR(256),
    @PasswordHash   NVARCHAR(500),
    @RoleId         UNIQUEIDENTIFIER,
    @IsActive       BIT,
    @CreatedAt      DATETIME2(7),
    @CreatedBy      NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [PasswordHash], [RoleId], [IsActive], [CreatedAt], [CreatedBy])
    VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @RoleId, @IsActive, @CreatedAt, @CreatedBy);

    SELECT @Id;
END
GO

-- SP: sp_Users_Update (now uses RoleId)
IF OBJECT_ID(N'[dbo].[sp_Users_Update]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_Update];
GO

CREATE PROCEDURE [dbo].[sp_Users_Update]
    @Id             UNIQUEIDENTIFIER,
    @FirstName      NVARCHAR(100),
    @LastName       NVARCHAR(100),
    @Email          NVARCHAR(256),
    @RoleId         UNIQUEIDENTIFIER,
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
        [RoleId]         = @RoleId,
        [IsActive]       = @IsActive,
        [LastModifiedAt] = @LastModifiedAt,
        [LastModifiedBy] = @LastModifiedBy
    WHERE [Id] = @Id
      AND [IsDeleted] = 0;
END
GO

-- SP: sp_Users_GetAll (now includes RoleName)
IF OBJECT_ID(N'[dbo].[sp_Users_GetAll]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetAll];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetAll]
    @Page     INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[PasswordHash],
        u.[RoleId],
        r.[Name] AS [RoleName],
        u.[IsActive],
        u.[IsDeleted],
        u.[CreatedAt],
        u.[CreatedBy],
        u.[LastModifiedAt],
        u.[LastModifiedBy]
    FROM [dbo].[Users] u
    INNER JOIN [dbo].[Roles] r ON r.[Id] = u.[RoleId]
    WHERE u.[IsDeleted] = 0
    ORDER BY u.[CreatedAt] DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- SP: sp_Users_GetActiveAdminCount (now uses Roles table)
IF OBJECT_ID(N'[dbo].[sp_Users_GetActiveAdminCount]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetActiveAdminCount];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetActiveAdminCount]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM [dbo].[Users] u
    INNER JOIN [dbo].[Roles] r ON r.[Id] = u.[RoleId]
    WHERE r.[Name] = N'Administrador'
      AND u.[IsActive] = 1
      AND u.[IsDeleted] = 0;
END
GO

-- SP: sp_Permissions_GetByUserId (now uses RoleId FK)
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetByUserId]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetByUserId];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT p.[Id], p.[Resource], p.[Action], p.[Description],
           p.[IsDeleted], p.[CreatedAt], p.[CreatedBy], p.[LastModifiedAt], p.[LastModifiedBy]
    FROM [dbo].[Permissions] p
    INNER JOIN [dbo].[RolePermissions] rp ON rp.[PermissionId] = p.[Id]
    INNER JOIN [dbo].[Roles] r ON r.[Id] = rp.[RoleId]
    INNER JOIN [dbo].[Users] u ON u.[RoleId] = r.[Id]
    WHERE u.[Id] = @UserId
      AND u.[IsDeleted] = 0
      AND u.[IsActive] = 1
      AND r.[IsDeleted] = 0
      AND r.[IsActive] = 1
      AND p.[IsDeleted] = 0
    ORDER BY p.[Resource], p.[Action];
END
GO

-- SP: sp_Permissions_UserHasPermission (now uses RoleId FK)
IF OBJECT_ID(N'[dbo].[sp_Permissions_UserHasPermission]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_UserHasPermission];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_UserHasPermission]
    @UserId   UNIQUEIDENTIFIER,
    @Resource NVARCHAR(100),
    @Action   NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM [dbo].[Permissions] p
    INNER JOIN [dbo].[RolePermissions] rp ON rp.[PermissionId] = p.[Id]
    INNER JOIN [dbo].[Roles] r ON r.[Id] = rp.[RoleId]
    INNER JOIN [dbo].[Users] u ON u.[RoleId] = r.[Id]
    WHERE u.[Id] = @UserId
      AND p.[Resource] = @Resource
      AND p.[Action] = @Action
      AND u.[IsDeleted] = 0
      AND u.[IsActive] = 1
      AND r.[IsDeleted] = 0
      AND r.[IsActive] = 1
      AND p.[IsDeleted] = 0;
END
GO

-- SP: sp_Roles_GetUsersCountByRoleName (now uses RoleId FK)
IF OBJECT_ID(N'[dbo].[sp_Roles_GetUsersCountByRoleName]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetUsersCountByRoleName];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetUsersCountByRoleName]
    @RoleName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM [dbo].[Users] u
    INNER JOIN [dbo].[Roles] r ON r.[Id] = u.[RoleId]
    WHERE r.[Name] = @RoleName AND u.[IsDeleted] = 0 AND u.[IsActive] = 1;
END
GO

-- Update seed: Re-assign admin user to 'Administrador' role (in case migration missed it)
UPDATE u
SET u.[RoleId] = r.[Id]
FROM [dbo].[Users] u
INNER JOIN [dbo].[Roles] r ON r.[Name] = N'Administrador'
WHERE u.[Email] = N'admin@crm.com' AND u.[IsDeleted] = 0;
GO

PRINT 'Migration completed: Users.Role → Users.RoleId (FK → Roles).';
GO
