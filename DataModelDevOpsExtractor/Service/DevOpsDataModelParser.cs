using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DataModelDevOpsExtractor.Service
{
    public static class DevOpsDataModelParser
    {
        private static readonly HashSet<string> DataModelHeaders = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "system",
            "systemtable",
            "table",
            "schemaname",
            "displaynameit",
            "displaynameen"
        };

        // Esempio di parsing di una sezione "Data Model - ..." da testo
        public static List<string[]> ParseDataModelSection(string text)
        {
            var rows = new List<string[]>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return rows;
            }

            var tableMatches = Regex.Matches(text, @"<table\b[\s\S]*?</table>", RegexOptions.IgnoreCase);
            if (tableMatches.Count == 0)
            {
                return ParseMarkdownTable(text);
            }

            foreach (Match tableMatch in tableMatches)
            {
                var tableRows = ParseHtmlTable(tableMatch.Value);
                var headerIndex = tableRows.FindIndex(row => IsDataModelHeader(row.Cells));
                if (headerIndex < 0)
                {
                    continue;
                }

                for (var rowIndex = headerIndex + 1; rowIndex < tableRows.Count; rowIndex++)
                {
                    if (tableRows[rowIndex].Cells.Count > 0)
                    {
                        rows.Add(tableRows[rowIndex].Cells.ToArray());
                    }
                }
            }

            return rows;
        }

        private static List<HtmlTableRow> ParseHtmlTable(string tableHtml)
        {
            var rows = new List<HtmlTableRow>();
            var rowMatches = Regex.Matches(tableHtml, @"<tr\b[^>]*>([\s\S]*?)</tr>", RegexOptions.IgnoreCase);
            foreach (Match rowMatch in rowMatches)
            {
                var cellMatches = Regex.Matches(rowMatch.Groups[1].Value, @"<(th|td)\b[^>]*>([\s\S]*?)</\1>", RegexOptions.IgnoreCase);
                var cells = new List<string>();
                foreach (Match cell in cellMatches)
                {
                    var cellText = Regex.Replace(cell.Groups[2].Value, "<.*?>", string.Empty);
                    cellText = WebUtility.HtmlDecode(cellText ?? string.Empty);
                    cellText = Regex.Replace(cellText, "[\u00A0\u200B\u200C\u200D\uFEFF]", " ");
                    cellText = Regex.Replace(cellText, @"[^\p{L}\p{N}\s\-_/().,:;\[\]]", string.Empty);
                    cellText = Regex.Replace(cellText, @"\s+", " ").Trim();
                    cells.Add(cellText);
                }

                if (cells.Count > 0)
                {
                    rows.Add(new HtmlTableRow { Cells = cells });
                }
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

                if (lineIndex + 1 >= lines.Length || !IsMarkdownSeparatorRow(lines[lineIndex + 1], cells.Length))
                {
                    continue;
                }

                lineIndex++;
                var isDataModelTable = IsDataModelHeader(cells);
                for (var dataRowIndex = lineIndex + 1; dataRowIndex < lines.Length; dataRowIndex++)
                {
                    if (!TryParseMarkdownRow(lines[dataRowIndex], out var dataCells) ||
                        IsMarkdownSeparatorRow(lines[dataRowIndex], dataCells.Length))
                    {
                        break;
                    }

                    if (isDataModelTable)
                    {
                        rows.Add(dataCells);
                    }

                    lineIndex = dataRowIndex;
                }
            }

            return rows;
        }

        private static bool IsDataModelHeader(IEnumerable<string> cells)
        {
            return cells.Any(cell => DataModelHeaders.Contains(NormalizeHeader(cell)));
        }

        private static string NormalizeHeader(string value)
        {
            return Regex.Replace((value ?? string.Empty).Trim().ToLowerInvariant(), @"[^a-z0-9]", string.Empty);
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

        private sealed class HtmlTableRow
        {
            public List<string> Cells { get; set; }
        }
    }
}
