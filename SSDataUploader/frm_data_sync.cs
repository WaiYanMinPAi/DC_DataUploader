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
    public partial class frm_data_sync : Form
    {
        public frm_data_sync()
        {
            InitializeComponent();
        }

        private void frm_data_sync_Load(object sender, EventArgs e)
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

        private void dtp_date_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                bindDataGrid();
            }
            catch (Exception ex)
            {
                LogIt("Client Error > dtp_date_ValueChanged > " + ex.ToString(), false);
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

        private void frm_data_sync_FormClosing(object sender, FormClosingEventArgs e)
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
                LogIt("DataGrid Binding. " + dtp_date.Value.Date.ToString(), false);
                total_count = 0;
                Upload_Count = 0;
                processedCount = 0;
                LINQ_VOMDataContext dc = new LINQ_VOMDataContext();

                dgv_list.AutoGenerateColumns = false;
                List<ref_individualUploadView> records = (from c in dc.ref_individualUploadViews
                                                          where c.CreatedOn.Date == dtp_date.Value.Date && c.Active == true
                                                          && c.LastUploadedOn == null
                                                          orderby c.Last_Upload_Duration descending
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
                    //OWIC
                    decimal duration2 = 0;
                    if (r.Cells["col_duration2"].Value != null)
                        decimal.TryParse(r.Cells["col_duration2"].Value.ToString(), out duration2);

                    if (duration < 0 && duration2 < 0)
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

        void SyncIt()
        {
            LogIt("SyncIt started. " + DateTime.Now.ToString(), true);
            while (dgv_list.Rows.Count > 0)
            {
                DateTime start_date = DateTime.Now;
                foreach (DataGridViewRow item in dgv_list.Rows)
                {
                    decimal individualDuration = item.Cells["col_duration"].Value == null ? 0 : decimal.Parse(item.Cells["col_duration"].Value.ToString());
                    decimal owicDuration = item.Cells["col_duration2"].Value == null ? 0 : decimal.Parse(item.Cells["col_duration2"].Value.ToString());

                    if (individualDuration >= 0 || owicDuration >= 0)
                    {
                        try
                        {
                            string ref_id = item.Cells[0].Value.ToString();
                            string StationCode = Properties.Settings.Default["StationCode"].ToString();

                            LINQ_CIDataContext dc = new LINQ_CIDataContext();
                            SR_CI.WebService_CISoapClient the_client_info = new SR_CI.WebService_CISoapClient();

                            #region Ref Individual
                            Ref_Individual the_individual = (from c in dc.Ref_Individuals where c.Ref_PersonalId == ref_id select c).FirstOrDefault();
                            SR_CI.Ref_Individual save_individual = new SR_CI.Ref_Individual();

                            LogIt("Start Save Ref_Individual > " + the_individual.PersonalNo, false);

                            #region bindIndividual                            
                            save_individual.Ref_PersonalId = the_individual.Ref_PersonalId;
                            save_individual.PersonalNo = the_individual.PersonalNo;
                            save_individual.Ref_FamilyId = the_individual.Ref_FamilyId;
                            save_individual.FamilyRelationship = the_individual.FamilyRelationship;
                            save_individual.FullName = the_individual.FullName;
                            save_individual.OtherName = the_individual.OtherName;
                            save_individual.FatherName = the_individual.FatherName;
                            save_individual.DateOfBirth = the_individual.DateOfBirth;
                            save_individual.Religion = the_individual.Religion;
                            save_individual.Nation = the_individual.Nation;
                            save_individual.Sex = the_individual.Sex;
                            save_individual.Job = the_individual.Job;
                            save_individual.Address = the_individual.Address;
                            save_individual.PersonalHistory = the_individual.PersonalHistory;
                            save_individual.OfficerRemark = the_individual.OfficerRemark;
                            save_individual.Active = the_individual.Active;
                            save_individual.CreatedBy = the_individual.CreatedBy;
                            save_individual.CreatedOn = the_individual.CreatedOn;
                            save_individual.ModifiedBy = the_individual.ModifiedBy;
                            save_individual.ModifiedOn = the_individual.ModifiedOn;
                            save_individual.LastAction = the_individual.LastAction;
                            save_individual.Note = the_individual.Note;
                            save_individual.PhotoID = the_individual.PhotoID;
                            save_individual.FingerPrintID1 = the_individual.FingerPrintID1;
                            save_individual.FingerPrintID2 = the_individual.FingerPrintID2;
                            save_individual.FingerPrintID3 = the_individual.FingerPrintID3;
                            save_individual.FingerPrintID4 = the_individual.FingerPrintID4;
                            save_individual.FingerPrintID5 = the_individual.FingerPrintID5;
                            save_individual.FingerPrintID6 = the_individual.FingerPrintID6;
                            save_individual.FingerPrintID7 = the_individual.FingerPrintID7;
                            save_individual.FingerPrintID8 = the_individual.FingerPrintID8;
                            save_individual.FingerPrintID9 = the_individual.FingerPrintID9;
                            save_individual.FingerPrintID10 = the_individual.FingerPrintID10;

                            save_individual.BirthPlace = the_individual.BirthPlace;
                            save_individual.DOB_dd = the_individual.DOB_dd;
                            save_individual.DOB_mm = the_individual.DOB_mm;
                            save_individual.DOB_yyy = the_individual.DOB_yyy;

                            save_individual.CarriedCardNo = the_individual.CarriedCardNo;
                            save_individual.CAddress_No = the_individual.CAddress_No;
                            save_individual.CAddress_Street = the_individual.CAddress_Street;
                            save_individual.CAddress_Village = the_individual.CAddress_Village;
                            save_individual.CAddress_VillageGroup = the_individual.CAddress_VillageGroup;
                            save_individual.CAddress_Township = the_individual.CAddress_Township;
                            save_individual.CAddress_Region = the_individual.CAddress_Region;
                            save_individual.CAddress_Part = the_individual.CAddress_Part;
                            save_individual.PAddress_No = the_individual.PAddress_No;
                            save_individual.PAddress_Street = the_individual.PAddress_Street;
                            save_individual.PAddress_Village = the_individual.PAddress_Village;
                            save_individual.PAddress_VillageGroup = the_individual.PAddress_VillageGroup;
                            save_individual.PAddress_Township = the_individual.PAddress_Township;
                            save_individual.PAddress_Region = the_individual.PAddress_Region;
                            save_individual.PAddress_Part = the_individual.PAddress_Part;
                            save_individual.MarriedStatus = the_individual.MarriedStatus;
                            save_individual.SpouceName = the_individual.SpouceName;

                            save_individual.SpouceAddress = the_individual.SpouceAddress;
                            save_individual.ChildrenInfo = the_individual.ChildrenInfo;
                            save_individual.BrotherInfo = the_individual.BrotherInfo;
                            save_individual.Age = the_individual.Age;
                            save_individual.motherName = the_individual.motherName;
                            save_individual.DocImage1 = the_individual.DocImage1;
                            save_individual.DocImage2 = the_individual.DocImage2;
                            save_individual.DocImage3 = the_individual.DocImage3;
                            save_individual.DocImage4 = the_individual.DocImage4;
                            save_individual.DocImage5 = the_individual.DocImage5;
                            save_individual.DocImage6 = the_individual.DocImage6;
                            save_individual.DocImage7 = the_individual.DocImage7;
                            save_individual.DocImage8 = the_individual.DocImage8;
                            save_individual.DocImage9 = the_individual.DocImage9;
                            save_individual.DocImage10 = the_individual.DocImage10;
                            save_individual.DocImage11 = the_individual.DocImage11;
                            save_individual.DocImage12 = the_individual.DocImage12;
                            save_individual.DocImage13 = the_individual.DocImage13;
                            save_individual.DocImage14 = the_individual.DocImage14;
                            save_individual.DocImage15 = the_individual.DocImage15;
                            save_individual.DocImage16 = the_individual.DocImage16;

                            save_individual.EyeLeftID = the_individual.EyeLeftID;
                            save_individual.EyeRightID = the_individual.EyeRightID;
                            save_individual.COINo = the_individual.COINo;
                            save_individual.LanguageAtHome = the_individual.LanguageAtHome;
                            save_individual.BarcodeImageID = the_individual.BarcodeImageID;
                            save_individual.CountryCode = the_individual.CountryCode;
                            save_individual.DateOfIssue = the_individual.DateOfIssue;
                            save_individual.PlaceOfIssue = the_individual.PlaceOfIssue;
                            save_individual.DateOfExpire = the_individual.DateOfExpire;
                            save_individual.SignatureID = the_individual.SignatureID;
                            save_individual.Transaction_ID = the_individual.Transaction_ID;
                            save_individual.Mark = the_individual.Mark;
                            save_individual.pinkcard_expiry = the_individual.pinkcard_expiry;

                            save_individual.PassportPhoto = the_individual.PassportPhoto;
                            save_individual.ptype = the_individual.ptype;
                            save_individual.issuetype = the_individual.issuetype;
                            save_individual.revoketype = the_individual.revoketype;
                            save_individual.PC_Id = the_individual.PC_Id;
                            save_individual.TP_Id = the_individual.TP_Id;
                            save_individual.UD_Id = the_individual.UD_Id;
                            save_individual.TR_Id = the_individual.TR_Id;
                            save_individual.LastUploadedOn = DateTime.Now;
                            #endregion

                            String result = the_client_info.SaveIndividual(save_individual, StationCode);
                            if (result != "success")
                            {
                                LogIt("Web Error > the_client_info.SaveIndividual > " + the_individual.PersonalNo + " > " + result, false);
                                throw new Exception("Web Error > the_client_info.SaveIndividual > " + the_individual.PersonalNo + " > " + result);
                            }
                            else LogIt("End Save Ref_Individual > " + the_individual.PersonalNo + " > " + result, false);
                            #endregion

                            #region OWIC
                            Ref_OWIC the_owic = (from c in dc.Ref_OWICs where c.PersonalNo == the_individual.PersonalNo && c.active == true select c).FirstOrDefault();
                            if (the_owic != null)
                            {
                                LogIt("Start Save Ref_OWIC > " + the_individual.PersonalNo, false);

                                #region bindOWIC
                                SR_CI.Ref_OWIC save_owic = new SR_CI.Ref_OWIC();
                                save_owic.regno = the_owic.regno;
                                save_owic.PersonalNo = the_owic.PersonalNo;
                                save_owic.COINo = the_owic.COINo;
                                save_owic.MMAgency = the_owic.MMAgency;
                                save_owic.FoAgency = the_owic.FoAgency;
                                save_owic.FoAddress = the_owic.FoAddress;
                                save_owic.DateOfIssue = the_owic.DateOfIssue;
                                save_owic.DateOfExpire = the_owic.DateOfExpire;
                                save_owic.active = the_owic.active;
                                save_owic.contact = the_owic.contact;
                                save_owic.corp = the_owic.corp;
                                save_owic.mrz1 = the_owic.mrz1;
                                save_owic.mrz2 = the_owic.mrz2;
                                save_owic.mrz3 = the_owic.mrz3;
                                save_owic.photostr = the_owic.photostr;
                                save_owic.uvstr = the_owic.uvstr;
                                save_owic.signstr = the_owic.signstr;
                                save_owic.qrstr = the_owic.qrstr;
                                save_owic.securetxt = the_owic.securetxt;
                                save_owic.goCountry = the_owic.goCountry;
                                save_owic.companyName = the_owic.companyName;
                                save_owic.pstatus = the_owic.pstatus;
                                save_owic.pcount = the_owic.pcount;
                                save_owic.CreatedBy = the_owic.CreatedBy;
                                save_owic.CreatedOn = the_owic.CreatedOn;
                                save_owic.ModifiedBy = the_owic.ModifiedBy;
                                save_owic.ModifiedOn = the_owic.ModifiedOn;
                                #endregion

                                String Owic_result = the_client_info.SaveOWIC(save_owic, StationCode);
                                if (Owic_result != "success")
                                {
                                    LogIt("Web Error > the_client_info.SaveOWIC > " + the_individual.PersonalNo + " > " + Owic_result, false);
                                    throw new Exception("Web Error > the_client_info.SaveOWIC > " + the_individual.PersonalNo + " > " + Owic_result);
                                }
                                else LogIt("End Save Ref_OWIC > " + the_individual.PersonalNo + " > " + Owic_result, false);
                            }
                            #endregion

                            #region Getting Image
                            LogIt("Start Save sysimage > " + the_individual.PersonalNo, false);
                            List<SysImage> the_images = (from c in dc.SysImages
                                                         where
                                                             c.ImageID == the_individual.PhotoID ||
                                                             c.ImageID == the_individual.FingerPrintID1 ||
                                                             c.ImageID == the_individual.FingerPrintID2 ||
                                                             c.ImageID == the_individual.FingerPrintID3 ||
                                                             c.ImageID == the_individual.FingerPrintID4 ||
                                                             c.ImageID == the_individual.FingerPrintID5 ||
                                                             c.ImageID == the_individual.FingerPrintID6 ||
                                                             c.ImageID == the_individual.FingerPrintID7 ||
                                                             c.ImageID == the_individual.FingerPrintID8 ||
                                                             c.ImageID == the_individual.FingerPrintID9 ||
                                                             c.ImageID == the_individual.FingerPrintID10 ||
                                                             c.ImageID == the_individual.FingerPrintID1 ||
                                                             c.ImageID == the_individual.DocImage1 ||
                                                             c.ImageID == the_individual.DocImage2 ||
                                                             c.ImageID == the_individual.DocImage3 ||
                                                             c.ImageID == the_individual.DocImage4 ||
                                                             c.ImageID == the_individual.DocImage5 ||
                                                             c.ImageID == the_individual.EyeLeftID ||
                                                             c.ImageID == the_individual.EyeRightID ||
                                                             c.ImageID == the_individual.DocImage6 ||
                                                             c.ImageID == the_individual.DocImage7 ||
                                                             c.ImageID == the_individual.DocImage8 ||
                                                             c.ImageID == the_individual.DocImage9 ||
                                                             c.ImageID == the_individual.DocImage10 ||
                                                             c.ImageID == the_individual.DocImage11 ||
                                                             c.ImageID == the_individual.DocImage12 ||
                                                             c.ImageID == the_individual.DocImage13 ||
                                                             c.ImageID == the_individual.DocImage14 ||
                                                             c.ImageID == the_individual.DocImage15 ||
                                                             c.ImageID == the_individual.DocImage16 ||
                                                             c.ImageID == the_individual.PassportPhoto
                                                         select c).ToList();

                            foreach (SysImage image in the_images)
                            {
                                SR_CI.WebService_CISoapClient the_client = new SR_CI.WebService_CISoapClient();
                                //checking is it new image or not.
                                bool is_new_image = the_client.IsNewImage(image.ImageID, StationCode);
                                if (!is_new_image) continue;

                                string imgResult = the_client.SaveImage((byte[])image.ImageData.ToArray(), image.ImageID, image.CreatedBy, image.CreatedOn, image.ImageRecordType, StationCode);
                                if (result != "success")
                                {
                                    LogIt("Web Error > the_client.SaveImage > " + the_individual.PersonalNo + " > " + imgResult, false);
                                    throw new Exception("Web Error > the_client.SaveImage > " + the_individual.PersonalNo + " > " + imgResult);
                                }
                                else LogIt("End Save sysimage > " + image.ImageRecordType + " > " + the_individual.PersonalNo + " > " + imgResult, false);
                            }
                            #endregion

                            the_individual.LastUploadedOn = DateTime.Now;

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
                            LogIt("End Save All Tbl > " + the_individual.PersonalNo, false);
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
                }

                bindDataGrid();
            }
            if (dgv_list.Rows.Count <= 0) changeUploadUI();
        }

        void changeUploadUI()
        {
            if (InvokeRequired) Invoke(new MethodInvoker(changeUploadUI));
            else
            {
                Cursor = this.Cursor == Cursors.WaitCursor ? Cursors.Default : Cursors.WaitCursor;
                dtp_date.Enabled = !dtp_date.Enabled;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Small_Image img = new Small_Image();
            img.ShowDialog();
        }

        //Without UI Update much.
        //private void startProcess()
        //{
        //    int daysToProcess = 30;
        //    for (int i = 0; i < daysToProcess; i++)
        //    {
        //        LINQ_CIDataContext dc = new LINQ_CIDataContext();
        //        DateTime iDay = DateTime.Today.AddDays(i).Date;
        //        int records = (from c in dc.ref_individualUploadViews
        //                       where c.CreatedOn.Date == iDay && c.Active == true
        //                       && (c.Last_Upload_Duration >= 0 || c.Last_Upload_Duration_OWIC >= 0)
        //                       orderby c.Last_Upload_Duration descending
        //                       select c).Count();
        //        if (records <= 0) continue;
        //        Thread threadFortheDay = new Thread(processTheDay);
        //        threadFortheDay.Name = iDay.ToString();
        //        threadFortheDay.Start(iDay);
        //    }
        //}

        //private void processTheDay(Object date)
        //{
        //    string userThread = Thread.CurrentThread.Name;
        //    DateTime procDate = (DateTime)date;
        //    LINQ_CIDataContext dcGetList = new LINQ_CIDataContext();
        //    List<ref_individualUploadView> records = (from c in dcGetList.ref_individualUploadViews
        //                                              where c.CreatedOn.Date == procDate.Date && c.Active == true
        //                                              && (c.Last_Upload_Duration >= 0 || c.Last_Upload_Duration_OWIC >= 0)
        //                                              orderby c.Last_Upload_Duration descending
        //                                              select c).ToList();
        //    int uploadCount = 0; DateTime startDT = DateTime.Now;
        //    foreach (var item in records)
        //    {
        //        string ref_id = item.Ref_PersonalId;
        //        string StationCode = Properties.Settings.Default["StationCode"].ToString();
        //        LINQ_CIDataContext dc = new LINQ_CIDataContext();
        //        SR_CI.WebService_CISoapClient the_client_info = new SR_CI.WebService_CISoapClient();

        //        #region Ref Individual
        //        Ref_Individual the_individual = (from c in dc.Ref_Individuals where c.Ref_PersonalId == ref_id select c).FirstOrDefault();
        //        SR_CI.Ref_Individual save_individual = new SR_CI.Ref_Individual();

        //        Log.Info(userThread, "Start Save Ref_Individual > ", the_individual.PersonalNo);

        //        save_individual.Ref_PersonalId = the_individual.Ref_PersonalId;
        //        save_individual.PersonalNo = the_individual.PersonalNo;
        //        save_individual.Ref_FamilyId = the_individual.Ref_FamilyId;
        //        save_individual.FamilyRelationship = the_individual.FamilyRelationship;
        //        save_individual.FullName = the_individual.FullName;
        //        save_individual.OtherName = the_individual.OtherName;
        //        save_individual.FatherName = the_individual.FatherName;
        //        save_individual.DateOfBirth = the_individual.DateOfBirth;
        //        save_individual.Religion = the_individual.Religion;
        //        save_individual.Nation = the_individual.Nation;
        //        save_individual.Sex = the_individual.Sex;
        //        save_individual.Job = the_individual.Job;
        //        save_individual.Address = the_individual.Address;
        //        save_individual.PersonalHistory = the_individual.PersonalHistory;
        //        save_individual.OfficerRemark = the_individual.OfficerRemark;
        //        save_individual.Active = the_individual.Active;
        //        save_individual.CreatedBy = the_individual.CreatedBy;
        //        save_individual.CreatedOn = the_individual.CreatedOn;
        //        save_individual.ModifiedBy = the_individual.ModifiedBy;
        //        save_individual.ModifiedOn = the_individual.ModifiedOn;
        //        save_individual.LastAction = the_individual.LastAction;
        //        save_individual.Note = the_individual.Note;
        //        save_individual.PhotoID = the_individual.PhotoID;
        //        save_individual.FingerPrintID1 = the_individual.FingerPrintID1;
        //        save_individual.FingerPrintID2 = the_individual.FingerPrintID2;
        //        save_individual.FingerPrintID3 = the_individual.FingerPrintID3;
        //        save_individual.FingerPrintID4 = the_individual.FingerPrintID4;
        //        save_individual.FingerPrintID5 = the_individual.FingerPrintID5;
        //        save_individual.FingerPrintID6 = the_individual.FingerPrintID6;
        //        save_individual.FingerPrintID7 = the_individual.FingerPrintID7;
        //        save_individual.FingerPrintID8 = the_individual.FingerPrintID8;
        //        save_individual.FingerPrintID9 = the_individual.FingerPrintID9;
        //        save_individual.FingerPrintID10 = the_individual.FingerPrintID10;

        //        save_individual.BirthPlace = the_individual.BirthPlace;
        //        save_individual.DOB_dd = the_individual.DOB_dd;
        //        save_individual.DOB_mm = the_individual.DOB_mm;
        //        save_individual.DOB_yyy = the_individual.DOB_yyy;

        //        save_individual.CarriedCardNo = the_individual.CarriedCardNo;
        //        save_individual.CAddress_No = the_individual.CAddress_No;
        //        save_individual.CAddress_Street = the_individual.CAddress_Street;
        //        save_individual.CAddress_Village = the_individual.CAddress_Village;
        //        save_individual.CAddress_VillageGroup = the_individual.CAddress_VillageGroup;
        //        save_individual.CAddress_Township = the_individual.CAddress_Township;
        //        save_individual.CAddress_Region = the_individual.CAddress_Region;
        //        save_individual.CAddress_Part = the_individual.CAddress_Part;
        //        save_individual.PAddress_No = the_individual.PAddress_No;
        //        save_individual.PAddress_Street = the_individual.PAddress_Street;
        //        save_individual.PAddress_Village = the_individual.PAddress_Village;
        //        save_individual.PAddress_VillageGroup = the_individual.PAddress_VillageGroup;
        //        save_individual.PAddress_Township = the_individual.PAddress_Township;
        //        save_individual.PAddress_Region = the_individual.PAddress_Region;
        //        save_individual.PAddress_Part = the_individual.PAddress_Part;
        //        save_individual.MarriedStatus = the_individual.MarriedStatus;
        //        save_individual.SpouceName = the_individual.SpouceName;

        //        save_individual.SpouceAddress = the_individual.SpouceAddress;
        //        save_individual.ChildrenInfo = the_individual.ChildrenInfo;
        //        save_individual.BrotherInfo = the_individual.BrotherInfo;
        //        save_individual.Age = the_individual.Age;
        //        save_individual.motherName = the_individual.motherName;
        //        save_individual.DocImage1 = the_individual.DocImage1;
        //        save_individual.DocImage2 = the_individual.DocImage2;
        //        save_individual.DocImage3 = the_individual.DocImage3;
        //        save_individual.DocImage4 = the_individual.DocImage4;
        //        save_individual.DocImage5 = the_individual.DocImage5;
        //        save_individual.DocImage6 = the_individual.DocImage6;
        //        save_individual.DocImage7 = the_individual.DocImage7;
        //        save_individual.DocImage8 = the_individual.DocImage8;
        //        save_individual.DocImage9 = the_individual.DocImage9;
        //        save_individual.DocImage10 = the_individual.DocImage10;
        //        save_individual.DocImage11 = the_individual.DocImage11;
        //        save_individual.DocImage12 = the_individual.DocImage12;
        //        save_individual.DocImage13 = the_individual.DocImage13;
        //        save_individual.DocImage14 = the_individual.DocImage14;
        //        save_individual.DocImage15 = the_individual.DocImage15;
        //        save_individual.DocImage16 = the_individual.DocImage16;

        //        save_individual.EyeLeftID = the_individual.EyeLeftID;
        //        save_individual.EyeRightID = the_individual.EyeRightID;
        //        save_individual.COINo = the_individual.COINo;
        //        save_individual.LanguageAtHome = the_individual.LanguageAtHome;
        //        save_individual.BarcodeImageID = the_individual.BarcodeImageID;
        //        save_individual.CountryCode = the_individual.CountryCode;
        //        save_individual.DateOfIssue = the_individual.DateOfIssue;
        //        save_individual.PlaceOfIssue = the_individual.PlaceOfIssue;
        //        save_individual.DateOfExpire = the_individual.DateOfExpire;
        //        save_individual.SignatureID = the_individual.SignatureID;
        //        save_individual.Transaction_ID = the_individual.Transaction_ID;
        //        save_individual.Mark = the_individual.Mark;
        //        save_individual.pinkcard_expiry = the_individual.pinkcard_expiry;

        //        save_individual.PassportPhoto = the_individual.PassportPhoto;
        //        save_individual.ptype = the_individual.ptype;
        //        save_individual.issuetype = the_individual.issuetype;
        //        save_individual.revoketype = the_individual.revoketype;
        //        save_individual.PC_Id = the_individual.PC_Id;
        //        save_individual.TP_Id = the_individual.TP_Id;
        //        save_individual.UD_Id = the_individual.UD_Id;
        //        save_individual.TR_Id = the_individual.TR_Id;
        //        save_individual.LastUploadedOn = DateTime.Now;

        //        String result = the_client_info.SaveIndividual(save_individual, StationCode);
        //        if (result != "success") Log.Info(userThread, "Web Error > the_client_info.SaveIndividual > ", the_individual.PersonalNo + " > " + result);
        //        else Log.Info(userThread, "End Save Ref_Individual > ", the_individual.PersonalNo + " > " + result);
        //        #endregion

        //        #region OWIC
        //        Ref_OWIC the_owic = (from c in dc.Ref_OWICs where c.PersonalNo == the_individual.PersonalNo && c.active == true select c).FirstOrDefault();
        //        if (the_owic != null)
        //        {
        //            Log.Info(userThread, "Start Save Ref_OWIC > ", the_individual.PersonalNo);

        //            SR_CI.Ref_OWIC save_owic = new SR_CI.Ref_OWIC();
        //            save_owic.regno = the_owic.regno;
        //            save_owic.PersonalNo = the_owic.PersonalNo;
        //            save_owic.COINo = the_owic.COINo;
        //            save_owic.MMAgency = the_owic.MMAgency;
        //            save_owic.FoAgency = the_owic.FoAgency;
        //            save_owic.FoAddress = the_owic.FoAddress;
        //            save_owic.DateOfIssue = the_owic.DateOfIssue;
        //            save_owic.DateOfExpire = the_owic.DateOfExpire;
        //            save_owic.active = the_owic.active;
        //            save_owic.contact = the_owic.contact;
        //            save_owic.corp = the_owic.corp;
        //            save_owic.mrz1 = the_owic.mrz1;
        //            save_owic.mrz2 = the_owic.mrz2;
        //            save_owic.mrz3 = the_owic.mrz3;
        //            save_owic.photostr = the_owic.photostr;
        //            save_owic.uvstr = the_owic.uvstr;
        //            save_owic.signstr = the_owic.signstr;
        //            save_owic.qrstr = the_owic.qrstr;
        //            save_owic.securetxt = the_owic.securetxt;
        //            save_owic.goCountry = the_owic.goCountry;
        //            save_owic.companyName = the_owic.companyName;
        //            save_owic.pstatus = the_owic.pstatus;
        //            save_owic.pcount = the_owic.pcount;
        //            save_owic.CreatedBy = the_owic.CreatedBy;
        //            save_owic.CreatedOn = the_owic.CreatedOn;
        //            save_owic.ModifiedBy = the_owic.ModifiedBy;
        //            save_owic.ModifiedOn = the_owic.ModifiedOn;

        //            String Owic_result = the_client_info.SaveOWIC(save_owic, StationCode);
        //            if (Owic_result != "success") Log.Info(userThread, "Web Error > the_client_info.SaveOWIC > ", the_individual.PersonalNo + " > " + Owic_result);
        //            else Log.Info(userThread, "End Save Ref_OWIC > ", the_individual.PersonalNo + " > " + Owic_result);
        //        }
        //        #endregion

        //        #region Images
        //        Log.Info(userThread, "Start Save sysimage > ", the_individual.PersonalNo);
        //        List<SysImage> the_images = (from c in dc.SysImages
        //                                     where
        //                                         c.ImageID == the_individual.PhotoID ||
        //                                         c.ImageID == the_individual.FingerPrintID1 ||
        //                                         c.ImageID == the_individual.FingerPrintID2 ||
        //                                         c.ImageID == the_individual.FingerPrintID3 ||
        //                                         c.ImageID == the_individual.FingerPrintID4 ||
        //                                         c.ImageID == the_individual.FingerPrintID5 ||
        //                                         c.ImageID == the_individual.FingerPrintID6 ||
        //                                         c.ImageID == the_individual.FingerPrintID7 ||
        //                                         c.ImageID == the_individual.FingerPrintID8 ||
        //                                         c.ImageID == the_individual.FingerPrintID9 ||
        //                                         c.ImageID == the_individual.FingerPrintID10 ||
        //                                         c.ImageID == the_individual.FingerPrintID1 ||
        //                                         c.ImageID == the_individual.DocImage1 ||
        //                                         c.ImageID == the_individual.DocImage2 ||
        //                                         c.ImageID == the_individual.DocImage3 ||
        //                                         c.ImageID == the_individual.DocImage4 ||
        //                                         c.ImageID == the_individual.DocImage5 ||
        //                                         c.ImageID == the_individual.EyeLeftID ||
        //                                         c.ImageID == the_individual.EyeRightID ||
        //                                         c.ImageID == the_individual.DocImage6 ||
        //                                         c.ImageID == the_individual.DocImage7 ||
        //                                         c.ImageID == the_individual.DocImage8 ||
        //                                         c.ImageID == the_individual.DocImage9 ||
        //                                         c.ImageID == the_individual.DocImage10 ||
        //                                         c.ImageID == the_individual.DocImage11 ||
        //                                         c.ImageID == the_individual.DocImage12 ||
        //                                         c.ImageID == the_individual.DocImage13 ||
        //                                         c.ImageID == the_individual.DocImage14 ||
        //                                         c.ImageID == the_individual.DocImage15 ||
        //                                         c.ImageID == the_individual.DocImage16 ||
        //                                         c.ImageID == the_individual.PassportPhoto
        //                                     select c).ToList();

        //        foreach (SysImage image in the_images)
        //        {
        //            SR_CI.WebService_CISoapClient the_client = new SR_CI.WebService_CISoapClient();
        //            //checking is it new image or not.
        //            bool is_new_image = the_client.IsNewImage(image.ImageID, StationCode);
        //            if (is_new_image == true)
        //            {
        //                string imgResult = the_client.SaveImage((byte[])image.ImageData.ToArray(), image.ImageID, image.CreatedBy, image.CreatedOn, image.ImageRecordType, StationCode);
        //                if (result != "success") Log.Info(userThread, "Web Error > the_client.SaveImage > ", the_individual.PersonalNo + " > " + imgResult);
        //                else Log.Info(userThread, "End Save sysimage > " + image.ImageRecordType + " > ", the_individual.PersonalNo + " > " + imgResult);
        //            }
        //        }
        //        #endregion

        //        the_individual.LastUploadedOn = DateTime.Now;
        //        uploadCount += 1;

        //        if (pb_progress_bar.InvokeRequired)
        //        {
        //            pb_progress_bar.Invoke(new MethodInvoker(delegate
        //            {
        //                pb_progress_bar.Value = uploadCount;
        //            }));
        //        }
        //        else
        //        {
        //            pb_progress_bar.Value = Upload_Count;
        //        }

        //        TimeSpan span = (DateTime.Now - startDT);

        //        changeLabelStatus(String.Format("Total Synced : {0}/{4} Records. Duraing   {1} hours, {2} minutes, {3} seconds",
        //                 (Upload_Count).ToString("N0"), span.Hours, span.Minutes, span.Seconds, (total_count).ToString("N0")));

        //        dc.SubmitChanges();
        //        Log.Info(userThread, "End Save All Tbl > ", the_individual.PersonalNo);
        //    }
        //}
    }
}
