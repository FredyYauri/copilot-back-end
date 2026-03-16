-- =============================================================================
-- CRM Database - RBAC Tables & Stored Procedures
-- =============================================================================

USE [CRM_DB];
GO

-- =============================================================================
-- TABLES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Table: Permissions
-- Description: Stores granular permissions (resource.action)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Permissions]
    (
        [Id]              UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Resource]        NVARCHAR(100)     NOT NULL,
        [Action]          NVARCHAR(100)     NOT NULL,
        [Description]     NVARCHAR(500)     NOT NULL,
        [IsDeleted]       BIT               NOT NULL DEFAULT 0,
        [CreatedAt]       DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy]       NVARCHAR(256)     NULL,
        [LastModifiedAt]  DATETIME2(7)      NULL,
        [LastModifiedBy]  NVARCHAR(256)     NULL,

        CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Permissions_Resource_Action] UNIQUE NONCLUSTERED ([Resource], [Action])
    );

    CREATE NONCLUSTERED INDEX [IX_Permissions_Resource]
        ON [dbo].[Permissions] ([Resource])
        WHERE [IsDeleted] = 0;

    PRINT 'Table [Permissions] created successfully.';
END
GO

-- -----------------------------------------------------------------------------
-- Table: Roles
-- Description: Stores system roles that group permissions
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Roles]
    (
        [Id]              UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Name]            NVARCHAR(100)     NOT NULL,
        [Description]     NVARCHAR(500)     NOT NULL,
        [IsActive]        BIT               NOT NULL DEFAULT 1,
        [IsSystem]        BIT               NOT NULL DEFAULT 0,
        [IsDeleted]       BIT               NOT NULL DEFAULT 0,
        [CreatedAt]       DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy]       NVARCHAR(256)     NULL,
        [LastModifiedAt]  DATETIME2(7)      NULL,
        [LastModifiedBy]  NVARCHAR(256)     NULL,

        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Roles_Name] UNIQUE NONCLUSTERED ([Name])
    );

    PRINT 'Table [Roles] created successfully.';
END
GO

-- -----------------------------------------------------------------------------
-- Table: RolePermissions
-- Description: Many-to-many relationship between Roles and Permissions
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermissions]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[RolePermissions]
    (
        [RoleId]          UNIQUEIDENTIFIER  NOT NULL,
        [PermissionId]    UNIQUEIDENTIFIER  NOT NULL,
        [AssignedAt]      DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]),
        CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_RolePermissions_PermissionId]
        ON [dbo].[RolePermissions] ([PermissionId]);

    PRINT 'Table [RolePermissions] created successfully.';
END
GO

