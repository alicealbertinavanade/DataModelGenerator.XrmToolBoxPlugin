using System;
using System.Windows.Forms;

namespace DataModelDevOpsExtractor.Service
{
    public sealed class UploadProgressService
    {
        private readonly ProgressBar progressBar;
        private readonly Label progressLabel;

        public UploadProgressService(ProgressBar progressBar, Label progressLabel)
        {
            this.progressBar = progressBar;
            this.progressLabel = progressLabel;
        }

        public void Reset()
        {
            if (progressBar == null || progressLabel == null)
            {
                return;
            }

            progressBar.Visible = false;
            progressLabel.Visible = false;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressLabel.Text = "0%";
        }

        public void HideForOtherAction()
        {
            Reset();
        }

        public void SetProgress(int percentage, string step)
        {
            if (progressBar == null || progressLabel == null)
            {
                return;
            }

            var safePercentage = Math.Max(0, Math.Min(100, percentage));
            progressBar.Visible = true;
            progressLabel.Visible = true;
            progressBar.Value = safePercentage;
            progressLabel.Text = string.IsNullOrWhiteSpace(step)
                ? $"{safePercentage}%"
                : $"{safePercentage}% - {step}";

            progressBar.Refresh();
            progressLabel.Refresh();
        }

        public void UpdateFromRow(int currentRow, int totalRows)
        {
            if (totalRows <= 0)
            {
                SetProgress(95, "Elaborazione");
                return;
            }

            var percentage = 10 + (int)Math.Round((currentRow / (double)totalRows) * 85d);
            SetProgress(percentage, $"Elaborazione {currentRow}/{totalRows}");
        }
    }
}
