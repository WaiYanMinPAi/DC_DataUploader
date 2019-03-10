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
using System.IO;
using SSDataUploader.LINQ.VOM;

namespace SSDataUploader
{

    public partial class frm_data_sync_noDate : Form
    {
        private string imgid = "";
        public frm_data_sync_noDate()
        {
            InitializeComponent();
        }

        private void frm_data_sync_noDate_Load(object sender, EventArgs e)
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

        private void frm_data_sync_noDate_FormClosing(object sender, FormClosingEventArgs e)
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
                LogIt("DataGrid Binding. ", false);
                total_count = 0;
                Upload_Count = 0;
                processedCount = 0;
                LINQ.VOM.LINQ_VOMEnrollDataContext dc = new LINQ.VOM.LINQ_VOMEnrollDataContext();
                LINQ.VOM.LINQ_VOMImageDataContext dci = new LINQ.VOM.LINQ_VOMImageDataContext();
                dgv_list.AutoGenerateColumns = false;
                List<Ref_IndividualSyncViewTotal> records = (from c in dc.Ref_IndividualSyncViewTotals

                                                             orderby c.Name descending
                                                             select c).ToList();

                SysImage image = (from c in dci.SysImages select c).FirstOrDefault();


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

                    //if ((records[i].ModifiedOn - records[i].UploadedOn).Value.TotalDays > 0)
                    if (records[i].UploadedOn == null)
                    {
                        r.DefaultCellStyle.BackColor = Color.YellowGreen;
                        r.DefaultCellStyle.ForeColor = Color.Black;
                        r.DefaultCellStyle.SelectionBackColor = Color.Yellow;
                        r.DefaultCellStyle.SelectionForeColor = Color.Black;
                        total_count = total_count + 1;
                    }

