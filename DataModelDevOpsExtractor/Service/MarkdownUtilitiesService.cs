using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DataModelDevOpsExtractor.Service
{
    public static class MarkdownUtilitiesService
    {
        public static void ConfigureMarkdownEditorAppearance(RichTextBox markdownTextBox)
        {
            if (markdownTextBox == null)
            {
                return;
            }

            markdownTextBox.Font = CreatePreferredMarkdownFont();
            markdownTextBox.AcceptsTab = true;
            markdownTextBox.DetectUrls = false;
            markdownTextBox.WordWrap = false;
        }

        public static string NormalizePrefix(string prefix)
        {
            var value = (prefix ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.EndsWith("_", StringComparison.Ordinal) ? value : value + "_";
        }

        public static string ExtractEntityNameFromMarkdown(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return "<entita>";
            }

            var tableHeaderMatch = Regex.Match(markdown, @"^\s*##\s*Table\s*:\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (tableHeaderMatch.Success)
            {
                var tableName = tableHeaderMatch.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    return tableName;
                }
            }

            var tableRowMatch = Regex.Match(markdown, @"^\|\s*[^|]+\|\s*([^|]+)\|", RegexOptions.Multiline);
            if (tableRowMatch.Success)
            {
                var tableName = tableRowMatch.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(tableName) && !string.Equals(tableName, "Table", StringComparison.OrdinalIgnoreCase))
                {
                    return tableName;
                }
            }

            return "<entita>";
        }

        public static string NormalizeForComparison(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Trim();
        }

        public static string BuildMarkdownTemplateFromPrimaryConnectionLanguages(IOrganizationService service)
        {
            var headers = new List<string>
            {
                "System",
                "Table",
                "Schema name"
            };

            foreach (var languageCode in GetDisplayLanguageCodes(service))
            {
                headers.Add($"Display name ({languageCode})");
            }

            headers.Add("Description");
            headers.Add("Column type");
            headers.Add("Lookup table");
            headers.Add("Additional data");
            headers.Add("Requirement level");
            headers.Add("Primary");
            headers.Add("Usage");

            var separator = string.Join(" | ", headers.Select(_ => "---"));
            var emptyRow = string.Join(" | ", headers.Select(_ => string.Empty));

            return string.Join(Environment.NewLine, new[]
            {
                "## Table: <table_logical_name>",
                "Name EN : <table_display_name_en>",
                "Name IT : <table_display_name_it>",
                $"| {string.Join(" | ", headers)} |",
                $"| {separator} |",
                $"| {emptyRow} |"
            });
        }

        private static Font CreatePreferredMarkdownFont()
        {
            var preferredFonts = new[] { "Cascadia Mono", "Cascadia Code", "Consolas", "Courier New" };
            var installed = new InstalledFontCollection();
            var installedNames = new HashSet<string>(installed.Families.Select(f => f.Name), StringComparer.OrdinalIgnoreCase);

            foreach (var preferred in preferredFonts)
            {
                if (installedNames.Contains(preferred))
                {
                    return new Font(preferred, 10.5f, FontStyle.Regular, GraphicsUnit.Point);
                }
            }

            return new Font(FontFamily.GenericMonospace, 10.5f, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static List<string> GetDisplayLanguageCodes(IOrganizationService service)
        {
            var fallback = new List<string> { "IT", "EN" };
            if (service == null)
            {
                return fallback;
            }

            var result = new List<string>();
            try
            {
                var response = (RetrieveAvailableLanguagesResponse)service.Execute(new RetrieveAvailableLanguagesRequest());
                var localeIds = response?.LocaleIds ?? new int[0];

                foreach (var localeId in localeIds.Where(id => id > 0).Distinct())
                {
                    try
                    {
                        var culture = CultureInfo.GetCultureInfo(localeId);
                        var code = (culture?.TwoLetterISOLanguageName ?? string.Empty).ToUpperInvariant();
                        if (!string.IsNullOrWhiteSpace(code) && !result.Contains(code, StringComparer.OrdinalIgnoreCase))
                        {
                            result.Add(code);
                        }
                    }
                    catch
                    {
                        // Ignora culture non risolvibili.
                    }
                }
            }
            catch
            {
                return fallback;
            }

            if (result.Count == 0)
            {
                return fallback;
            }

            if (!result.Contains("IT", StringComparer.OrdinalIgnoreCase))
            {
                result.Add("IT");
            }

            if (!result.Contains("EN", StringComparer.OrdinalIgnoreCase))
            {
                result.Add("EN");
            }

            return result;
        }
    }
}
