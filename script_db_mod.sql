/*
Run this script on:

        ulv7thrmac.database.windows.net.test-skins02-db    -  This database will be modified

to synchronize it with:

        ulv7thrmac.database.windows.net.papelclubmodulesdev

You are recommended to back up your database before running this script

Script created by SQL Compare version 10.4.8 from Red Gate Software Ltd at 8/12/2014 11:01:24 AM

*/
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Altering [dbo].[Dnn_Roles]'
GO
ALTER TABLE [dbo].[Dnn_Roles] ADD
[IsListable] [bit] NOT NULL CONSTRAINT [ColumnDefault_c33dfcf6-703b-4320-910a-0c918e2477a6] DEFAULT ((0))
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[Dnn_AddRole]'
GO
ALTER PROCEDURE [dbo].[Dnn_AddRole]
@PortalID INT, @RoleGroupId INT, @RoleName NVARCHAR (50), @Description NVARCHAR (1000), @ServiceFee MONEY, @BillingPeriod INT, @BillingFrequency CHAR (1), @TrialFee MONEY, @TrialPeriod INT, @TrialFrequency CHAR (1), @IsPublic BIT, @AutoAssignment BIT, @RSVPCode NVARCHAR (50), @IconFile NVARCHAR (100), @CreatedByUserID INT, @Status INT, @SecurityMode INT, @IsSystemRole BIT, @IsListable BIT
AS
INSERT INTO dbo.Dnn_Roles (
   PortalId,
   RoleGroupId,
   RoleName,
   Description,
   ServiceFee,
   BillingPeriod,
   BillingFrequency,
   TrialFee,
   TrialPeriod,
   TrialFrequency,
   IsPublic,
   AutoAssignment,
   RSVPCode,
   IconFile,
   CreatedByUserID,
   CreatedOnDate,
   LastModifiedByUserID,
   LastModifiedOnDate,
   Status,
   SecurityMode,
   IsSystemRole,
   IsListable
 )
 VALUES (
   @PortalID,
   @RoleGroupId,
   @RoleName,
   @Description,
   @ServiceFee,
   @BillingPeriod,
   @BillingFrequency,
   @TrialFee,
   @TrialPeriod,
   @TrialFrequency,
   @IsPublic,
   @AutoAssignment,
   @RSVPCode,
   @IconFile,
   @CreatedByUserID,
   getdate(),
   @CreatedByUserID,
   getdate(),
   @Status,
   @SecurityMode,
   @IsSystemRole,
   @IsListable
 )
 SELECT SCOPE_IDENTITY()
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[Dnn_UpdateRole]'
GO
ALTER PROCEDURE [dbo].[Dnn_UpdateRole]
@RoleId INT, @RoleGroupId INT, @RoleName NVARCHAR (50), @Description NVARCHAR (1000), @ServiceFee MONEY, @BillingPeriod INT, @BillingFrequency CHAR (1), @TrialFee MONEY, @TrialPeriod INT, @TrialFrequency CHAR (1), @IsPublic BIT, @AutoAssignment BIT, @RSVPCode NVARCHAR (50), @IconFile NVARCHAR (100), @LastModifiedByUserID INT, @Status INT, @SecurityMode INT, @IsSystemRole BIT, @IsListable BIT
AS
UPDATE dbo.Dnn_Roles
 SET    RoleGroupId   = @RoleGroupId,
     RoleName    = @RoleName,
     Description   = @Description,
     ServiceFee   = @ServiceFee,
     BillingPeriod  = @BillingPeriod,
     BillingFrequency  = @BillingFrequency,
     TrialFee    = @TrialFee,
     TrialPeriod   = @TrialPeriod,
     TrialFrequency  = @TrialFrequency,
     IsPublic    = @IsPublic,
     AutoAssignment  = @AutoAssignment,
     RSVPCode    = @RSVPCode,
     IconFile    = @IconFile,
     LastModifiedByUserID = @LastModifiedByUserID,
     LastModifiedOnDate  = getdate(),
     Status    = @Status,
     SecurityMode   = @SecurityMode,
     IsSystemRole   = @IsSystemRole,
     IsListable = @IsListable
 WHERE  RoleId = @RoleId
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO
