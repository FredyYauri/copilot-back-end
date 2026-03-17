-- =============================================
-- Script de creación: Stored Procedures del módulo Directorio - Clientes
-- =============================================

USE [CRM_DB];
GO

-- =============================================
-- sp_Clients_GetById
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_GetById]'))
    DROP PROCEDURE [dbo].[sp_Clients_GetById]
GO
CREATE PROCEDURE [dbo].[sp_Clients_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Nombre], [Ruc], [Dni], [Direccion], [Distrito], [Referencia], [Telefono],
           [IsActive], [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Clients]
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- =============================================
-- sp_Clients_GetAll (paginado)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_GetAll]'))
    DROP PROCEDURE [dbo].[sp_Clients_GetAll]
GO
CREATE PROCEDURE [dbo].[sp_Clients_GetAll]
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Nombre], [Ruc], [Dni], [Direccion], [Distrito], [Referencia], [Telefono],
           [IsActive], [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Clients]
    WHERE [IsDeleted] = 0
    ORDER BY [CreatedAt] DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- =============================================
-- sp_Clients_GetTotalCount
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_GetTotalCount]'))
    DROP PROCEDURE [dbo].[sp_Clients_GetTotalCount]
GO
CREATE PROCEDURE [dbo].[sp_Clients_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM [dbo].[Clients] WHERE [IsDeleted] = 0;
END
GO

-- =============================================
-- sp_Clients_ExistsByRuc
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_ExistsByRuc]'))
    DROP PROCEDURE [dbo].[sp_Clients_ExistsByRuc]
GO
CREATE PROCEDURE [dbo].[sp_Clients_ExistsByRuc]
    @Ruc NVARCHAR(11)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM [dbo].[Clients] WHERE [Ruc] = @Ruc AND [IsDeleted] = 0;
END
GO

-- =============================================
-- sp_Clients_ExistsByDni
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_ExistsByDni]'))
    DROP PROCEDURE [dbo].[sp_Clients_ExistsByDni]
GO
CREATE PROCEDURE [dbo].[sp_Clients_ExistsByDni]
    @Dni NVARCHAR(8)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM [dbo].[Clients] WHERE [Dni] = @Dni AND [IsDeleted] = 0;
END
GO

-- =============================================
-- sp_Clients_Insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_Insert]'))
    DROP PROCEDURE [dbo].[sp_Clients_Insert]
GO
CREATE PROCEDURE [dbo].[sp_Clients_Insert]
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(200),
    @Ruc NVARCHAR(11) = NULL,
    @Dni NVARCHAR(8) = NULL,
    @Direccion NVARCHAR(500),
    @Distrito NVARCHAR(100),
    @Referencia NVARCHAR(500) = NULL,
    @Telefono NVARCHAR(19),
    @IsActive BIT,
    @CreatedAt DATETIME2,
    @CreatedBy NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[Clients] ([Id], [Nombre], [Ruc], [Dni], [Direccion], [Distrito], [Referencia], [Telefono], [IsActive], [CreatedAt], [CreatedBy])
    VALUES (@Id, @Nombre, @Ruc, @Dni, @Direccion, @Distrito, @Referencia, @Telefono, @IsActive, @CreatedAt, @CreatedBy);

    SELECT @Id;
END
GO

-- =============================================
-- sp_Clients_Update
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_Update]'))
    DROP PROCEDURE [dbo].[sp_Clients_Update]
GO
CREATE PROCEDURE [dbo].[sp_Clients_Update]
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(200),
    @Ruc NVARCHAR(11) = NULL,
    @Dni NVARCHAR(8) = NULL,
    @Direccion NVARCHAR(500),
    @Distrito NVARCHAR(100),
    @Referencia NVARCHAR(500) = NULL,
    @Telefono NVARCHAR(19),
    @IsActive BIT,
    @LastModifiedAt DATETIME2,
    @LastModifiedBy NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Clients]
    SET [Nombre] = @Nombre,
        [Ruc] = @Ruc,
        [Dni] = @Dni,
        [Direccion] = @Direccion,
        [Distrito] = @Distrito,
        [Referencia] = @Referencia,
        [Telefono] = @Telefono,
        [IsActive] = @IsActive,
        [LastModifiedAt] = @LastModifiedAt,
        [LastModifiedBy] = @LastModifiedBy
    WHERE [Id] = @Id AND [IsDeleted] = 0;
END
GO

-- =============================================
-- sp_Clients_Search (búsqueda por nombre, RUC o DNI)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Clients_Search]'))
    DROP PROCEDURE [dbo].[sp_Clients_Search]
GO
CREATE PROCEDURE [dbo].[sp_Clients_Search]
    @SearchTerm NVARCHAR(200),
    @MaxResults INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@MaxResults)
        [Id], [Nombre], [Ruc], [Dni], [Direccion], [Distrito], [Referencia], [Telefono],
        [IsActive], [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[Clients]
    WHERE [IsDeleted] = 0
      AND ([Nombre] LIKE '%' + @SearchTerm + '%'
           OR [Ruc] LIKE '%' + @SearchTerm + '%'
           OR [Dni] LIKE '%' + @SearchTerm + '%')
    ORDER BY [Nombre];
END
GO

-- =============================================
-- sp_ClientContacts_GetByClientId
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ClientContacts_GetByClientId]'))
    DROP PROCEDURE [dbo].[sp_ClientContacts_GetByClientId]
