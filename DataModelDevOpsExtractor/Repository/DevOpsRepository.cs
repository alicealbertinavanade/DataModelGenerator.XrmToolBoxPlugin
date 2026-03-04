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
using static ScintillaNET.Style;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using DataModelDevOpsExtractor.Model;
using System.Windows;

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
    }
}
