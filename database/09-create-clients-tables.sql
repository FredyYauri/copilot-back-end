-- =============================================
-- Script de creación: Tablas del módulo Directorio - Clientes
-- =============================================

USE [CRM_DB];
GO

-- Tabla Clients
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Clients] (
        [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Nombre]          NVARCHAR(200)    NOT NULL,
        [Ruc]             NVARCHAR(11)     NULL,
        [Dni]             NVARCHAR(8)      NULL,
        [Direccion]       NVARCHAR(500)    NOT NULL,
        [Distrito]        NVARCHAR(100)    NOT NULL,
        [Referencia]      NVARCHAR(500)    NULL,
        [Telefono]        NVARCHAR(19)     NOT NULL,
        [IsActive]        BIT              NOT NULL DEFAULT 1,
        [IsDeleted]       BIT              NOT NULL DEFAULT 0,
        [CreatedAt]       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy]       NVARCHAR(256)    NULL,
        [LastModifiedAt]  DATETIME2        NULL,
        [LastModifiedBy]  NVARCHAR(256)    NULL,
        CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id])
    );

    CREATE INDEX [IX_Clients_Ruc] ON [dbo].[Clients] ([Ruc]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Clients_Dni] ON [dbo].[Clients] ([Dni]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Clients_Nombre] ON [dbo].[Clients] ([Nombre]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Clients_IsActive] ON [dbo].[Clients] ([IsActive]) WHERE [IsDeleted] = 0;
END
GO

-- Tabla ClientContacts
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClientContacts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ClientContacts] (
        [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [ClientId]        UNIQUEIDENTIFIER NOT NULL,
        [Nombre]          NVARCHAR(200)    NOT NULL,
        [Cargo]           NVARCHAR(100)    NULL,
        [Telefono]        NVARCHAR(19)     NULL,
        [Correo]          NVARCHAR(256)    NULL,
        [Comentarios]     NVARCHAR(500)    NULL,
        [IsDeleted]       BIT              NOT NULL DEFAULT 0,
        [CreatedAt]       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy]       NVARCHAR(256)    NULL,
        [LastModifiedAt]  DATETIME2        NULL,
        [LastModifiedBy]  NVARCHAR(256)    NULL,
        CONSTRAINT [PK_ClientContacts] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ClientContacts_Clients] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients]([Id])
    );

    CREATE INDEX [IX_ClientContacts_ClientId] ON [dbo].[ClientContacts] ([ClientId]) WHERE [IsDeleted] = 0;
END
GO

-- Tabla ClientCommercialInfo
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClientCommercialInfo]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ClientCommercialInfo] (
        [ClientId]          UNIQUEIDENTIFIER NOT NULL,
        [AsesorComercial]   NVARCHAR(200)    NULL,
        [CodigoAsesor]      NVARCHAR(50)     NULL,
        [MedioCaptacion]    NVARCHAR(100)    NULL,
        [CentralRiesgo]     NVARCHAR(100)    NULL,
        [LineaCredito]      DECIMAL(18,2)    NULL,
        [Comentarios]       NVARCHAR(500)    NULL,
        CONSTRAINT [PK_ClientCommercialInfo] PRIMARY KEY CLUSTERED ([ClientId]),
        CONSTRAINT [FK_ClientCommercialInfo_Clients] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients]([Id])
    );
END
GO
