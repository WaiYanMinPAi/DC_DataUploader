using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SSDataUploader.LINQ;
using System.Threading;

namespace SSDataUploader
{
    public partial class frm_data_sync_PassportFee : Form
    {
        public frm_data_sync_PassportFee()
        {
            InitializeComponent();
        }

        private void frm_data_sync_PassportFee_Load(object sender, EventArgs e)
        {
            try
            {
                LogIt("Program Started.", true);
                bindDataGrid();
            }
            catch (Exception ex)
            {
                LogIt("Client Error > frm_data_sync_Load > " + ex.ToString(), false);
            }
        }
        Int32 total_count = 0;
        Int32 Upload_Count = 0;
        Int32 processedCount = 0;
        Thread syncThread = null;

        void bindDataGrid()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(bindDataGrid));
            }
            else
            {
                LogIt("DataGrid Binding. ", false);
                total_count = 0;
                Upload_Count = 0;
                processedCount = 0;
                LINQ_PPEnrollDataContext dc = new LINQ_PPEnrollDataContext();

                dgv_list.AutoGenerateColumns = false;
                List<Ref_PassportFee> records = (from c in dc.Ref_PassportFees
                                                where c.Active == true
                                                orderby c.PassportNo descending
                                                select c).ToList();
                dgv_list.DataSource = records;
                int i = 0;
                LogIt(records.Count + " record(s) loaded.", true);
                foreach (DataGridViewRow r in dgv_list.Rows)
                {
                    //Individual
                    dgv_list.Rows[i].Cells["srno"].Value = i + 1;
                    decimal duration = 0;
                    if (r.Cells["col_duration"].Value != null)
                        decimal.TryParse(r.Cells["col_duration"].Value.ToString(), out duration);
                    ////OWIC
                    //decimal duration2 = 0;
                    //if (r.Cells["col_duration2"].Value != null)
                    //    decimal.TryParse(r.Cells["col_duration2"].Value.ToString(), out duration2);

                    if (duration < 0)
                    {
                        r.DefaultCellStyle.BackColor = Color.SeaGreen;
                        r.DefaultCellStyle.ForeColor = Color.White;
                        r.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
                        r.DefaultCellStyle.SelectionForeColor = Color.White;
                    }
                    else
                    {
                        r.DefaultCellStyle.BackColor = Color.Maroon;
                        r.DefaultCellStyle.ForeColor = Color.White;
                        r.DefaultCellStyle.SelectionBackColor = Color.Maroon;
                        r.DefaultCellStyle.SelectionForeColor = Color.White;
                        total_count = total_count + 1;
                    }
                    i++;
                }
                pb_progress_bar.Value = 0;
                pb_progress_bar.Maximum = total_count;
            }
        }
        void changeUploadUI()
        {
            if (InvokeRequired) Invoke(new MethodInvoker(changeUploadUI));
            else
            {
                Cursor = this.Cursor == Cursors.WaitCursor ? Cursors.Default : Cursors.WaitCursor;
                btn_upload.Enabled = !btn_upload.Enabled;

            }
        }

        void refreshProgressBar()
        {
            if (InvokeRequired) Invoke(new MethodInvoker(refreshProgressBar));
            else pb_progress_bar.Value = processedCount;//Upload_Count; 
        }

        void refreshGrid()
        {
            if (InvokeRequired) Invoke(new MethodInvoker(refreshGrid));
            else dgv_list.Refresh();
        }

        void LogIt(string message, bool showUI)
        {
            Log.Info("Admin", "", message);

            if (showUI)
            {
                if (lbl_running_count.InvokeRequired)
                {
                    lbl_running_count.Invoke(new MethodInvoker(delegate
                    {
                        lbl_running_count.Text = message;
                    }));
                }
                else lbl_running_count.Text = message;
            }
            else
            {

                if (result.InvokeRequired)
                {
                    result.Invoke(new MethodInvoker(delegate
                    {
                        result.Text = message;
                    }));
                }
                else result.Text = message;
            }
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            try
            {
                changeUploadUI();

                //syncThread = new Thread(SyncIt);
                syncThread.Name = DateTime.Now.ToString("ddMMyyyyHHmmss");
                syncThread.Start();
            }
            catch (Exception ex)
            {
                LogIt("Client Error > dtp_date_ValueChanged > " + ex.ToString(), false);
            }
        }
        
        private void frm_data_sync_PassportFee_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (syncThread != null)
                {
                    if (syncThread.ThreadState == ThreadState.Running) syncThread.Abort();
                }
            }
            catch (Exception) { }
        }
    }
}
