using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataModelDevOpsExtractor
{
    public static class DevOpsWorkItemFetcher
    {
        public static async Task<List<string>> FetchWorkItemDescriptionsAsync(string orgUrl, string project, string pat, IEnumerable<int> ids)
        {
            var creds = new VssBasicCredential(string.Empty, pat);
            var connection = new VssConnection(new Uri(orgUrl), creds);
            var witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var result = new List<string>();
            var workItems = await witClient.GetWorkItemsAsync(ids.ToList(), expand: WorkItemExpand.All);
            foreach (var wi in workItems)
            {
                if (wi.Fields.TryGetValue("System.Description", out var descObj))
                {
                    result.Add(descObj?.ToString() ?? string.Empty);
                }
            }
            return result;
        }
    }
}
