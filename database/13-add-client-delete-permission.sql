-- =============================================================================
-- CRM Database - Add clients.delete permission and sp_Clients_Delete procedure
-- =============================================================================

USE [CRM_DB];
GO

-- =============================================================================
-- STEP 1: Create sp_Clients_Delete stored procedure (soft delete)
-- =============================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_Delete]'))
    DROP PROCEDURE [dbo].[sp_Clients_Delete]
GO
CREATE PROCEDURE [dbo].[sp_Clients_Delete]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Clients]
    SET [IsDeleted] = 1, [IsActive] = 0, [LastModifiedAt] = SYSUTCDATETIME()
    WHERE [Id] = @Id AND [IsDeleted] = 0;

    -- Soft delete associated contacts
    UPDATE [dbo].[ClientContacts]
    SET [IsDeleted] = 1, [LastModifiedAt] = SYSUTCDATETIME()
    WHERE [ClientId] = @Id AND [IsDeleted] = 0;
END
GO

-- =============================================================================
-- STEP 2: Insert clients.delete permission
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = N'clients' AND [Action] = N'delete' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [Type], [IsActive], [CreatedAt], [IsDeleted])
    VALUES (NEWID(), N'clients', N'delete', N'Eliminar clientes del directorio', N'action', 1, GETUTCDATE(), 0);
    PRINT 'Permission clients.delete created.';
END
GO

-- =============================================================================
-- STEP 3: Assign clients.delete permission to Administrador role
-- =============================================================================
DECLARE @AdminRoleId UNIQUEIDENTIFIER;
SELECT @AdminRoleId = [Id] FROM [dbo].[Roles] WHERE [Name] = N'Administrador' AND [IsDeleted] = 0;

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
    SELECT @AdminRoleId, p.[Id], GETUTCDATE()
    FROM [dbo].[Permissions] p
    WHERE p.[Resource] = N'clients'
      AND p.[Action] = N'delete'
      AND p.[IsDeleted] = 0
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[RolePermissions] rp
          WHERE rp.[RoleId] = @AdminRoleId AND rp.[PermissionId] = p.[Id]
      );

    PRINT 'Permission clients.delete assigned to Administrador role.';
END
GO
