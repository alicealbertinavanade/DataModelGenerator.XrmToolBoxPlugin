using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataModelDevOpsExtractor.Model;

namespace DataModelDevOpsExtractor.Service
{
    public class DataModelService
    {

        public DataModelService()
        {
        }
        public async Task<List<string[]>> getDataModelRows(string connectionString, string[] txtTaskIds)
        {
            var allRows = new List<string[]>();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("Connection string mancante. Inserisci la connection string prima di procedere.");
                return null;
            }

            var ids = new List<int>();
            foreach (var id in txtTaskIds)
            {
                if (int.TryParse(id.Trim(), out int num)) ids.Add(num);
            }
            if (ids.Count == 0)
            {
                MessageBox.Show("Nessun ID valido.");
                return null;
            }

            var descriptions = await DevOpsWorkItemFetcher.FetchWorkItemDescriptionsAsync(connectionString, ids);

            // Filtra solo i task con la struttura richiesta
            var filteredDescriptions = descriptions.Where(desc => desc.Contains("System")).ToList();

            if (filteredDescriptions.Count == 0)
            {
                MessageBox.Show("Nessun data model con la struttura richiesta trovato nei task.");
                return null;
            }

            // Estrai le righe del data model da ogni descrizione filtrata
            foreach (var desc in filteredDescriptions)
            {
                var rows = DevOpsDataModelParser.ParseDataModelSection(desc);
                allRows.AddRange(rows);
            }
            if (allRows.Count == 0)
            {
                MessageBox.Show("Nessun data model trovato nei task.");
                return null;
            }
            return allRows;
        }

        public async Task<List<DataModelTaskRow>> getDataModelRowsWithTableNames(string connectionString,string prefix, string[] txtTaskIds)
        {
            var result = new List<DataModelTaskRow>();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("Connection string mancante. Inserisci la connection string prima di procedere.");
                return null;
            }

            var ids = new List<int>();
            foreach (var id in txtTaskIds)
            {
                if (int.TryParse(id.Trim(), out int num)) ids.Add(num);
            }
            if (ids.Count == 0)
            {
                MessageBox.Show("Nessun ID valido.");
                return null;
            }

            var descriptions = await DevOpsWorkItemFetcher.FetchWorkItemDescriptionsAsync(connectionString, ids);
            var filteredDescriptions = descriptions.Where(desc => desc.Contains("System")).ToList();

            if (filteredDescriptions.Count == 0)
            {
                MessageBox.Show("Nessun data model con la struttura richiesta trovato nei task.");
                return null;
            }

            foreach (var desc in filteredDescriptions)
            {
                var rows = DevOpsDataModelParser.ParseDataModelSection(desc);
                if (rows == null || rows.Count == 0)
                {
                    continue;
                }

                var tableName = rows.Select(r => r.ElementAtOrDefault(1)).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
                var tableLabelEn = ExtractTaskName(desc, tableName, prefix, "EN");
                var tableLabelIt = ExtractTaskName(desc, tableName, prefix, "IT");
                var fallbackLabel = BuildLabelFromTableName(tableName, prefix);

                if (string.IsNullOrWhiteSpace(tableLabelEn))
                    tableLabelEn = fallbackLabel;
                if (string.IsNullOrWhiteSpace(tableLabelIt))
                    tableLabelIt = fallbackLabel;

                foreach (var row in rows)
                {
                    result.Add(new DataModelTaskRow
                    {
                        Row = row,
                        TableDisplayNameEn = tableLabelEn,
                        TableDisplayNameIt = tableLabelIt,
                        TableName = tableName
                    });
                }
            }

            if (result.Count == 0)
            {
                MessageBox.Show("Nessun data model trovato nei task.");
                return null;
            }

