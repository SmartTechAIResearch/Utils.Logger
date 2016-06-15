/*
Copyright (C) 2014 Dieter Vandroemme
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using SizingServers.Util;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SizingServers.Log.Examples {
    public partial class MailLoggerExample : Form {
        public MailLoggerExample() {
            InitializeComponent();
            var logger = Loggers.GetLogger<MailLogger>();
            Loggers.SetUseLogger(logger, true);
            Loggers.SetUseLogger(Loggers.GetLogger<FileLogger>(), false);
            logger.LogEntryWritten += logger_LogEntryWritten;
            logger.LogWriteException += logger_LogWriteException;
            this.HandleCreated += MailLoggerExample_HandleCreated;

            //See Program.cs for an example of flushing the logger on application exit.
        }

        private void MailLoggerExample_HandleCreated(object sender, EventArgs e) {
            this.HandleCreated -= MailLoggerExample_HandleCreated;
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
        }

        private void logger_LogEntryWritten(object sender, WriteLogEntryEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(o => {
                richTextBox1.Text = "You've got mail.";
            }, null);
        }
        private void logger_LogWriteException(object sender, System.IO.ErrorEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(o => {
                richTextBox1.Text = "Failed to log. " + e.GetException();
            }, null);
        }
        private void button1_Click(object sender, EventArgs e) {
            SetMailLogger();
            try {
                throw new Exception("Whoopsie");
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Caught whoopsie", ex, new object[] { sender, e });
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            SetMailLogger();
            throw new Exception("Oh noze!");
        }
        private void SetMailLogger() {
            var logger = Loggers.GetLogger<MailLogger>();
            logger.SMTPServer = txtSMTPServer.Text;
            logger.SMTPServerPort = (int)nudSMTPServerPort.Value;
            logger.UseTLS_SSL = chkTLS_SSL.Checked;
            logger.Username = txtUsername.Text;
            logger.Password = txtPassword.Text;
            logger.FromEMailAddress = txtFrom.Text;
            logger.ToEMailAddress = txtTo.Text;
        }
    }
}
