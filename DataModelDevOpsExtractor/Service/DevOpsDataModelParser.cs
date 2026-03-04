using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataModelDevOpsExtractor.Service
{
    public static class DevOpsDataModelParser
    {
        // Esempio di parsing di una sezione "Data Model - ..." da testo
        public static List<string[]> ParseDataModelSection(string text)
        {
            var rows = new List<string[]>();
            // Trova il blocco <table ...>...</table>
            var tableMatch = Regex.Match(text, @"<table[\s\S]*?</table>", RegexOptions.IgnoreCase);
            if (!tableMatch.Success) return rows;
            var tableHtml = tableMatch.Value;

            // Trova tutte le righe <tr>...</tr>
            var rowMatches = Regex.Matches(tableHtml, @"<tr[\s\S]*?</tr>", RegexOptions.IgnoreCase);
            foreach (Match rowMatch in rowMatches)
            {
                var rowHtml = rowMatch.Value;
                // Trova tutte le celle <td>...</td>
                var cellMatches = Regex.Matches(rowHtml, @"<td[\s\S]*?>(.*?)</td>", RegexOptions.IgnoreCase);
                var cells = new List<string>();
                foreach (Match cell in cellMatches)
                {
                    // Rimuovi eventuali tag HTML interni e trimma
                    var cellText = Regex.Replace(cell.Groups[1].Value, "<.*?>", string.Empty).Trim();
                    cells.Add(cellText);
                }
                if (cells.Count > 0)
                    rows.Add(cells.ToArray());
            }
            return rows;
        }
    }
}
