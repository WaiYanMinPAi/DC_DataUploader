using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SSDataUploader.LINQ;
using System.Threading;
using System.IO;

namespace SSDataUploader
{
    public partial class Small_Image : Form
    {
        string con1 = Properties.Settings.Default.PAI_CI_CentralConnectionString;
        string con2 = Properties.Settings.Default.OWIC_Small_ImageConnectionString;

        public Small_Image()
        {
            InitializeComponent();
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            img();
        }
        private Image Resize(Image img)
        {
            int w = img.Width / 4;
            int h = img.Height / 4;
            Bitmap bmp = new Bitmap(w, h);
            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.DrawImage(img, 0, 0, w, h);
            return (Image)bmp;
        }
        private void CompressImage(Image sourceImage, int imageQuality, string savePath)
        {
            try
            {
                //Create an ImageCodecInfo-object for the codec information
                ImageCodecInfo jpegCodec = null;

                //Set quality factor for compression
                EncoderParameter imageQualitysParameter = new EncoderParameter(
                            System.Drawing.Imaging.Encoder.Quality, imageQuality);

                //List all avaible codecs (system wide)
                ImageCodecInfo[] alleCodecs = ImageCodecInfo.GetImageEncoders();

                EncoderParameters codecParameter = new EncoderParameters(1);
                codecParameter.Param[0] = imageQualitysParameter;

                //Find and choose JPEG codec
                for (int i = 0; i < alleCodecs.Length; i++)
                {
                    if (alleCodecs[i].MimeType == "image/jpeg")
                    {
                        jpegCodec = alleCodecs[i];
                        break;
                    }
                }

                //Save compressed image
                sourceImage.Save(savePath, jpegCodec, codecParameter);
                sourceImage.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private void ClearFile()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Properties.Settings.Default.ImgPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        Int32 total_count = 0;
        Int32 Upload_Count = 0;
        private void img()
        {
            total_count = 0;
            Upload_Count = 0;

            try
            {
            DateTime start_date = DateTime.Now;
            changeLabelStatus("Starting Synchronization. Please wait");
            LINQ_CIDataContext dc = new LINQ_CIDataContext(con1);

            //string dt = DateTime.TryParse(dtp_date.Value.Date).ToString();

            //List<GetSmallImgResult> records = dc.GetSmallImg(dtp_date.Value.Date).ToList();
            List<To_Insert_Small_Img> records = dc.To_Insert_Small_Imgs.ToList();

            if (records.Count > 0)
            {
                total_count = records.Count;
                Log.Info("Admin", "Small Image Count > " + records.Count.ToString(), dtp_date.Value.ToShortDateString());
                DialogResult dr = MessageBox.Show("Total " + records.Count.ToString() + " small image need to save !!!", "Confirmation for Save Small Image", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (dr == DialogResult.OK)
                {
                    for (int i = 0; i < records.Count; i++)
                    {
                        //if (records[i].small == null)
                        //{
                            SysImage recordImg = (from c in dc.SysImages where c.ImageID == records[i].ImageID && c.Active == true select c).FirstOrDefault();

                            var arrayBinary = recordImg.ImageData.ToArray();

                            using (MemoryStream ms = new MemoryStream(arrayBinary))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                                //image.Save(Properties.Settings.Default.ImgPath.ToString() + "\\" + records[i].ImageID + "_big.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                //System.Drawing.Image img = System.Drawing.Image.FromFile(Properties.Settings.Default.ImgPath.ToString() + "\\" + records[i].ImageID + "_big.jpg");
                                //img = Resize(img);
                                image = Resize(image);
                                //img.Save(Properties.Settings.Default.ImgPath.ToString() + "\\" + records[i].big + "_midium.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                //CompressImage(Image.FromFile(Properties.Settings.Default.ImgPath + "\\" + records[i].ImageID + "_big.jpg"), 50, Properties.Settings.Default.ImgPath + "\\" + records[i].ImageID + "_small.jpg");
                                CompressImage(image, 50, Properties.Settings.Default.ImgPath + "\\" + records[i].ImageID + "_small.jpg");

                                FileStream fs = new FileStream(Properties.Settings.Default.ImgPath + "\\" + records[i].ImageID + "_small.jpg", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                byte[] bImage = new byte[fs.Length];
                                fs.Read(bImage, 0, Convert.ToInt32(fs.Length));
                                fs.Close();

                                Linq_SImgDataContext dc1 = new Linq_SImgDataContext(con2);
                                SmallImage the_simage = new SmallImage()
                                {
                                    CreatedBy = recordImg.CreatedBy,
                                    ModifiedBy = recordImg.ModifiedBy,
                                    ModifiedOn = recordImg.ModifiedOn,
                                    CreatedOn = recordImg.CreatedOn,
                                    ImageID = recordImg.ImageID,
                                    LastAction = recordImg.LastAction,
                                    ImageData = bImage,
                                    ImageRecordType = recordImg.ImageRecordType

                                };
                                dc1.SmallImages.InsertOnSubmit(the_simage);
                                dc1.SubmitChanges();

                                recordImg.SmallImg = true;
                                dc.SubmitChanges();

                                Upload_Count = Upload_Count + 1;

                                //break;
                            }
                        //}

                            TimeSpan span = (DateTime.Now - start_date);

                            changeLabelStatus(String.Format("Total Synced : {0}/{4} Records. Duraing   {1} hours, {2} minutes, {3} seconds",
                                     (Upload_Count).ToString("N0"), span.Hours, span.Minutes, span.Seconds, (total_count).ToString("N0")));
                    }
                }
                else
                {

                }

                Log.Info("Admin", "End Small Image Save > " + records.Count.ToString(), dtp_date.Value.ToShortDateString());
                MessageBox.Show("Completed > " + records.Count.ToString() + " >> " + dtp_date.Value.ToShortDateString());
            }
            else {
                MessageBox.Show("No new record for small image");
                changeLabelStatus("No new record for small image");
            }



            changeLabelStatus(lbl_running_count.Text + " Completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (button1.InvokeRequired)
                {
                    button1.Invoke(new MethodInvoker(delegate
                    {
                        button1.Enabled = true;
                    }));
                }
                else
                {
                    button1.Enabled = true;
                }

                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.Cursor = Cursors.Default;
                    }));
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }


        }

        void changeLabelStatus(string message)
        {

            if (lbl_running_count.InvokeRequired)
            {
                lbl_running_count.Invoke(new MethodInvoker(delegate
                {
                    lbl_running_count.Text = message;
                }));
            }
            else
            {
                lbl_running_count.Text = message;
            }

        }

        private void Small_Image_Load(object sender, EventArgs e)
        {
            ClearFile();
            comboBox1.Text = "CI1";
        }

        private void Small_Image_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearFile();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "Stop")
            {
                this.button1.Text = "START SYNC";
                changeLabelStatus("");
                comboBox1.Enabled = true;
                Thread th = new Thread(img);
                //th.Start();
                th.Abort();
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;
                this.button1.Text = "Stop";
                comboBox1.Enabled = false;
                Thread thread = new Thread(img);
                thread.Start();
                //img();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "CI1")
            {
                //Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                //Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI_Small_Img;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI1;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI1_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            if (comboBox1.Text == "CI2")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI2;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI2_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI3")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI3;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI3_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI4")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI4;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI4_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            if (comboBox1.Text == "CI5")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI5;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI5_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI6")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI6;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI6_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI7")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI7;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI7_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI8")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI8;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI8_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI9")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI9;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI9_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI10")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI10;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI10_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }
            else if (comboBox1.Text == "CI11")
            {
                Properties.Settings.Default["PAI_CI_CentralConnectionString"] = "Data Source=.;Initial Catalog=CI11;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
                Properties.Settings.Default["OWIC_Small_ImageConnectionString"] = "Data Source=.;Initial Catalog=CI11_OWIC_Small_Image;Persist Security Info=True;User ID=sa;Password=p@ssw0rd";
            }

            con1 = Properties.Settings.Default.PAI_CI_CentralConnectionString;
            con2 = Properties.Settings.Default.OWIC_Small_ImageConnectionString;

        }
    }
}
