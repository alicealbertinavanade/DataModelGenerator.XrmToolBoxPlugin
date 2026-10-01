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
using DataModelDevOpsExtractor.Repository;

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

            var descriptions = await FetchDataModelDescriptionsAsync(connectionString, txtTaskIds);

            // Estrai le righe del data model da ogni descrizione filtrata
            foreach (var desc in descriptions)
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

            var descriptions = await FetchDataModelDescriptionsAsync(connectionString, txtTaskIds);
            if (descriptions.Count == 0)
            {
                MessageBox.Show("Nessun data model trovato nei task o nella pagina Wiki indicata.");
                return null;
            }

            foreach (var desc in descriptions)
            {
                var rows = DevOpsDataModelParser.ParseDataModelSection(desc);
                if (rows == null || rows.Count == 0)
                {
                    continue;
                }

                result.AddRange(BuildTaskRowsForTables(desc, rows, prefix));
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

            return BuildMarkdownFromTaskRows(taskRows);
        }

        internal static List<DataModelTaskRow> BuildTaskRowsForTables(string description, IList<string[]> rows, string prefix)
        {
            var result = new List<DataModelTaskRow>();
            if (rows == null || rows.Count == 0)
            {
                return result;
            }

            var declaredTableMatch = Regex.Match(description ?? string.Empty, @"^\s*##\s*Table\s*:\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var declaredTableName = declaredTableMatch.Success ? declaredTableMatch.Groups[1].Value.Trim() : null;
            var primaryTableName = !string.IsNullOrWhiteSpace(declaredTableName)
                ? declaredTableName
                : rows.Select(row => row?.ElementAtOrDefault(1)).FirstOrDefault(name => !string.IsNullOrWhiteSpace(name))?.Trim();

            var primaryLabelEn = ExtractTaskName(description, primaryTableName, prefix, "EN");
            var primaryLabelIt = ExtractTaskName(description, primaryTableName, prefix, "IT");
            var primaryFallback = BuildLabelFromTableName(primaryTableName, prefix);
            if (string.IsNullOrWhiteSpace(primaryLabelEn))
                primaryLabelEn = primaryFallback;
            if (string.IsNullOrWhiteSpace(primaryLabelIt))
                primaryLabelIt = primaryFallback;

            foreach (var row in rows)
            {
                var tableName = row?.ElementAtOrDefault(1)?.Trim();
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    continue;
                }

                var isPrimaryTable = string.Equals(tableName, primaryTableName, StringComparison.OrdinalIgnoreCase);
                var fallbackLabel = BuildLabelFromTableName(tableName, prefix);
                result.Add(new DataModelTaskRow
                {
                    Row = row,
                    TableName = tableName,
                    TableDisplayNameEn = isPrimaryTable && !string.IsNullOrWhiteSpace(primaryLabelEn) ? primaryLabelEn : fallbackLabel,
                    TableDisplayNameIt = isPrimaryTable && !string.IsNullOrWhiteSpace(primaryLabelIt) ? primaryLabelIt : fallbackLabel
                });
            }

            return result;
        }

        internal static string BuildMarkdownFromTaskRows(IEnumerable<DataModelTaskRow> taskRows)
        {
            var sb = new StringBuilder();
            var grouped = (taskRows ?? Enumerable.Empty<DataModelTaskRow>())
                .Where(r => !string.IsNullOrWhiteSpace(r.TableName))
                .GroupBy(r => r.TableName, StringComparer.OrdinalIgnoreCase);

            foreach (var group in grouped)
            {
                var first = group.First();
                sb.AppendLine($"## Table: {group.Key}");
                sb.AppendLine($"Name EN : {first.TableDisplayNameEn}");
                sb.AppendLine($"Name IT : {first.TableDisplayNameIt}");
                sb.AppendLine("| System | Table | Schema name | Display name (IT) | Display name (EN) | Description | Column type | Lookup table | Additional data | Requirement level | Primary | Usage |");
                sb.AppendLine("|---|---|---|---|---|---|---|---|---|---|---|---|");

                foreach (var item in group)
                {
                    var row = item.Row ?? new string[0];
                    var values = new string[12];
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

        private static async Task<List<string>> FetchDataModelDescriptionsAsync(string connectionString, string[] sourceInput)
        {
            var values = (sourceInput ?? new string[0])
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .ToArray();

            if (values.Length == 1 && Uri.TryCreate(values[0], UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                if (!DevOpsWikiPageReference.TryParse(values[0], out var wikiPage))
                {
                    throw new ArgumentException("URL Wiki non riconosciuto. Incolla il link completo a una pagina Wiki Azure DevOps.");
                }

                var repository = new DevOpsRepository(connectionString);
                return new List<string> { await repository.GetWikiPageContentAsync(wikiPage).ConfigureAwait(false) };
            }

            var ids = values
                .Select(value => int.TryParse(value, out var id) ? (int?)id : null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToArray();

            if (ids.Length == 0)
            {
                return new List<string>();
            }

            var descriptions = await DevOpsWorkItemFetcher.FetchWorkItemDescriptionsAsync(connectionString, ids).ConfigureAwait(false);
            return descriptions
                .Where(description => DevOpsDataModelParser.ParseDataModelSection(description).Count > 0)
                .ToList();
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
            HeaderMapping currentHeaderMapping = null;

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
                    currentHeaderMapping = null;
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

                if (!line.Contains("|") || line.Replace(" ", "").StartsWith("|---"))
                    continue;

                var cells = line.Trim('|').Split('|').Select(c => c.Trim().Replace("\\|", "|")).ToArray();
                if (cells.Length < 10)
                    continue;

                if (string.Equals(cells.ElementAtOrDefault(0), "System", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(cells.ElementAtOrDefault(1), "Table", StringComparison.OrdinalIgnoreCase))
                {
                    currentHeaderMapping = BuildHeaderMapping(cells);
                    continue;
                }

                var row = new string[12];
                if (currentHeaderMapping == null)
                {
                    // Backward compatibility: fixed legacy column order.
                    for (var index = 0; index < row.Length; index++)
                    {
                        row[index] = index < cells.Length ? cells[index] : string.Empty;
                    }

                    // Legacy layout had Usage at index 10 and no Primary column.
                    if (cells.Length == 11)
                    {
                        row[11] = row[10];
                        row[10] = string.Empty;
                    }
                }
                else
                {
                    row[0] = GetCellValue(cells, currentHeaderMapping.SystemIndex);
                    row[1] = GetCellValue(cells, currentHeaderMapping.TableIndex);
                    row[2] = GetCellValue(cells, currentHeaderMapping.SchemaNameIndex);

                    var displayByLanguage = currentHeaderMapping.DisplayNameIndexesByLanguage;
                    row[3] = GetPreferredDisplayName(cells, displayByLanguage, "IT");
                    row[4] = GetPreferredDisplayName(cells, displayByLanguage, "EN");

                    row[5] = GetCellValue(cells, currentHeaderMapping.DescriptionIndex);
                    row[6] = GetCellValue(cells, currentHeaderMapping.ColumnTypeIndex);
                    row[7] = GetCellValue(cells, currentHeaderMapping.LookupTableIndex);
                    row[8] = GetCellValue(cells, currentHeaderMapping.AdditionalDataIndex);
                    row[9] = GetCellValue(cells, currentHeaderMapping.RequirementLevelIndex);
                    row[10] = GetCellValue(cells, currentHeaderMapping.PrimaryIndex);
                    row[11] = GetCellValue(cells, currentHeaderMapping.UsageIndex);
                }

                var tableName = ResolveTableName(currentTableName, row);

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

        private static string GetPreferredDisplayName(string[] cells, Dictionary<string, int> displayIndexesByLanguage, string preferredLanguage)
        {
            if (displayIndexesByLanguage == null || displayIndexesByLanguage.Count == 0)
            {
                return string.Empty;
            }

            if (displayIndexesByLanguage.TryGetValue(preferredLanguage, out var preferredIndex))
            {
                return GetCellValue(cells, preferredIndex);
            }

            var firstAvailable = displayIndexesByLanguage.Values.FirstOrDefault();
            return GetCellValue(cells, firstAvailable);
        }

        private static string GetCellValue(string[] cells, int index)
        {
            return index >= 0 && index < cells.Length ? cells[index] : string.Empty;
        }

        private static HeaderMapping BuildHeaderMapping(string[] headers)
        {
            var mapping = new HeaderMapping
            {
                SystemIndex = FindHeaderIndex(headers, "system"),
                TableIndex = FindHeaderIndex(headers, "table"),
                SchemaNameIndex = FindHeaderIndex(headers, "schemaname"),
                DescriptionIndex = FindHeaderIndex(headers, "description"),
                ColumnTypeIndex = FindHeaderIndex(headers, "columntype"),
                LookupTableIndex = FindHeaderIndex(headers, "lookuptable"),
                AdditionalDataIndex = FindHeaderIndex(headers, "additionaldata"),
                RequirementLevelIndex = FindHeaderIndex(headers, "requirementlevel"),
                PrimaryIndex = FindHeaderIndex(headers, "primary"),
                UsageIndex = FindHeaderIndex(headers, "usage")
            };

            var displayNameMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < headers.Length; index++)
            {
                var header = headers[index] ?? string.Empty;
                var match = Regex.Match(header, @"^\s*Display\s*name\s*\(([^\)]+)\)\s*$", RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    continue;
                }

                var language = match.Groups[1].Value.Trim().ToUpperInvariant();
                if (!string.IsNullOrWhiteSpace(language) && !displayNameMap.ContainsKey(language))
                {
                    displayNameMap[language] = index;
                }
            }

            // Legacy fallback when the markdown still uses fixed columns without explicit language token parsing.
            if (displayNameMap.Count == 0)
            {
                var displayNameItIndex = FindHeaderIndex(headers, "displayname(it)");
                var displayNameEnIndex = FindHeaderIndex(headers, "displayname(en)");

                if (displayNameItIndex >= 0)
                {
                    displayNameMap["IT"] = displayNameItIndex;
                }

                if (displayNameEnIndex >= 0)
                {
                    displayNameMap["EN"] = displayNameEnIndex;
                }
            }

            mapping.DisplayNameIndexesByLanguage = displayNameMap;
            return mapping;
        }

        private static int FindHeaderIndex(string[] headers, string expected)
        {
            var normalizedExpected = NormalizeHeader(expected);
            for (var index = 0; index < headers.Length; index++)
            {
                if (NormalizeHeader(headers[index]) == normalizedExpected)
                {
                    return index;
                }
            }

            return -1;
        }

        private static string NormalizeHeader(string value)
        {
            return Regex.Replace((value ?? string.Empty).Trim().ToLowerInvariant(), "\\s+", string.Empty);
        }

        private sealed class HeaderMapping
        {
            public int SystemIndex { get; set; }
            public int TableIndex { get; set; }
            public int SchemaNameIndex { get; set; }
            public int DescriptionIndex { get; set; }
            public int ColumnTypeIndex { get; set; }
            public int LookupTableIndex { get; set; }
            public int AdditionalDataIndex { get; set; }
            public int RequirementLevelIndex { get; set; }
            public int PrimaryIndex { get; set; }
            public int UsageIndex { get; set; }
            public Dictionary<string, int> DisplayNameIndexesByLanguage { get; set; }
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

        private static string ResolveTableName(string currentTableName, string[] row)
        {
            if (!string.IsNullOrWhiteSpace(currentTableName))
            {
                return currentTableName.Trim();
            }

            if (row == null || row.Length <= 1)
            {
                return string.Empty;
            }

            var tableName = row[1]?.Trim();
            return string.IsNullOrWhiteSpace(tableName) ? string.Empty : tableName;
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

            normalized = normalized.Replace("_", " ").Replace("-", " ");
            normalized = Regex.Replace(normalized, "(?<=[a-z])([A-Z])", " $1");
            normalized = Regex.Replace(normalized, "\\s+", " ").Trim();

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized.ToLowerInvariant());
        }
    }
}
