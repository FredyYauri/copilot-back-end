-- =============================================================================
-- CRM Database - Enhance Permissions Module
-- Adds Type, IsActive columns and CRUD stored procedures for Permissions
-- =============================================================================

USE [CRM_DB];
GO

-- =============================================================================
-- STEP 1: Add new columns to Permissions table
-- =============================================================================

-- Add Type column (page, action, button, field, api)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND name = N'Type')
BEGIN
    ALTER TABLE [dbo].[Permissions]
    ADD [Type] NVARCHAR(50) NOT NULL DEFAULT N'action';

    PRINT 'Column [Type] added to [Permissions].';
END
GO

-- Add IsActive column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND name = N'IsActive')
BEGIN
    ALTER TABLE [dbo].[Permissions]
    ADD [IsActive] BIT NOT NULL DEFAULT 1;

    PRINT 'Column [IsActive] added to [Permissions].';
END
GO

-- =============================================================================
-- STEP 2: Migrate existing permissions to set Type based on Action
-- =============================================================================
UPDATE [dbo].[Permissions]
SET [Type] = CASE
    WHEN [Action] IN (N'read')                             THEN N'page'
    WHEN [Action] IN (N'create', N'update', N'delete', N'close', N'assign') THEN N'action'
    WHEN [Action] IN (N'export')                           THEN N'button'
    ELSE N'action'
END
WHERE [IsDeleted] = 0;

PRINT 'Existing permissions migrated with Type values.';
GO

-- =============================================================================
-- STEP 3: Update existing Permission SPs to include new columns
-- =============================================================================

-- SP: sp_Permissions_GetById (updated)
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetById]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetById];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Resource], [Action], [Description], [Type], [IsActive],
           [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Permissions]
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- SP: sp_Permissions_GetAll (updated)
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetAll]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetAll];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Resource], [Action], [Description], [Type], [IsActive],
           [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
    ORDER BY [Resource], [Action];
END
GO

-- SP: sp_Permissions_GetByResource (updated)
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetByResource]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetByResource];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetByResource]
    @Resource NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Resource], [Action], [Description], [Type], [IsActive],
           [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Permissions]
    WHERE [Resource] = @Resource AND [IsDeleted] = 0
    ORDER BY [Action];
END
GO

-- SP: sp_Permissions_GetByUserId (updated)
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetByUserId]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetByUserId];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT p.[Id], p.[Resource], p.[Action], p.[Description], p.[Type], p.[IsActive],
           p.[IsDeleted], p.[CreatedAt], p.[CreatedBy], p.[LastModifiedAt], p.[LastModifiedBy]
    FROM [dbo].[Permissions] p
    INNER JOIN [dbo].[RolePermissions] rp ON rp.[PermissionId] = p.[Id]
    INNER JOIN [dbo].[Users] u ON u.[RoleId] = rp.[RoleId]
    WHERE u.[Id] = @UserId
      AND u.[IsDeleted] = 0
      AND u.[IsActive] = 1
      AND p.[IsDeleted] = 0
      AND p.[IsActive] = 1
    ORDER BY p.[Resource], p.[Action];
END
GO

-- SP: sp_Permissions_UserHasPermission (updated)
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
    INNER JOIN [dbo].[Users] u ON u.[RoleId] = rp.[RoleId]
    WHERE u.[Id] = @UserId
      AND p.[Resource] = @Resource
      AND p.[Action] = @Action
      AND u.[IsDeleted] = 0
      AND u.[IsActive] = 1
      AND p.[IsDeleted] = 0
      AND p.[IsActive] = 1;
END
GO

