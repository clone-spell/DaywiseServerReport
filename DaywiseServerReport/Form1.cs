using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaywiseServerReport
{
    public partial class Form1 : Form
    {
        public DateTime fromDate = DateTime.Now;
        public DateTime toDate = DateTime.Now;
        public string project = "";
        public string strOutput = "";

        public Form1()
        {
            InitializeComponent();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbProject.SelectedIndex = 0;
            txtOutput.Text = "Kiosk Id\tLowest Receipt No.\tHighest Receipt No.\tNumber Of Receipt\tTotal Amount\n";
        }


        //button clicks
        private void btnSearch_Click(object sender, EventArgs e)
        {
            strOutput = "";
            project = cbProject.Text;
            fromDate = dateFrom.Value;
            toDate = dateFrom.Value;

            if (!backgroundWorker.IsBusy)
            {
                progressBar.Value = 0;
                backgroundWorker.RunWorkerAsync();
                btnSearch.Enabled = false;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtOutput.Text);
            }
            catch
            {

            }
            finally
            {
                MessageBox.Show("Successfuly copied!", "Success",MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.FileName = cbProject.Text + "_" + dateFrom.Value.ToString("dd.MM.yyyy") + "_" + dateTo.Value.ToString("dd.MM.yyyy") + ".txt";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    File.WriteAllText(filePath, txtOutput.Text);
                    MessageBox.Show("Text saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving text: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancal_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
            btnSearch.Enabled = true;
        }


        //background worker
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DbContext dbContext = new DbContext();
            List<string> kioskIds = dbContext.FindKioskIds(project,fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));

            double totalNoOfLoop = ((toDate - fromDate).Days+1) * kioskIds.Count;
            int runningDateCount = 1;
            int runningIdCount = 1;

            for (DateTime inputDate = fromDate; inputDate <= toDate; inputDate = inputDate.AddDays(1))
            {
                foreach (string kioskId in kioskIds)
                {
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundWorker.CancelAsync();
                    }

                    strOutput += dbContext.FindDetails(kioskId, project, inputDate.ToString("yyyy-MM-dd")) + Environment.NewLine;

                    System.Threading.Thread.Sleep(10);

                    int progressPercentage = (int)((double)(runningDateCount*runningIdCount/ totalNoOfLoop) * 100);
                    backgroundWorker.ReportProgress(progressPercentage);

                    runningIdCount++;
                }
                runningDateCount++;
            }

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtOutput.Text += strOutput;
            btnSearch.Enabled = true;
        }
    }
}
