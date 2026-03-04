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
            this.buttonUploadDataModel = new System.Windows.Forms.Button();
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
            this.txtConnectionString.Size = new System.Drawing.Size(531, 60);
            this.txtConnectionString.TabIndex = 1;
            // 
            // txtTaskIds
            // 
            this.txtTaskIds.Location = new System.Drawing.Point(212, 111);
            this.txtTaskIds.Margin = new System.Windows.Forms.Padding(4);
            this.txtTaskIds.Name = "txtTaskIds";
            this.txtTaskIds.Size = new System.Drawing.Size(532, 22);
            this.txtTaskIds.TabIndex = 3;
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(212, 175);
            this.btnExtract.Margin = new System.Windows.Forms.Padding(4);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(160, 28);
            this.btnExtract.TabIndex = 4;
            this.btnExtract.Text = "Estrai Data Model CSV";
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
            this.lblTaskIds.Location = new System.Drawing.Point(13, 113);
            this.lblTaskIds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTaskIds.Name = "lblTaskIds";
            this.lblTaskIds.Size = new System.Drawing.Size(185, 16);
            this.lblTaskIds.TabIndex = 2;
            this.lblTaskIds.Text = "Task IDs (comma separated):";
            // 
            // buttonUploadDataModel
            // 
            this.buttonUploadDataModel.Location = new System.Drawing.Point(391, 175);
            this.buttonUploadDataModel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonUploadDataModel.Name = "buttonUploadDataModel";
            this.buttonUploadDataModel.Size = new System.Drawing.Size(160, 28);
            this.buttonUploadDataModel.TabIndex = 6;
            this.buttonUploadDataModel.Text = "Carica Data Model";
            this.buttonUploadDataModel.UseVisualStyleBackColor = true;
            this.buttonUploadDataModel.Click += new System.EventHandler(this.buttonUploadDataModel_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtnSave,
            this.toolStripBtnDataModelEnv});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1088, 27);
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
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.buttonUploadDataModel);
            this.Controls.Add(this.lblConnectionString);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.lblTaskIds);
            this.Controls.Add(this.txtTaskIds);
            this.Controls.Add(this.btnExtract);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PluginControl";
            this.Size = new System.Drawing.Size(1088, 340);
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripBtnSave;
        private System.Windows.Forms.ToolStripButton toolStripBtnDataModelEnv;
        private System.Windows.Forms.Button buttonUploadDataModel;
    }
}
