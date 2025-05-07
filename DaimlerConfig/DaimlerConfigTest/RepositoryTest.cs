using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Xunit;
using Moq;
using System.Data;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Infrastructure;

namespace DaimlerConfigTest
{
    public class RepositoryTests
    {
        [Fact]
        public async Task Add_ShouldInsertEntity()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockConnectionFactory = new Mock<IDbConnectionFactory>();
            mockConnectionFactory
                .Setup(factory => factory.CreateConnection())
                .Returns(mockConnection.Object);

            var repository = new Repository<TestEntity>(mockConnectionFactory.Object);

            var testEntity = new TestEntity
            {
                TestEntityID = 1,
                Name = "Test",
                LastModified = DateTime.Now
            };

            // Act
            await repository.Add(testEntity);

            // Assert
            mockConnectionFactory.Verify(factory => factory.CreateConnection(), Times.Once);
            mockConnection.Verify(conn => conn.Open(), Times.Once);
        }

        public class TestEntity
        {
            public int TestEntityID { get; set; }
            public string Name { get; set; }
            public DateTime LastModified { get; set; }
        }
    }
}
