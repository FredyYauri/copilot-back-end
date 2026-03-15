-- =============================================================================
-- CRM Database - Creation Script
-- Run this script on SQL Server to create the database
-- =============================================================================

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CRM_DB')
BEGIN
    CREATE DATABASE [CRM_DB];
END
GO

USE [CRM_DB];
GO

PRINT 'Database CRM_DB created successfully.';
GO
