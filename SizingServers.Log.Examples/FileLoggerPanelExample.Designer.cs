namespace SizingServers.Log.Examples {
    partial class FileLoggerPanelExample {
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
            this.fileLoggerPanel1 = new WinForms.FileLoggerPanel();
            this.SuspendLayout();
            // 
            // fileLoggerPanel1
            // 
            this.fileLoggerPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileLoggerPanel1.Location = new System.Drawing.Point(0, 0);
            this.fileLoggerPanel1.Name = "fileLoggerPanel1";
            this.fileLoggerPanel1.Size = new System.Drawing.Size(784, 561);
            this.fileLoggerPanel1.TabIndex = 0;
            this.fileLoggerPanel1.Text = "FileLoggerPanel";
            // 
            // FileLoggerPanelExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.fileLoggerPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "FileLoggerPanelExample";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FileLoggerPanel Example";
            this.ResumeLayout(false);

        }

        #endregion

        private WinForms.FileLoggerPanel fileLoggerPanel1;


    }
}

