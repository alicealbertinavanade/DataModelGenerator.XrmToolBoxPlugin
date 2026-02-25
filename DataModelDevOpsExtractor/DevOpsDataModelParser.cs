using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Avanade.XrmToolbox.DataModelDevOpsExtractor
{
    public static class DevOpsDataModelParser
    {
        // Esempio di parsing di una sezione "Data Model - ..." da testo
        public static List<string[]> ParseDataModelSection(string text)
        {
            var rows = new List<string[]>();
            var regex = new Regex(@"Data Model - [^\n]+\n(?<header>.+)\n(?<body>(?:.+\n)+)", RegexOptions.Multiline);
            var match = regex.Match(text);
            if (!match.Success) return rows;
            var header = match.Groups["header"].Value.Trim();
            var body = match.Groups["body"].Value.Trim();
            var lines = body.Split('\n');
            foreach (var line in lines)
                {
                    var cols = Regex.Split(line.Trim(), @"\s{2,}|\t"); // split by 2+ spaces or tab
                    if (cols.Length > 1) rows.Add(cols);
            }
            return rows;
        }
    }
}
