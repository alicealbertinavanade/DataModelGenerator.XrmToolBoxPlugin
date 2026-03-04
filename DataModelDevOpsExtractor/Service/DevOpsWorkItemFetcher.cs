using DataModelDevOpsExtractor.Repository;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModelDevOpsExtractor.Repository;

namespace DataModelDevOpsExtractor.Service
{
    public static class DevOpsWorkItemFetcher
    {
        public static async Task<List<string>> FetchWorkItemDescriptionsAsync(string connStr, IEnumerable<int> ids)
        {
            var devopsRepo = new DevOpsRepository(connStr);
            var items = await devopsRepo.GetMRDetailsById(ids.ToArray());

            var result = new List<string>();
            foreach (var wi in items)
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
