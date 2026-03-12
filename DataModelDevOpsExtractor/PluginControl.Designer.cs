namespace DataModelDevOpsExtractor
{
    partial class PluginControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
			this.txtConnectionString = new System.Windows.Forms.TextBox();
			this.txtTaskIds = new System.Windows.Forms.TextBox();
			this.btnExtract = new System.Windows.Forms.Button();
			this.lblConnectionString = new System.Windows.Forms.Label();
			this.lblTaskIds = new System.Windows.Forms.Label();
			this.lblPrefix = new System.Windows.Forms.Label();
			this.txtPrefix = new System.Windows.Forms.TextBox();
			this.buttonUploadDataModel = new System.Windows.Forms.Button();
			this.buttonUploadAmbiente = new System.Windows.Forms.Button();
			this.buttonLoadMarkdown = new System.Windows.Forms.Button();
			this.txtMarkdown = new System.Windows.Forms.RichTextBox();
			this.lblMarkdown = new System.Windows.Forms.Label();
			this.lblSolutionName = new System.Windows.Forms.Label();
			this.txtSolutionName = new System.Windows.Forms.TextBox();
			this.lblUploadSummary = new System.Windows.Forms.Label();
			this.txtUploadSummary = new System.Windows.Forms.TextBox();
			this.progressBarUploadAmbiente = new System.Windows.Forms.ProgressBar();
			this.lblUploadProgress = new System.Windows.Forms.Label();
			this.lblParentTaskId = new System.Windows.Forms.Label();
			this.txtParentTaskId = new System.Windows.Forms.TextBox();
			this.buttonCreateTask = new System.Windows.Forms.Button();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripBtnSave = new System.Windows.Forms.ToolStripButton();
			this.toolStripBtnDataModelEnv = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtConnectionString
			// 
			this.txtConnectionString.Location = new System.Drawing.Point(212, 38);
			this.txtConnectionString.Margin = new System.Windows.Forms.Padding(4);
			this.txtConnectionString.Multiline = true;
			this.txtConnectionString.Name = "txtConnectionString";
			this.txtConnectionString.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtConnectionString.Size = new System.Drawing.Size(670, 60);
			this.txtConnectionString.TabIndex = 1;
			// 
			// txtTaskIds
			// 
			this.txtTaskIds.Location = new System.Drawing.Point(212, 147);
			this.txtTaskIds.Margin = new System.Windows.Forms.Padding(4);
			this.txtTaskIds.Name = "txtTaskIds";
			this.txtTaskIds.Size = new System.Drawing.Size(670, 22);
			this.txtTaskIds.TabIndex = 3;
			// 
			// btnExtract
			// 
			this.btnExtract.Location = new System.Drawing.Point(1147, 260);
			this.btnExtract.Margin = new System.Windows.Forms.Padding(4);
			this.btnExtract.Name = "btnExtract";
			this.btnExtract.Size = new System.Drawing.Size(244, 28);
			this.btnExtract.TabIndex = 6;
			this.btnExtract.Text = "Export Data Model to CSV";
			this.btnExtract.UseVisualStyleBackColor = true;
			this.btnExtract.Click += new System.EventHandler(this.BtnExtract_Click);
			// 
			// lblConnectionString
			// 
			this.lblConnectionString.AutoSize = true;
			this.lblConnectionString.Location = new System.Drawing.Point(13, 41);
			this.lblConnectionString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblConnectionString.Name = "lblConnectionString";
			this.lblConnectionString.Size = new System.Drawing.Size(167, 16);
			this.lblConnectionString.TabIndex = 0;
			this.lblConnectionString.Text = "DevOps Connection String:";
			// 
			// lblTaskIds
			// 
			this.lblTaskIds.AutoSize = true;
			this.lblTaskIds.Location = new System.Drawing.Point(13, 150);
			this.lblTaskIds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblTaskIds.Name = "lblTaskIds";
			this.lblTaskIds.Size = new System.Drawing.Size(185, 16);
			this.lblTaskIds.TabIndex = 2;
			this.lblTaskIds.Text = "Task IDs (comma separated):";
			// 
			// lblPrefix
			// 
			this.lblPrefix.AutoSize = true;
			this.lblPrefix.Location = new System.Drawing.Point(13, 113);
			this.lblPrefix.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblPrefix.Name = "lblPrefix";
			this.lblPrefix.Size = new System.Drawing.Size(43, 16);
			this.lblPrefix.TabIndex = 4;
			this.lblPrefix.Text = "Prefix:";
			// 
			// txtPrefix
			// 
			this.txtPrefix.Location = new System.Drawing.Point(212, 110);
			this.txtPrefix.Margin = new System.Windows.Forms.Padding(4);
			this.txtPrefix.Name = "txtPrefix";
			this.txtPrefix.Size = new System.Drawing.Size(670, 22);
			this.txtPrefix.TabIndex = 6;
			// 
			// buttonUploadDataModel
			// 
			this.buttonUploadDataModel.Location = new System.Drawing.Point(895, 260);
			this.buttonUploadDataModel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonUploadDataModel.Name = "buttonUploadDataModel";
			this.buttonUploadDataModel.Size = new System.Drawing.Size(244, 28);
			this.buttonUploadDataModel.TabIndex = 11;
			this.buttonUploadDataModel.Text = "Upload to Data Model Env";
			this.buttonUploadDataModel.UseVisualStyleBackColor = true;
			this.buttonUploadDataModel.Click += new System.EventHandler(this.buttonUploadDataModel_Click);
			// 
			// buttonUploadAmbiente
			// 
			this.buttonUploadAmbiente.Location = new System.Drawing.Point(895, 224);
			this.buttonUploadAmbiente.Margin = new System.Windows.Forms.Padding(4);
			this.buttonUploadAmbiente.Name = "buttonUploadAmbiente";
			this.buttonUploadAmbiente.Size = new System.Drawing.Size(244, 28);
			this.buttonUploadAmbiente.TabIndex = 12;
			this.buttonUploadAmbiente.Text = "Upload to Env";
			this.buttonUploadAmbiente.UseVisualStyleBackColor = true;
			this.buttonUploadAmbiente.Click += new System.EventHandler(this.buttonUploadAmbiente_Click);
			// 
			// buttonLoadMarkdown
			// 
			this.buttonLoadMarkdown.Location = new System.Drawing.Point(895, 144);
			this.buttonLoadMarkdown.Margin = new System.Windows.Forms.Padding(4);
			this.buttonLoadMarkdown.Name = "buttonLoadMarkdown";
			this.buttonLoadMarkdown.Size = new System.Drawing.Size(244, 28);
			this.buttonLoadMarkdown.TabIndex = 10;
			this.buttonLoadMarkdown.Text = "Load Markdown From Tasks";
			this.buttonLoadMarkdown.UseVisualStyleBackColor = true;
			this.buttonLoadMarkdown.Click += new System.EventHandler(this.buttonLoadMarkdown_Click);
			// 
			// txtMarkdown
			// 
			this.txtMarkdown.Location = new System.Drawing.Point(16, 351);
			this.txtMarkdown.Margin = new System.Windows.Forms.Padding(4);
			this.txtMarkdown.Name = "txtMarkdown";
			this.txtMarkdown.Size = new System.Drawing.Size(1158, 374);
			this.txtMarkdown.TabIndex = 12;
			this.txtMarkdown.Text = "";
			this.txtMarkdown.WordWrap = false;
			// 
			// lblMarkdown
			// 
			this.lblMarkdown.AutoSize = true;
			this.lblMarkdown.Location = new System.Drawing.Point(13, 331);
			this.lblMarkdown.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblMarkdown.Name = "lblMarkdown";
			this.lblMarkdown.Size = new System.Drawing.Size(145, 16);
			this.lblMarkdown.TabIndex = 8;
			this.lblMarkdown.Text = "Data Model Markdown:";
			// 
			// lblSolutionName
			// 
			this.lblSolutionName.AutoSize = true;
			this.lblSolutionName.Location = new System.Drawing.Point(13, 230);
			this.lblSolutionName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblSolutionName.Name = "lblSolutionName";
			this.lblSolutionName.Size = new System.Drawing.Size(98, 16);
			this.lblSolutionName.TabIndex = 14;
			this.lblSolutionName.Text = "Solution Name:";
			// 
			// txtSolutionName
			// 
			this.txtSolutionName.Location = new System.Drawing.Point(212, 227);
			this.txtSolutionName.Margin = new System.Windows.Forms.Padding(4);
			this.txtSolutionName.Name = "txtSolutionName";
			this.txtSolutionName.Size = new System.Drawing.Size(670, 22);
			this.txtSolutionName.TabIndex = 8;
			// 
			// lblUploadSummary
			// 
			this.lblUploadSummary.AutoSize = true;
			this.lblUploadSummary.Location = new System.Drawing.Point(1206, 331);
			this.lblUploadSummary.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblUploadSummary.Name = "lblUploadSummary";
			this.lblUploadSummary.Size = new System.Drawing.Size(115, 16);
			this.lblUploadSummary.TabIndex = 12;
			this.lblUploadSummary.Text = "Upload Summary:";
			this.lblUploadSummary.Visible = false;
			// 
			// txtUploadSummary
			// 
			this.txtUploadSummary.Location = new System.Drawing.Point(1209, 351);
			this.txtUploadSummary.Margin = new System.Windows.Forms.Padding(4);
			this.txtUploadSummary.Multiline = true;
			this.txtUploadSummary.Name = "txtUploadSummary";
			this.txtUploadSummary.ReadOnly = true;
			this.txtUploadSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtUploadSummary.Size = new System.Drawing.Size(348, 374);
			this.txtUploadSummary.TabIndex = 13;
			this.txtUploadSummary.Visible = false;
			// 
			// progressBarUploadAmbiente
			// 
			this.progressBarUploadAmbiente.Location = new System.Drawing.Point(16, 267);
			this.progressBarUploadAmbiente.Margin = new System.Windows.Forms.Padding(4);
			this.progressBarUploadAmbiente.Name = "progressBarUploadAmbiente";
			this.progressBarUploadAmbiente.Size = new System.Drawing.Size(700, 16);
			this.progressBarUploadAmbiente.TabIndex = 15;
			this.progressBarUploadAmbiente.Visible = false;
			// 
			// lblUploadProgress
			// 
			this.lblUploadProgress.AutoSize = true;
			this.lblUploadProgress.Location = new System.Drawing.Point(724, 266);
			this.lblUploadProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblUploadProgress.Name = "lblUploadProgress";
			this.lblUploadProgress.Size = new System.Drawing.Size(26, 16);
			this.lblUploadProgress.TabIndex = 16;
			this.lblUploadProgress.Text = "0%";
			this.lblUploadProgress.Visible = false;
			// 
			// lblParentTaskId
			// 
			this.lblParentTaskId.AutoSize = true;
			this.lblParentTaskId.Location = new System.Drawing.Point(192, 748);
			this.lblParentTaskId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblParentTaskId.Name = "lblParentTaskId";
			this.lblParentTaskId.Size = new System.Drawing.Size(99, 16);
			this.lblParentTaskId.TabIndex = 17;
			this.lblParentTaskId.Text = "Parent Task ID:";
			// 
			// txtParentTaskId
			// 
			this.txtParentTaskId.Location = new System.Drawing.Point(299, 742);
			this.txtParentTaskId.Margin = new System.Windows.Forms.Padding(4);
			this.txtParentTaskId.Name = "txtParentTaskId";
			this.txtParentTaskId.Size = new System.Drawing.Size(364, 22);
			this.txtParentTaskId.TabIndex = 4;
			// 
			// buttonCreateTask
			// 
			this.buttonCreateTask.Location = new System.Drawing.Point(681, 739);
			this.buttonCreateTask.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCreateTask.Name = "buttonCreateTask";
			this.buttonCreateTask.Size = new System.Drawing.Size(298, 28);
			this.buttonCreateTask.TabIndex = 13;
			this.buttonCreateTask.Text = "Crea Task da Markdown";
			this.buttonCreateTask.UseVisualStyleBackColor = true;
			this.buttonCreateTask.Click += new System.EventHandler(this.buttonCreateTask_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtnSave,
            this.toolStripBtnDataModelEnv});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1634, 27);
			this.toolStrip1.TabIndex = 100;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripBtnSave
			// 
			this.toolStripBtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripBtnSave.Name = "toolStripBtnSave";
			this.toolStripBtnSave.Size = new System.Drawing.Size(149, 24);
			this.toolStripBtnSave.Text = "Salva Configurazioni";
			this.toolStripBtnSave.Click += new System.EventHandler(this.BtnSave_Click);
			// 
			// toolStripBtnDataModelEnv
			// 
			this.toolStripBtnDataModelEnv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripBtnDataModelEnv.Name = "toolStripBtnDataModelEnv";
			this.toolStripBtnDataModelEnv.Size = new System.Drawing.Size(122, 24);
			this.toolStripBtnDataModelEnv.Text = "Data Model Env:";
			this.toolStripBtnDataModelEnv.Click += new System.EventHandler(this.ToolStripBtnDataModelEnv_Click);
			// 
			// PluginControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblUploadProgress);
			this.Controls.Add(this.progressBarUploadAmbiente);
			this.Controls.Add(this.txtUploadSummary);
			this.Controls.Add(this.lblUploadSummary);
			this.Controls.Add(this.buttonCreateTask);
			this.Controls.Add(this.txtParentTaskId);
			this.Controls.Add(this.lblParentTaskId);
			this.Controls.Add(this.txtSolutionName);
			this.Controls.Add(this.lblSolutionName);
			this.Controls.Add(this.txtMarkdown);
			this.Controls.Add(this.lblMarkdown);
			this.Controls.Add(this.txtPrefix);
			this.Controls.Add(this.lblPrefix);
			this.Controls.Add(this.buttonUploadAmbiente);
			this.Controls.Add(this.buttonLoadMarkdown);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.buttonUploadDataModel);
			this.Controls.Add(this.lblConnectionString);
			this.Controls.Add(this.txtConnectionString);
			this.Controls.Add(this.lblTaskIds);
			this.Controls.Add(this.txtTaskIds);
			this.Controls.Add(this.btnExtract);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PluginControl";
			this.Size = new System.Drawing.Size(1634, 822);
			this.Load += new System.EventHandler(this.PluginControl_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.TextBox txtTaskIds;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.Label lblTaskIds;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripBtnSave;
        private System.Windows.Forms.ToolStripButton toolStripBtnDataModelEnv;
        private System.Windows.Forms.Button buttonUploadDataModel;
        private System.Windows.Forms.Button buttonUploadAmbiente;
        private System.Windows.Forms.Button buttonLoadMarkdown;
        private System.Windows.Forms.RichTextBox txtMarkdown;
        private System.Windows.Forms.Label lblMarkdown;
        private System.Windows.Forms.Label lblSolutionName;
        private System.Windows.Forms.TextBox txtSolutionName;
        private System.Windows.Forms.Label lblUploadSummary;
        private System.Windows.Forms.TextBox txtUploadSummary;
		private System.Windows.Forms.ProgressBar progressBarUploadAmbiente;
		private System.Windows.Forms.Label lblUploadProgress;
		private System.Windows.Forms.Label lblParentTaskId;
		private System.Windows.Forms.TextBox txtParentTaskId;
		private System.Windows.Forms.Button buttonCreateTask;
    }
}
