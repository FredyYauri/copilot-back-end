-- =============================================================================
-- CRM Database - Add Permission Management permissions
-- Adds permissions.read, permissions.create, permissions.update
-- and assigns them to the Administrador role
-- =============================================================================

USE [CRM_DB];
GO

-- =============================================================================
-- STEP 1: Insert new permissions for the Permissions module
-- =============================================================================

-- permissions.read
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = N'permissions' AND [Action] = N'read' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [Type], [IsActive], [CreatedAt], [IsDeleted])
    VALUES (NEWID(), N'permissions', N'read', N'Ver listado de permisos del sistema', N'page', 1, GETUTCDATE(), 0);
    PRINT 'Permission permissions.read created.';
END
GO

-- permissions.create
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = N'permissions' AND [Action] = N'create' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [Type], [IsActive], [CreatedAt], [IsDeleted])
    VALUES (NEWID(), N'permissions', N'create', N'Crear nuevos permisos en el sistema', N'action', 1, GETUTCDATE(), 0);
    PRINT 'Permission permissions.create created.';
END
GO

-- permissions.update
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = N'permissions' AND [Action] = N'update' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [Type], [IsActive], [CreatedAt], [IsDeleted])
    VALUES (NEWID(), N'permissions', N'update', N'Editar permisos existentes y cambiar su estado', N'action', 1, GETUTCDATE(), 0);
    PRINT 'Permission permissions.update created.';
END
GO

-- =============================================================================
-- STEP 2: Assign new permissions to Administrador role
-- =============================================================================

DECLARE @AdminRoleId UNIQUEIDENTIFIER;
SELECT @AdminRoleId = [Id] FROM [dbo].[Roles] WHERE [Name] = N'Administrador' AND [IsDeleted] = 0;

IF @AdminRoleId IS NOT NULL
BEGIN
    -- Assign all new permissions that are not yet assigned
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
    SELECT @AdminRoleId, p.[Id], GETUTCDATE()
    FROM [dbo].[Permissions] p
    WHERE p.[Resource] = N'permissions'
      AND p.[IsDeleted] = 0
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[RolePermissions] rp
          WHERE rp.[RoleId] = @AdminRoleId AND rp.[PermissionId] = p.[Id]
      );

    PRINT 'Permissions assigned to Administrador role.';
END
GO
