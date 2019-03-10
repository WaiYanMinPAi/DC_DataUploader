using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SSDataUploader
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Small_Image());
            Application.Run(new frm_data_sync_noDate());
        }
    }
}
