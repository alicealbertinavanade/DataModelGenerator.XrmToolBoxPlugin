using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
    }
}
