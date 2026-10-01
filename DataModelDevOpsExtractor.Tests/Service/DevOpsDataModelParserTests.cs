using DataModelDevOpsExtractor.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataModelDevOpsExtractor.Tests.Service
{
    [TestClass]
    public class DevOpsDataModelParserTests
    {
        [TestMethod]
        public void ParseDataModelSection_SkipsHeaderAndKeepsTableValueFromDataRow()
        {
            var description = @"<table>
<tr><th>System</th><th>Table</th><th>Schema name</th></tr>
<tr><td>CRM</td><td>dmt_account</td><td>account_id</td></tr>
</table>";

            var rows = DevOpsDataModelParser.ParseDataModelSection(description);

            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("CRM", rows[0][0]);
            Assert.AreEqual("dmt_account", rows[0][1]);
        }

        [TestMethod]
        public void ParseDataModelSection_RecognizesMarkdownHeaderOnlyWhenFollowedBySeparator()
        {
            var markdown = @"| Colonna1 | Colonna2 |
| --- |--|
| CRM | dmt_account |";

            var rows = DevOpsDataModelParser.ParseDataModelSection(markdown);

            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("CRM", rows[0][0]);
            Assert.AreEqual("dmt_account", rows[0][1]);
        }

        [TestMethod]
        public void ParseDataModelSection_DoesNotTreatKnownColumnNamesAsHeaderWithoutSeparator()
        {
            var markdown = @"| System | Table |
| CRM | dmt_account |";

            var rows = DevOpsDataModelParser.ParseDataModelSection(markdown);

            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual("System", rows[0][0]);
            Assert.AreEqual("Table", rows[0][1]);
        }
    }
}