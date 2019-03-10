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
    public partial class frm_data_sync_Passport : Form
    {
        public frm_data_sync_Passport()
        {
            InitializeComponent();
        }

        private void frm_data_sync_Passport_Load(object sender, EventArgs e)
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
                List<Ref_Passport> records = (from c in dc.Ref_Passports
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

                syncThread = new Thread(SyncIt);
                syncThread.Name = DateTime.Now.ToString("ddMMyyyyHHmmss");
                syncThread.Start();
            }
            catch (Exception ex)
            {
                LogIt("Client Error > dtp_date_ValueChanged > " + ex.ToString(), false);
            }
        }

        void SyncIt()
        {
            LogIt("SyncIt started. " + DateTime.Now.ToString(), true);
            int no = (int)dgv_list.SelectedCells[1].Value;
            if (dgv_list.Rows.Count >= no)
            //while (true)
            {
                DateTime start_date = DateTime.Now;
                foreach (DataGridViewRow item in dgv_list.Rows)
                {
                    try
                    {
                        string refPP_id = item.Cells[0].Value.ToString();
                        string PP_Enrollment = Properties.Settings.Default["PP_Enrollment"].ToString();
                        LINQ_PPEnrollDataContext dc = new LINQ_PPEnrollDataContext();
                        #region Ref_Passport
                        Ref_Passport the_passport = (from c in dc.Ref_Passports where c.Ref_PassportId == refPP_id select c).FirstOrDefault();
                        SR_PP.Ref_Passport save_passport = new SR_PP.Ref_Passport();
                        LogIt("Start save Ref_Passport >" + the_passport.Ref_PassportId, false);
                        #region bind_passport
                        save_passport.Ref_PassportId = the_passport.Ref_PassportId;
                        save_passport.PersonalNo = the_passport.PersonalNo;
                        save_passport.PassportType = the_passport.PassportType;
                        save_passport.PassportBookType = the_passport.PassportBookType;
                        save_passport.CountryCode = the_passport.CountryCode;
                        save_passport.PassportNo = the_passport.PassportNo;
                        save_passport.Name = the_passport.Name;
                        save_passport.Nationality = the_passport.Nationality;
                        save_passport.DateOfBirth = the_passport.DateOfBirth;
                        save_passport.Sex = the_passport.Sex;
                        save_passport.DateOfIssue = the_passport.DateOfIssue;
                        save_passport.Active = the_passport.Active;
                        save_passport.CreatedBy = the_passport.CreatedBy;
                        save_passport.CreatedOn = the_passport.CreatedOn;
                        save_passport.ModifiedBy = the_passport.ModifiedBy;
                        save_passport.ModifiedOn = the_passport.ModifiedOn;
                        save_passport.MRZ1 = the_passport.MRZ2;
                        #endregion
                        SR_PP.WSDataSyncSoapClient the_passport_info = new SR_PP.WSDataSyncSoapClient();
                        string result = the_passport_info.SavePassportData(save_passport, "BKK");
                        if (result != "Success")
                        {
                            LogIt("Web Error > the_client_info.SaveIndividual > " + the_passport.Ref_PassportId + " > " + result, false);
                            throw new Exception("Web Error > the_client_info.SaveIndividual > " + the_passport.Ref_PassportId + " > " + result);
                        }
                        else LogIt("End Save Ref_Individual > " + the_passport.Ref_PassportId + " > " + result, false);

                        #endregion
                        #region refreshUI
                        item.Cells["Column6"].Value = DateTime.Now;
                        item.DefaultCellStyle.BackColor = Color.SeaGreen;
                        item.DefaultCellStyle.ForeColor = Color.White;
                        item.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
                        item.DefaultCellStyle.SelectionForeColor = Color.White;

                        refreshGrid();

                        Upload_Count += 1;
                        processedCount += 1;

                        refreshProgressBar();
                        #endregion

                        dc.SubmitChanges();
                        LogIt("End Save All Tbl > " + the_passport.Ref_PassportId, false);
                        TimeSpan span = (DateTime.Now - start_date);
                        LogIt(String.Format("Total Synced : {0}/{4} Records. Duraing   {1} hours, {2} minutes, {3} seconds",
                                             (Upload_Count).ToString("N0"), span.Hours, span.Minutes, span.Seconds, (total_count).ToString("N0")), true);

                    }
                    catch (Exception ex)
                    {
                        LogIt("" + ex.ToString(), false);

                        item.DefaultCellStyle.BackColor = Color.Yellow;
                        item.DefaultCellStyle.ForeColor = Color.Black;
                        item.DefaultCellStyle.SelectionBackColor = Color.Yellow;
                        item.DefaultCellStyle.SelectionForeColor = Color.Black;

                        refreshGrid();
                        processedCount += 1;
                        refreshProgressBar();

                        TimeSpan span = (DateTime.Now - start_date);
                        LogIt(String.Format("Total Synced : {0}/{4} Records. Duraing   {1} hours, {2} minutes, {3} seconds",
                                             (Upload_Count).ToString("N0"), span.Hours, span.Minutes, span.Seconds, (total_count).ToString("N0")), true);
                        continue;
                    }
                }
                bindDataGrid();
            }
            else
            {
                if (syncThread.ThreadState == ThreadState.Running) syncThread.Abort();
                changeUploadUI();
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
        void changeUploadUI()
        {
            if (InvokeRequired) Invoke(new MethodInvoker(changeUploadUI));
            else
            {
                Cursor = this.Cursor == Cursors.WaitCursor ? Cursors.Default : Cursors.WaitCursor;
                btn_upload.Enabled = !btn_upload.Enabled;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (syncThread != null)
                {
                    if (syncThread.ThreadState == ThreadState.Running) syncThread.Abort();
                    changeUploadUI();
                }
            }
            catch (Exception) { }
        }

        private void frm_data_sync_Passport_FormClosing(object sender, FormClosingEventArgs e)
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
