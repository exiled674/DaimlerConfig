using Xunit;
using Dapper;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Text.Json;
using Windows.System;

namespace DaimlerConfigTest
{
    public class OperationRepositoryTest : IDisposable
    {

        private readonly IDbConnection _connection;
        private readonly OperationRepository operationRepository;

        public OperationRepositoryTest()
        {

            
           
            var connectionFactory = new SqlServerConnectionFactory("Server = 92.205.188.134, 1433; Initial Catalog = DConfigTest; Persist Security Info = False; User ID = SA; Password = 580=YQc8Tn1:mNdsoJ.8WeLVHMXIqWO2I5; MultipleActiveResultSets = False; Encrypt = False; TrustServerCertificate = True; Connection Timeout = 30; ");

            //Erstellt eine Verbindung zur Datenbank über den Pfad
            _connection = connectionFactory.CreateConnection();


            //Methode um die Datenbank mit Testdaten zu befüllen
            SetUpData();

            //Stationrepo mit Verbindung intiialisiert
            operationRepository = new OperationRepository(connectionFactory);
        }


        


        internal void SetUpData()
        {
            // Line-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM Line WHERE lineName = 'Line1')
            INSERT INTO Line (lineName) VALUES ('Line1');
        IF NOT EXISTS (SELECT 1 FROM Line WHERE lineName = 'Line2')
            INSERT INTO Line (lineName) VALUES ('Line2');");

            // StationType-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM StationType WHERE stationTypeName = 'StationType1')
            INSERT INTO StationType (stationTypeName) VALUES ('StationType1');
        IF NOT EXISTS (SELECT 1 FROM StationType WHERE stationTypeName = 'StationType2')
            INSERT INTO StationType (stationTypeName) VALUES ('StationType2');");

            // Station-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM Station WHERE assemblystation = 'AssemblyStation1' AND stationName = 'Station1')
            INSERT INTO Station (assemblystation, stationName, stationTypeID, lineID, lastModified) VALUES
            ('AssemblyStation1', 'Station1', 1, 1, '2023-10-01');
        IF NOT EXISTS (SELECT 1 FROM Station WHERE assemblystation = 'AssemblyStation2' AND stationName = 'Station2')
            INSERT INTO Station (assemblystation, stationName, stationTypeID, lineID, lastModified) VALUES
            ('AssemblyStation2', 'Station2', 2, 2, '2023-10-02');");

            // ToolClass-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM ToolClass WHERE toolClassName = 'ToolClass1')
            INSERT INTO ToolClass (toolClassName) VALUES ('ToolClass1');
        IF NOT EXISTS (SELECT 1 FROM ToolClass WHERE toolClassName = 'ToolClass2')
            INSERT INTO ToolClass (toolClassName) VALUES ('ToolClass2');");

            // ToolType-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM ToolType WHERE toolTypeName = 'ToolType1' AND toolClassID = 1)
            INSERT INTO ToolType (toolTypeName, toolClassID) VALUES ('ToolType1', 1);
        IF NOT EXISTS (SELECT 1 FROM ToolType WHERE toolTypeName = 'ToolType2' AND toolClassID = 2)
            INSERT INTO ToolType (toolTypeName, toolClassID) VALUES ('ToolType2', 2);");

            // Tool-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM Tool WHERE toolShortname = 'Tool1' AND toolTypeID = 1 AND stationID = 1)
            INSERT INTO Tool (toolShortname, toolDescription, toolTypeID, stationID, ipAddressDevice, plcName, dbNoSend, dbNoReceive, preCheckByte, addressSendDB, addressReceiveDB, lastModified) VALUES
            ('Tool1', 'Description1', 1, 1, '192.168.0.1', 'PLC1', 'DB1', 'DB2', 0, '100', '200', '2023-10-01');
        IF NOT EXISTS (SELECT 1 FROM Tool WHERE toolShortname = 'Tool2' AND toolTypeID = 2 AND stationID = 2)
            INSERT INTO Tool (toolShortname, toolDescription, toolTypeID, stationID, ipAddressDevice, plcName, dbNoSend, dbNoReceive, preCheckByte, addressSendDB, addressReceiveDB, lastModified) VALUES
            ('Tool2', 'Description2', 2, 2, '192.168.0.2', 'PLC2', 'DB3', 'DB4', 1, '101', '201', '2023-10-02');");

            // DecisionClass-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM DecisionClass WHERE decisionClassName = 'DecisionClass1')
            INSERT INTO DecisionClass (decisionClassName) VALUES ('DecisionClass1');
        IF NOT EXISTS (SELECT 1 FROM DecisionClass WHERE decisionClassName = 'DecisionClass2')
            INSERT INTO DecisionClass (decisionClassName) VALUES ('DecisionClass2');");

            // SavingClass-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM SavingClass WHERE savingClassName = 'SavingClass1')
            INSERT INTO SavingClass (savingClassName) VALUES ('SavingClass1');
        IF NOT EXISTS (SELECT 1 FROM SavingClass WHERE savingClassName = 'SavingClass2')
            INSERT INTO SavingClass (savingClassName) VALUES ('SavingClass2');");

            // GenerationClass-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM GenerationClass WHERE generationClassName = 'GenerationClass1')
            INSERT INTO GenerationClass (generationClassName) VALUES ('GenerationClass1');
        IF NOT EXISTS (SELECT 1 FROM GenerationClass WHERE generationClassName = 'GenerationClass2')
            INSERT INTO GenerationClass (generationClassName) VALUES ('GenerationClass2');");

            // VerificationClass-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM VerificationClass WHERE verificationClassName = 'VerificationClass1')
            INSERT INTO VerificationClass (verificationClassName) VALUES ('VerificationClass1');
        IF NOT EXISTS (SELECT 1 FROM VerificationClass WHERE verificationClassName = 'VerificationClass2')
            INSERT INTO VerificationClass (verificationClassName) VALUES ('VerificationClass2');");

            // QGate-Daten einfügen
            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM QGate WHERE qGateName = 'QGate1')
            INSERT INTO QGate (qGateName) VALUES ('QGate1');
        IF NOT EXISTS (SELECT 1 FROM QGate WHERE qGateName = 'QGate2')
            INSERT INTO QGate (qGateName) VALUES ('QGate2');");
        }



