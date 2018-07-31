namespace Monexa.DET
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Web service ping",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract accounts",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract account details",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract account pricing",
            ""}, -1, System.Drawing.SystemColors.InactiveCaption, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract subscribers",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract sub-accounts",
            ""}, -1, System.Drawing.SystemColors.InactiveCaption, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract invoices",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract invoice details",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract transactions",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract plans",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract plan details",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract price books",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract price book details",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract price sheets",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract price sheet details",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract service bundles",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract rate modifications",
            ""}, -1, System.Drawing.SystemColors.GrayText, System.Drawing.Color.Empty, null);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnExtract = new System.Windows.Forms.Button();
            this.bwAPI_old = new System.ComponentModel.BackgroundWorker();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listTasks = new System.Windows.Forms.ListView();
            this.colTaskName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTaskStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskImageList = new System.Windows.Forms.ImageList(this.components);
            this.lblCommand = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cboCommands = new System.Windows.Forms.ComboBox();
            this.txtResponse = new System.Windows.Forms.TextBox();
            this.cbSend = new System.Windows.Forms.CheckBox();
            this.cbWriteToFile = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(12, 12);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(119, 23);
            this.btnExtract.TabIndex = 0;
            this.btnExtract.Text = "Extract Data";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // bwAPI_old
            // 
            this.bwAPI_old.WorkerReportsProgress = true;
            this.bwAPI_old.WorkerSupportsCancellation = true;
            this.bwAPI_old.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwAPI_DoWork);
            this.bwAPI_old.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwAPI_ProgressChanged);
            this.bwAPI_old.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwAPI_RunWorkerCompleted);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(137, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 41);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(532, 384);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listTasks);
            this.tabPage1.Controls.Add(this.lblCommand);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(524, 358);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Extraction Status";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listTasks
            // 
            this.listTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTaskName,
            this.colTaskStatus});
            this.listTasks.FullRowSelect = true;
            this.listTasks.GridLines = true;
            listViewItem1.Tag = "PingServer";
            listViewItem2.Tag = "AllAccounts";
            listViewItem3.Tag = "AccountDetails";
            listViewItem4.Tag = "AccountPricing";
            listViewItem5.Tag = "Subscribers";
            listViewItem6.Tag = "SubAccounts";
            listViewItem7.Tag = "Invoices";
            listViewItem8.Tag = "InvoiceDetails";
            listViewItem9.Tag = "Transactions";
            listViewItem10.Tag = "Plans";
            listViewItem11.Tag = "PlanDetails";
            listViewItem12.Tag = "PriceBooks";
            listViewItem13.Tag = "PriceBookDetails";
            listViewItem14.Tag = "PriceSheets";
            listViewItem15.Tag = "PriceSheetDetails";
            listViewItem16.Tag = "ServiceBundles";
            listViewItem17.Tag = "RateModifications";
            this.listTasks.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17});
            this.listTasks.Location = new System.Drawing.Point(3, 6);
            this.listTasks.Name = "listTasks";
            this.listTasks.Size = new System.Drawing.Size(515, 349);
            this.listTasks.SmallImageList = this.taskImageList;
            this.listTasks.TabIndex = 11;
            this.listTasks.UseCompatibleStateImageBehavior = false;
            this.listTasks.View = System.Windows.Forms.View.Details;
            // 
            // colTaskName
            // 
            this.colTaskName.Text = "Task";
            this.colTaskName.Width = 236;
            // 
            // colTaskStatus
            // 
            this.colTaskStatus.Text = "Status";
            this.colTaskStatus.Width = 242;
            // 
            // taskImageList
            // 
            this.taskImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("taskImageList.ImageStream")));
            this.taskImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.taskImageList.Images.SetKeyName(0, "hourglass.png");
            this.taskImageList.Images.SetKeyName(1, "check.png");
            this.taskImageList.Images.SetKeyName(2, "error.png");
            this.taskImageList.Images.SetKeyName(3, "triangle.png");
            // 
            // lblCommand
            // 
            this.lblCommand.AutoSize = true;
            this.lblCommand.Location = new System.Drawing.Point(250, 222);
            this.lblCommand.Name = "lblCommand";
            this.lblCommand.Size = new System.Drawing.Size(0, 13);
            this.lblCommand.TabIndex = 10;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtSummary);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(524, 358);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Error Details";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtSummary
            // 
            this.txtSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSummary.Location = new System.Drawing.Point(6, 6);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSummary.Size = new System.Drawing.Size(506, 346);
            this.txtSummary.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cboCommands);
            this.tabPage3.Controls.Add(this.txtResponse);
            this.tabPage3.Controls.Add(this.cbSend);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(524, 358);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cboCommands
            // 
            this.cboCommands.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCommands.FormattingEnabled = true;
            this.cboCommands.Location = new System.Drawing.Point(6, 6);
            this.cboCommands.Name = "cboCommands";
            this.cboCommands.Size = new System.Drawing.Size(238, 21);
            this.cboCommands.TabIndex = 12;
            // 
            // txtResponse
            // 
            this.txtResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResponse.Location = new System.Drawing.Point(3, 31);
            this.txtResponse.Multiline = true;
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.Size = new System.Drawing.Size(512, 321);
            this.txtResponse.TabIndex = 10;
            // 
            // cbSend
            // 
            this.cbSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSend.AutoSize = true;
            this.cbSend.Location = new System.Drawing.Point(412, 8);
            this.cbSend.Name = "cbSend";
            this.cbSend.Size = new System.Drawing.Size(100, 17);
            this.cbSend.TabIndex = 11;
            this.cbSend.Text = "Send Request?";
            this.cbSend.UseVisualStyleBackColor = true;
            // 
            // cbWriteToFile
            // 
            this.cbWriteToFile.AutoSize = true;
            this.cbWriteToFile.Location = new System.Drawing.Point(456, 16);
            this.cbWriteToFile.Name = "cbWriteToFile";
            this.cbWriteToFile.Size = new System.Drawing.Size(79, 17);
            this.cbWriteToFile.TabIndex = 8;
            this.cbWriteToFile.Text = "Write to file";
            this.cbWriteToFile.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 437);
            this.Controls.Add(this.cbWriteToFile);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExtract);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Monexa Data Extraction Tool";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExtract;
        private System.ComponentModel.BackgroundWorker bwAPI_old;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtSummary;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.ComboBox cboCommands;
        private System.Windows.Forms.TextBox txtResponse;
        private System.Windows.Forms.CheckBox cbSend;
        private System.Windows.Forms.CheckBox cbWriteToFile;
        private System.Windows.Forms.ListView listTasks;
        private System.Windows.Forms.ColumnHeader colTaskName;
        private System.Windows.Forms.ColumnHeader colTaskStatus;
        private System.Windows.Forms.ImageList taskImageList;
    }
}

