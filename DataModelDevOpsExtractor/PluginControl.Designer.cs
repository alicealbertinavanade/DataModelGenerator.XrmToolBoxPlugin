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
            this.btnSave = new System.Windows.Forms.Button();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.lblTaskIds = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(213, 15);
            this.txtConnectionString.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(532, 22);
            this.txtConnectionString.TabIndex = 1;
            // 
            // txtTaskIds
            // 
            this.txtTaskIds.Location = new System.Drawing.Point(212, 52);
            this.txtTaskIds.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTaskIds.Name = "txtTaskIds";
            this.txtTaskIds.Size = new System.Drawing.Size(532, 22);
            this.txtTaskIds.TabIndex = 3;
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(187, 92);
            this.btnExtract.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(160, 28);
            this.btnExtract.TabIndex = 4;
            this.btnExtract.Text = "Estrai Data Model";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click_1);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(360, 92);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(160, 28);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Salva Connection";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(13, 18);
            this.lblConnectionString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(167, 16);
            this.lblConnectionString.TabIndex = 0;
            this.lblConnectionString.Text = "DevOps Connection String:";
            // 
            // lblTaskIds
            // 
            this.lblTaskIds.AutoSize = true;
            this.lblTaskIds.Location = new System.Drawing.Point(13, 55);
            this.lblTaskIds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTaskIds.Name = "lblTaskIds";
            this.lblTaskIds.Size = new System.Drawing.Size(185, 16);
            this.lblTaskIds.TabIndex = 2;
            this.lblTaskIds.Text = "Task IDs (comma separated):";
            // 
            // PluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblConnectionString);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.lblTaskIds);
            this.Controls.Add(this.txtTaskIds);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.btnSave);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "PluginControl";
            this.Size = new System.Drawing.Size(800, 148);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.TextBox txtTaskIds;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.Label lblTaskIds;
        private System.Windows.Forms.Button btnSave;
    }
}
