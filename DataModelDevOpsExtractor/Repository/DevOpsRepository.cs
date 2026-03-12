using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using DataModelDevOpsExtractor.Model;
using System.Windows;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System.Net;
using System.Text.RegularExpressions;

namespace DataModelDevOpsExtractor.Repository
{
    public class DevOpsRepository
    {
        VssConnection connection;
        DevOpsConnectionString parsedConnectionString;

        public DevOpsRepository(string connectionString)
        {
            this.parsedConnectionString = DevOpsConnectionString.Parse(connectionString);
            this.connection = new VssConnection(parsedConnectionString.OrgUrl, new VssBasicCredential(string.Empty, this.parsedConnectionString.PersonalAccessToken));
        }


        public async Task<IList<WorkItem>> GetMRDetailsById(params int[] Ids)
        {
            // create instance of work item tracking http client
            using (var httpClient = new WorkItemTrackingHttpClient(this.parsedConnectionString.OrgUrl, new VssBasicCredential(string.Empty, this.parsedConnectionString.PersonalAccessToken)))
            {
                // build a list of the fields we want to see
                var fields = new[] { "System.Id", "System.Title", "System.State", "System.Description"};

                // get work items for the ids found in query
                return await httpClient.GetWorkItemsAsync(Ids, fields).ConfigureAwait(false);
            }
        }

        public async Task<List<WorkItem>> GetMRList(string queryId)
        {
            if (!Guid.TryParse(queryId, out Guid parsedGuid))
            {
                MessageBox.Show("Inserisci un GUID formalmente valido please ;)");
                return new List<WorkItem>();
            }

            // create instance of work item tracking http client
            using (var httpClient = new WorkItemTrackingHttpClient(this.parsedConnectionString.OrgUrl, new VssBasicCredential(string.Empty, this.parsedConnectionString.PersonalAccessToken)))
            {

                // build a list of the fields we want to see
                var fields = new[] { "System.Id", "System.Title", "System.State",
                    "Custom.D365Manualoperations","Custom.D365Deleteoperations", "Custom.D365Solution","Custom.D365SpecialOperations", "Custom.ArtifactVersion", "Custom.ArtifactVersionNOPROD", "Custom.SpecialQuery","Custom.D365Batch" };

                // get work items for the ids found in query
                var items = await httpClient.QueryByIdAsync(new Guid(queryId)).ConfigureAwait(false);

                if (!items.WorkItems.Any())
                {
                    return new List<WorkItem>();
                }
                var results = await httpClient.GetWorkItemsAsync(items.WorkItems.Select(i => i.Id), fields).ConfigureAwait(false);

                //  var results = items.WorkItems.Select(wi => new WorkItem()
                //{
                //	Id = wi.Id			
                // });
                return results.ToList();
            }
        }

