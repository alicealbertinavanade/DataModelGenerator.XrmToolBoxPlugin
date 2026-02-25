using System.Windows.Forms;
using System.Drawing;
namespace Avanade.XrmToolbox.DataModelDevOpsExtractor
{
    partial class PluginControl
    {
        private System.ComponentModel.IContainer components = null;
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
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(10, 15);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(120, 13);
            this.lblConnectionString.Text = "DevOps Connection String:";
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(140, 12);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(400, 20);
            // 
            // lblTaskIds
            // 
            this.lblTaskIds.AutoSize = true;
            this.lblTaskIds.Location = new System.Drawing.Point(10, 45);
            this.lblTaskIds.Name = "lblTaskIds";
            this.lblTaskIds.Size = new System.Drawing.Size(120, 13);
            this.lblTaskIds.Text = "Task IDs (comma separated):";
            // 
            // txtTaskIds
            // 
            this.txtTaskIds.Location = new System.Drawing.Point(140, 42);
            this.txtTaskIds.Name = "txtTaskIds";
            this.txtTaskIds.Size = new System.Drawing.Size(400, 20);
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(140, 75);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(120, 23);
            this.btnExtract.Text = "Estrai Data Model";
            this.btnExtract.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(270, 75);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 23);
            this.btnSave.Text = "Salva Connection";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // PluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblConnectionString);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.lblTaskIds);
            this.Controls.Add(this.txtTaskIds);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.btnSave);
            this.Name = "PluginControl";
            this.Size = new System.Drawing.Size(600, 120);
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
