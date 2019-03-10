using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSDataUploader.Controllers
{
    class ControllerImage
    {
        public static string imgDatabase = "";
        public static string PServer = "10.1.4.17";
        public static string PUser = "sa";
        public static string PPassword = "p@ssw0rd";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static string PP_Enrollment_ConnectionString()
        {
            return Properties.Settings.Default["PP_Enrollment"].ToString();
        }
        public static string PPImage_Enrollment_ConnectionString()
        {
            return Properties.Settings.Default["PP_DocumentConnectionString"].ToString();
        }
        public static string get_FingerConnectionString()
        {
            imgDatabase = "PP_Finger";
            return string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", PServer, imgDatabase, PUser, PPassword);
        }
        public static string get_IrisConnectionString()
        {
            imgDatabase = "PP_Iris";
            return string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", PServer, imgDatabase, PUser, PPassword);
        }
        public static string get_DocumentConnectionString()
        {
            imgDatabase = "PP_Document";
            return string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", PServer, imgDatabase, PUser, PPassword);
        }
        public static string get_PhotoConnectionString()
        {
            imgDatabase = "PP_Photo";
            return string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", PServer, imgDatabase, PUser, PPassword);
        }
    }
}
