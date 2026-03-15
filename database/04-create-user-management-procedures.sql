-- =============================================================================
-- CRM Database - User Management Stored Procedures
-- =============================================================================

USE [CRM_DB];
GO

-- =============================================================================
-- SP: sp_Users_GetAll
-- Description: Gets all users with pagination (excludes soft-deleted)
-- =============================================================================
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
    WHERE [IsDeleted] = 0
    ORDER BY [CreatedAt] DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- =============================================================================
-- SP: sp_Users_GetTotalCount
-- Description: Gets the total count of non-deleted users
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_GetTotalCount]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetTotalCount];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM [dbo].[Users]
    WHERE [IsDeleted] = 0;
END
GO

-- =============================================================================
-- SP: sp_Users_GetActiveAdminCount
-- Description: Gets the count of active administrators
-- =============================================================================
IF OBJECT_ID(N'[dbo].[sp_Users_GetActiveAdminCount]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Users_GetActiveAdminCount];
GO

CREATE PROCEDURE [dbo].[sp_Users_GetActiveAdminCount]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM [dbo].[Users]
    WHERE [Role] = N'Admin'
      AND [IsActive] = 1
      AND [IsDeleted] = 0;
END
GO

PRINT 'User management stored procedures created successfully.';
GO