                    else
                    {
                        r.DefaultCellStyle.BackColor = Color.DarkOrange;
                        r.DefaultCellStyle.ForeColor = Color.White;
                        r.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
                        r.DefaultCellStyle.SelectionForeColor = Color.White;
                        total_count = total_count + 1;
                    }
                    i++;
                }
                pb_progress_bar.Value = 0;
                pb_progress_bar.Maximum = total_count;
            }
        }
        private void GetImage()
        {
            LINQ_VOMImageDataContext dc = new LINQ_VOMImageDataContext();
            SysImage records = (from c in dc.SysImages where c.ImageID == imgid select c).FirstOrDefault();
            if (records != null)
            {
                var arrayBinary = records.ImageData.ToArray();
                using (MemoryStream ms = new MemoryStream(arrayBinary))
                {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(Properties.Settings.Default.ImgPath + "\\" + records.PassengerId + records.ImageRecordType + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }

        }
        private void SyncIt()
        {
            LogIt("SyncIt started. " + DateTime.Now.ToString(), true);
            try
            {
                int no = (int)dgv_list.SelectedCells[1].Value;

                if (dgv_list.Rows.Count >= no)
                //while (true)
                {
                    DateTime start_date = DateTime.Now;
                    foreach (DataGridViewRow item in dgv_list.Rows)
                    {
                        try
                        {
                            string ref_id = item.Cells[0].Value.ToString();
                            string person_id = item.Cells[2].Value.ToString();
                            string passport_no = item.Cells[6].Value.ToString();
                            //string ref_id = "c7180c8f-cc3f-4363-bbc5-00b7d73d359b";
                            string PP_Enrollment = Properties.Settings.Default["PP_Enrollment"].ToString();
                            LINQ_VOMEnrollDataContext dc = new LINQ_VOMEnrollDataContext();
                            LINQ_VOMImageDataContext dcimg = new LINQ_VOMImageDataContext();

                            #region Parameters

                            SR_VOM_Data.Ref_Individual save_individual = new SR_VOM_Data.Ref_Individual();
                            SR_VOM_Data.PersonInfo save_person = new SR_VOM_Data.PersonInfo();
                            SR_VOM_Data.Ref_Passport save_passport = new SR_VOM_Data.Ref_Passport();
                            SR_VOM_Data.BlackListHit save_BLHit = new SR_VOM_Data.BlackListHit();
                            SR_VOM_Data.Print_Slip save_printSlip = new SR_VOM_Data.Print_Slip();
                            SR_VOM_Data.Print_VisaInfo save_printVisaInfo = new SR_VOM_Data.Print_VisaInfo();
                            SR_VOM_Data.Ref_Blacklist save_blackList = new SR_VOM_Data.Ref_Blacklist();
                            SR_VOM_Data.Ref_Children save_chilren = new SR_VOM_Data.Ref_Children();
                            SR_VOM_Data.Ref_Slip save_slip = new SR_VOM_Data.Ref_Slip();
                            SR_VOM_Data.VisaDestroy save_visaDestroy = new SR_VOM_Data.VisaDestroy();
                            #endregion
                            SR_VOM_Data.WSDataSyncSoapClient the_visa_info = new SR_VOM_Data.WSDataSyncSoapClient();
                            SR_VOM_Image.WSImageSyncSoapClient the_image_data = new SR_VOM_Image.WSImageSyncSoapClient();
                            #region VOM
                            #region binePersonInfo
                            PersonInfo the_person = (from c in dc.PersonInfos where c.PersonId == person_id select c).FirstOrDefault();
                            if (the_person != null)
                            {
                                LogIt("Start save PersonInfo > " + the_person.PersonId, false);
                                save_person.PersonId = the_person.PersonId;
                                save_person.FirstName = the_person.FirstName;
                                save_person.LastName = the_person.LastName;
                                save_person.DOB = the_person.DOB;
                                save_person.Nationality = the_person.Nationality;
                                save_person.Sex = the_person.Sex;
                                save_person.Occupation = the_person.Occupation;
                                save_person.MAddress = the_person.MAddress;
                                save_person.PAddress = the_person.PAddress;
                                save_person.TelNo = the_person.TelNo;
                                save_person.Email = the_person.Email;
                                save_person.Active = the_person.Active;
                                save_person.CreatedBy = the_person.CreatedBy;
                                save_person.CreatedOn = the_person.CreatedOn;
                                save_person.ModifiedBy = the_person.ModifiedBy;
                                save_person.ModifiedOn = the_person.ModifiedOn;
                                string result = the_visa_info.SaveVomPersonInfo(save_person, Properties.Settings.Default.StationCode);
                                if (result != "success")
                                {
                                    LogIt("Web Error > the_client_info.SaveIndividual > " + the_person.PersonId + " > " + result, false);
                                    throw new Exception("Web Error > the_client_info.SaveIndividual > " + the_person.PersonId + " > " + result);
                                }
                                else LogIt("End Save Ref_Individual > " + the_person.PersonId + " > " + result, false);

                            }
                            #endregion

                            #region bindPassport
                            Ref_Passport the_passport = (from c in dc.Ref_Passports where c.PersonId == person_id select c).FirstOrDefault();
                            if (the_passport != null)
                            {
                                save_passport.Ref_PassportId = the_passport.Ref_PassportId;
                                save_passport.PassportType = the_passport.PassportType;
                                save_passport.CountryCode = the_passport.CountryCode;
                                save_passport.PassportNo = the_passport.PassportNo;
                                save_passport.DateOfIssue = the_passport.DateOfIssue;
                                save_passport.DateOfExpire = the_passport.DateOfExpire;
                                save_passport.MRZ1 = the_passport.MRZ1;
                                save_passport.MRZ2 = the_passport.MRZ2;
                                save_passport.MRZ3 = the_passport.MRZ3;
                                save_passport.PersonId = the_passport.PersonId;
                                save_passport.Active = the_passport.Active;
                                save_passport.CreatedBy = the_passport.CreatedBy;
                                save_passport.CreatedOn = the_passport.CreatedOn;
                                save_passport.ModifiedBy = the_passport.ModifiedBy;
                                save_passport.ModifiedOn = the_passport.ModifiedOn;
                                string result = the_visa_info.SaveVomRef_Passport(save_passport, Properties.Settings.Default.StationCode);
                                if (result != "success")
                                {
                                    LogIt("Web Error > the_client_info.SaveIndividual > " + the_passport.PersonId + " > " + result, false);
                                    throw new Exception("Web Error > the_client_info.SaveIndividual > " + the_passport.PersonId + " > " + result);
                                }
                                else LogIt("End Save Ref_Individual > " + the_passport.PersonId + " > " + result, false);

                            }
                            #endregion

                            #region bindIndividual
                            Ref_Individual the_individual = (from c in dc.Ref_Individuals where c.Ref_IndividualId == ref_id select c).FirstOrDefault();
                            if (the_individual != null)
                            {
                                LogIt("Start Save Ref_Individual > " + the_individual.Ref_IndividualId, false);
                                save_individual.Ref_IndividualId = the_individual.Ref_IndividualId;
                                save_individual.SlipNo = the_individual.SlipNo;
                                save_individual.VisitPurpose = the_individual.VisitPurpose;
                                save_individual.VisaValidity = the_individual.VisaValidity;
                                save_individual.TypeOfBusiness = the_individual.TypeOfBusiness;
                                save_individual.CompanyName = the_individual.CompanyName;
                                save_individual.RecommendMinistry = the_individual.RecommendMinistry;
                                save_individual.VisaDateOfIssue = the_individual.VisaDateOfIssue;
                                save_individual.VisaDateOfExpiry = the_individual.VisaDateOfExpiry;
                                save_individual.VisaNo = the_individual.VisaNo;
                                save_individual.VisaFee = the_individual.VisaFee;
                                save_individual.Currency = the_individual.Currency;
                                save_individual.VisaType = the_individual.VisaType;
                                save_individual.Entries = the_individual.Entries;
                                save_individual.VisaName = the_individual.VisaName;
                                save_individual.PlaceOfIssue = the_individual.PlaceOfIssue;
                                save_individual.Annotation = the_individual.Annotation;
                                save_individual.Staydays = the_individual.Staydays;
                                save_individual.VisaCode = the_individual.VisaCode;
                                save_individual.Remark = the_individual.Remark;
                                save_individual.CounterNo = the_individual.CounterNo;
                                save_individual.Status = the_individual.Status;
                                save_individual.TotalInfant = the_individual.TotalInfant;
                                save_individual.VisaMRZ1 = the_individual.VisaMRZ1;
                                save_individual.VisaMRZ2 = the_individual.VisaMRZ2;
                                save_individual.WatchListResult = the_individual.WatchListResult;
                                save_individual.WatchListPoint = the_individual.WatchListPoint;
                                save_individual.BlackListHitId = the_individual.BlackListHitId;
                                save_individual.PersonId = the_individual.PersonId;
                                save_individual.Ref_PassportId = the_individual.Ref_PassportId;
                                save_individual.Active = the_individual.Active;
                                save_individual.CreatedBy = the_individual.CreatedBy;
                                save_individual.CreatedOn = the_individual.CreatedOn;
                                save_individual.ModifiedBy = the_individual.ModifiedBy;
                                save_individual.ModifiedOn = the_individual.ModifiedOn;
                                save_individual.isDestroy = the_individual.isDestroy;
                                save_individual.QueueNo = the_individual.QueueNo;
                                save_individual.AppointmentDate = the_individual.AppointmentDate;
                                save_individual.ApprovedBy = the_individual.ApprovedBy;
                                save_individual.ApprovedOn = the_individual.ApprovedOn;
                                save_individual.CollectedBy = the_individual.CollectedBy;
                                save_individual.CollectedOn = the_individual.CollectedOn;

                                the_individual.UploadedOn = DateTime.Now;



                                string result = the_visa_info.SaveVomData(save_individual, Properties.Settings.Default.StationCode);
                                if (result != "success")
                                {
                                    LogIt("Web Error > the_client_info.SaveIndividual > " + the_individual.Ref_IndividualId + " > " + result, false);
                                    throw new Exception("Web Error > the_client_info.SaveIndividual > " + the_individual.Ref_IndividualId + " > " + result);
                                }
                                else LogIt("End Save Ref_Individual > " + the_individual.Ref_IndividualId + " > " + result, false);
                            }
                            #endregion

                            #region BlackListHit
                            List<BlackListHit> theBLHit = (from c in dc.BlackListHits where c.PassportNo == passport_no orderby c.HitOn descending select c).ToList();
                            if (theBLHit != null)
                            {
                                foreach (BlackListHit bl in theBLHit)
                                {
                                    save_BLHit.BlackListHitId = bl.BlackListHitId;
                                    save_BLHit.BlackListId = bl.BlackListId;
                                    save_BLHit.RefIndividualId = bl.RefIndividualId;
                                    save_BLHit.FirstName = bl.FirstName;
                                    save_BLHit.LastName = bl.LastName;
                                    save_BLHit.DateOfBirth = bl.DateOfBirth;
                                    save_BLHit.Sex = bl.Sex;
                                    save_BLHit.Nationality = bl.Nationality;
                                    save_BLHit.PassportNo = bl.PassportNo;
                                    save_BLHit.BLResult = bl.BLResult;
                                    save_BLHit.BLPoint = bl.BLPoint;
                                    save_BLHit.HitOn = bl.HitOn;
                                    string result = the_visa_info.SaveVom_BlackListHit(save_BLHit, Properties.Settings.Default.StationCode);
                                    if (result != "success")
                                    {
                                        LogIt("Web Error > the_client_info.saveBlackListHit > " + the_individual.Ref_IndividualId + " > " + result, false);
                                        throw new Exception("Web Error > the_client_info.saveBlackListHit > " + the_individual.Ref_IndividualId + " > " + result);
                                    }
                                    else LogIt("End Save BlackListHit > " + the_individual.Ref_IndividualId + " > " + result, false);
                                }
                            }
                            #endregion

                            #region SlipPrint
                            List<Print_Slip> the_slipPrint = (from c in dc.Print_Slips where c.PassportNo == passport_no orderby c.CreatedOn descending select c).ToList();
                            if (the_slipPrint != null)
                            {
                                foreach (Print_Slip ps in the_slipPrint)
                                {
                                    save_printSlip.PrintSlipId = ps.PrintSlipId;
                                    save_printSlip.SlipNo = ps.SlipNo;
                                    save_printSlip.Name = ps.Name;
                                    save_printSlip.Sex = ps.Sex;
                                    save_printSlip.DateOfBirth = ps.DateOfBirth;
                                    save_printSlip.PassportNo = ps.PassportNo;
                                    save_printSlip.Nationality = ps.Nationality;
                                    save_printSlip.VisaName = ps.VisaName;
                                    save_printSlip.VisaFee = ps.VisaFee;
                                    save_printSlip.Active = ps.Active;
                                    save_printSlip.CreatedBy = ps.CreatedBy;
                                    save_printSlip.CreatedOn = ps.CreatedOn;
                                    save_printSlip.ModifiedBy = ps.ModifiedBy;
                                    save_printSlip.ModifiedOn = ps.ModifiedOn;
                                    string result = the_visa_info.SaveVom_PrintSlip(save_printSlip, Properties.Settings.Default.StationCode);
                                    if (result != "success")
                                    {
                                        LogIt("Web Error > the_client_info.savePrintSlip > " + the_individual.Ref_IndividualId + " > " + result, false);
                                        throw new Exception("Web Error > the_client_info.savePrintSlip > " + the_individual.Ref_IndividualId + " > " + result);
                                    }
                                    else LogIt("End Save savePrintSlip > " + the_individual.Ref_IndividualId + " > " + result, false);
                                }
                            }
                            #endregion

                            #region PrintVisaInfo
                            List<Print_VisaInfo> the_VisaInfo = (from c in dc.Print_VisaInfos where c.Ref_IndividualId == ref_id select c).ToList();
                            if (the_VisaInfo != null)
                            {
                                foreach (Print_VisaInfo pvi in the_VisaInfo)
                                {
                                    save_printVisaInfo.Print_VisaInfo_ID = pvi.Print_VisaInfo_ID;
                                    save_printVisaInfo.VisaNo = pvi.VisaNo;
                                    save_printVisaInfo.VisaType = pvi.VisaType;
                                    save_printVisaInfo.PrintedBy = pvi.PrintedBy;
                                    save_printVisaInfo.PrintedOn = pvi.PrintedOn;
                                    save_printVisaInfo.Ref_IndividualId = pvi.Ref_IndividualId;
                                    string result = the_visa_info.SaveVom_PrintVisaInfo(save_printVisaInfo, Properties.Settings.Default.StationCode);
                                    if (result != "success")
                                    {
                                        LogIt("Web Error > the_client_info.savePrintVisaInfo > " + the_individual.Ref_IndividualId + " > " + result, false);
                                        throw new Exception("Web Error > the_client_info.savePrintVisaInfo > " + the_individual.Ref_IndividualId + " > " + result);
                                    }
                                    else LogIt("End Save savePrintVisaInfo > " + the_individual.Ref_IndividualId + " > " + result, false);

                                }
                            }
                            #endregion

                            #region BlackList
                            List<Ref_Blacklist> the_blackList = (from c in dc.Ref_Blacklists where c.PPNo == passport_no select c).ToList();
                            if (the_blackList != null)
                            {
                                foreach (Ref_Blacklist blst in the_blackList)
                                {
                                    save_blackList.BlacklistID = blst.BlacklistID;
                                    save_blackList.Name = blst.Name;
                                    save_blackList.DOB = blst.DOB;
                                    save_blackList.Sex = blst.Sex;
                                    save_blackList.PPNo = blst.PPNo;
                                    save_blackList.NrcNo = blst.NrcNo;
                                    save_blackList.Country = blst.Country;
                                    save_blackList.Nationality = blst.Nationality;
                                    save_blackList.FatherName = blst.FatherName;
                                    save_blackList.MotherName = blst.MotherName;
                                    save_blackList.Address = blst.Address;
                                    save_blackList.LetterNo = blst.LetterNo;
                                    save_blackList.Reason = blst.Reason;
                                    save_blackList.Remark = blst.Remark;
                                    save_blackList.IsDeleted = blst.IsDeleted;
                                    save_blackList.IsExported = blst.IsExported;
                                    save_blackList.CreatedOn = blst.CreatedOn;
                                    save_blackList.CreatedBy = blst.CreatedBy;
                                    save_blackList.ModifiedOn = blst.ModifiedOn;
                                    save_blackList.ModifiedBy = blst.ModifiedBy;
                                    string result = the_visa_info.SaveVom_BlackList(save_blackList, Properties.Settings.Default.StationCode);
                                    if (result != "success")
                                    {
                                        LogIt("Web Error > the_client_info.saveBlackList > " + the_individual.Ref_IndividualId + " > " + result, false);
                                        throw new Exception("Web Error > the_client_info.saveBlackList > " + the_individual.Ref_IndividualId + " > " + result);
                                    }
                                    else LogIt("End Save saveBlackList > " + the_individual.Ref_IndividualId + " > " + result, false);

                                }
                            }
                            #endregion

                            #region Children
                            List<Ref_Children> the_children = (from c in dc.Ref_Childrens where c.Ref_IndividualId == ref_id select c).ToList();
                            if (the_children != null)
                            {
                                foreach (Ref_Children child in the_children)
                                {
                                    save_chilren.Ref_ChildlId = child.Ref_ChildlId;
                                    save_chilren.Ref_IndividualId = child.Ref_IndividualId;
                                    save_chilren.FullName = child.FullName;
                                    save_chilren.Gender = child.Gender;
                                    save_chilren.DateOfBirth = child.DateOfBirth;
                                    save_chilren.Type = child.Type;
                                    save_chilren.CreatedOn = child.CreatedOn;
                                    save_chilren.CreatedBy = child.CreatedBy;
                                    save_chilren.ModifiedOn = child.ModifiedOn;
                                    save_chilren.ModifiedBy = child.ModifiedBy;
                                    save_chilren.Active = child.Active;
                                    string result = the_visa_info.SaveVom_Children(save_chilren, Properties.Settings.Default.StationCode);
                                    if (result != "success")
                                    {
                                        LogIt("Web Error > the_client_info.saveChildren > " + the_individual.Ref_IndividualId + " > " + result, false);
                                        throw new Exception("Web Error > the_client_info.saveChildren > " + the_individual.Ref_IndividualId + " > " + result);
                                    }
                                    else LogIt("End Save saveChildren > " + the_individual.Ref_IndividualId + " > " + result, false);

                                }
                            }
                            #endregion

                            #region Slip
                            List<Ref_Slip> the_slip = (from c in dc.Ref_Slips where c.Ref_PersonalId == ref_id select c).ToList();
                            if (the_slip != null)
                            {
                                foreach (Ref_Slip slip in the_slip)
                                {
                                    save_slip.SlipID = slip.SlipID;
                                    save_slip.QueueNo = slip.QueueNo;
                                    save_slip.SlipNo = slip.SlipNo;
                                    save_slip.Ref_PersonalId = slip.Ref_PersonalId;
                                    save_slip.Active = slip.Active;
                                    save_slip.CreatedOn = slip.CreatedOn;
                                    save_slip.CreatedBy = slip.CreatedBy;
                                    save_slip.ModifiedOn = slip.ModifiedOn;
                                    save_slip.ModifiedBy = slip.ModifiedBy;
                                    string result = the_visa_info.SaveVom_Slip(save_slip, Properties.Settings.Default.StationCode);
                                    if (result != "success")
                                    {
                                        LogIt("Web Error > the_client_info.saveSlip > " + the_individual.Ref_IndividualId + " > " + result, false);
                                        throw new Exception("Web Error > the_client_info.saveSlip > " + the_individual.Ref_IndividualId + " > " + result);
                                    }
                                    else LogIt("End Save save slip > " + the_individual.Ref_IndividualId + " > " + result, false);

                                }
                            }
                            #endregion

                            #region VisaDestroy
                            List<VisaDestroy> the_visaDestroy = (from c in dc.VisaDestroys where c.Ref_IndividualId == ref_id select c).ToList();
                            if (the_visaDestroy != null)
                            {
                                foreach (VisaDestroy visadestroy in the_visaDestroy)
                                {
                                    save_visaDestroy.VisaDestroyId = visadestroy.VisaDestroyId;
                                    save_visaDestroy.Ref_IndividualId = visadestroy.Ref_IndividualId;
                                    save_visaDestroy.SlipNo = visadestroy.SlipNo;
                                    save_visaDestroy.MAddress = visadestroy.MAddress;
                                    save_visaDestroy.MTelNo = visadestroy.MTelNo;
                                    save_visaDestroy.MEmail = visadestroy.MEmail;
                                    save_visaDestroy.VisitPurpose = visadestroy.VisitPurpose;
                                    save_visaDestroy.VisaValidity = visadestroy.VisaValidity;
                                    save_visaDestroy.TypeOfBusiness = visadestroy.TypeOfBusiness;
                                    save_visaDestroy.CompanyName = visadestroy.CompanyName;
                                    save_visaDestroy.VisaDateOfIssue = visadestroy.VisaDateOfIssue;
                                    save_visaDestroy.VisaDateOfExpiry = visadestroy.VisaDateOfExpiry;
                                    save_visaDestroy.VisaNo = visadestroy.VisaNo;
                                    save_visaDestroy.VisaFee = visadestroy.VisaFee;
                                    save_visaDestroy.Currency = visadestroy.Currency;
                                    save_visaDestroy.VisaType = visadestroy.VisaType;
                                    save_visaDestroy.Entries = visadestroy.Entries;
                                    save_visaDestroy.VisaName = visadestroy.VisaName;
                                    save_visaDestroy.PlaceOfIssue = visadestroy.PlaceOfIssue;
                                    save_visaDestroy.Annotation = visadestroy.Annotation;
                                    save_visaDestroy.Staydays = visadestroy.Staydays;
                                    save_visaDestroy.VisaCode = visadestroy.VisaCode;
                                    save_visaDestroy.Remark = visadestroy.Remark;
                                    save_visaDestroy.CounterNo = visadestroy.CounterNo;
                                    save_visaDestroy.Status = visadestroy.Status;
                                    save_visaDestroy.TotalInfant = visadestroy.TotalInfant;
                                    save_visaDestroy.VisaMRZ1 = visadestroy.VisaMRZ1;
                                    save_visaDestroy.VisaMRZ2 = visadestroy.VisaMRZ2;
                                    save_visaDestroy.BlackListHitId = visadestroy.BlackListHitId;
                                    save_visaDestroy.PersonId = visadestroy.PersonId;
                                    save_visaDestroy.Ref_PassportId = visadestroy.Ref_PassportId;
                                    save_visaDestroy.Active = visadestroy.Active;
                                    save_visaDestroy.CreatedOn = visadestroy.CreatedOn;
                                    save_visaDestroy.CreatedBy = visadestroy.CreatedBy;
                                    save_visaDestroy.ModifiedOn = visadestroy.ModifiedOn;
                                    save_visaDestroy.ModifiedBy = visadestroy.ModifiedBy;
                                    string result = the_visa_info.SaveVom_VisaDestroy(save_visaDestroy, Properties.Settings.Default.StationCode);
                                    if (result != "success")
                                    {
                                        LogIt("Web Error > the_client_info.saveVisaDestroy > " + the_individual.Ref_IndividualId + " > " + result, false);
                                        throw new Exception("Web Error > the_client_info.saveVisaDestroy > " + the_individual.Ref_IndividualId + " > " + result);
                                    }
                                    else LogIt("End Save save Visa Destroy > " + the_individual.Ref_IndividualId + " > " + result, false);

                                }
                            }
                            #endregion

                            #region Image
                            List<SysImage> image_data = (from c in dcimg.SysImages where c.PassengerId == the_individual.Ref_PassportId && ((c.ModifiedOn - c.UploadedOn).Value.TotalDays > 0 || c.UploadedOn == null) select c).ToList();
                            if (image_data != null)
                            {
                                foreach (SysImage s in image_data)
                                {

                                    SR_VOM_Image.SysImage SavePhoto = new SR_VOM_Image.SysImage();
                                    #region bindReport
                                    imgid = s.ImageID;
                                    //if(imgid != null)
                                    //{
                                    //    GetImage();
                                    //}
                                    SavePhoto.ImageID = s.ImageID;
                                    SavePhoto.ImageData = (byte[])s.ImageData.ToArray();
                                    SavePhoto.Active = s.Active;
                                    SavePhoto.CreatedBy = s.CreatedBy;
                                    SavePhoto.CreatedOn = s.CreatedOn;
                                    SavePhoto.ModifiedBy = s.ModifiedBy;
                                    SavePhoto.ModifiedOn = s.ModifiedOn;
                                    SavePhoto.LastAction = s.LastAction;
                                    SavePhoto.ImageRecordType = s.ImageRecordType;
                                    SavePhoto.Imagestr = s.Imagestr;
                                    SavePhoto.PassengerId = s.PassengerId;

                                    //SavePhoto.Imagestr = Properties.Settings.Default.ImgPath +"\\" + s.PersonalNo + s.ImageRecordType+".jpg";
                                    s.UploadedOn = DateTime.Now;
                                    #endregion
                                    LogIt("Start save Photo >" + SavePhoto.ImageID, false);
                                    the_image_data.SaveVomPhoto(SavePhoto, Properties.Settings.Default.StationCode);
                                }
                            }


                            #endregion
                            #endregion
                            #region refreshUI
                            //item.Cells["Column6"].Value = DateTime.Now;
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
                            LogIt("End Save All Tbl > " + the_individual.Ref_IndividualId, false);
                            TimeSpan span = (DateTime.Now - start_date);
                            LogIt(String.Format("Total Synced : {0}/{4} Records. Duraing   {1} hours, {2} minutes, {3} seconds",
                                                 (Upload_Count).ToString("N0"), span.Hours, span.Minutes, span.Seconds, (total_count).ToString("N0")), true);
                        }
                        catch (Exception ex)
                        {
                            LogIt("" + ex.ToString(), false);

                            item.DefaultCellStyle.BackColor = Color.Maroon;
                            item.DefaultCellStyle.ForeColor = Color.White;
                            item.DefaultCellStyle.SelectionBackColor = Color.Maroon;
                            item.DefaultCellStyle.SelectionForeColor = Color.White;

                            refreshGrid();
                            processedCount += 1;
                            refreshProgressBar();

                            TimeSpan span = (DateTime.Now - start_date);
                            LogIt(String.Format("Total Synced : {0}/{4} Records. Duraing   {1} hours, {2} minutes, {3} seconds",
                                                 (Upload_Count).ToString("N0"), span.Hours, span.Minutes, span.Seconds, (total_count).ToString("N0")), true);
                            continue;
                        }

                        //}
                    }


                    bindDataGrid();
                    changeUploadUI();
                    //Thread.Sleep(10000);
                }
                else
                {
                    if (syncThread.ThreadState == ThreadState.Running) syncThread.Abort();
                    changeUploadUI();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There is no data to sync!");


                changeUploadUI();


            }


            //if (dgv_list.Rows.Count <= 0) changeUploadUI();
        }
        public void loadImage()
        {

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

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    Small_Image img = new Small_Image();
        //    img.ShowDialog();
        //}

        private void button1_Click_1(object sender, EventArgs e)
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