-- =============================================================================
-- SEED: Default Permissions
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = N'users' AND [Action] = N'create')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [CreatedAt], [CreatedBy])
    VALUES
        (NEWID(), N'users',         N'create', N'Crear usuarios',                       SYSUTCDATETIME(), N'system'),
        (NEWID(), N'users',         N'read',   N'Ver usuarios',                         SYSUTCDATETIME(), N'system'),
        (NEWID(), N'users',         N'update', N'Actualizar usuarios',                  SYSUTCDATETIME(), N'system'),
        (NEWID(), N'users',         N'delete', N'Eliminar usuarios',                    SYSUTCDATETIME(), N'system'),
        (NEWID(), N'roles',         N'create', N'Crear roles',                          SYSUTCDATETIME(), N'system'),
        (NEWID(), N'roles',         N'read',   N'Ver roles',                            SYSUTCDATETIME(), N'system'),
        (NEWID(), N'roles',         N'update', N'Actualizar roles',                     SYSUTCDATETIME(), N'system'),
        (NEWID(), N'roles',         N'delete', N'Eliminar roles',                       SYSUTCDATETIME(), N'system'),
        (NEWID(), N'roles',         N'assign', N'Asignar permisos a roles',             SYSUTCDATETIME(), N'system'),
        (NEWID(), N'clients',       N'create', N'Crear clientes',                       SYSUTCDATETIME(), N'system'),
        (NEWID(), N'clients',       N'read',   N'Ver clientes',                         SYSUTCDATETIME(), N'system'),
        (NEWID(), N'clients',       N'update', N'Actualizar clientes',                  SYSUTCDATETIME(), N'system'),
        (NEWID(), N'clients',       N'delete', N'Eliminar clientes',                    SYSUTCDATETIME(), N'system'),
        (NEWID(), N'opportunities', N'create', N'Crear oportunidades',                  SYSUTCDATETIME(), N'system'),
        (NEWID(), N'opportunities', N'read',   N'Ver oportunidades',                    SYSUTCDATETIME(), N'system'),
        (NEWID(), N'opportunities', N'update', N'Actualizar oportunidades',             SYSUTCDATETIME(), N'system'),
        (NEWID(), N'opportunities', N'close',  N'Cerrar oportunidades',                 SYSUTCDATETIME(), N'system'),
        (NEWID(), N'reports',       N'read',   N'Ver reportes',                         SYSUTCDATETIME(), N'system'),
        (NEWID(), N'reports',       N'export', N'Exportar reportes',                    SYSUTCDATETIME(), N'system'),
        (NEWID(), N'dashboard',     N'read',   N'Ver dashboard',                        SYSUTCDATETIME(), N'system');

    PRINT 'Default permissions seeded successfully.';
END
GO

-- =============================================================================
-- SEED: Default Roles (System roles)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = N'Administrador')
BEGIN
    DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ManagerRoleId UNIQUEIDENTIFIER = NEWID();
    DECLARE @SellerRoleId UNIQUEIDENTIFIER = NEWID();
    DECLARE @SupportRoleId UNIQUEIDENTIFIER = NEWID();
    DECLARE @MarketingRoleId UNIQUEIDENTIFIER = NEWID();

    INSERT INTO [dbo].[Roles] ([Id], [Name], [Description], [IsActive], [IsSystem], [CreatedAt], [CreatedBy])
    VALUES
        (@AdminRoleId,     N'Administrador', N'Acceso completo al sistema. Gestiona usuarios, roles y configuración.',           1, 1, SYSUTCDATETIME(), N'system'),
        (@ManagerRoleId,   N'Gerente',       N'Gestiona clientes, oportunidades y reportes. Supervisa equipos.',                 1, 1, SYSUTCDATETIME(), N'system'),
        (@SellerRoleId,    N'Vendedor',      N'Gestiona sus propios clientes y oportunidades.',                                  1, 1, SYSUTCDATETIME(), N'system'),
        (@SupportRoleId,   N'Soporte',       N'Acceso de solo lectura a clientes y oportunidades para brindar soporte.',         1, 1, SYSUTCDATETIME(), N'system'),
        (@MarketingRoleId, N'Marketing',     N'Acceso a reportes y dashboard para análisis de datos.',                           1, 1, SYSUTCDATETIME(), N'system');

    -- Assign ALL permissions to Administrador
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
    SELECT @AdminRoleId, [Id], SYSUTCDATETIME()
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0;

    -- Assign permissions to Gerente
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
    SELECT @ManagerRoleId, [Id], SYSUTCDATETIME()
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
      AND (
            ([Resource] = N'clients')
         OR ([Resource] = N'opportunities')
         OR ([Resource] = N'reports')
         OR ([Resource] = N'dashboard' AND [Action] = N'read')
         OR ([Resource] = N'users' AND [Action] = N'read')
      );

    -- Assign permissions to Vendedor
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
    SELECT @SellerRoleId, [Id], SYSUTCDATETIME()
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
      AND (
            ([Resource] = N'clients' AND [Action] IN (N'create', N'read', N'update'))
         OR ([Resource] = N'opportunities' AND [Action] IN (N'create', N'read', N'update'))
         OR ([Resource] = N'dashboard' AND [Action] = N'read')
      );

    -- Assign permissions to Soporte
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
    SELECT @SupportRoleId, [Id], SYSUTCDATETIME()
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
      AND (
            ([Resource] = N'clients' AND [Action] = N'read')
         OR ([Resource] = N'opportunities' AND [Action] = N'read')
         OR ([Resource] = N'dashboard' AND [Action] = N'read')
      );

    -- Assign permissions to Marketing
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
    SELECT @MarketingRoleId, [Id], SYSUTCDATETIME()
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
      AND (
            ([Resource] = N'reports')
         OR ([Resource] = N'dashboard' AND [Action] = N'read')
         OR ([Resource] = N'clients' AND [Action] = N'read')
      );

    PRINT 'Default roles and role-permission assignments seeded successfully.';