GO
CREATE PROCEDURE [dbo].[sp_ClientContacts_GetByClientId]
    @ClientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [ClientId], [Nombre], [Cargo], [Telefono], [Correo], [Comentarios],
           [IsDeleted], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    FROM [dbo].[ClientContacts]
    WHERE [ClientId] = @ClientId AND [IsDeleted] = 0;
END
GO

-- =============================================
-- sp_ClientContacts_Insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ClientContacts_Insert]'))
    DROP PROCEDURE [dbo].[sp_ClientContacts_Insert]
GO
CREATE PROCEDURE [dbo].[sp_ClientContacts_Insert]
    @Id UNIQUEIDENTIFIER,
    @ClientId UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(200),
    @Cargo NVARCHAR(100) = NULL,
    @Telefono NVARCHAR(19) = NULL,
    @Correo NVARCHAR(256) = NULL,
    @Comentarios NVARCHAR(500) = NULL,
    @CreatedAt DATETIME2,
    @CreatedBy NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[ClientContacts] ([Id], [ClientId], [Nombre], [Cargo], [Telefono], [Correo], [Comentarios], [CreatedAt], [CreatedBy])
    VALUES (@Id, @ClientId, @Nombre, @Cargo, @Telefono, @Correo, @Comentarios, @CreatedAt, @CreatedBy);

    SELECT @Id;
END
GO

-- =============================================
-- sp_ClientContacts_DeleteByClientId (soft delete)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ClientContacts_DeleteByClientId]'))
    DROP PROCEDURE [dbo].[sp_ClientContacts_DeleteByClientId]
GO
CREATE PROCEDURE [dbo].[sp_ClientContacts_DeleteByClientId]
    @ClientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[ClientContacts]
    SET [IsDeleted] = 1, [LastModifiedAt] = SYSUTCDATETIME()
    WHERE [ClientId] = @ClientId AND [IsDeleted] = 0;
END
GO

-- =============================================
-- sp_ClientCommercialInfo_GetByClientId
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ClientCommercialInfo_GetByClientId]'))
    DROP PROCEDURE [dbo].[sp_ClientCommercialInfo_GetByClientId]
GO
CREATE PROCEDURE [dbo].[sp_ClientCommercialInfo_GetByClientId]
    @ClientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [ClientId], [AsesorComercial], [CodigoAsesor], [MedioCaptacion], [CentralRiesgo], [LineaCredito], [Comentarios]
    FROM [dbo].[ClientCommercialInfo]
    WHERE [ClientId] = @ClientId;
END
GO

-- =============================================
-- sp_ClientCommercialInfo_Insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ClientCommercialInfo_Insert]'))
    DROP PROCEDURE [dbo].[sp_ClientCommercialInfo_Insert]
GO
CREATE PROCEDURE [dbo].[sp_ClientCommercialInfo_Insert]
    @ClientId UNIQUEIDENTIFIER,
    @AsesorComercial NVARCHAR(200) = NULL,
    @CodigoAsesor NVARCHAR(50) = NULL,
    @MedioCaptacion NVARCHAR(100) = NULL,
    @CentralRiesgo NVARCHAR(100) = NULL,
    @LineaCredito DECIMAL(18,2) = NULL,
    @Comentarios NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[ClientCommercialInfo] ([ClientId], [AsesorComercial], [CodigoAsesor], [MedioCaptacion], [CentralRiesgo], [LineaCredito], [Comentarios])
    VALUES (@ClientId, @AsesorComercial, @CodigoAsesor, @MedioCaptacion, @CentralRiesgo, @LineaCredito, @Comentarios);
END
GO

-- =============================================
-- sp_ClientCommercialInfo_Update
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ClientCommercialInfo_Update]'))
    DROP PROCEDURE [dbo].[sp_ClientCommercialInfo_Update]
GO
CREATE PROCEDURE [dbo].[sp_ClientCommercialInfo_Update]
    @ClientId UNIQUEIDENTIFIER,
    @AsesorComercial NVARCHAR(200) = NULL,
    @CodigoAsesor NVARCHAR(50) = NULL,
    @MedioCaptacion NVARCHAR(100) = NULL,
    @CentralRiesgo NVARCHAR(100) = NULL,
    @LineaCredito DECIMAL(18,2) = NULL,
    @Comentarios NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[ClientCommercialInfo]
    SET [AsesorComercial] = @AsesorComercial,
        [CodigoAsesor] = @CodigoAsesor,
        [MedioCaptacion] = @MedioCaptacion,
        [CentralRiesgo] = @CentralRiesgo,
        [LineaCredito] = @LineaCredito,
        [Comentarios] = @Comentarios
    WHERE [ClientId] = @ClientId;
END
GO

-- =============================================
-- Permisos del módulo Clientes
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = 'clients' AND [Action] = 'create')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [Type], [IsActive], [CreatedAt])
    VALUES
        (NEWID(), 'clients', 'create', 'Crear clientes', 'page', 1, SYSUTCDATETIME()),
        (NEWID(), 'clients', 'read', 'Ver clientes', 'page', 1, SYSUTCDATETIME()),
        (NEWID(), 'clients', 'update', 'Actualizar clientes', 'action', 1, SYSUTCDATETIME());
END
GO
