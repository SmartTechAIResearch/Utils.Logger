namespace SizingServers.Log.WinForms {
    partial class FileLoggerPanel {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.clmTimestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMember = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSourceFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmParameters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmException = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmReadableWatsonBucketParameters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmJson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboLogLevel = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cboLog = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSet = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnClear = new System.Windows.Forms.Button();
            this.chkShowCellView = new System.Windows.Forms.CheckBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.rtxt = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmTimestamp,
            this.clmLevel,
            this.clmDescription,
            this.clmMember,
            this.clmSourceFile,
            this.clmLine,
            this.clmParameters,
            this.clmException,
            this.clmReadableWatsonBucketParameters,
            this.clmJson});
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgv.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv.Size = new System.Drawing.Size(818, 536);
            this.dgv.TabIndex = 4;
            this.dgv.VirtualMode = true;
            this.dgv.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellContentClick);
            this.dgv.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgv_CellValueNeeded);
            // 
            // clmTimestamp
            // 
            this.clmTimestamp.HeaderText = "";
            this.clmTimestamp.Name = "clmTimestamp";
            this.clmTimestamp.ReadOnly = true;
            // 
            // clmLevel
            // 
            this.clmLevel.HeaderText = "Level";
            this.clmLevel.Name = "clmLevel";
            this.clmLevel.ReadOnly = true;
            // 
            // clmDescription
            // 
            this.clmDescription.HeaderText = "Description";
            this.clmDescription.Name = "clmDescription";
            this.clmDescription.ReadOnly = true;
            // 
            // clmMember
            // 
            this.clmMember.HeaderText = "Member";
            this.clmMember.Name = "clmMember";
            this.clmMember.ReadOnly = true;
            // 
            // clmSourceFile
            // 
            this.clmSourceFile.HeaderText = "Source File";
            this.clmSourceFile.Name = "clmSourceFile";
            this.clmSourceFile.ReadOnly = true;
            // 
            // clmLine
            // 
            this.clmLine.HeaderText = "Line";
            this.clmLine.Name = "clmLine";
            this.clmLine.ReadOnly = true;
            // 
            // clmParameters
            // 
            this.clmParameters.HeaderText = "parameters";
            this.clmParameters.Name = "clmParameters";
            this.clmParameters.ReadOnly = true;
            // 
            // clmException
            // 
            this.clmException.HeaderText = "Exception";
            this.clmException.Name = "clmException";
            this.clmException.ReadOnly = true;
            // 
            // clmReadableWatsonBucketParameters
            // 
            this.clmReadableWatsonBucketParameters.HeaderText = "Watson Buckets";
            this.clmReadableWatsonBucketParameters.Name = "clmReadableWatsonBucketParameters";
            this.clmReadableWatsonBucketParameters.ReadOnly = true;
            // 
            // clmJson
            // 
            this.clmJson.HeaderText = "JSON";
            this.clmJson.Name = "clmJson";
            this.clmJson.ReadOnly = true;
            this.clmJson.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Log:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Log level:";
            // 
            // cboLogLevel
            // 
            this.cboLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLogLevel.BackColor = System.Drawing.Color.White;
            this.cboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLogLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboLogLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboLogLevel.FormattingEnabled = true;
            this.cboLogLevel.Items.AddRange(new object[] {
            "Info",
            "Warning",
            "Error",
            "Fatal"});
            this.cboLogLevel.Location = new System.Drawing.Point(73, 45);
            this.cboLogLevel.Name = "cboLogLevel";
            this.cboLogLevel.Size = new System.Drawing.Size(705, 21);
            this.cboLogLevel.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Silver;
            this.panel2.Location = new System.Drawing.Point(72, 44);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(707, 23);
            this.panel2.TabIndex = 17;
            // 
            // cboLog
            // 
            this.cboLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLog.BackColor = System.Drawing.Color.White;
            this.cboLog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboLog.FormattingEnabled = true;
            this.cboLog.Location = new System.Drawing.Point(73, 13);
            this.cboLog.Name = "cboLog";
            this.cboLog.Size = new System.Drawing.Size(705, 21);
            this.cboLog.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Location = new System.Drawing.Point(72, 12);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 3, 6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(707, 23);
            this.panel1.TabIndex = 19;
            // 
            // btnSet
            // 
            this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSet.BackColor = System.Drawing.Color.White;
            this.btnSet.Enabled = false;
            this.btnSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSet.Location = new System.Drawing.Point(784, 45);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(46, 23);
            this.btnSet.TabIndex = 3;
            this.btnSet.Text = "Set";
            this.toolTip.SetToolTip(this.btnSet, "Log only entries with the given and higher levels. For instance: Level Warning wi" +
        "ll not log entries with Level Info. But will log everything above that.");
            this.btnSet.UseVisualStyleBackColor = false;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.BackColor = System.Drawing.Color.White;
            this.btnClear.Enabled = false;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(784, 13);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(46, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.toolTip.SetToolTip(this.btnClear, "Click to remove all log files.");
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkShowCellView
            // 
            this.chkShowCellView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowCellView.AutoSize = true;
            this.chkShowCellView.Location = new System.Drawing.Point(12, 618);
            this.chkShowCellView.Name = "chkShowCellView";
            this.chkShowCellView.Size = new System.Drawing.Size(97, 17);
            this.chkShowCellView.TabIndex = 1000;
            this.chkShowCellView.Text = "Show cell view";
            this.toolTip.SetToolTip(this.chkShowCellView, "Show a view when a cell is selected, if checked.");
            this.chkShowCellView.UseVisualStyleBackColor = true;
            this.chkShowCellView.CheckedChanged += new System.EventHandler(this.chkShowCellView_CheckedChanged);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(12, 76);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dgv);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rtxt);
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new System.Drawing.Size(818, 536);
            this.splitContainer.SplitterDistance = 396;
            this.splitContainer.TabIndex = 37;
            // 
            // rtxt
            // 
            this.rtxt.BackColor = System.Drawing.Color.White;
            this.rtxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxt.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxt.Location = new System.Drawing.Point(0, 0);
            this.rtxt.Name = "rtxt";
            this.rtxt.ReadOnly = true;
            this.rtxt.Size = new System.Drawing.Size(150, 46);
            this.rtxt.TabIndex = 0;
            this.rtxt.Text = "";
            // 
            // FileLoggerPanel
            // 
            this.ClientSize = new System.Drawing.Size(842, 647);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.chkShowCellView);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.cboLog);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboLogLevel);
            this.Controls.Add(this.panel2);
            this.Name = "FileLoggerPanel";
            this.Text = "FileLoggerPanel";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboLogLevel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cboLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox chkShowCellView;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.RichTextBox rtxt;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmTimestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMember;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSourceFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmException;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmReadableWatsonBucketParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmJson;
    }
}