END
GO

-- =============================================================================
-- STORED PROCEDURES: Permissions
-- =============================================================================

-- SP: sp_Permissions_GetById
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetById]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetById];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Resource], [Action], [Description], [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Permissions]
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- SP: sp_Permissions_GetAll
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetAll]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetAll];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Resource], [Action], [Description], [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
    ORDER BY [Resource], [Action];
END
GO

-- SP: sp_Permissions_GetByResource
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetByResource]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetByResource];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetByResource]
    @Resource NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Resource], [Action], [Description], [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Permissions]
    WHERE [Resource] = @Resource AND [IsDeleted] = 0
    ORDER BY [Action];
END
GO

-- SP: sp_Permissions_GetDistinctResources
IF OBJECT_ID(N'[dbo].[sp_Permissions_GetDistinctResources]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Permissions_GetDistinctResources];
GO

CREATE PROCEDURE [dbo].[sp_Permissions_GetDistinctResources]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT [Resource]
    FROM [dbo].[Permissions]
    WHERE [IsDeleted] = 0
    ORDER BY [Resource];
END
GO

-- SP: sp_Permissions_GetByUserId
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
    INNER JOIN [dbo].[Users] u ON u.[Role] = r.[Name]
    WHERE u.[Id] = @UserId
      AND u.[IsDeleted] = 0
      AND u.[IsActive] = 1
      AND r.[IsDeleted] = 0
      AND r.[IsActive] = 1
      AND p.[IsDeleted] = 0
    ORDER BY p.[Resource], p.[Action];
END
GO

-- SP: sp_Permissions_UserHasPermission
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
    INNER JOIN [dbo].[Users] u ON u.[Role] = r.[Name]
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

-- =============================================================================
-- STORED PROCEDURES: Roles
-- =============================================================================

-- SP: sp_Roles_GetById
IF OBJECT_ID(N'[dbo].[sp_Roles_GetById]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetById];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Name], [Description], [IsActive], [IsSystem], [IsDeleted],
           [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Roles]
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- SP: sp_Roles_GetByName
IF OBJECT_ID(N'[dbo].[sp_Roles_GetByName]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetByName];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetByName]
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Name], [Description], [IsActive], [IsSystem], [IsDeleted],
           [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Roles]
    WHERE [Name] = @Name AND [IsDeleted] = 0;
END
GO

-- SP: sp_Roles_ExistsByName
IF OBJECT_ID(N'[dbo].[sp_Roles_ExistsByName]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_ExistsByName];
GO

CREATE PROCEDURE [dbo].[sp_Roles_ExistsByName]
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM [dbo].[Roles]
    WHERE [Name] = @Name AND [IsDeleted] = 0;
END
GO

-- SP: sp_Roles_GetAll
IF OBJECT_ID(N'[dbo].[sp_Roles_GetAll]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetAll];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetAll]
    @Page     INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [Description], [IsActive], [IsSystem], [IsDeleted],
           [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Roles]
    WHERE [IsDeleted] = 0
    ORDER BY [Name]
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- SP: sp_Roles_GetTotalCount
IF OBJECT_ID(N'[dbo].[sp_Roles_GetTotalCount]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetTotalCount];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM [dbo].[Roles] WHERE [IsDeleted] = 0;
END
GO

-- SP: sp_Roles_Insert
IF OBJECT_ID(N'[dbo].[sp_Roles_Insert]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_Insert];
GO

CREATE PROCEDURE [dbo].[sp_Roles_Insert]
    @Id          UNIQUEIDENTIFIER,
    @Name        NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive    BIT,
    @IsSystem    BIT,
    @CreatedAt   DATETIME2(7),
    @CreatedBy   NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Roles] ([Id], [Name], [Description], [IsActive], [IsSystem], [CreatedAt], [CreatedBy])
    VALUES (@Id, @Name, @Description, @IsActive, @IsSystem, @CreatedAt, @CreatedBy);

    SELECT @Id;
END
GO

-- SP: sp_Roles_Update
IF OBJECT_ID(N'[dbo].[sp_Roles_Update]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_Update];
GO

CREATE PROCEDURE [dbo].[sp_Roles_Update]
    @Id             UNIQUEIDENTIFIER,
    @Name           NVARCHAR(100),
    @Description    NVARCHAR(500),
    @IsActive       BIT,
    @LastModifiedAt DATETIME2(7),
    @LastModifiedBy NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Roles]
    SET [Name]           = @Name,
        [Description]    = @Description,
        [IsActive]       = @IsActive,
        [LastModifiedAt] = @LastModifiedAt,
        [LastModifiedBy] = @LastModifiedBy
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- SP: sp_Roles_GetPermissions
IF OBJECT_ID(N'[dbo].[sp_Roles_GetPermissions]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetPermissions];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetPermissions]
    @RoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.[Id], p.[Resource], p.[Action], p.[Description],
           p.[IsDeleted], p.[CreatedAt], p.[CreatedBy], p.[LastModifiedAt], p.[LastModifiedBy]
    FROM [dbo].[Permissions] p
    INNER JOIN [dbo].[RolePermissions] rp ON rp.[PermissionId] = p.[Id]
    WHERE rp.[RoleId] = @RoleId AND p.[IsDeleted] = 0
    ORDER BY p.[Resource], p.[Action];
END
GO

-- SP: sp_RolePermissions_Assign
IF OBJECT_ID(N'[dbo].[sp_RolePermissions_Assign]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_RolePermissions_Assign];
GO

CREATE PROCEDURE [dbo].[sp_RolePermissions_Assign]
    @RoleId       UNIQUEIDENTIFIER,
    @PermissionId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [RoleId] = @RoleId AND [PermissionId] = @PermissionId)
    BEGIN
        INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [AssignedAt])
        VALUES (@RoleId, @PermissionId, SYSUTCDATETIME());
    END
END
GO

-- SP: sp_RolePermissions_Remove
IF OBJECT_ID(N'[dbo].[sp_RolePermissions_Remove]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_RolePermissions_Remove];
GO

CREATE PROCEDURE [dbo].[sp_RolePermissions_Remove]
    @RoleId       UNIQUEIDENTIFIER,
    @PermissionId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[RolePermissions]
    WHERE [RoleId] = @RoleId AND [PermissionId] = @PermissionId;
END
GO

-- SP: sp_RolePermissions_RemoveAllByRole
IF OBJECT_ID(N'[dbo].[sp_RolePermissions_RemoveAllByRole]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_RolePermissions_RemoveAllByRole];
GO

CREATE PROCEDURE [dbo].[sp_RolePermissions_RemoveAllByRole]
    @RoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[RolePermissions] WHERE [RoleId] = @RoleId;
END
GO

-- SP: sp_Roles_GetUsersCountByRoleName
IF OBJECT_ID(N'[dbo].[sp_Roles_GetUsersCountByRoleName]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Roles_GetUsersCountByRoleName];
GO

CREATE PROCEDURE [dbo].[sp_Roles_GetUsersCountByRoleName]
    @RoleName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM [dbo].[Users]
    WHERE [Role] = @RoleName AND [IsDeleted] = 0 AND [IsActive] = 1;
END
GO

PRINT 'RBAC stored procedures created successfully.';
GO
