-- =============================================
-- Script de creación: Stored Procedures del módulo Directorio - Proveedores
-- =============================================

USE [CRM_DB];
GO

-- =============================================
-- sp_Suppliers_GetById
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Suppliers_GetById]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_Suppliers_GetById];
GO
CREATE PROCEDURE [dbo].[sp_Suppliers_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Ruc, Telefono, Direccion, Distrito, Ciudad, Correo, PaginaWeb,
           NumeroCuenta, Banco, Productos, Observaciones, IsActive,
           CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy, IsDeleted
    FROM [dbo].[Suppliers]
    WHERE Id = @Id AND IsDeleted = 0;
END
GO

-- =============================================
-- sp_Suppliers_GetAll
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Suppliers_GetAll]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_Suppliers_GetAll];
GO
CREATE PROCEDURE [dbo].[sp_Suppliers_GetAll]
    @Page INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Ruc, Telefono, Direccion, Distrito, Ciudad, Correo, PaginaWeb,
           NumeroCuenta, Banco, Productos, Observaciones, IsActive,
           CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy, IsDeleted
    FROM [dbo].[Suppliers]
    WHERE IsDeleted = 0
    ORDER BY CreatedAt DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- =============================================
-- sp_Suppliers_GetTotalCount
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Suppliers_GetTotalCount]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_Suppliers_GetTotalCount];
GO
CREATE PROCEDURE [dbo].[sp_Suppliers_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM [dbo].[Suppliers] WHERE IsDeleted = 0;
END
GO

-- =============================================
-- sp_Suppliers_ExistsByRuc
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Suppliers_ExistsByRuc]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_Suppliers_ExistsByRuc];
GO
CREATE PROCEDURE [dbo].[sp_Suppliers_ExistsByRuc]
    @Ruc NVARCHAR(11)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM [dbo].[Suppliers] WHERE Ruc = @Ruc AND IsDeleted = 0;
END
GO

-- =============================================
-- sp_Suppliers_Insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Suppliers_Insert]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_Suppliers_Insert];
GO
CREATE PROCEDURE [dbo].[sp_Suppliers_Insert]
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(200),
    @Ruc NVARCHAR(11) = NULL,
    @Telefono NVARCHAR(19) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @Distrito NVARCHAR(100) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Correo NVARCHAR(200) = NULL,
    @PaginaWeb NVARCHAR(300) = NULL,
    @NumeroCuenta NVARCHAR(50) = NULL,
    @Banco NVARCHAR(100) = NULL,
    @Productos NVARCHAR(1000) = NULL,
    @Observaciones NVARCHAR(1000) = NULL,
    @IsActive BIT,
    @CreatedAt DATETIME2,
    @CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[Suppliers]
        (Id, Nombre, Ruc, Telefono, Direccion, Distrito, Ciudad, Correo, PaginaWeb,
         NumeroCuenta, Banco, Productos, Observaciones, IsActive, CreatedAt, CreatedBy)
    VALUES
        (@Id, @Nombre, @Ruc, @Telefono, @Direccion, @Distrito, @Ciudad, @Correo, @PaginaWeb,
         @NumeroCuenta, @Banco, @Productos, @Observaciones, @IsActive, @CreatedAt, @CreatedBy);
    SELECT @Id;
END
GO

-- =============================================
-- sp_Suppliers_Update
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Suppliers_Update]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_Suppliers_Update];
GO
CREATE PROCEDURE [dbo].[sp_Suppliers_Update]
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(200),
    @Ruc NVARCHAR(11) = NULL,
    @Telefono NVARCHAR(19) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @Distrito NVARCHAR(100) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Correo NVARCHAR(200) = NULL,
    @PaginaWeb NVARCHAR(300) = NULL,
    @NumeroCuenta NVARCHAR(50) = NULL,
    @Banco NVARCHAR(100) = NULL,
    @Productos NVARCHAR(1000) = NULL,
    @Observaciones NVARCHAR(1000) = NULL,
    @IsActive BIT,
    @LastModifiedAt DATETIME2,
    @LastModifiedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Suppliers]
    SET Nombre = @Nombre, Ruc = @Ruc, Telefono = @Telefono,
        Direccion = @Direccion, Distrito = @Distrito, Ciudad = @Ciudad,
        Correo = @Correo, PaginaWeb = @PaginaWeb,
        NumeroCuenta = @NumeroCuenta, Banco = @Banco,
        Productos = @Productos, Observaciones = @Observaciones,
        IsActive = @IsActive, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
    WHERE Id = @Id AND IsDeleted = 0;
