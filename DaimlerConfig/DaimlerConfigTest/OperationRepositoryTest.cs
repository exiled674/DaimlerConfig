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



            //Stationrepo mit Verbindung intiialisiert
            operationRepository = new OperationRepository(connectionFactory);



        }



        [Fact]
        public async Task GetAll_ReturnsSameCountAsDirectSelect()
        {
            

            // Act: use repository
            var repoResults = (await operationRepository.GetAll()).ToList();

            // Direct count via raw SQL
            var expectedCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Operation");

            // Assert counts match
            Assert.Equal(expectedCount, repoResults.Count);
        }

        [Fact]
        public async Task Get_ReturnsOperation44_WhenExists()
        {
            // Arrange
            const int id = 44;
            // Fetch expected via raw SQL
            var expected = await _connection.QuerySingleAsync<Operation>(
                "SELECT * FROM [Operation] WHERE operationID = @Id", new { Id = id });

            // Act
            var actual = await operationRepository.Get(id);

            // Assert
            Assert.NotNull(actual);
            // Compare each property
            Assert.Equal(expected.operationID, actual.operationID);
            Assert.Equal(expected.toolID, actual.toolID);
            Assert.Equal(expected.operationShortname, actual.operationShortname);
            Assert.Equal(expected.operationDescription, actual.operationDescription);
            Assert.Equal(expected.operationSequence, actual.operationSequence);
            Assert.Equal(expected.operationSequenceGroup, actual.operationSequenceGroup);
            Assert.Equal(expected.operationDecisionCriteria, actual.operationDecisionCriteria);
            Assert.Equal(expected.alwaysPerform, actual.alwaysPerform);
            Assert.Equal(expected.decisionClassID, actual.decisionClassID);
            Assert.Equal(expected.generationClassID, actual.generationClassID);
            Assert.Equal(expected.verificationClassID, actual.verificationClassID);
            Assert.Equal(expected.savingClassID, actual.savingClassID);
            Assert.Equal(expected.parallel, actual.parallel);
            Assert.Equal(expected.qGateID, actual.qGateID);
            Assert.Equal(expected.lastModified, actual.lastModified);
        }

        [Fact]
        public async Task Add_InsertsNewOperation_WhenNotExists()
        {
            // Arrange
            var newOp = new Operation
            {
                operationShortname = "TestAdd",
                toolID = 111,
                alwaysPerform = false,
                decisionClassID = 4,
                generationClassID = 1,
                verificationClassID = 4,
                savingClassID = 4,
                parallel = false,
                qGateID = 3,
                lastModified = DateTime.UtcNow
            };

            // Act
            await operationRepository.Add(newOp);

            // Fetch inserted via raw SQL
            var inserted = await _connection.QuerySingleOrDefaultAsync<Operation>(
                "SELECT TOP 1 * FROM [Operation] WHERE operationShortname = @Name AND toolID = @ToolId ORDER BY operationID DESC",
                new { Name = newOp.operationShortname, ToolId = newOp.toolID });

            // Assert
            Assert.NotNull(inserted);
            Assert.Equal(newOp.operationShortname, inserted.operationShortname);
            Assert.Equal(newOp.toolID, inserted.toolID);
            Assert.Equal(newOp.decisionClassID, inserted.decisionClassID);

            // Cleanup: delete the test record
            await _connection.ExecuteAsync(
                "DELETE FROM [Operation] WHERE operationID = @Id",
                new { Id = inserted.operationID });
        }

        [Fact]
        public async Task Delete_RemovesOperation_WhenExists()
        {
            // Arrange: Neue Operation erstellen
            var tempOperation = new Operation
            {
                operationShortname = "ToBeDeleted",
                toolID = 111,
                alwaysPerform = false,
                decisionClassID = 4,
                generationClassID = 1,
                verificationClassID = 4,
                savingClassID = 4,
                parallel = false,
                qGateID = 3,
                lastModified = DateTime.UtcNow
            };

            // In DB einfügen
            await operationRepository.Add(tempOperation);

            // Wieder auslesen (um ID zu bekommen)
            var inserted = await _connection.QuerySingleOrDefaultAsync<Operation>(
                "SELECT TOP 1 * FROM [Operation] WHERE operationShortname = @Name AND toolID = @ToolId ORDER BY operationID DESC",
                new { Name = tempOperation.operationShortname, ToolId = tempOperation.toolID });

            Assert.NotNull(inserted);

            // Act: Löschen mit Repository
            await operationRepository.Delete(inserted);

            // Assert: Sicherstellen, dass sie gelöscht wurde
            var deleted = await _connection.QuerySingleOrDefaultAsync<Operation>(
                "SELECT * FROM [Operation] WHERE operationID = @Id",
                new { Id = inserted.operationID });

            Assert.Null(deleted);
        }


        [Fact]
        public async Task Update_ChangesOperationShortname_WhenCalled()
        {
            // Arrange: Neue Operation erstellen
            var tempOperation = new Operation
            {
                operationShortname = "BeforeUpdate",
                toolID = 111,
                alwaysPerform = false,
                decisionClassID = 4,
                generationClassID = 1,
                verificationClassID = 4,
                savingClassID = 4,
                parallel = false,
                qGateID = 3,
                lastModified = DateTime.UtcNow
            };

            // In DB einfügen
            await operationRepository.Add(tempOperation);

            // Wieder auslesen (inkl. ID)
            var inserted = await _connection.QuerySingleOrDefaultAsync<Operation>(
                "SELECT TOP 1 * FROM [Operation] WHERE operationShortname = @Name AND toolID = @ToolId ORDER BY operationID DESC",
                new { Name = tempOperation.operationShortname, ToolId = tempOperation.toolID });

            Assert.NotNull(inserted);

            // Act: Änderung vornehmen und Update aufrufen
            inserted.operationShortname = "AfterUpdate";
            inserted.lastModified = DateTime.UtcNow;

            await operationRepository.Update(inserted);

            // Aus DB erneut lesen
            var updated = await _connection.QuerySingleOrDefaultAsync<Operation>(
                "SELECT * FROM [Operation] WHERE operationID = @Id",
                new { Id = inserted.operationID });

            // Assert: prüfen ob Änderung übernommen wurde
            Assert.NotNull(updated);
            Assert.Equal("AfterUpdate", updated.operationShortname);

            // Cleanup
            await operationRepository.Delete(updated);
        }

        [Fact]
        public async Task GetByName_ReturnsCorrectOperation_WhenExists()
        {
            // Arrange
            var uniqueName = "GetByNameTestOp_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var newOp = new Operation
            {
                operationShortname = uniqueName,
                toolID = 111,
                alwaysPerform = false,
                decisionClassID = 4,
                generationClassID = 1,
                verificationClassID = 4,
                savingClassID = 4,
                parallel = false,
                qGateID = 3,
                lastModified = DateTime.UtcNow
            };

            await operationRepository.Add(newOp);

            // Hole eingefügte Operation mit SQL, um sicherzugehen
            var inserted = await _connection.QuerySingleOrDefaultAsync<Operation>(
                "SELECT TOP 1 * FROM [Operation] WHERE operationShortname = @Name ORDER BY operationID DESC",
                new { Name = uniqueName });

            Assert.NotNull(inserted);

            // Act
            var result = await operationRepository.GetByName(uniqueName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inserted.operationID, result.operationID);
            Assert.Equal(inserted.operationShortname, result.operationShortname);
            Assert.Equal(inserted.toolID, result.toolID);

            // Cleanup
            await operationRepository.Delete(inserted);
        }

        [Fact]
        public async Task GetOperationsFromTool_ReturnsOperationsWithGivenToolID()
        {
            // Arrange
            var uniqueShortname = "ToolOpTest_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            var testToolId = 111;

            var newOp = new Operation
            {
                operationShortname = uniqueShortname,
                toolID = testToolId,
                alwaysPerform = false,
                decisionClassID = 4,
                generationClassID = 1,
                verificationClassID = 4,
                savingClassID = 4,
                parallel = false,
                qGateID = 3,
                lastModified = DateTime.UtcNow
            };

            await operationRepository.Add(newOp);

            // Hole die eingefügte Operation mit SQL
            var inserted = await _connection.QuerySingleOrDefaultAsync<Operation>(
                "SELECT TOP 1 * FROM [Operation] WHERE operationShortname = @Name ORDER BY operationID DESC",
                new { Name = uniqueShortname });

            Assert.NotNull(inserted);

            // Act
            var result = (await operationRepository.GetOperationsFromTool(testToolId)).ToList();

            // Assert
            Assert.NotEmpty(result);
            Assert.Contains(result, op => op.operationID == inserted.operationID);

            // Cleanup
            await operationRepository.Delete(inserted);
        }



        public void Dispose()
        {
            _connection.Dispose();
        }

    }
}
