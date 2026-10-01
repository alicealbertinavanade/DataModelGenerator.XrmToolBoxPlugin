using DataModelDevOpsExtractor.Service;
using DataModelDevOpsExtractor.Model;
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
        public void ParseDataModelSection_AcceptsMarkdownTableWithOneRecognizedHeader()
        {
            var markdown = @"| Colonna generica | Display name (EN) |
| --- |--|
| CRM | dmt_account |";

            var rows = DevOpsDataModelParser.ParseDataModelSection(markdown);

            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("CRM", rows[0][0]);
            Assert.AreEqual("dmt_account", rows[0][1]);
        }

        [TestMethod]
        public void ParseDataModelSection_IgnoresMarkdownTableWithoutSeparator()
        {
            var markdown = @"| System | Table |
| CRM | dmt_account |";

            var rows = DevOpsDataModelParser.ParseDataModelSection(markdown);

            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod]
        public void ParseDataModelSection_IgnoresUnrelatedMarkdownTable()
        {
            var markdown = @"| Colonna1 | Colonna2 |
| --- | --- |
| Valore 1 | Valore 2 |";

            var rows = DevOpsDataModelParser.ParseDataModelSection(markdown);

            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod]
        public void ParseDataModelSection_IgnoresUnrelatedHtmlTable()
        {
            var html = @"<table>
<tr><th>Nome</th><th>Valore</th></tr>
<tr><td>Account</td><td>Contoso</td></tr>
</table>";

            var rows = DevOpsDataModelParser.ParseDataModelSection(html);

            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod]
        public void DevOpsWikiPageReference_TryParse_ExtractsProjectWikiAndPageId()
        {
            var url = "https://dev.azure.com/DevOps-Applications-EGL/Front%20End%20Eni/_wiki/wikis/Front-End-Eni.wiki/19940/-D365-MA2611-TAD-BroadBand-Amministrativi-Cessazione-Amministrativa-Errata-Attivazione";

            var parsed = DevOpsWikiPageReference.TryParse(url, out var pageReference);

            Assert.IsTrue(parsed);
            Assert.AreEqual("Front End Eni", pageReference.Project);
            Assert.AreEqual("Front-End-Eni.wiki", pageReference.WikiIdentifier);
            Assert.AreEqual(19940, pageReference.PageId);
        }

        [TestMethod]
        public void DevOpsWikiPageReference_TryParse_ReturnsFalseForTaskUrl()
        {
            var parsed = DevOpsWikiPageReference.TryParse("https://dev.azure.com/example/project/_workitems/edit/123", out var pageReference);

            Assert.IsFalse(parsed);
            Assert.IsNull(pageReference);
        }
    }
}