        public async Task<WorkItem> CreateChildTaskAsync(int parentTaskId, string title, string description)
        {
            if (parentTaskId <= 0)
            {
                throw new ArgumentException("Parent Task ID non valido", nameof(parentTaskId));
            }

            var safeTitle = string.IsNullOrWhiteSpace(title) ? "[D365] - Modifica DATA MODEL entita" : title.Trim();
            var safeDescription = BuildAzureDevOpsHtmlDescription(description);

            using (var httpClient = new WorkItemTrackingHttpClient(this.parsedConnectionString.OrgUrl, new VssBasicCredential(string.Empty, this.parsedConnectionString.PersonalAccessToken)))
            {
                var patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Title",
                        Value = safeTitle
                    },
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Description",
                        Value = safeDescription
                    },
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/relations/-",
                        Value = new
                        {
                            rel = "System.LinkTypes.Hierarchy-Reverse",
                            url = BuildWorkItemUrl(parentTaskId)
                        }
                    }
                };

                return await httpClient.CreateWorkItemAsync(patchDocument, this.parsedConnectionString.Project, "Task").ConfigureAwait(false);
            }
        }

        private static string BuildAzureDevOpsHtmlDescription(string description)
        {
            var raw = (description ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "<div><p> </p><br><table style=\"box-sizing:border-box;border-collapse:collapse;margin:0px 0px 16px;cursor:default;display:block;overflow-x:auto;font-size:15px;\"><thead style=\"box-sizing:border-box;\"><tr style=\"box-sizing:border-box;\"></tr></thead><tbody style=\"box-sizing:border-box;\"></tbody></table><br><pre></pre> </div>";
            }

            // If caller already provides HTML, keep it as-is.
            if (Regex.IsMatch(raw, @"<\s*(div|p|br|table|ul|ol|li|span|pre|h[1-6])\b", RegexOptions.IgnoreCase))
            {
                return raw;
            }

            var tables = ExtractMarkdownTables(raw);
            if (tables.Count == 0)
            {
                var encoded = WebUtility.HtmlEncode(raw);
                return $"<div><p> </p><br><table style=\"box-sizing:border-box;border-collapse:collapse;margin:0px 0px 16px;cursor:default;display:block;overflow-x:auto;font-size:15px;\"><thead style=\"box-sizing:border-box;\"></thead><tbody style=\"box-sizing:border-box;\"></tbody></table><br><pre>{encoded}</pre> </div>";
            }

            const string tableStyle = "box-sizing:border-box;border-collapse:collapse;margin:0px 0px 16px;cursor:default;display:block;overflow-x:auto;font-size:15px;";
            const string theadStyle = "box-sizing:border-box;";
            const string trStyle = "box-sizing:border-box;";
            const string thStyle = "box-sizing:border-box;font-size:0.9375rem;font-weight:600;text-align:left;border-color:rgb(234, 234, 234);border-style:solid;border-width:1px;padding:13px 11px;";
            const string tbodyStyle = "box-sizing:border-box;";
            const string tdStyle = "box-sizing:border-box;border-color:rgb(234, 234, 234);border-style:solid;border-width:1px;text-align:left;padding:10px 13px;min-width:50px;max-width:1000px;";

            var html = new StringBuilder();
            html.Append("<div><p> </p><br>");

            foreach (var table in tables)
            {
                html.Append($"<table style=\"{tableStyle}\">");
                html.Append($"<thead style=\"{theadStyle}\"><tr style=\"{trStyle}\">");

                foreach (var header in table.Headers)
                {
                    html.Append($"<th style=\"{thStyle}\">{WebUtility.HtmlEncode(header)}</th>");
                }

                html.Append("</tr></thead>");
                html.Append($"<tbody style=\"{tbodyStyle}\">");

                foreach (var row in table.Rows)
                {
                    html.Append($"<tr style=\"{trStyle}\">");
                    for (var i = 0; i < table.Headers.Count; i++)
                    {
                        var cell = i < row.Count ? row[i] : string.Empty;
                        html.Append($"<td style=\"{tdStyle}\">{WebUtility.HtmlEncode(cell)}</td>");
                    }

                    html.Append("</tr>");
                }

                html.Append("</tbody></table>");
            }

            html.Append("<br><pre></pre> </div>");
            return html.ToString();
        }

        private static List<MarkdownTableBlock> ExtractMarkdownTables(string markdown)
        {
            var result = new List<MarkdownTableBlock>();
            var lines = markdown.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = (lines[i] ?? string.Empty).Trim();
                if (!IsPipeTableLine(line))
                {
                    continue;
                }

                var headerCells = ParseMarkdownRowCells(line);
                if (headerCells.Count == 0)
                {
                    continue;
                }

                var nextIndex = i + 1;
                if (nextIndex >= lines.Length)
                {
                    continue;
                }

                var separatorLine = (lines[nextIndex] ?? string.Empty).Trim();
                if (!IsSeparatorRow(separatorLine))
                {
                    continue;
                }

                var tableRows = new List<List<string>>();
                var cursor = nextIndex + 1;
                while (cursor < lines.Length)
                {
                    var rowLine = (lines[cursor] ?? string.Empty).Trim();
                    rowLine = RemoveSpecialCharactersFromRowLine(rowLine);
					if (!IsPipeTableLine(rowLine) || IsSeparatorRow(rowLine))
                    {
                        break;
                    }

                    var rowCells = ParseMarkdownRowCells(rowLine);
                    if (rowCells.Count > 0)
                    {
                        tableRows.Add(rowCells);
                    }

                    cursor++;
                }

                result.Add(new MarkdownTableBlock
                {
                    Headers = headerCells,
                    Rows = tableRows
                });

                i = cursor - 1;
            }

            return result;
        }

        private static string RemoveSpecialCharactersFromRowLine(string rowLine)
        {
            if (string.IsNullOrEmpty(rowLine))
            {
                return string.Empty;
            }

            // Remove control and invisible formatting chars that break markdown parsing.
            return Regex.Replace(rowLine, @"[\u0000-\u001F\u007F\u00A0\u200B-\u200D\uFEFF]", string.Empty);
        }

        private static bool IsPipeTableLine(string line)
        {
            return !string.IsNullOrWhiteSpace(line) && (line.StartsWith("|", StringComparison.Ordinal) || line.Contains("|"));
        }

        private static bool IsSeparatorRow(string line)
        {
            var cells = ParseMarkdownRowCells(line);
            if (cells.Count == 0)
            {
                return false;
            }

            return cells.All(c => Regex.IsMatch(c.Trim(), @"^:?-{3,}:?$"));
        }

        private static List<string> ParseMarkdownRowCells(string row)
        {
            var line = (row ?? string.Empty).Trim();
            if (line.StartsWith("|", StringComparison.Ordinal))
            {
                line = line.Substring(1);
            }

            if (line.EndsWith("|", StringComparison.Ordinal))
            {
                line = line.Substring(0, line.Length - 1);
            }

            var result = new List<string>();
            var current = new StringBuilder();
            var escaped = false;

            foreach (var ch in line)
            {
                if (escaped)
                {
                    current.Append(ch);
                    escaped = false;
                    continue;
                }

                if (ch == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (ch == '|')
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }

                current.Append(ch);
            }

            result.Add(current.ToString().Trim());
            return result;
        }

        private sealed class MarkdownTableBlock
        {
            public List<string> Headers { get; set; }
            public List<List<string>> Rows { get; set; }
        }

        private string BuildWorkItemUrl(int workItemId)
        {
            var baseUrl = this.parsedConnectionString.OrgUrl.ToString().TrimEnd('/');
            return $"{baseUrl}/_apis/wit/workItems/{workItemId}";
        }
    }
}
