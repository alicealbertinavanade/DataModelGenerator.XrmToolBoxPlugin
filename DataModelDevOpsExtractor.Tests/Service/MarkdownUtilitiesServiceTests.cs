using DataModelDevOpsExtractor.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataModelDevOpsExtractor.Tests.Service
{
    [TestClass]
    public class MarkdownUtilitiesServiceTests
    {
        [TestMethod]
        public void NormalizePrefix_WithValidPrefix_ReturnsLowercase()
        {
            // Arrange
            var prefix = "TEST_";

            // Act
            var result = MarkdownUtilitiesService.NormalizePrefix(prefix);

            // Assert
            Assert.AreEqual("test_", result);
        }

        [TestMethod]
        public void NormalizePrefix_WithoutUnderscore_AddsUnderscore()
        {
            // Arrange
            var prefix = "test";

            // Act
            var result = MarkdownUtilitiesService.NormalizePrefix(prefix);

            // Assert
            Assert.AreEqual("test_", result);
        }

        [TestMethod]
        public void NormalizePrefix_WithWhitespace_TrimsAndNormalizes()
        {
            // Arrange
            var prefix = "  TEST  ";

            // Act
            var result = MarkdownUtilitiesService.NormalizePrefix(prefix);

            // Assert
            Assert.AreEqual("test_", result);
        }

        [TestMethod]
        public void NormalizePrefix_WithEmptyString_ReturnsUnderscore()
        {
            // Arrange
            var prefix = "";

            // Act
            var result = MarkdownUtilitiesService.NormalizePrefix(prefix);

            // Assert
            Assert.AreEqual("_", result);
        }

        [TestMethod]
        public void NormalizePrefix_WithNull_ReturnsUnderscore()
        {
            // Arrange
            string prefix = null;

            // Act
            var result = MarkdownUtilitiesService.NormalizePrefix(prefix);

            // Assert
            Assert.AreEqual("_", result);
        }

        [TestMethod]
        public void NormalizePrefix_WithMultipleUnderscores_KeepsOne()
        {
            // Arrange
            var prefix = "test___";

            // Act
            var result = MarkdownUtilitiesService.NormalizePrefix(prefix);

            // Assert
            Assert.AreEqual("test_", result);
        }

        [TestMethod]
        public void NormalizePrefix_WithMixedCase_NormalizesToLowercase()
        {
            // Arrange
            var prefix = "TeSt_PrEfIx_";

            // Act
            var result = MarkdownUtilitiesService.NormalizePrefix(prefix);

            // Assert
            Assert.AreEqual("test_prefix_", result);
        }

        [TestMethod]
        public void ParseDataModelMarkdown_WithoutTableHeader_UsesTableColumnAndStripsPrefixForLabel()
        {
            // Arrange
            var markdown = @"| System | Table | Schema name | Display name (IT) | Display name (EN) | Description | Column type | Lookup table | Additional data | Requirement level | Primary | Usage |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Account | dmt_account | account_id | ID account | Account ID | Identificativo account | String |  |  | Required | Yes | Standard |
| Contact | dmt_contact | contact_name | Nome contatto | Contact name | Nome del contatto | String |  |  | Optional | No | Standard |";

            // Act
            var result = new DataModelService().ParseDataModelMarkdown(markdown, "dmt_");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("dmt_account", result[0].TableName);
            Assert.AreEqual("Account", result[0].TableDisplayNameEn);
            Assert.AreEqual("Contact", result[1].TableDisplayNameEn);
        }

        [TestMethod]
        public void BuildLabelFromTableName_WithPrefix_StripsOnlyPrefix()
        {
            // Arrange
            var value = "dmt_user_profile";

            // Act
            var label = typeof(DataModelService).GetMethod("BuildLabelFromTableName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { value, "dmt_" });

            // Assert
            Assert.AreEqual("User Profile", label);
        }
    }
}
