using DataModelDevOpsExtractor.Model;
using DataModelDevOpsExtractor.Repository;
using DataModelDevOpsExtractor.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace DataModelDevOpsExtractor.Tests.Repository
{
    [TestClass]
    public class DataModelRepositoryTests
    {
        private FakeOrganizationService _fakeService;
        private DataModelRepository _repository;
        private const string PREFIX = "test_";

        [TestInitialize]
        public void Setup()
        {
            _fakeService = new FakeOrganizationService();
            _repository = new DataModelRepository(_fakeService, PREFIX);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _fakeService.Clear();
        }

        [TestMethod]
        public void GetTableByName_WhenTableExists_ReturnsEntity()
        {
            // Arrange
            var tableName = "contact";
            var tableEntity = TestDataBuilder.CreateTableEntity(PREFIX, tableName);
            _fakeService.AddEntity(tableEntity);

            // Act
            var result = _repository.getTableByName(tableName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Entities.Count);
            Assert.AreEqual(tableName.ToLowerInvariant(), result.Entities[0][PREFIX + "name"]);
        }

        [TestMethod]
        public void GetTableByName_WhenTableDoesNotExist_ReturnsEmptyCollection()
        {
            // Arrange
            var tableName = "nonexistent";

            // Act
            var result = _repository.getTableByName(tableName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Entities.Count);
        }

        [TestMethod]
        public void GetOrCreateTable_WhenTableDoesNotExist_CreatesNewTable()
        {
            // Arrange
            var tableName = "new_table";
            var system = "TestSystem";
            var nameEn = "New Table EN";
            var nameIt = "New Table IT";

            // Act
            var result = _repository.GetOrCreateTable(tableName, system, nameEn, nameIt);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.Id);
            Assert.AreEqual(tableName.ToLowerInvariant(), result[PREFIX + "name"]);
            Assert.AreEqual(system, result[PREFIX + "systemid"]);
            Assert.AreEqual(nameEn, result[PREFIX + "label_en"]);
            Assert.AreEqual(nameIt, result[PREFIX + "label_it"]);
        }

        [TestMethod]
        public void GetOrCreateTable_WhenTableExists_ReturnsExistingTable()
        {
            // Arrange
            var tableName = "existing_table";
            var existingEntity = TestDataBuilder.CreateTableEntity(PREFIX, tableName);
            _fakeService.AddEntity(existingEntity);

            // Act
            var result = _repository.GetOrCreateTable(tableName, "System", "EN", "IT");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(existingEntity.Id, result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetOrCreateTable_WhenMultipleTablesExist_ThrowsException()
        {
            // Arrange
            var tableName = "duplicate_table";
            _fakeService.AddEntity(TestDataBuilder.CreateTableEntity(PREFIX, tableName));
            _fakeService.AddEntity(TestDataBuilder.CreateTableEntity(PREFIX, tableName));

            // Act
            _repository.GetOrCreateTable(tableName, "System", "EN", "IT");

            // Assert - Exception expected
        }

        [TestMethod]
        public void ColumnExists_WhenColumnExists_ReturnsTrue()
        {
            // Arrange
            var tableEntity = TestDataBuilder.CreateTableEntity(PREFIX, "test_table");
            _fakeService.AddEntity(tableEntity);
            var columnEntity = TestDataBuilder.CreateColumnEntity(PREFIX, "test_column", tableEntity.Id);
            _fakeService.AddEntity(columnEntity);

            // Act
            var result = _repository.ColumnExists("test_column", tableEntity.Id);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ColumnExists_WhenColumnDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var tableEntity = TestDataBuilder.CreateTableEntity(PREFIX, "test_table");
            _fakeService.AddEntity(tableEntity);

            // Act
            var result = _repository.ColumnExists("nonexistent_column", tableEntity.Id);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetOrCreateColumn_WhenColumnDoesNotExist_CreatesNewColumn()
        {
            // Arrange
            var tableEntity = TestDataBuilder.CreateTableEntity(PREFIX, "test_table");
            _fakeService.AddEntity(tableEntity);
            var columnName = "new_column";
            var displayNameIt = "Nuova Colonna";
            var displayNameEn = "New Column";

            // Act
            var result = _repository.GetOrCreateColumn(
                columnName,
                tableEntity,
                "additionalData",
                displayNameIt,
                displayNameEn,
                "description",
                "String",
                "",
                "None",
                "IN_USE"
            );

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.Id);
            Assert.AreEqual(columnName.ToLowerInvariant(), result[PREFIX + "schemaname"]);
            Assert.AreEqual(displayNameIt, result[PREFIX + "displayname_it"]);
            Assert.AreEqual(displayNameEn, result[PREFIX + "displayname_en"]);
        }

        [TestMethod]
        public void GetOrCreateColumn_WhenColumnExists_ReturnsExistingColumn()
        {
            // Arrange
            var tableEntity = TestDataBuilder.CreateTableEntity(PREFIX, "test_table");
            _fakeService.AddEntity(tableEntity);
            var existingColumn = TestDataBuilder.CreateColumnEntity(PREFIX, "existing_column", tableEntity.Id);
            _fakeService.AddEntity(existingColumn);

            // Act
            var result = _repository.GetOrCreateColumn(
                "existing_column",
                tableEntity,
                "",
                "IT",
                "EN",
                "desc",
                "String",
                "",
                "None",
                "IN_USE"
            );

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(existingColumn.Id, result.Id);
        }

        [TestMethod]
        public void GetOrCreateColumn_WithLookupType_SetsLookupTableReference()
        {
            // Arrange
            var tableEntity = TestDataBuilder.CreateTableEntity(PREFIX, "test_table");
            _fakeService.AddEntity(tableEntity);
            var lookupTableEntity = TestDataBuilder.CreateTableEntity(PREFIX, "lookup_table");
            _fakeService.AddEntity(lookupTableEntity);

            // Act
            var result = _repository.GetOrCreateColumn(
                "lookup_column",
                tableEntity,
                "",
                "IT",
                "EN",
                "desc",
                "Lookup",
                "lookup_table",
                "None",
                "IN_USE"
            );

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains(PREFIX + "lookuptableid"));
            var lookupRef = result[PREFIX + "lookuptableid"] as EntityReference;
            Assert.IsNotNull(lookupRef);
            Assert.AreEqual(lookupTableEntity.Id, lookupRef.Id);
        }

        [TestMethod]
        public void GetOrCreateColumn_WithInvalidColumnType_HandlesGracefully()
        {
            // Arrange
            var tableEntity = TestDataBuilder.CreateTableEntity(PREFIX, "test_table");
            _fakeService.AddEntity(tableEntity);

            // Act
            var result = _repository.GetOrCreateColumn(
                "test_column",
                tableEntity,
                "",
                "IT",
                "EN",
                "desc",
                "InvalidType",
                "",
                "None",
                "IN_USE"
            );

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
