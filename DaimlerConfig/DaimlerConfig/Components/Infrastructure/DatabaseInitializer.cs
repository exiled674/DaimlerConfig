using System;
using Dapper;
using DaimlerConfig.Components.Infrastructure;

namespace DaimlerConfig.Components.Infrastructure
{
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _factory;

        public DatabaseInitializer(IDbConnectionFactory factory)
            => _factory = factory;

        public void EnsureCreated()
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var ddl = @"
-- LINE TABLE
IF OBJECT_ID(N'[Line]', 'U') IS NULL
BEGIN
    CREATE TABLE [Line] (
        [lineID] INT IDENTITY(1,1) PRIMARY KEY,
        [lineName] NVARCHAR(255) NOT NULL UNIQUE,
        [lastModified] DATETIME2 NULL
    );
END

-- STATIONTYPE TABLE
IF OBJECT_ID(N'[StationType]', 'U') IS NULL
BEGIN
    CREATE TABLE [StationType] (
        [stationTypeID] INT IDENTITY(1,1) PRIMARY KEY,
        [stationTypeName] NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- STATION TABLE
IF OBJECT_ID(N'[Station]', 'U') IS NULL
BEGIN
    CREATE TABLE [Station] (
        [stationID] INT IDENTITY(1,1) PRIMARY KEY,
        [assemblystation] NVARCHAR(255) NOT NULL,
        [stationName] NVARCHAR(255) NULL,
        [stationTypeID] INT NULL,
        [lineID] INT NULL,
        [lastModified] DATETIME2 NULL,
        CONSTRAINT FK_Station_StationType FOREIGN KEY([stationTypeID]) REFERENCES [StationType]([stationTypeID]),
        CONSTRAINT FK_Station_Line FOREIGN KEY([lineID]) REFERENCES [Line]([lineID])
    );
END

-- TOOLCLASS TABLE
IF OBJECT_ID(N'[ToolClass]', 'U') IS NULL
BEGIN
    CREATE TABLE [ToolClass] (
        [toolClassID] INT IDENTITY(1,1) PRIMARY KEY,
        [toolClassName] NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- TOOLTYPE TABLE
IF OBJECT_ID(N'[ToolType]', 'U') IS NULL
BEGIN
    CREATE TABLE [ToolType] (
        [toolTypeID] INT IDENTITY(1,1) PRIMARY KEY,
        [toolTypeName] NVARCHAR(255) NOT NULL UNIQUE,
        [toolClassID] INT NOT NULL,
        CONSTRAINT FK_ToolType_ToolClass FOREIGN KEY([toolClassID]) REFERENCES [ToolClass]([toolClassID])
    );
END

-- TOOL TABLE
IF OBJECT_ID(N'[Tool]', 'U') IS NULL
BEGIN
    CREATE TABLE [Tool] (
        [toolID] INT IDENTITY(1,1) PRIMARY KEY,
        [toolShortname] NVARCHAR(255) NULL,
        [toolDescription] NVARCHAR(MAX) NULL,
        [toolTypeID] INT NULL,
        [stationID] INT NOT NULL,
        [ipAddressDevice] NVARCHAR(100) NULL,
        [plcName] NVARCHAR(100) NULL,
        [dbNoSend] NVARCHAR(50) NULL,
        [dbNoReceive] NVARCHAR(50) NULL,
        [preCheckByte] INT NOT NULL DEFAULT 0,
        [addressSendDB] INT NOT NULL DEFAULT 0,
        [addressReceiveDB] INT NOT NULL DEFAULT 0,
        [lastModified] DATETIME2 NULL,
        CONSTRAINT FK_Tool_ToolType FOREIGN KEY([toolTypeID]) REFERENCES [ToolType]([toolTypeID]),
        CONSTRAINT FK_Tool_Station FOREIGN KEY([stationID]) REFERENCES [Station]([stationID])
    );
END

-- DECISIONCLASS TABLE
IF OBJECT_ID(N'[DecisionClass]', 'U') IS NULL
BEGIN
    CREATE TABLE [DecisionClass] (
        [decisionClassID] INT IDENTITY(1,1) PRIMARY KEY,
        [decisionClassName] NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- SAVINGCLASS TABLE
IF OBJECT_ID(N'[SavingClass]', 'U') IS NULL
BEGIN
    CREATE TABLE [SavingClass] (
        [savingClassID] INT IDENTITY(1,1) PRIMARY KEY,
        [savingClassName] NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- GENERATIONCLASS TABLE
IF OBJECT_ID(N'[GenerationClass]', 'U') IS NULL
BEGIN
    CREATE TABLE [GenerationClass] (
        [generationClassID] INT IDENTITY(1,1) PRIMARY KEY,
        [generationClassName] NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- VERIFICATIONCLASS TABLE
IF OBJECT_ID(N'[VerificationClass]', 'U') IS NULL
BEGIN
    CREATE TABLE [VerificationClass] (
        [verificationClassID] INT IDENTITY(1,1) PRIMARY KEY,
        [verificationClassName] NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- LINK TABLES
IF OBJECT_ID(N'[GenerationClass_has_ToolType]', 'U') IS NULL
BEGIN
    CREATE TABLE [GenerationClass_has_ToolType] (
        [toolTypeID] INT NOT NULL,
        [generationClassID] INT NOT NULL,
        PRIMARY KEY([toolTypeID],[generationClassID]),
        CONSTRAINT FK_GenClass_ToolType FOREIGN KEY([toolTypeID]) REFERENCES [ToolType]([toolTypeID]),
        CONSTRAINT FK_GenClass_GenerationClass FOREIGN KEY([generationClassID]) REFERENCES [GenerationClass]([generationClassID])
    );
END

IF OBJECT_ID(N'[VerificationClass_has_ToolType]', 'U') IS NULL
BEGIN
    CREATE TABLE [VerificationClass_has_ToolType] (
        [toolTypeID] INT NOT NULL,
        [verificationClassID] INT NOT NULL,
        PRIMARY KEY([toolTypeID],[verificationClassID]),
        CONSTRAINT FK_VerClass_ToolType FOREIGN KEY([toolTypeID]) REFERENCES [ToolType]([toolTypeID]),
        CONSTRAINT FK_VerClass_VerificationClass FOREIGN KEY([verificationClassID]) REFERENCES [VerificationClass]([verificationClassID])
    );
END

IF OBJECT_ID(N'[DecisionClass_has_ToolType]', 'U') IS NULL
BEGIN
    CREATE TABLE [DecisionClass_has_ToolType] (
        [toolTypeID] INT NOT NULL,
        [decisionClassID] INT NOT NULL,
        PRIMARY KEY([toolTypeID],[decisionClassID]),
        CONSTRAINT FK_DecClass_ToolType FOREIGN KEY([toolTypeID]) REFERENCES [ToolType]([toolTypeID]),
        CONSTRAINT FK_DecClass_DecisionClass FOREIGN KEY([decisionClassID]) REFERENCES [DecisionClass]([decisionClassID])
    );
END

IF OBJECT_ID(N'[SavingClass_has_ToolType]', 'U') IS NULL
BEGIN
    CREATE TABLE [SavingClass_has_ToolType] (
        [toolTypeID] INT NOT NULL,
        [savingClassID] INT NOT NULL,
        PRIMARY KEY([toolTypeID],[savingClassID]),
        CONSTRAINT FK_SavClass_ToolType FOREIGN KEY([toolTypeID]) REFERENCES [ToolType]([toolTypeID]),
        CONSTRAINT FK_SavClass_SavingClass FOREIGN KEY([savingClassID]) REFERENCES [SavingClass]([savingClassID])
    );
END

-- QGATE TABLE
IF OBJECT_ID(N'[QGate]', 'U') IS NULL
BEGIN
    CREATE TABLE [QGate] (
        [qGateID] INT IDENTITY(1,1) PRIMARY KEY,
        [qGateName] NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- OPERATION TABLE
IF OBJECT_ID(N'[Operation]', 'U') IS NULL
BEGIN
    CREATE TABLE [Operation] (
        [operationID] INT IDENTITY(1,1) PRIMARY KEY,
        [operationShortname] NVARCHAR(255) NULL,
        [operationDescription] NVARCHAR(MAX) NULL,
        [operationSequenceGroup] NVARCHAR(255) NULL,
        [operationSequence] NVARCHAR(255) NULL,
        [operationDecisionCriteria] NVARCHAR(MAX) NULL,
        [alwaysPerform] BIT NOT NULL DEFAULT 0,
        [decisionClassID] INT NULL,
        [savingClassID] INT NULL,
        [generationClassID] INT NULL,
        [verificationClassID] INT NULL,
        [toolID] INT NOT NULL,
        [parallel] BIT NOT NULL DEFAULT 0,
        [lastModified] DATETIME2 NULL,
        [qGateID] INT NULL,
        CONSTRAINT FK_Op_DecisionClass FOREIGN KEY([decisionClassID]) REFERENCES [DecisionClass]([decisionClassID]),
        CONSTRAINT FK_Op_SavingClass FOREIGN KEY([savingClassID]) REFERENCES [SavingClass]([savingClassID]),
        CONSTRAINT FK_Op_GenerationClass FOREIGN KEY([generationClassID]) REFERENCES [GenerationClass]([generationClassID]),
        CONSTRAINT FK_Op_VerificationClass FOREIGN KEY([verificationClassID]) REFERENCES [VerificationClass]([verificationClassID]),
        CONSTRAINT FK_Op_Tool FOREIGN KEY([toolID]) REFERENCES [Tool]([toolID]),
        CONSTRAINT FK_Op_QGate FOREIGN KEY([qGateID]) REFERENCES [QGate]([qGateID])
    );
END
";

            conn.Execute(ddl);
        }
    }
}
