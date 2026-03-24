using DataModelDevOpsExtractor.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataModelDevOpsExtractor.Tests.Model
{
    [TestClass]
    public class UploadStatusEntryTests
    {
        [TestMethod]
        public void UploadStatusEntry_CanBeCreated()
        {
            // Act
            var entry = new UploadStatusEntry
            {
                Kind = "Table",
                TableName = "test_table",
                ColumnName = null,
                Status = UploadResultStatus.Created,
                ErrorMessage = null
            };

            // Assert
            Assert.IsNotNull(entry);
            Assert.AreEqual("Table", entry.Kind);
            Assert.AreEqual("test_table", entry.TableName);
            Assert.IsNull(entry.ColumnName);
            Assert.AreEqual(UploadResultStatus.Created, entry.Status);
            Assert.IsNull(entry.ErrorMessage);
        }

        [TestMethod]
        public void UploadStatusEntry_ForColumn_StoresColumnName()
        {
            // Act
            var entry = new UploadStatusEntry
            {
                Kind = "Column",
                TableName = "test_table",
                ColumnName = "test_column",
                Status = UploadResultStatus.Existing,
                ErrorMessage = null
            };

            // Assert
            Assert.AreEqual("Column", entry.Kind);
            Assert.AreEqual("test_table", entry.TableName);
            Assert.AreEqual("test_column", entry.ColumnName);
            Assert.AreEqual(UploadResultStatus.Existing, entry.Status);
        }

        [TestMethod]
        public void UploadStatusEntry_WithError_StoresErrorMessage()
        {
            // Act
            var entry = new UploadStatusEntry
            {
                Kind = "Column",
                TableName = "test_table",
                ColumnName = "test_column",
                Status = UploadResultStatus.Error,
                ErrorMessage = "Column creation failed"
            };

            // Assert
            Assert.AreEqual(UploadResultStatus.Error, entry.Status);
            Assert.AreEqual("Column creation failed", entry.ErrorMessage);
        }

        [TestMethod]
        public void UploadResultStatus_HasCorrectValues()
        {
            // Assert
            Assert.AreEqual(0, (int)UploadResultStatus.Created);
            Assert.AreEqual(1, (int)UploadResultStatus.Existing);
            Assert.AreEqual(2, (int)UploadResultStatus.Error);
        }
    }
}
