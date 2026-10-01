using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataModelDevOpsExtractor.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataModelDevOpsExtractor.Tests.Service
{
    [TestClass]
    public class DataModelServiceTests
    {
        [TestMethod]
        public void BuildMarkdownFromTaskRows_SplitsDifferentTablesAndRegroupsInterleavedRows()
        {
            var description = @"## Table: egl_userprofile
Name EN : Userprofile
Name IT : Userprofile";
            var rows = new List<string[]>
            {
                CreateRow("egl_userprofile", "egl_resolution_gas_car_amm"),
                CreateRow("task", "egl_rc3_rcg3_desiredactivationdate"),
                CreateRow("egl_userprofile", "egl_resetting_incorrect_transfer_switch")
            };

            var taskRows = DataModelService.BuildTaskRowsForTables(description, rows, "egl_");
            var markdown = DataModelService.BuildMarkdownFromTaskRows(taskRows);

            var userProfileStart = markdown.IndexOf("## Table: egl_userprofile", System.StringComparison.Ordinal);
            var taskStart = markdown.IndexOf("## Table: task", System.StringComparison.Ordinal);
            Assert.IsTrue(userProfileStart >= 0);
            Assert.IsTrue(taskStart > userProfileStart);

            var userProfileSection = markdown.Substring(userProfileStart, taskStart - userProfileStart);
            var taskSection = markdown.Substring(taskStart);
            Assert.AreEqual(2, Regex.Matches(userProfileSection, @"\| D365 \| egl_userprofile \|").Count);
            Assert.AreEqual(1, Regex.Matches(taskSection, @"\| D365 \| task \|").Count);
            StringAssert.Contains(userProfileSection, "Name EN : Userprofile");
            StringAssert.Contains(taskSection, "Name EN : Task");
        }

        private static string[] CreateRow(string table, string schemaName)
        {
            return new[]
            {
                "D365", table, schemaName, string.Empty, string.Empty, string.Empty,
                "String", string.Empty, string.Empty, "Not required", "IN USE", string.Empty
            };
        }
    }
}