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
              assemblystation TEXT,
              stationName TEXT,
              StationType_stationTypeID INTEGER,
              lastModified TEXT,
              FOREIGN KEY (StationType_stationTypeID) REFERENCES StationType(stationTypeID)
            );

            CREATE TABLE IF NOT EXISTS ToolClass (
              toolClassID INTEGER PRIMARY KEY AUTOINCREMENT,
              toolClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS ToolType (
              toolTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
              toolTypeName TEXT NOT NULL UNIQUE,
              ToolClass_toolClassID INTEGER NOT NULL,
              FOREIGN KEY (ToolClass_toolClassID) REFERENCES ToolClass(toolClassID)
            );

            CREATE TABLE IF NOT EXISTS Tool (
              toolID INTEGER PRIMARY KEY AUTOINCREMENT,
              toolShortname TEXT,
              toolDescription TEXT,
              ToolType_toolTypeID INTEGER,
              Station_stationID INTEGER NOT NULL,
              IPAdresse_Device TEXT,
              ""PLC-Name"" TEXT,
              DB_NoSend TEXT,
              DB_NoReceive TEXT,
              preCheck_Byte INTEGER DEFAULT 0,
              adress_sendDB INTEGER DEFAULT 0,
              adress_receiveDB INTEGER DEFAULT 0,
              lastModified TEXT,
              FOREIGN KEY (ToolType_toolTypeID) REFERENCES ToolType(toolTypeID),
              FOREIGN KEY (Station_stationID) REFERENCES Station(stationID)
            );

            CREATE TABLE IF NOT EXISTS DecisionClass (
              idDecisionClass INTEGER PRIMARY KEY,
              decisionClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS SavingClass (
              idSavingClass INTEGER PRIMARY KEY,
              savingClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS GenerationClass (
              idGenerationClass INTEGER PRIMARY KEY,
              generationClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS VerificationClass (
              idVerificationClass INTEGER PRIMARY KEY,
              verificationClassName TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS GenerationClass_has_ToolType (
            ToolType_toolTypeID INTEGER NOT NULL,
            GenerationClass_idGenerationClass INTEGER NOT NULL,
            PRIMARY KEY (ToolType_toolTypeID, GenerationClass_idGenerationClass),
            FOREIGN KEY (ToolType_toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (GenerationClass_idGenerationClass) REFERENCES GenerationClass(idGenerationClass)
            );

            CREATE TABLE IF NOT EXISTS VerificationClass_has_ToolType (
            ToolType_toolTypeID INTEGER NOT NULL,
            VerificationClass_idVerificationClass INTEGER NOT NULL,
            PRIMARY KEY (ToolType_toolTypeID, VerificationClass_idVerificationClass),
            FOREIGN KEY (ToolType_toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (VerificationClass_idVerificationClass) REFERENCES VerificationClass(idVerificationClass)
            );


            CREATE TABLE IF NOT EXISTS DecisionClass_has_ToolType (
            ToolType_toolTypeID INTEGER NOT NULL,
            DecisionClass_idDecisionClass INTEGER NOT NULL,
            PRIMARY KEY (ToolType_toolTypeID, DecisionClass_idDecisionClass),
            FOREIGN KEY (ToolType_toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (DecisionClass_idDecisionClass) REFERENCES DecisionClass(idDecisionClass)
            );

            CREATE TABLE IF NOT EXISTS SavingClass_has_ToolType (
            ToolType_toolTypeID INTEGER NOT NULL,
            SavingClass_idSavingClass INTEGER NOT NULL,
            PRIMARY KEY (ToolType_toolTypeID, SavingClass_idSavingClass),
            FOREIGN KEY (ToolType_toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (SavingClass_idSavingClass) REFERENCES SavingClass(idSavingClass)
            );




            CREATE TABLE IF NOT EXISTS QGate (
              QGateID INTEGER PRIMARY KEY,
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
              DecisionClass_decisionClassID INTEGER,
              SavingClass_savingClassID INTEGER,
              GenerationClass_generationClassID INTEGER,
              VerificationClass_verificationClassID INTEGER,
              Tool_toolID INTEGER NOT NULL,
              parallel INTEGER NOT NULL DEFAULT 0,
              lastModified TEXT,
              QGate_QGateID INTEGER,
              FOREIGN KEY (DecisionClass_decisionClassID) REFERENCES DecisionClass(idDecisionClass),
              FOREIGN KEY (SavingClass_savingClassID) REFERENCES SavingClass(idSavingClass),
              FOREIGN KEY (GenerationClass_generationClassID) REFERENCES GenerationClass(idGenerationClass),
              FOREIGN KEY (VerificationClass_verificationClassID) REFERENCES VerificationClass(idVerificationClass),
              FOREIGN KEY (Tool_toolID) REFERENCES Tool(toolID),
              FOREIGN KEY (QGate_QGateID) REFERENCES QGate(QGateID)
            );

            ";

            conn.Execute(ddl);
            

        }
    }
}
