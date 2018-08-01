namespace Monexa.DET
{
    partial class MainForm
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
            ""}, -1, System.Drawing.SystemColors.InactiveCaption, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract price sheet details",
            ""}, -1, System.Drawing.SystemColors.InactiveCaption, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract service bundles",
            ""}, -1, System.Drawing.SystemColors.InactiveCaption, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem(new string[] {
            "    Extract rate modifications",
            ""}, -1, System.Drawing.SystemColors.InactiveCaption, System.Drawing.Color.Empty, null);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numPageSize = new System.Windows.Forms.NumericUpDown();
            this.listTasks = new System.Windows.Forms.ListView();
            this.colTaskName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTaskStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskImageList = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.lblCommand = new System.Windows.Forms.Label();
            this.cbWriteToFile = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPageSize)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExtract.Location = new System.Drawing.Point(12, 416);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(220, 23);
            this.btnExtract.TabIndex = 0;
            this.btnExtract.Text = "Extract Data";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(466, 416);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
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
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(532, 398);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.numPageSize);
            this.tabPage1.Controls.Add(this.listTasks);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.lblCommand);
            this.tabPage1.Controls.Add(this.cbWriteToFile);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(524, 372);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Extraction Status";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // numPageSize
            // 
            this.numPageSize.Location = new System.Drawing.Point(275, 12);
            this.numPageSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPageSize.Name = "numPageSize";
            this.numPageSize.Size = new System.Drawing.Size(50, 20);
            this.numPageSize.TabIndex = 12;
            this.numPageSize.ThousandsSeparator = true;
            this.numPageSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
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
            this.listTasks.Location = new System.Drawing.Point(3, 43);
            this.listTasks.Name = "listTasks";
            this.listTasks.Size = new System.Drawing.Size(515, 326);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Results page size:";
            // 
            // lblCommand
            // 
            this.lblCommand.AutoSize = true;
            this.lblCommand.Location = new System.Drawing.Point(250, 222);
            this.lblCommand.Name = "lblCommand";
            this.lblCommand.Size = new System.Drawing.Size(0, 13);
            this.lblCommand.TabIndex = 10;
            // 
            // cbWriteToFile
            // 
            this.cbWriteToFile.AutoSize = true;
            this.cbWriteToFile.Location = new System.Drawing.Point(17, 13);
            this.cbWriteToFile.Name = "cbWriteToFile";
            this.cbWriteToFile.Size = new System.Drawing.Size(112, 17);
            this.cbWriteToFile.TabIndex = 8;
            this.cbWriteToFile.Text = "Write results to file";
            this.cbWriteToFile.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtSummary);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(524, 372);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Summary";
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
            this.txtSummary.Size = new System.Drawing.Size(512, 360);
            this.txtSummary.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 451);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExtract);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "Monexa Data Extraction Tool";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPageSize)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtSummary;
        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.ListView listTasks;
        private System.Windows.Forms.ColumnHeader colTaskName;
        private System.Windows.Forms.ColumnHeader colTaskStatus;
        private System.Windows.Forms.ImageList taskImageList;
        private System.Windows.Forms.CheckBox cbWriteToFile;
        private System.Windows.Forms.NumericUpDown numPageSize;
        private System.Windows.Forms.Label label2;
    }
}