-- SP: sp_Roles_GetPermissions (updated to include new columns)
IF OBJECT_ID(N'[dbo].[sp_Roles_GetPermissions]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetPermissions];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetPermissions]
    @RoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.[Id], p.[Resource], p.[Action], p.[Description], p.[Type], p.[IsActive],
           p.[IsDeleted], p.[CreatedAt], p.[CreatedBy], p.[LastModifiedAt], p.[LastModifiedBy]
    FROM [dbo].[Permissions] p
    INNER JOIN [dbo].[RolePermissions] rp ON rp.[PermissionId] = p.[Id]
    WHERE rp.[RoleId] = @RoleId AND p.[IsDeleted] = 0
    ORDER BY p.[Resource], p.[Action];
END
GO

-- =============================================================================
-- STEP 4: New CRUD stored procedures for Permissions
-- =============================================================================

-- SP: sp_Permissions_GetPaged
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetPaged]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetPaged];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetPaged]
    @Page     INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Resource], [Action], [Description], [Type], [IsActive],
           [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
    ORDER BY [Resource], [Action]
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- SP: sp_Permissions_GetTotalCount
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetTotalCount]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetTotalCount];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM [dbo].[Permissions] WHERE [IsDeleted] = 0;
END
GO

-- SP: sp_Permissions_Insert
IF OBJECT_ID(N'[dbo].[sp_Permissions_Insert]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_Insert];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_Insert]
    @Id          UNIQUEIDENTIFIER,
    @Resource    NVARCHAR(100),
    @Action      NVARCHAR(100),
    @Description NVARCHAR(500),
    @Type        NVARCHAR(50),
    @IsActive    BIT,
    @CreatedAt   DATETIME2(7),
    @CreatedBy   NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [Type], [IsActive], [CreatedAt], [CreatedBy])
    VALUES (@Id, @Resource, @Action, @Description, @Type, @IsActive, @CreatedAt, @CreatedBy);

    SELECT @Id;
END
GO

-- SP: sp_Permissions_Update
IF OBJECT_ID(N'[dbo].[sp_Permissions_Update]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_Update];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_Update]
    @Id             UNIQUEIDENTIFIER,
    @Resource       NVARCHAR(100),
    @Action         NVARCHAR(100),
    @Description    NVARCHAR(500),
    @Type           NVARCHAR(50),
    @LastModifiedAt DATETIME2(7),
    @LastModifiedBy NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Permissions]
    SET [Resource]       = @Resource,
        [Action]         = @Action,
        [Description]    = @Description,
        [Type]           = @Type,
        [LastModifiedAt] = @LastModifiedAt,
        [LastModifiedBy] = @LastModifiedBy
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- SP: sp_Permissions_ToggleStatus
IF OBJECT_ID(N'[dbo].[sp_Permissions_ToggleStatus]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_ToggleStatus];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_ToggleStatus]
    @Id             UNIQUEIDENTIFIER,
    @IsActive       BIT,
    @LastModifiedAt DATETIME2(7),
    @LastModifiedBy NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Permissions]
    SET [IsActive]       = @IsActive,
        [LastModifiedAt] = @LastModifiedAt,
        [LastModifiedBy] = @LastModifiedBy
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- SP: sp_Permissions_ExistsByResourceAction
IF OBJECT_ID(N'[dbo].[sp_Permissions_ExistsByResourceAction]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_ExistsByResourceAction];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_ExistsByResourceAction]
    @Resource NVARCHAR(100),
    @Action   NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM [dbo].[Permissions]
    WHERE [Resource] = @Resource AND [Action] = @Action AND [IsDeleted] = 0;
END
GO

-- SP: sp_Permissions_GetPermissionCodes_ByUserId (returns just resource.action codes)
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetCodes_ByUserId]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetCodes_ByUserId];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetCodes_ByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT p.[Resource] + N'.' + p.[Action] AS [Code]
    FROM [dbo].[Permissions] p
    INNER JOIN [dbo].[RolePermissions] rp ON rp.[PermissionId] = p.[Id]
    INNER JOIN [dbo].[Users] u ON u.[RoleId] = rp.[RoleId]
    WHERE u.[Id] = @UserId
      AND u.[IsDeleted] = 0
      AND u.[IsActive] = 1
      AND p.[IsDeleted] = 0
      AND p.[IsActive] = 1
    ORDER BY [Code];
END
GO

PRINT 'Permissions module enhanced successfully.';
GO
