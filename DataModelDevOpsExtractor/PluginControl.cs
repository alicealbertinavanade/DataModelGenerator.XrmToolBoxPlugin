using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace DataModelDevOpsExtractor
{
    public partial class PluginControl : MultipleConnectionsPluginControlBase
    {
        // Proprietà richieste da MultipleConnectionsPluginControlBase
        public new IOrganizationService Service { get; set; }
        public new ConnectionDetail ConnectionDetail { get; set; }
        public new event EventHandler OnRequestConnection;
        public new event EventHandler OnCloseTool;
        public new event EventHandler OnWorkAsync;

        public PluginControl()
        {
            InitializeComponent();
            btnExtract.Click += BtnExtract_Click;
            btnSave.Click += BtnSave_Click;
            // Carica la connection string salvata
            txtConnectionString.Text = UserConfig.LoadConnectionString();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            UserConfig.SaveConnectionString(txtConnectionString.Text.Trim());
            MessageBox.Show("Connection string salvata.");
        }

        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            btnExtract.Enabled = false;
            try
            {
                // Parsing input
                var connStr = txtConnectionString.Text.Trim();
                var taskIds = txtTaskIds.Text.Split(',');
                var ids = new List<int>();
                foreach (var id in taskIds)
                {
                    if (int.TryParse(id.Trim(), out int num)) ids.Add(num);
                }
                if (ids.Count == 0) { MessageBox.Show("Nessun ID valido."); return; }

                // Parsing connection string (esempio: https://dev.azure.com/org;project;PAT)
                var parts = connStr.Split(';');
                if (parts.Length < 3) { MessageBox.Show("Connection string non valida. Usa: https://dev.azure.com/org;project;PAT"); return; }
                var orgUrl = parts[0];
                var project = parts[1];
                var pat = parts[2];

                // Fetch work item descriptions
                var descriptions = await DevOpsWorkItemFetcher.FetchWorkItemDescriptionsAsync(orgUrl, project, pat, ids);

                // Headers come da CSV allegato
                string[] headers = new[] {
                    "(Do Not Modify) Column","(Do Not Modify) Row Checksum","(Do Not Modify) Modified On","System (Table) (Table)","Table","Schema name","Display name (IT)","Display name (EN)","Description","Column type","Lookup table","Additional data","Requirement level","Usage","Modified On","Modified By","Comments"
                };
                var allRows = new List<string[]>();
                foreach (var desc in descriptions)
                {
                    var rows = DevOpsDataModelParser.ParseDataModelSection(desc);
                    allRows.AddRange(rows);
                }
                if (allRows.Count == 0) { MessageBox.Show("Nessun data model trovato nei task."); return; }

                // Genera file Excel
                var downloads = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
                var filePath = System.IO.Path.Combine(downloads, $"DataModel_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                DataModelExcelExporter.ExportToExcel(allRows, headers, filePath);

                // Apri cartella
                System.Diagnostics.Process.Start("explorer.exe", downloads);
                MessageBox.Show($"File creato: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}");
            }
            finally
            {
                btnExtract.Enabled = true;
            }
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            Service = newService;
            ConnectionDetail = detail;
        }
        public override void ClosingPlugin(XrmToolBox.Extensibility.PluginCloseInfo info) { }
        protected override void ConnectionDetailsUpdated(NotifyCollectionChangedEventArgs e)
        {
            // Implementa la logica necessaria qui, oppure lascia vuoto se non ti serve
        }

        private void btnExtract_Click_1(object sender, EventArgs e)
        {

        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {

        }
    }
}