END
GO

-- =============================================
-- sp_Suppliers_Search
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Suppliers_Search]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_Suppliers_Search];
GO
CREATE PROCEDURE [dbo].[sp_Suppliers_Search]
    @SearchTerm NVARCHAR(200),
    @MaxResults INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@MaxResults) Id, Nombre, Ruc, Telefono, Direccion, Distrito, Ciudad, Correo, PaginaWeb,
           NumeroCuenta, Banco, Productos, Observaciones, IsActive,
           CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy, IsDeleted
    FROM [dbo].[Suppliers]
    WHERE IsDeleted = 0
      AND (Nombre LIKE '%' + @SearchTerm + '%'
           OR Ruc LIKE '%' + @SearchTerm + '%')
    ORDER BY Nombre;
END
GO

-- =============================================
-- sp_SupplierContacts_GetBySupplierId
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SupplierContacts_GetBySupplierId]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_SupplierContacts_GetBySupplierId];
GO
CREATE PROCEDURE [dbo].[sp_SupplierContacts_GetBySupplierId]
    @SupplierId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, SupplierId, Nombre, Cargo, Telefono, Correo,
           CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy, IsDeleted
    FROM [dbo].[SupplierContacts]
    WHERE SupplierId = @SupplierId AND IsDeleted = 0;
END
GO

-- =============================================
-- sp_SupplierContacts_Insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SupplierContacts_Insert]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_SupplierContacts_Insert];
GO
CREATE PROCEDURE [dbo].[sp_SupplierContacts_Insert]
    @Id UNIQUEIDENTIFIER,
    @SupplierId UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(200),
    @Cargo NVARCHAR(100) = NULL,
    @Telefono NVARCHAR(19) = NULL,
    @Correo NVARCHAR(200) = NULL,
    @CreatedAt DATETIME2,
    @CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[SupplierContacts]
        (Id, SupplierId, Nombre, Cargo, Telefono, Correo, CreatedAt, CreatedBy)
    VALUES
        (@Id, @SupplierId, @Nombre, @Cargo, @Telefono, @Correo, @CreatedAt, @CreatedBy);
    SELECT @Id;
END
GO

-- =============================================
-- sp_SupplierContacts_DeleteBySupplierId
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SupplierContacts_DeleteBySupplierId]') AND type = N'P')
    DROP PROCEDURE [dbo].[sp_SupplierContacts_DeleteBySupplierId];
GO
CREATE PROCEDURE [dbo].[sp_SupplierContacts_DeleteBySupplierId]
    @SupplierId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[SupplierContacts]
    SET IsDeleted = 1, LastModifiedAt = SYSUTCDATETIME()
    WHERE SupplierId = @SupplierId AND IsDeleted = 0;
END
GO

-- =============================================
-- Insertar permisos de proveedores
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = 'suppliers' AND [Action] = 'create')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [CreatedAt])
    VALUES (NEWID(), 'suppliers', 'create', 'Crear proveedores', SYSUTCDATETIME());
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = 'suppliers' AND [Action] = 'read')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [CreatedAt])
    VALUES (NEWID(), 'suppliers', 'read', 'Ver proveedores', SYSUTCDATETIME());
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = 'suppliers' AND [Action] = 'update')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Description], [CreatedAt])
    VALUES (NEWID(), 'suppliers', 'update', 'Actualizar proveedores', SYSUTCDATETIME());
END
GO
