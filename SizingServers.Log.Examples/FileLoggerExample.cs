/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using SizingServers.Util;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SizingServers.Log.Examples {
    public partial class FileLoggerExample : Form {
        public FileLoggerExample() {
            InitializeComponent();
            Loggers.GetLogger<FileLogger>().LogEntryWritten += FileLoggerExample_LogEntryWritten;
            this.HandleCreated += FileLoggerExample_HandleCreated;

            //See Program.cs for an example of flushing the logger on application exit.
        }

        private void FileLoggerExample_HandleCreated(object sender, EventArgs e) {
            this.HandleCreated -= FileLoggerExample_HandleCreated;
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
        }

        private void FileLoggerExample_LogEntryWritten(object sender, WriteLogEntryEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(o => {
                linkLabel1.Visible = true;
                linkLabel1.Text = Loggers.GetLogger<FileLogger>().CurrentLogFile;
            }, null);
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                throw new Exception("Whoopsie");
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Caught whoopsie", ex, new object[] { sender, e, null });
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            throw new Exception("Oh noze!");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(linkLabel1.Text);
        }
    }
}
