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
            var filteredDescriptions = descriptions.Where(desc => desc.Contains("System (Table")).ToList();

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
            var filteredDescriptions = descriptions.Where(desc => desc.Contains("System (Table")).ToList();

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
                var fallbackLabel = BuildLabelFromTableName(tableName);

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

        private static string ExtractTaskName(string description, string tableName, string prefix, string language)
        {
            if (string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(language))
                return null;

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
                return tableName.Replace(prefix, "");
            }

            var value = WebUtility.HtmlDecode(Regex.Replace(match.Groups[1].Value, "<.*?>", " ")).Trim();
            return string.IsNullOrWhiteSpace(value) ? tableName.Replace(prefix, "") : value;
        }

        private static string BuildLabelFromTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return string.Empty;

            var normalized = tableName.Trim();
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
