using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace DataModelDevOpsExtractor.Service
{
    public static class DevOpsDataModelParser
    {
        // Esempio di parsing di una sezione "Data Model - ..." da testo
        public static List<string[]> ParseDataModelSection(string text)
        {
            var rows = new List<string[]>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return rows;
            }

            // Trova il blocco <table ...>...</table>
            var tableMatch = Regex.Match(text, @"<table[\s\S]*?</table>", RegexOptions.IgnoreCase);
            if (!tableMatch.Success)
            {
                return ParseMarkdownTable(text);
            }

            var tableHtml = tableMatch.Value;

            // Trova tutte le righe <tr>...</tr>
            var rowMatches = Regex.Matches(tableHtml, @"<tr[\s\S]*?</tr>", RegexOptions.IgnoreCase);
            foreach (Match rowMatch in rowMatches)
            {
                var rowHtml = rowMatch.Value;
                if (Regex.IsMatch(rowHtml, @"<th\b", RegexOptions.IgnoreCase))
                {
                    continue;
                }

                // Trova tutte le celle <td>...</td>
                var cellMatches = Regex.Matches(rowHtml, @"<td\b[^>]*>(.*?)</td>", RegexOptions.IgnoreCase);
                var cells = new List<string>();
                foreach (Match cell in cellMatches)
                {
                    // Rimuovi eventuali tag HTML interni e trimma
                    var cellText = Regex.Replace(cell.Groups[1].Value, "<.*?>", string.Empty);
                    cellText = WebUtility.HtmlDecode(cellText ?? string.Empty);
                    cellText = Regex.Replace(cellText, "[\u00A0\u200B\u200C\u200D\uFEFF]", " ");
                    cellText = Regex.Replace(cellText, @"[^\p{L}\p{N}\s\-_/().,:;\[\]]", string.Empty);
                    cellText = Regex.Replace(cellText, @"\s+", " ").Trim();
                    cells.Add(cellText);
                }
                if (cells.Count > 0)
                    rows.Add(cells.ToArray());
            }
            return rows;
        }

        private static List<string[]> ParseMarkdownTable(string text)
        {
            var rows = new List<string[]>();
            var lines = text.Split(new[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.None);

            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                if (!TryParseMarkdownRow(lines[lineIndex], out var cells))
                {
                    continue;
                }

                if (lineIndex + 1 < lines.Length && IsMarkdownSeparatorRow(lines[lineIndex + 1], cells.Length))
                {
                    lineIndex++;
                    continue;
                }

                if (!IsMarkdownSeparatorRow(lines[lineIndex], cells.Length))
                {
                    rows.Add(cells);
                }
            }

            return rows;
        }

        private static bool TryParseMarkdownRow(string line, out string[] cells)
        {
            cells = null;
            if (string.IsNullOrWhiteSpace(line) || !line.Contains("|"))
            {
                return false;
            }

            cells = line.Trim().Trim('|').Split('|');
            for (var index = 0; index < cells.Length; index++)
            {
                cells[index] = WebUtility.HtmlDecode(cells[index].Trim()).Replace("\\|", "|");
            }

            return cells.Length > 0;
        }

        private static bool IsMarkdownSeparatorRow(string line, int expectedCellCount)
        {
            if (!TryParseMarkdownRow(line, out var cells) || cells.Length != expectedCellCount)
            {
                return false;
            }

            foreach (var cell in cells)
            {
                if (!Regex.IsMatch(cell, @"^:?-{2,}:?$"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
