/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SizingServers.Log {
    /// <summary>
    /// <para>This is my take on an application logger for any 64 bit .Net 4.5 (and up) Windows desktop (maybe other app types, untested) app. (Yes, I know there is Log4Net, this was a fun little project, and well suited for my needs)</para>
    /// <para>It logs entries as JSON to mail using a SMTP server and the SMTP delivery network method.</para>
    /// <para>Read mail functionality is not provided. I do not think that is a handy feature.</para>
    /// <para></para>
    /// <para>It subscribes to AppDomain.CurrentDomain.UnhandledException, Meaning that unhandled (app breaking) exceptions should always get logged for the current application domain.</para>
    /// <para>Corrupted state exceptions are also taken care of. BUT only if this logger is set to used, using the Loggers.SetUseLogger function!</para>
    /// <para></para>
    /// <para>Last but not least, read this: http://www.codeproject.com/Articles/9538/Exception-Handling-Best-Practices-in-NET. </para>
    /// </summary>
    public class MailLogger : Logger {

        #region Fields
        private static MailLogger _logger = new MailLogger();
        private int _mailSendTimeout = 10000;
        private string _subjectPrefix = "Logger";
        #endregion

        #region Properties
        /// <summary>
        /// For instance: smtp.gmail.com
        /// </summary>
        public string SMTPServer { get; set; }
        /// <summary>
        /// For instance: 587 for gmail.
        /// </summary>
        public int SMTPServerPort { get; set; }
        /// <summary>
        /// For instance: True for gmail.
        /// </summary>
        public bool UseTLS_SSL { get; set; }

        /// <summary>
        /// The timeout in milliseconds, the default is 10 000.
        /// </summary>
        public int MailSendTimeout {
            get { return _mailSendTimeout; }
            set {
                if (_mailSendTimeout < 1)
                    throw new ArgumentOutOfRangeException("Timout");
                _mailSendTimeout = value;
            }
        }

        /// <summary>
        /// In doubt, leave blank, FromEMailAddress will be used.
        /// </summary>
        public string Username { get; set; }
        /// <summary></summary>
        public string Password { get; set; }

        /// <summary>
        /// Can be any e-mail address, ToEMailAddress is the important one.
        /// </summary>
        public string FromEMailAddress { get; set; }
        /// <summary></summary>
        public string ToEMailAddress { get; set; }

        /// <summary>
        /// <para>Default: "Logger"</para>
        /// <para>Example: "My fancy feature generating application!"</para>
        /// <para>" level " + level is added.</para>
        /// </summary>
        public string SubjectPrefix { get { return _subjectPrefix; } set { _subjectPrefix = value; } }
        #endregion

        #region Constructor
        private MailLogger() { base.WriteLogEntry += MailLogger_WriteLogEntry; }
        #endregion

        #region Functions
        /// <summary></summary>
        public static MailLogger GetInstance() { return _logger; }
        private void MailLogger_WriteLogEntry(object sender, WriteLogEntryEventArgs e) {
            if (string.IsNullOrWhiteSpace(SMTPServer)) throw new ArgumentException("Host must be filled in.");
            if (string.IsNullOrWhiteSpace(Password)) throw new ArgumentException("Password must be filled in.");
            if (string.IsNullOrWhiteSpace(FromEMailAddress)) throw new ArgumentException("FromEMailAddress must be filled in.");
            if (string.IsNullOrWhiteSpace(ToEMailAddress)) throw new ArgumentException("ToEMailAddress must be filled in.");

            var client = SMTPServerPort < 1 ? new SmtpClient(SMTPServer) : new SmtpClient(SMTPServer, SMTPServerPort);
            client.EnableSsl = UseTLS_SSL;
            client.Timeout = _mailSendTimeout;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            string username = Username;
            if (!string.IsNullOrWhiteSpace(username)) username = FromEMailAddress;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(Password)) {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, Password);
            }

            var msg = new MailMessage(ToEMailAddress, FromEMailAddress, _subjectPrefix + " level " + e.Entry.Level, e.JSON);
            msg.SubjectEncoding = msg.BodyEncoding = UTF8Encoding.UTF8;
            msg.IsBodyHtml = true;
            //Set the priority depending on the level.
            msg.Priority = e.Entry.Level > Level.Info ? (e.Entry.Level > Level.Warning ? MailPriority.High : MailPriority.Normal) : MailPriority.Low;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(msg);
        }
        #endregion
    }
}
