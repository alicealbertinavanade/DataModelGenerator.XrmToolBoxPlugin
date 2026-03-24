using DataModelDevOpsExtractor.Model;
using DataModelDevOpsExtractor.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataModelDevOpsExtractor.Tests.Model
{
    [TestClass]
    public class DataModelTaskRowTests
    {
        [TestMethod]
        public void DataModelTaskRow_CanBeCreated()
        {
            // Act
            var row = new DataModelTaskRow
            {
                Row = new string[] { "System", "Table", "Schema" },
                TableName = "TestTable",
                TableDisplayNameEn = "Test Table EN",
                TableDisplayNameIt = "Test Table IT"
            };

            // Assert
            Assert.IsNotNull(row);
            Assert.IsNotNull(row.Row);
            Assert.AreEqual(3, row.Row.Length);
            Assert.AreEqual("TestTable", row.TableName);
            Assert.AreEqual("Test Table EN", row.TableDisplayNameEn);
            Assert.AreEqual("Test Table IT", row.TableDisplayNameIt);
        }

        [TestMethod]
        public void DataModelTaskRow_TestDataBuilder_CreatesValidRow()
        {
            // Act
            var row = TestDataBuilder.CreateDataModelTaskRow(
                system: "TestSystem",
                table: "test_table",
                schemaName: "test_column"
            );

            // Assert
            Assert.IsNotNull(row);
            Assert.IsNotNull(row.Row);
            Assert.AreEqual(12, row.Row.Length);
            Assert.AreEqual("TestSystem", row.Row[0]);
            Assert.AreEqual("test_table", row.Row[1]);
            Assert.AreEqual("test_column", row.Row[2]);
        }

        [TestMethod]
        public void DataModelTaskRow_WithAllFields_StoresDataCorrectly()
        {
            // Arrange & Act
            var row = TestDataBuilder.CreateDataModelTaskRow(
                system: "System1",
                table: "account",
                schemaName: "account_name",
                displayNameIt: "Nome Account",
                displayNameEn: "Account Name",
                description: "The name of the account",
                columnType: "String",
                lookupTable: "",
                additionalData: "MaxLength=100",
                requirementLevel: "ApplicationRequired",
                primary: "Y",
                usage: "IN_USE"
            );

            // Assert
            Assert.AreEqual("System1", row.Row[0]);
            Assert.AreEqual("account", row.Row[1]);
            Assert.AreEqual("account_name", row.Row[2]);
            Assert.AreEqual("Nome Account", row.Row[3]);
            Assert.AreEqual("Account Name", row.Row[4]);
            Assert.AreEqual("The name of the account", row.Row[5]);
            Assert.AreEqual("String", row.Row[6]);
            Assert.AreEqual("", row.Row[7]);
            Assert.AreEqual("MaxLength=100", row.Row[8]);
            Assert.AreEqual("ApplicationRequired", row.Row[9]);
            Assert.AreEqual("Y", row.Row[10]);
            Assert.AreEqual("IN_USE", row.Row[11]);
        }
    }
}
