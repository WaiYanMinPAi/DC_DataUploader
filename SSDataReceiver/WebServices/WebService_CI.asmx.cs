using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SSDataReceiver.LINQ;

namespace SSDataReceiver.WebServices
{
    /// <summary>
    /// Summary description for WebService_CI
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService_CI : System.Web.Services.WebService
    {
        public string GetConnectionString(String StationCode)
        {
//#if DEBUG
//            return "Data Source=.;Initial Catalog=CI_Portal;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
//#endif

            if (StationCode == "CI1")
            {
                //Dev
                //return "Data Source=.;Initial Catalog=PAI_CI_Central;Persist Security Info=True;User ID=rgbdeveloper;Password=P@ssw0rd";
                //Live
                //return @"Data Source=.\sqlexpress;Initial Catalog=PAI_CI_Central;Persist Security Info=True;User ID=SBSAdministrator;Password=System@t1cP@ssw0rd";
                return "Data Source=172.16.15.25;Initial Catalog=CI1;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI2")
            {
                //Dev
                //return "Data Source=.;Initial Catalog=PAI_CI_Central;Persist Security Info=True;User ID=rgbdeveloper;Password=P@ssw0rd";
                //Live
                //return "Data Source=.;Initial Catalog=PAI_CI_Central;Persist Security Info=True;User ID=SBSAdministrator;Password=System@t1cP@ssw0rd";
                return "Data Source=172.16.15.25;Initial Catalog=CI2;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI3")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI3;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI4")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI4;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI5")
            {
                //Dev
                return "Data Source=.;Initial Catalog=PAI_CI_Central;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                //Live
                //return "Data Source=172.16.15.25;Initial Catalog=CI5;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI6")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI6;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI7")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI7;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI8")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI8;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI9")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI9;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI10")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI10;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (StationCode == "CI11")
            {
                return "Data Source=172.16.15.25;Initial Catalog=CI11;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }

            return "";
        }

        [WebMethod]
        public string SaveOWIC(Ref_OWIC the_individual, String StationCode)
        {
            try
            {
                Linq_CIDataContext dc = new Linq_CIDataContext(GetConnectionString(StationCode));
                Ref_OWIC save_individual = (from c in dc.Ref_OWICs where c.regno == the_individual.regno select c).FirstOrDefault();
                if (save_individual == null)
                {
                    save_individual = new Ref_OWIC()
                    {
                        regno = the_individual.regno
                    };
                    dc.Ref_OWICs.InsertOnSubmit(save_individual);
                }
                save_individual.PersonalNo = the_individual.PersonalNo;
                save_individual.CreatedBy = the_individual.CreatedBy;
                save_individual.CreatedOn = the_individual.CreatedOn;
                save_individual.ModifiedBy = the_individual.ModifiedBy;
                save_individual.ModifiedOn = the_individual.ModifiedOn;
                save_individual.COINo = the_individual.COINo;
                save_individual.MMAgency = the_individual.MMAgency;
                save_individual.FoAgency = the_individual.FoAgency;
                save_individual.FoAddress = the_individual.FoAddress;
                save_individual.DateOfIssue = the_individual.DateOfIssue;
                save_individual.DateOfExpire = the_individual.DateOfExpire;
                save_individual.active = the_individual.active;
                save_individual.contact = the_individual.contact;
                save_individual.corp = the_individual.corp;
                save_individual.mrz1 = the_individual.mrz1;
                save_individual.mrz2 = the_individual.mrz2;
                save_individual.mrz3 = the_individual.mrz3;
                save_individual.photostr = the_individual.photostr;
                save_individual.uvstr = the_individual.uvstr;
                save_individual.signstr = the_individual.signstr;
                save_individual.qrstr = the_individual.qrstr;
                save_individual.securetxt = the_individual.securetxt;
                save_individual.goCountry = the_individual.goCountry;
                save_individual.companyName = the_individual.companyName;
                save_individual.pstatus = the_individual.pstatus;
                save_individual.pcount = the_individual.pcount;


                dc.SubmitChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [WebMethod]
        public string SaveIndividual(Ref_Individual the_individual, String StationCode)
        {
            try
            {
                Linq_CIDataContext dc = new Linq_CIDataContext(GetConnectionString(StationCode));
                Ref_Individual save_individual = (from c in dc.Ref_Individuals where c.Ref_PersonalId == the_individual.Ref_PersonalId select c).FirstOrDefault();
                if (save_individual == null)
                {
                    save_individual = new Ref_Individual()
                    {
                        Ref_PersonalId = the_individual.Ref_PersonalId
                    };
                    dc.Ref_Individuals.InsertOnSubmit(save_individual);
                }
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

                dc.SubmitChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public bool IsNewImage(string ImageID, string StationCode)
        {

            Linq_CIDataContext dc = new Linq_CIDataContext(GetConnectionString(StationCode));
            if ((from c in dc.sysimages where c.ImageID == ImageID select c).Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [WebMethod]
        public string SaveImage(Byte[] ImageData, string ImageID, string CreatedBy, DateTime CreatedOn, string ImageRecordType, string StationCode)
        {
            try
            {
                Linq_CIDataContext dc = new Linq_CIDataContext(GetConnectionString(StationCode));
                if ((from c in dc.sysimages where c.ImageID == ImageID select c).Count() == 0)
                {
                    sysimage the_image = new sysimage()
                    {
                        Active = true,
                        CreatedBy = CreatedBy,
                        CreatedOn = CreatedOn,
                        ImageData = ImageData,
                        ImageID = ImageID,
                        ImageRecordType = ImageRecordType,
                        LastAction = Guid.NewGuid().ToString(),
                        ModifiedBy = CreatedBy,
                        ModifiedOn = CreatedOn,
                    };
                    dc.sysimages.InsertOnSubmit(the_image);
                    dc.SubmitChanges();
                }
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
