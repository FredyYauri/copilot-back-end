-- =============================================
-- Script de creación: Tablas del módulo Directorio - Proveedores
-- =============================================

USE [CRM_DB];
GO

-- Tabla Suppliers
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Suppliers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Suppliers] (
        [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Nombre]          NVARCHAR(200)    NOT NULL,
        [Ruc]             NVARCHAR(11)     NULL,
        [Telefono]        NVARCHAR(19)     NULL,
        [Direccion]       NVARCHAR(500)    NULL,
        [Distrito]        NVARCHAR(100)    NULL,
        [Ciudad]          NVARCHAR(100)    NULL,
        [Correo]          NVARCHAR(200)    NULL,
        [PaginaWeb]       NVARCHAR(300)    NULL,
        [NumeroCuenta]    NVARCHAR(50)     NULL,
        [Banco]           NVARCHAR(100)    NULL,
        [Productos]       NVARCHAR(1000)   NULL,
        [Observaciones]   NVARCHAR(1000)   NULL,
        [IsActive]        BIT              NOT NULL DEFAULT 1,
        [CreatedAt]       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy]       NVARCHAR(100)    NULL,
        [LastModifiedAt]  DATETIME2        NULL,
        [LastModifiedBy]  NVARCHAR(100)    NULL,
        [IsDeleted]       BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_Suppliers_Nombre] ON [dbo].[Suppliers] ([Nombre]);
    CREATE NONCLUSTERED INDEX [IX_Suppliers_Ruc] ON [dbo].[Suppliers] ([Ruc]) WHERE [Ruc] IS NOT NULL;
    CREATE NONCLUSTERED INDEX [IX_Suppliers_IsDeleted] ON [dbo].[Suppliers] ([IsDeleted]);
END
GO

-- Tabla SupplierContacts
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SupplierContacts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SupplierContacts] (
        [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [SupplierId]      UNIQUEIDENTIFIER NOT NULL,
        [Nombre]          NVARCHAR(200)    NOT NULL,
        [Cargo]           NVARCHAR(100)    NULL,
        [Telefono]        NVARCHAR(19)     NULL,
        [Correo]          NVARCHAR(200)    NULL,
        [CreatedAt]       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy]       NVARCHAR(100)    NULL,
        [LastModifiedAt]  DATETIME2        NULL,
        [LastModifiedBy]  NVARCHAR(100)    NULL,
        [IsDeleted]       BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_SupplierContacts] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_SupplierContacts_Suppliers] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers]([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_SupplierContacts_SupplierId] ON [dbo].[SupplierContacts] ([SupplierId]);
END
GO
