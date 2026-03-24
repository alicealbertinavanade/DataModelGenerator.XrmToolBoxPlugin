using DataModelDevOpsExtractor.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataModelDevOpsExtractor.Tests.Model
{
    [TestClass]
    public class EnumTests
    {
        [TestMethod]
        public void ColumnTypeCode_HasExpectedValues()
        {
            // Assert
            Assert.IsTrue(Enum.IsDefined(typeof(ColumnTypeCode), (int)ColumnTypeCode.String));
            Assert.IsTrue(Enum.IsDefined(typeof(ColumnTypeCode), (int)ColumnTypeCode.Lookup));
            Assert.IsTrue(Enum.IsDefined(typeof(ColumnTypeCode), (int)ColumnTypeCode.DateTime));
        }

        [TestMethod]
        public void RequirementLevelCode_HasExpectedValues()
        {
            // Assert
            Assert.IsTrue(Enum.IsDefined(typeof(RequirementLevelCode), (int)RequirementLevelCode.None));
            Assert.IsTrue(Enum.IsDefined(typeof(RequirementLevelCode), (int)RequirementLevelCode.SystemRequired));
            Assert.IsTrue(Enum.IsDefined(typeof(RequirementLevelCode), (int)RequirementLevelCode.ApplicationRequired));
        }

        [TestMethod]
        public void UsageCode_HasExpectedValues()
        {
            // Assert
            Assert.IsTrue(Enum.IsDefined(typeof(UsageCode), (int)UsageCode.IN_USE));
            Assert.IsTrue(Enum.IsDefined(typeof(UsageCode), (int)UsageCode.DEPRECATED));
            Assert.IsTrue(Enum.IsDefined(typeof(UsageCode), (int)UsageCode.TO_BE_CREATED));
        }

        [TestMethod]
        public void UploadResultStatus_HasExpectedValues()
        {
            // Assert
            Assert.IsTrue(Enum.IsDefined(typeof(UploadResultStatus), (int)UploadResultStatus.Created));
            Assert.IsTrue(Enum.IsDefined(typeof(UploadResultStatus), (int)UploadResultStatus.Existing));
            Assert.IsTrue(Enum.IsDefined(typeof(UploadResultStatus), (int)UploadResultStatus.Error));
        }

        [TestMethod]
        public void ColumnTypeCode_ParseFromString_Success()
        {
            // Arrange
            var value = "String";

            // Act
            var result = Enum.TryParse<ColumnTypeCode>(value, true, out var parsed);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(ColumnTypeCode.String, parsed);
        }

        [TestMethod]
        public void RequirementLevelCode_ParseFromString_Success()
        {
            // Arrange
            var value = "ApplicationRequired";

            // Act
            var result = Enum.TryParse<RequirementLevelCode>(value, true, out var parsed);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(RequirementLevelCode.ApplicationRequired, parsed);
        }

        [TestMethod]
        public void UsageCode_ParseFromString_CaseInsensitive()
        {
            // Arrange
            var value = "in_use";

            // Act
            var result = Enum.TryParse<UsageCode>(value, true, out var parsed);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(UsageCode.IN_USE, parsed);
        }
    }
}
