namespace SizingServers.Log.Examples {
    partial class Examples {
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnFileLogger = new System.Windows.Forms.Button();
            this.btnMailLogger = new System.Windows.Forms.Button();
            this.btnFileLoggerPanel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.btnFileLogger);
            this.flowLayoutPanel1.Controls.Add(this.btnFileLoggerPanel);
            this.flowLayoutPanel1.Controls.Add(this.btnMailLogger);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(784, 562);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnFileLogger
            // 
            this.btnFileLogger.AutoSize = true;
            this.btnFileLogger.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFileLogger.BackColor = System.Drawing.Color.White;
            this.btnFileLogger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFileLogger.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileLogger.Location = new System.Drawing.Point(6, 6);
            this.btnFileLogger.Name = "btnFileLogger";
            this.btnFileLogger.Size = new System.Drawing.Size(78, 25);
            this.btnFileLogger.TabIndex = 0;
            this.btnFileLogger.Text = "FileLogger";
            this.btnFileLogger.UseVisualStyleBackColor = false;
            this.btnFileLogger.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnMailLogger
            // 
            this.btnMailLogger.AutoSize = true;
            this.btnMailLogger.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMailLogger.BackColor = System.Drawing.Color.White;
            this.btnMailLogger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMailLogger.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMailLogger.Location = new System.Drawing.Point(206, 6);
            this.btnMailLogger.Name = "btnMailLogger";
            this.btnMailLogger.Size = new System.Drawing.Size(81, 25);
            this.btnMailLogger.TabIndex = 2;
            this.btnMailLogger.Text = "MailLogger";
            this.btnMailLogger.UseVisualStyleBackColor = false;
            this.btnMailLogger.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnFileLoggerPanel
            // 
            this.btnFileLoggerPanel.AutoSize = true;
            this.btnFileLoggerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFileLoggerPanel.BackColor = System.Drawing.Color.White;
            this.btnFileLoggerPanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFileLoggerPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileLoggerPanel.Location = new System.Drawing.Point(90, 6);
            this.btnFileLoggerPanel.Name = "btnFileLoggerPanel";
            this.btnFileLoggerPanel.Size = new System.Drawing.Size(110, 25);
            this.btnFileLoggerPanel.TabIndex = 1;
            this.btnFileLoggerPanel.Text = "FileLoggerPanel";
            this.btnFileLoggerPanel.UseVisualStyleBackColor = false;
            this.btnFileLoggerPanel.Click += new System.EventHandler(this.btn_Click);
            // 
            // Examples
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Examples";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SizingServers.Log Examples";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnFileLogger;
        private System.Windows.Forms.Button btnMailLogger;
        private System.Windows.Forms.Button btnFileLoggerPanel;
    }
}