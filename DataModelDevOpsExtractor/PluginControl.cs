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
using DataModelDevOpsExtractor.Service;
using DataModelDevOpsExtractor.Model;
using DataModelDevOpsExtractor.Repository;

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
        private string dataModelEnvConnectionString = string.Empty;

        public PluginControl()
        {
            InitializeComponent();
            // Carica la connection string e la datamodeluri salvate
            txtConnectionString.Text = UserConfig.LoadConnectionString();
            // Carica la seconda connessione se presente
            dataModelEnvConnectionString = UserConfig.LoadDataModelEnvConnectionString();
        }
        private void ToolStripBtnDataModelEnv_Click(object sender, EventArgs e)
        {
            AddAdditionalOrganization();

            if (this.AdditionalConnectionDetails.Count == 0)
            {
                return;
            }

            if (this.AdditionalConnectionDetails != null && this.AdditionalConnectionDetails.Count > 1)
                this.RemoveAdditionalOrganization(this.AdditionalConnectionDetails[0]);

            toolStripBtnDataModelEnv.Text = $"Data Model Env: {this.AdditionalConnectionDetails[0].ConnectionName}";
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            UserConfig.SaveConnectionString(txtConnectionString.Text.Trim());
            MessageBox.Show("Connection string e DataModel URI salvate.");
        }

        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            btnExtract.Enabled = false;
            try
            {
                var dataModelService = new DataModelService(); 
                var allRows = await dataModelService.getDataModelRows(
                    txtConnectionString.Text.Trim(),
                    txtTaskIds.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                );
                // Header CSV
                string[] headers = new[] {
                    "System","Table","Schema name","Display name (IT)","Display name (EN)","Description","Column type","Lookup table","Additional data","Requirement level"
                };

                // Salva il file CSV nella cartella Data
                var dataDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                if (!System.IO.Directory.Exists(dataDir))
                    System.IO.Directory.CreateDirectory(dataDir);
                var filePath = System.IO.Path.Combine(dataDir, $"DataModel_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                using (var writer = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(string.Join(";", headers));
                    foreach (var row in allRows)
                    {
                        var cleaned = row.Select(cell =>
                            System.Text.RegularExpressions.Regex.Replace(
                                System.Net.WebUtility.HtmlDecode(cell ?? ""),
                                "[\u00A0\u200B\u200C\u200D\uFEFF]", // spazi non standard e caratteri invisibili
                                " "
                            ).Replace("\n", " ").Replace("\r", " ").Replace(";", ",").Trim()
                        ).ToArray();
                        writer.WriteLine(string.Join(";", cleaned));
                    }
                }

                MessageBox.Show($"File CSV creato: {filePath}");
                System.Diagnostics.Process.Start("explorer.exe", dataDir);
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
            base.UpdateConnection(newService, detail, actionName, parameter);
        }
        public override void ClosingPlugin(XrmToolBox.Extensibility.PluginCloseInfo info) { }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var connStr = txtConnectionString.Text.Trim();
                var dataDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                if (!System.IO.Directory.Exists(dataDir))
                    System.IO.Directory.CreateDirectory(dataDir);
                var filePath = System.IO.Path.Combine(dataDir, "connection.txt");
                var uriPath = System.IO.Path.Combine(dataDir, "datamodeluri.txt");
                System.IO.File.WriteAllText(filePath, connStr);
                UserConfig.SaveConnectionString(connStr);
                MessageBox.Show($"Connection string salvata in: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel salvataggio: {ex.Message}");
            }
        }
        protected override void ConnectionDetailsUpdated(NotifyCollectionChangedEventArgs e)
        {
            return;
        }

        private async void buttonUploadDataModel_Click(object sender, EventArgs e)
        {
            // Usa la seconda connection string (Data Model Env)
            if (string.IsNullOrWhiteSpace(dataModelEnvConnectionString))
            {
                MessageBox.Show("Connection string Data Model Env mancante. Configurala prima dal menu.");
                return;
            }

            var dataModelService = new DataModelService();
            var allRows = await dataModelService.getDataModelRows(
                dataModelEnvConnectionString,
                txtTaskIds.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            );

            var dataModelCrmService = this.AdditionalConnectionDetails.FirstOrDefault().GetCrmServiceClient();
            var prefixEnv = this.AdditionalConnectionDetails.FirstOrDefault().ConnectionName.Split('-').FirstOrDefault() ?? "env";
            var dataModelRepo = new DataModelRepository(dataModelCrmService, prefixEnv);

            foreach (var row in allRows)
            {
                // Assumi: row = [System, Table, Schema name, Display name (IT), Display name (EN), Description, Column type, Lookup table, Additional data, Requirement level, Usage]
                var system = row.ElementAtOrDefault(0)?.Trim();
                var table = row.ElementAtOrDefault(1)?.Trim();
                var schemaName = row.ElementAtOrDefault(2)?.Trim();
                var displayNameIt = row.ElementAtOrDefault(3)?.Trim();
                var displayNameEn = row.ElementAtOrDefault(4)?.Trim();
                var description = row.ElementAtOrDefault(5)?.Trim();
                var columnType = row.ElementAtOrDefault(6)?.Trim();
                var lookupTable = row.ElementAtOrDefault(7)?.Trim();
                var additionalData = row.ElementAtOrDefault(8)?.Trim();
                var requirementLevel = row.ElementAtOrDefault(9)?.Trim();
                var usage = row.ElementAtOrDefault(10)?.Trim();

                // 1. Verifica/crea tabella egl_table
                var tableEn = dataModelRepo.GetOrCreateTable(
                    table, 
                    system, 
                    displayNameEn, 
                    displayNameIt
                    ); // Implementa GetOrCreateTable
                if (tableEn == null)
                {
                    MessageBox.Show($"Errore nella creazione o recupero della tabella per {table} ({system}). La tabella {table} non sarà creata.");
                    continue;
                }
                
                // 2. Verifica se la colonna esiste già (per schemaName e tableId)
                var column = dataModelRepo.GetOrCreateColumn(
                    schemaName, 
                    tableEn,
                    additionalData,
                    displayNameIt,
                    displayNameEn,
                    description,
                    columnType,
                    lookupTable,
                    requirementLevel,
                    usage);
                if (column == null)
                {
                    MessageBox.Show($"Errore nella creazione o recupero della colonna {schemaName} per {table} ({system}). La colonna {schemaName} non sarà creata.");
                    continue;
                }

            }

            MessageBox.Show("Upload completato. Solo le colonne nuove sono state create.");
        }
    }
}
