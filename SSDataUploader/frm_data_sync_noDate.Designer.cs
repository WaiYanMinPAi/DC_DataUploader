namespace SSDataUploader
{
    partial class frm_data_sync_noDate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_upload = new System.Windows.Forms.Button();
            this.dgv_list = new System.Windows.Forms.DataGridView();
            this.pb_progress_bar = new System.Windows.Forms.ProgressBar();
            this.lbl_running_count = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.result = new System.Windows.Forms.TextBox();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.srno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PersonID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateOfBirth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlaceOfBirth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NIRC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VisaNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VisaName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecordStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModifiedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UploadedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_list)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_upload
            // 
            this.btn_upload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_upload.BackColor = System.Drawing.Color.SeaGreen;
            this.btn_upload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_upload.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_upload.ForeColor = System.Drawing.Color.White;
            this.btn_upload.Location = new System.Drawing.Point(12, 403);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(152, 71);
            this.btn_upload.TabIndex = 0;
            this.btn_upload.Text = "Sync Now";
            this.btn_upload.UseVisualStyleBackColor = false;
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // dgv_list
            // 
            this.dgv_list.AllowUserToAddRows = false;
            this.dgv_list.AllowUserToDeleteRows = false;
            this.dgv_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_list.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.srno,
            this.PersonID,
            this.Column3,
            this.DateOfBirth,
            this.PlaceOfBirth,
            this.NIRC,
            this.VisaNumber,
            this.VisaName,
            this.RecordStatus,
            this.ModifiedOn,
            this.UploadedOn,
            this.col_duration});
            this.dgv_list.Location = new System.Drawing.Point(12, 29);
            this.dgv_list.Name = "dgv_list";
            this.dgv_list.ReadOnly = true;
            this.dgv_list.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_list.Size = new System.Drawing.Size(988, 350);
            this.dgv_list.TabIndex = 2;
            // 
            // pb_progress_bar
            // 
            this.pb_progress_bar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_progress_bar.Location = new System.Drawing.Point(12, 385);
            this.pb_progress_bar.Name = "pb_progress_bar";
            this.pb_progress_bar.Size = new System.Drawing.Size(988, 12);
            this.pb_progress_bar.TabIndex = 4;
            // 
            // lbl_running_count
            // 
            this.lbl_running_count.AutoSize = true;
            this.lbl_running_count.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_running_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_running_count.Location = new System.Drawing.Point(12, 9);
            this.lbl_running_count.Name = "lbl_running_count";
            this.lbl_running_count.Size = new System.Drawing.Size(28, 17);
            this.lbl_running_count.TabIndex = 7;
            this.lbl_running_count.Text = "....";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackColor = System.Drawing.Color.Firebrick;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(181, 403);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 71);
            this.button1.TabIndex = 8;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // result
            // 
            this.result.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.result.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.result.Location = new System.Drawing.Point(339, 403);
            this.result.Multiline = true;
            this.result.Name = "result";
            this.result.ReadOnly = true;
            this.result.Size = new System.Drawing.Size(661, 71);
            this.result.TabIndex = 9;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Ref_IndividualId";
            this.Column1.HeaderText = "id";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Visible = false;
            this.Column1.Width = 50;
            // 
            // srno
            // 
            this.srno.HeaderText = "No.";
            this.srno.Name = "srno";
            this.srno.ReadOnly = true;
            this.srno.Width = 50;
            // 
            // PersonID
            // 
            this.PersonID.DataPropertyName = "PersonId";
            this.PersonID.HeaderText = "PersonID";
            this.PersonID.Name = "PersonID";
            this.PersonID.ReadOnly = true;
            this.PersonID.Visible = false;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.DataPropertyName = "Name";
            this.Column3.HeaderText = "Name";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // DateOfBirth
            // 
            this.DateOfBirth.DataPropertyName = "DOB";
            this.DateOfBirth.HeaderText = "Date of Birth";
            this.DateOfBirth.Name = "DateOfBirth";
            this.DateOfBirth.ReadOnly = true;
            // 
            // PlaceOfBirth
            // 
            this.PlaceOfBirth.DataPropertyName = "Sex";
            this.PlaceOfBirth.HeaderText = "Gender";
            this.PlaceOfBirth.Name = "PlaceOfBirth";
            this.PlaceOfBirth.ReadOnly = true;
            // 
            // NIRC
            // 
            this.NIRC.DataPropertyName = "PassportNo";
            this.NIRC.HeaderText = "Passport Number";
            this.NIRC.Name = "NIRC";
            this.NIRC.ReadOnly = true;
            this.NIRC.Width = 150;
            // 
            // VisaNumber
            // 
            this.VisaNumber.DataPropertyName = "VisaNo";
            this.VisaNumber.HeaderText = "Visa Number";
            this.VisaNumber.Name = "VisaNumber";
            this.VisaNumber.ReadOnly = true;
            // 
            // VisaName
            // 
            this.VisaName.DataPropertyName = "VisaName";
            this.VisaName.HeaderText = "Visa Name";
            this.VisaName.Name = "VisaName";
            this.VisaName.ReadOnly = true;
            // 
            // RecordStatus
            // 
            this.RecordStatus.DataPropertyName = "record_status";
            this.RecordStatus.HeaderText = "Status";
            this.RecordStatus.Name = "RecordStatus";
            this.RecordStatus.ReadOnly = true;
            // 
            // ModifiedOn
            // 
            this.ModifiedOn.DataPropertyName = "ModifiedOn";
            dataGridViewCellStyle1.Format = "dd/MM/yyyy hh:mm tt";
            this.ModifiedOn.DefaultCellStyle = dataGridViewCellStyle1;
            this.ModifiedOn.HeaderText = "Modified On";
            this.ModifiedOn.Name = "ModifiedOn";
            this.ModifiedOn.ReadOnly = true;
            this.ModifiedOn.Width = 150;
            // 
            // UploadedOn
            // 
            this.UploadedOn.DataPropertyName = "UploadedOn";
            this.UploadedOn.HeaderText = "UploadedOn";
            this.UploadedOn.Name = "UploadedOn";
            this.UploadedOn.ReadOnly = true;
            this.UploadedOn.Visible = false;
            // 
            // col_duration
            // 
            this.col_duration.DataPropertyName = "LastAction";
            this.col_duration.HeaderText = "duration";
            this.col_duration.Name = "col_duration";
            this.col_duration.ReadOnly = true;
            this.col_duration.Visible = false;
            // 
            // frm_data_sync_noDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 486);
            this.Controls.Add(this.result);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbl_running_count);
            this.Controls.Add(this.pb_progress_bar);
            this.Controls.Add(this.dgv_list);
            this.Controls.Add(this.btn_upload);
            this.Name = "frm_data_sync_noDate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Sync";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_data_sync_noDate_FormClosing);
            this.Load += new System.EventHandler(this.frm_data_sync_noDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_list)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_upload;
        private System.Windows.Forms.DataGridView dgv_list;
        private System.Windows.Forms.ProgressBar pb_progress_bar;
        private System.Windows.Forms.Label lbl_running_count;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox result;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn srno;
        private System.Windows.Forms.DataGridViewTextBoxColumn PersonID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateOfBirth;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlaceOfBirth;
        private System.Windows.Forms.DataGridViewTextBoxColumn NIRC;
        private System.Windows.Forms.DataGridViewTextBoxColumn VisaNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn VisaName;
        private System.Windows.Forms.DataGridViewTextBoxColumn RecordStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModifiedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UploadedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_duration;
    }
}

