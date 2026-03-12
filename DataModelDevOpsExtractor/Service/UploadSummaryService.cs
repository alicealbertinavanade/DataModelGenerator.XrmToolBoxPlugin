using DataModelDevOpsExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataModelDevOpsExtractor.Service
{
    public static class UploadSummaryService
    {
        public static string BuildUploadSummaryText(List<UploadStatusEntry> statuses)
        {
            statuses = statuses ?? new List<UploadStatusEntry>();

            var createdCount = statuses.Count(s => s.Status == UploadResultStatus.Created);
            var existingCount = statuses.Count(s => s.Status == UploadResultStatus.Existing);
            var errorCount = statuses.Count(s => s.Status == UploadResultStatus.Error);

            var lines = new List<string>
            {
                "Upload Summary",
                $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                string.Empty,
                $"Created: {createdCount}",
                $"Existing: {existingCount}",
                $"Error: {errorCount}",
                string.Empty,
                "Tables"
            };

            var tableItems = statuses
                .Where(s => string.Equals(s.Kind, "Table", StringComparison.OrdinalIgnoreCase))
                .GroupBy(s => s.TableName ?? string.Empty)
                .Select(g => g.Last())
                .OrderBy(s => s.TableName)
                .ToList();

            if (tableItems.Count == 0)
            {
                lines.Add("- [INFO] Nessuna tabella elaborata");
            }
            else
            {
                foreach (var item in tableItems)
                {
                    lines.Add($"- [{GetStatusLabel(item.Status)}] {item.TableName}{FormatError(item.ErrorMessage)}");
                }
            }

            lines.Add(string.Empty);
            lines.Add("Columns");

            var columnItems = statuses
                .Where(s => string.Equals(s.Kind, "Column", StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.TableName)
                .ThenBy(s => s.ColumnName)
                .ToList();

            if (columnItems.Count == 0)
            {
                lines.Add("- [INFO] Nessuna colonna elaborata");
            }
            else
            {
                foreach (var item in columnItems)
                {
                    lines.Add($"- [{GetStatusLabel(item.Status)}] {item.TableName}.{item.ColumnName}{FormatError(item.ErrorMessage)}");
                }
            }

            return string.Join(Environment.NewLine, lines).Trim();
        }

        public static void ShowSummary(TextBox summaryTextBox, Label summaryLabel, List<UploadStatusEntry> statuses)
        {
            if (summaryTextBox == null || summaryLabel == null)
            {
                return;
            }

            summaryTextBox.Text = BuildUploadSummaryText(statuses);
            summaryTextBox.Visible = true;
            summaryLabel.Visible = true;
        }

        public static void HideSummary(TextBox summaryTextBox, Label summaryLabel)
        {
            if (summaryTextBox == null || summaryLabel == null)
            {
                return;
            }

            summaryTextBox.Text = string.Empty;
            summaryTextBox.Visible = false;
            summaryLabel.Visible = false;
        }

        private static string GetStatusLabel(UploadResultStatus status)
        {
            switch (status)
            {
                case UploadResultStatus.Created:
                    return "CREATED";
                case UploadResultStatus.Existing:
                    return "EXISTING";
                default:
                    return "ERROR";
            }
        }

        private static string FormatError(string error)
        {
            return string.IsNullOrWhiteSpace(error) ? string.Empty : $" - {error}";
        }
    }
}