        [Fact]
        public async void AddOperationTest_Works()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            var operation = new Operation
            {
                operationShortname = "Operation_" + timestamp,
                operationDescription = "Description_" + timestamp,
                operationSequenceGroup = "Group1",
                operationSequence = "Sequence1",
                operationDecisionCriteria = "Criteria1",
                alwaysPerform = true,
                decisionClassID = 1,
                savingClassID = 1,
                generationClassID = 1,
                verificationClassID = 1,
                toolID = 1,
                parallel = false,
                qGateID = 1,
                lastModified = DateTime.Now
            };

            await operationRepository.Add(operation);

            var result = await _connection.QueryFirstOrDefaultAsync<Operation>(
                "SELECT * FROM Operation WHERE operationShortname = @operationShortname",
                new { operation.operationShortname });

            Assert.NotNull(result);
            Assert.Equal(operation.operationShortname, result.operationShortname);
            Assert.Equal(operation.operationDescription, result.operationDescription);
            Assert.Equal(operation.operationSequenceGroup, result.operationSequenceGroup);
            Assert.Equal(operation.operationSequence, result.operationSequence);
            Assert.Equal(operation.operationDecisionCriteria, result.operationDecisionCriteria);
            Assert.Equal(operation.alwaysPerform, result.alwaysPerform);
            Assert.Equal(operation.decisionClassID, result.decisionClassID);
            Assert.Equal(operation.savingClassID, result.savingClassID);
            Assert.Equal(operation.generationClassID, result.generationClassID);
            Assert.Equal(operation.verificationClassID, result.verificationClassID);
            Assert.Equal(operation.toolID, result.toolID);
            Assert.Equal(operation.parallel, result.parallel);
            Assert.Equal(operation.qGateID, result.qGateID);
            //Assert.Equal(operation.lastModified.ToString("yyyy-MM-dd HH:mm:ss"), result.lastModified.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        [Fact]
        public async void DeleteOperationTest_Works()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM Operation WHERE operationShortname = @operationShortname)
        BEGIN
            INSERT INTO Operation (operationShortname, operationDescription, operationSequenceGroup, operationSequence, operationDecisionCriteria, alwaysPerform, decisionClassID, savingClassID, generationClassID, verificationClassID, toolID, parallel, qGateID, lastModified)
            VALUES (@operationShortname, @operationDescription, @operationSequenceGroup, @operationSequence, @operationDecisionCriteria, @alwaysPerform, @decisionClassID, @savingClassID, @generationClassID, @verificationClassID, @toolID, @parallel, @qGateID, @lastModified);
        END",
                new
                {
                    operationShortname = "Operation_" + timestamp,
                    operationDescription = "Description_" + timestamp,
                    operationSequenceGroup = "Group1",
                    operationSequence = "Sequence1",
                    operationDecisionCriteria = "Criteria1",
                    alwaysPerform = 1,
                    decisionClassID = 1,
                    savingClassID = 1,
                    generationClassID = 1,
                    verificationClassID = 1,
                    toolID = 1,
                    parallel = 0,
                    qGateID = 1,
                    lastModified = DateTime.Now
                });

            var operationToDelete = await _connection.QueryFirstOrDefaultAsync<Operation>(
                "SELECT * FROM Operation WHERE operationShortname = @operationShortname",
                new { operationShortname = "Operation_" + timestamp });
            Assert.NotNull(operationToDelete);

            await operationRepository.Delete(operationToDelete);

            var deletedOperation = await _connection.QueryFirstOrDefaultAsync<Operation>(
                "SELECT * FROM Operation WHERE operationShortname = @operationShortname",
                new { operationShortname = "Operation_" + timestamp });
            Assert.Null(deletedOperation);
        }


        [Fact]
        public async void UpdateOperationTest_Works()
        {
            string initialTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            var insertData = new
            {
                operationShortname = "Operation_" + initialTimestamp,
                operationDescription = "Description_" + initialTimestamp,
                operationSequenceGroup = "Group1",
                operationSequence = "Sequence1",
                operationDecisionCriteria = "Criteria1",
                alwaysPerform = 1,
                decisionClassID = 1,
                savingClassID = 1,
                generationClassID = 1,
                verificationClassID = 1,
                toolID = 1,
                parallel = 0,
                qGateID = 1,
                lastModified = DateTime.Now
            };

            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM Operation WHERE operationShortname = @operationShortname)
        BEGIN
            INSERT INTO Operation (
                operationShortname, operationDescription, operationSequenceGroup, operationSequence,
                operationDecisionCriteria, alwaysPerform, decisionClassID, savingClassID,
                generationClassID, verificationClassID, toolID, parallel, qGateID, lastModified)
            VALUES (
                @operationShortname, @operationDescription, @operationSequenceGroup, @operationSequence,
                @operationDecisionCriteria, @alwaysPerform, @decisionClassID, @savingClassID,
                @generationClassID, @verificationClassID, @toolID, @parallel, @qGateID, @lastModified);
        END",
                insertData);

            var operationToUpdate = await _connection.QueryFirstOrDefaultAsync<Operation>(
                "SELECT * FROM Operation WHERE operationShortname = @operationShortname",
                new { insertData.operationShortname });

            Assert.NotNull(operationToUpdate);

            string updateTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            operationToUpdate.operationShortname += "_updated_" + updateTimestamp;
            operationToUpdate.operationDescription += "_updated_" + updateTimestamp;
            operationToUpdate.operationSequenceGroup = "Group2";
            operationToUpdate.operationSequence = "Sequence2";
            operationToUpdate.operationDecisionCriteria = "Criteria2";
            operationToUpdate.alwaysPerform = false;
            operationToUpdate.decisionClassID = 2;
            operationToUpdate.savingClassID = 2;
            operationToUpdate.generationClassID = 2;
            operationToUpdate.verificationClassID = 2;
            operationToUpdate.toolID = 2;
            operationToUpdate.parallel = true;
            operationToUpdate.qGateID = 2;
            operationToUpdate.lastModified = DateTime.Now;

            await operationRepository.Update(operationToUpdate);

            var updatedOperation = await _connection.QueryFirstOrDefaultAsync<Operation>(
                "SELECT * FROM Operation WHERE operationID = @operationID",
                new { operationToUpdate.operationID });

            Assert.NotNull(updatedOperation);
            Assert.Equal(operationToUpdate.operationShortname, updatedOperation.operationShortname);
            Assert.Equal(operationToUpdate.operationDescription, updatedOperation.operationDescription);
            Assert.Equal(operationToUpdate.operationSequenceGroup, updatedOperation.operationSequenceGroup);
            Assert.Equal(operationToUpdate.operationSequence, updatedOperation.operationSequence);
            Assert.Equal(operationToUpdate.operationDecisionCriteria, updatedOperation.operationDecisionCriteria);
            Assert.Equal(operationToUpdate.alwaysPerform, updatedOperation.alwaysPerform);
            Assert.Equal(operationToUpdate.decisionClassID, updatedOperation.decisionClassID);
            Assert.Equal(operationToUpdate.savingClassID, updatedOperation.savingClassID);
            Assert.Equal(operationToUpdate.generationClassID, updatedOperation.generationClassID);
            Assert.Equal(operationToUpdate.verificationClassID, updatedOperation.verificationClassID);
            Assert.Equal(operationToUpdate.toolID, updatedOperation.toolID);
            Assert.Equal(operationToUpdate.parallel, updatedOperation.parallel);
            Assert.Equal(operationToUpdate.qGateID, updatedOperation.qGateID);
            //Assert.Equal(operationToUpdate.lastModified.ToString("yyyy-MM-dd HH:mm:ss"), updatedOperation.lastModified.ToString("yyyy-MM-dd HH:mm:ss"));
        }



        [Fact]
        public async void GetOperationByIdTest_Works()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            DateTime now = DateTime.Now;

            var originalOperation = new
            {
                operationShortname = "Operation_" + timestamp,
                operationDescription = "Description_" + timestamp,
                operationSequenceGroup = "Group1",
                operationSequence = "Sequence1",
                operationDecisionCriteria = "Criteria1",
                alwaysPerform = true,
                decisionClassID = 1,
                savingClassID = 1,
                generationClassID = 1,
                verificationClassID = 1,
                toolID = 1,
                parallel = false,
                qGateID = 1,
                lastModified = now
            };

            _connection.Execute(@"
        IF NOT EXISTS (SELECT 1 FROM Operation WHERE operationShortname = @operationShortname)
        BEGIN
            INSERT INTO Operation (
                operationShortname, operationDescription, operationSequenceGroup, operationSequence,
                operationDecisionCriteria, alwaysPerform, decisionClassID, savingClassID,
                generationClassID, verificationClassID, toolID, parallel, qGateID, lastModified)
            VALUES (
                @operationShortname, @operationDescription, @operationSequenceGroup, @operationSequence,
                @operationDecisionCriteria, @alwaysPerform, @decisionClassID, @savingClassID,
                @generationClassID, @verificationClassID, @toolID, @parallel, @qGateID, @lastModified);
        END",
                originalOperation);

            var operationId = await _connection.QuerySingleAsync<int>(
                "SELECT operationID FROM Operation WHERE operationShortname = @operationShortname",
                new { originalOperation.operationShortname });

            var retrievedOperation = await operationRepository.Get(operationId);

            Assert.NotNull(retrievedOperation);
            Assert.Equal(operationId, retrievedOperation.operationID);
            Assert.Equal(originalOperation.operationShortname, retrievedOperation.operationShortname);
            Assert.Equal(originalOperation.operationDescription, retrievedOperation.operationDescription);
            Assert.Equal(originalOperation.operationSequenceGroup, retrievedOperation.operationSequenceGroup);
            Assert.Equal(originalOperation.operationSequence, retrievedOperation.operationSequence);
            Assert.Equal(originalOperation.operationDecisionCriteria, retrievedOperation.operationDecisionCriteria);
            Assert.Equal(originalOperation.alwaysPerform, retrievedOperation.alwaysPerform);
            Assert.Equal(originalOperation.decisionClassID, retrievedOperation.decisionClassID);
            Assert.Equal(originalOperation.savingClassID, retrievedOperation.savingClassID);
            Assert.Equal(originalOperation.generationClassID, retrievedOperation.generationClassID);
            Assert.Equal(originalOperation.verificationClassID, retrievedOperation.verificationClassID);
            Assert.Equal(originalOperation.toolID, retrievedOperation.toolID);
            Assert.Equal(originalOperation.parallel, retrievedOperation.parallel);
            Assert.Equal(originalOperation.qGateID, retrievedOperation.qGateID);
            //Assert.Equal(originalOperation.lastModified.ToString("yyyy-MM-dd HH:mm:ss"), retrievedOperation.lastModified.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        [Fact]
        public async void GetAllOperationsTest_Works()
        {
            var expectedCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Operation;");

            var allOperations = await operationRepository.GetAll();

            Assert.NotNull(allOperations);
            Assert.Equal(expectedCount, allOperations.Count());
        }



        public void Dispose()
        {
            _connection.Dispose();
        }

    }
}