            return result;
        }

        public async Task<string> getDataModelMarkdown(string connectionString, string prefix, string[] txtTaskIds)
        {
            var taskRows = await getDataModelRowsWithTableNames(connectionString, prefix, txtTaskIds);
            if (taskRows == null || taskRows.Count == 0)
                return null;

            var sb = new StringBuilder();
            var grouped = taskRows
                .Where(r => !string.IsNullOrWhiteSpace(r.TableName))
                .GroupBy(r => r.TableName);

            foreach (var group in grouped)
            {
                var first = group.First();
                sb.AppendLine($"## Table: {group.Key}");
                sb.AppendLine($"Name EN : {first.TableDisplayNameEn}");
                sb.AppendLine($"Name IT : {first.TableDisplayNameIt}");
                sb.AppendLine("| System | Table | Schema name | Display name (IT) | Display name (EN) | Description | Column type | Lookup table | Additional data | Requirement level | Usage |");
                sb.AppendLine("|---|---|---|---|---|---|---|---|---|---|---|");

                foreach (var item in group)
                {
                    var row = item.Row ?? new string[0];
                    var values = new string[11];
                    for (var index = 0; index < values.Length; index++)
                    {
                        var value = row.ElementAtOrDefault(index) ?? string.Empty;
                        values[index] = value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ").Trim();
                    }

                    sb.AppendLine($"| {string.Join(" | ", values)} |");
                }

                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }

        public List<DataModelTaskRow> ParseDataModelMarkdown(string markdown, string prefix)
        {
            var result = new List<DataModelTaskRow>();
            if (string.IsNullOrWhiteSpace(markdown))
                return result;

            var lines = markdown.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            string currentTableName = null;
            string currentNameEn = null;
            string currentNameIt = null;

            foreach (var rawLine in lines)
            {
                var line = rawLine?.Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("## Table:", StringComparison.OrdinalIgnoreCase))
                {
                    currentTableName = line.Substring("## Table:".Length).Trim();
                    currentNameEn = null;
                    currentNameIt = null;
                    continue;
                }

                if (line.StartsWith("Name EN", StringComparison.OrdinalIgnoreCase))
                {
                    currentNameEn = ExtractNameFromMarkdownLine(line);
                    continue;
                }

                if (line.StartsWith("Name IT", StringComparison.OrdinalIgnoreCase))
                {
                    currentNameIt = ExtractNameFromMarkdownLine(line);
                    continue;
                }

                if (!line.StartsWith("|") || line.Replace(" ", "").StartsWith("|---"))
                    continue;

                var cells = line.Trim('|').Split('|').Select(c => c.Trim().Replace("\\|", "|")).ToArray();
                if (cells.Length < 10)
                    continue;

                if (string.Equals(cells.ElementAtOrDefault(0), "System", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(cells.ElementAtOrDefault(1), "Table", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var row = new string[11];
                for (var index = 0; index < row.Length; index++)
                {
                    row[index] = index < cells.Length ? cells[index] : string.Empty;
                }

                var tableName = !string.IsNullOrWhiteSpace(currentTableName)
                    ? currentTableName
                    : row.ElementAtOrDefault(1);

                var fallbackLabel = BuildLabelFromTableName(tableName, prefix);
                var nameEn = string.IsNullOrWhiteSpace(currentNameEn) ? fallbackLabel : currentNameEn;
                var nameIt = string.IsNullOrWhiteSpace(currentNameIt) ? fallbackLabel : currentNameIt;

                result.Add(new DataModelTaskRow
                {
                    Row = row,
                    TableName = tableName,
                    TableDisplayNameEn = nameEn,
                    TableDisplayNameIt = nameIt
                });
            }

            return result;
        }

        private static string ExtractTaskName(string description, string tableName, string prefix, string language)
        {
            if (string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(language))
                return BuildLabelFromTableName(tableName, prefix);

            var htmlPattern = $@"(?is)Name\s*{Regex.Escape(language)}\s*:\s*(.*?)(?:<br\b[^>]*>|</p>|$)";
            var match = Regex.Match(description, htmlPattern);

            if (!match.Success)
            {
                var plainText = Regex.Replace(WebUtility.HtmlDecode(description), "<.*?>", " ");
                var plainPattern = $@"(?is)[\s\S]*?Name\s*{Regex.Escape(language)}\s*:\s*(.+?)(?:\r?\n|$)[\s\S]*";
                match = Regex.Match(plainText, plainPattern);
            }

            if (!match.Success)
            {
                return BuildLabelFromTableName(tableName, prefix);
            }

            var value = WebUtility.HtmlDecode(Regex.Replace(match.Groups[1].Value, "<.*?>", " ")).Trim();
            return string.IsNullOrWhiteSpace(value) ? BuildLabelFromTableName(tableName, prefix) : value;
        }

        private static string ExtractNameFromMarkdownLine(string line)
        {
            var separatorIndex = line.IndexOf(':');
            if (separatorIndex < 0 || separatorIndex >= line.Length - 1)
                return string.Empty;

            return line.Substring(separatorIndex + 1).Trim();
        }

        private static string BuildLabelFromTableName(string tableName, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return string.Empty;

            var normalized = tableName.Trim();

            if (!string.IsNullOrWhiteSpace(prefix) && normalized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(prefix.Length);
            }

            var prefixSeparatorIndex = normalized.IndexOf('_');
            if (prefixSeparatorIndex >= 0 && prefixSeparatorIndex < normalized.Length - 1)
            {
                normalized = normalized.Substring(prefixSeparatorIndex + 1);
            }

            normalized = normalized.Replace("_", " ").Replace("-", " ");
            normalized = Regex.Replace(normalized, "(?<=[a-z])([A-Z])", " $1");
            normalized = Regex.Replace(normalized, "\\s+", " ").Trim();

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized.ToLowerInvariant());
        }
    }
}
