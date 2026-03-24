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
    }
}
