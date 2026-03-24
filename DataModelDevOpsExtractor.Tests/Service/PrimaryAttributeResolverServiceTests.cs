using DataModelDevOpsExtractor.Service;
using DataModelDevOpsExtractor.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DataModelDevOpsExtractor.Tests.Service
{
    [TestClass]
    public class PrimaryAttributeResolverServiceTests
    {
        [TestMethod]
        public void BuildPrimaryAttributeMap_WithNullInput_ReturnsEmptyDictionary()
        {
            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_WithEmptyList_ReturnsEmptyDictionary()
        {
            // Arrange
            var emptyList = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>();

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(emptyList);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_WithSinglePrimaryColumn_ReturnsSingleEntry()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "test_name",
                    primary: "Y"
                )
            };

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.ContainsKey("test_table"));
            Assert.AreEqual("test_name", result["test_table"].SchemaName);
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_WithNonPrimaryColumns_ReturnsEmpty()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "column1",
                    primary: "N"
                ),
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "column2",
                    primary: "N"
                )
            };

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_WithMultipleTables_ReturnsMultipleEntries()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "table1",
                    schemaName: "name1",
                    primary: "Y"
                ),
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "table1",
                    schemaName: "column1",
                    primary: "N"
                ),
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "table2",
                    schemaName: "name2",
                    primary: "Y"
                )
            };

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.ContainsKey("table1"));
            Assert.IsTrue(result.ContainsKey("table2"));
            Assert.AreEqual("name1", result["table1"].SchemaName);
            Assert.AreEqual("name2", result["table2"].SchemaName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildPrimaryAttributeMap_WithDuplicatePrimaryColumns_ThrowsException()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "name1",
                    primary: "Y"
                ),
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "name2",
                    primary: "Y"
                )
            };

            // Act
            PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert - Exception expected
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_WithSamePrimaryColumnTwice_AllowsDuplicate()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "name1",
                    primary: "Y"
                ),
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "name1",
                    primary: "Y"
                )
            };

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("name1", result["test_table"].SchemaName);
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_IsCaseInsensitive()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "TEST_TABLE",
                    schemaName: "name1",
                    primary: "Y"
                )
            };

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert
            Assert.IsTrue(result.ContainsKey("test_table"));
            Assert.IsTrue(result.ContainsKey("TEST_TABLE"));
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_WithEmptyTableName_SkipsRow()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "",
                    schemaName: "name1",
                    primary: "Y"
                )
            };

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void BuildPrimaryAttributeMap_PreservesAllFieldsFromRow()
        {
            // Arrange
            var rows = new List<DataModelDevOpsExtractor.Model.DataModelTaskRow>
            {
                TestDataBuilder.CreateDataModelTaskRow(
                    table: "test_table",
                    schemaName: "test_name",
                    displayNameIt: "Nome Test IT",
                    displayNameEn: "Test Name EN",
                    description: "Test Description",
                    columnType: "String",
                    additionalData: "MaxLength=100",
                    requirementLevel: "ApplicationRequired",
                    primary: "Y"
                )
            };

            // Act
            var result = PrimaryAttributeResolverService.BuildPrimaryAttributeMap(rows);

            // Assert
            Assert.AreEqual(1, result.Count);
            var primaryAttr = result["test_table"];
            Assert.AreEqual("test_name", primaryAttr.SchemaName);
            Assert.AreEqual("Nome Test IT", primaryAttr.DisplayNameIt);
            Assert.AreEqual("Test Name EN", primaryAttr.DisplayNameEn);
            Assert.AreEqual("Test Description", primaryAttr.Description);
            Assert.AreEqual("String", primaryAttr.ColumnType);
            Assert.AreEqual("MaxLength=100", primaryAttr.AdditionalData);
            Assert.AreEqual("ApplicationRequired", primaryAttr.RequirementLevel);
        }
    }
}
