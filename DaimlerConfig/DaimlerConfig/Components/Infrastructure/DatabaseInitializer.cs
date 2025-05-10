using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

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
            PRAGMA foreign_keys = ON;

            CREATE TABLE IF NOT EXISTS StationType (
              stationTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
              stationTypeName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS Station (
              stationID INTEGER PRIMARY KEY AUTOINCREMENT,
              assemblystation TEXT NOT NULL,
              stationName TEXT,
              stationTypeID INTEGER,
              lastModified TEXT,
              FOREIGN KEY (stationTypeID) REFERENCES StationType(stationTypeID)
            );

            CREATE TABLE IF NOT EXISTS ToolClass (
              toolClassID INTEGER PRIMARY KEY AUTOINCREMENT,
              toolClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS ToolType (
              toolTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
              toolTypeName TEXT NOT NULL UNIQUE,
              toolClassID INTEGER NOT NULL,
              FOREIGN KEY (toolClassID) REFERENCES ToolClass(toolClassID)
            );

            CREATE TABLE IF NOT EXISTS Tool (
              toolID INTEGER PRIMARY KEY AUTOINCREMENT,
              toolShortname TEXT,
              toolDescription TEXT,
              toolTypeID INTEGER,
              stationID INTEGER NOT NULL,
              ipAddressDevice TEXT,
              plcName TEXT,
              dbNoSend TEXT,
              dbNoReceive TEXT,
              preCheckByte INTEGER DEFAULT 0,
              addressSendDB INTEGER DEFAULT 0,
              addressReceiveDB INTEGER DEFAULT 0,
              lastModified TEXT,
              FOREIGN KEY (toolTypeID) REFERENCES ToolType(toolTypeID),
              FOREIGN KEY (stationID) REFERENCES Station(stationID)
            );

            CREATE TABLE IF NOT EXISTS DecisionClass (
              decisionClassID INTEGER PRIMARY KEY AUTOINCREMENT,
              decisionClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS SavingClass (
              savingClassID INTEGER PRIMARY KEY AUTOINCREMENT,
              savingClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS GenerationClass (
              generationClassID INTEGER PRIMARY KEY AUTOINCREMENT,
              generationClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS VerificationClass (
              verificationClassID INTEGER PRIMARY KEY AUTOINCREMENT,
              verificationClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS GenerationClass_has_ToolType (
            toolTypeID INTEGER NOT NULL,
            generationClassID INTEGER NOT NULL,
            PRIMARY KEY (toolTypeID, generationClassID),
            FOREIGN KEY toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (generationClassID) REFERENCES GenerationClass(generationClassID)
            );

            CREATE TABLE IF NOT EXISTS VerificationClass_has_ToolType (
            toolTypeID INTEGER NOT NULL,
            verificationClassID INTEGER NOT NULL,
            PRIMARY KEY (toolTypeID, verificationClassID),
            FOREIGN KEY (toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (verificationClassID) REFERENCES VerificationClass(verificationClassID)
            );


            CREATE TABLE IF NOT EXISTS DecisionClass_has_ToolType (
            toolTypeID INTEGER NOT NULL,
            decisionClassID INTEGER NOT NULL,
            PRIMARY KEY (toolTypeID, decisionClassID),
            FOREIGN KEY (toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (decisionClassID) REFERENCES DecisionClass(decisionClassID)
            );

            CREATE TABLE IF NOT EXISTS SavingClass_has_ToolType (
            toolTypeID INTEGER NOT NULL,
            savingClassID INTEGER NOT NULL,
            PRIMARY KEY (toolTypeID, savingClassID),
            FOREIGN KEY (toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (savingClassID) REFERENCES SavingClass(savingClassID)
            );

            CREATE TABLE IF NOT EXISTS QGate (
              qGateID INTEGER PRIMARY KEY AUTOINCREMENT,
              qGateName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS Operation (
              operationID INTEGER PRIMARY KEY AUTOINCREMENT,
              operationShortname TEXT,
              operationDescription TEXT,
              operationSequenceGroup TEXT,
              operationSequence TEXT,
              operationDecisionCriteria TEXT,
              alwaysPerform INTEGER NOT NULL DEFAULT 0,
              decisionClassID INTEGER,
              savingClassID INTEGER,
              generationClassID INTEGER,
              verificationClassID INTEGER,
              toolID INTEGER NOT NULL,
              parallel INTEGER NOT NULL DEFAULT 0,
              lastModified TEXT,
              qGateID INTEGER,
              FOREIGN KEY (decisionClassID) REFERENCES DecisionClass(decisionClassID),
              FOREIGN KEY (savingClassID) REFERENCES SavingClass(savingClassID),
              FOREIGN KEY (generationClassID) REFERENCES GenerationClass(generationClassID),
              FOREIGN KEY (verificationClassID) REFERENCES VerificationClass(verificationClassID),
              FOREIGN KEY (toolID) REFERENCES Tool(toolID),
              FOREIGN KEY (qGateID) REFERENCES QGate(qGateID)
            );

            ";

            conn.Execute(ddl);


        }
    }
}
