/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SizingServers.Log {
    /// <summary>
    /// <para>This is my take on an application logger for any 64 bit .Net 4.5 (and up) Windows desktop (maybe other app types, untested) app. (Yes, I know there is Log4Net, this was a fun little project, and well suited for my needs)</para>
    /// <para>It logs entries as JSON to a plain-text file in a folder 'logs' (one log file per day). You can deserialize a log file to an IEnumerable of Logger.Entry's.</para>
    /// <para></para>
    /// <para>It subscribes to AppDomain.CurrentDomain.UnhandledException, Meaning that unhandled (app breaking) exceptions should always get logged for the current application domain.</para>
    /// <para>Corrupted state exceptions are also taken care of. BUT only if this logger is set to used, using the Loggers.SetUseLogger function!</para>
    /// <para></para> 
    /// <para>Last but not least, read this: http://www.codeproject.com/Articles/9538/Exception-Handling-Best-Practices-in-NET. </para>
    /// </summary>
    public class FileLogger : Logger {

        #region Fields
        private static FileLogger _logger = new FileLogger();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the path to the log dir, this cannot be changed.
        /// </summary>
        public string LogDir { get { return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs"); } }
        /// <summary>
        /// Gets the path to the current log file: one log per day. This cannot be changed.
        /// </summary>
        public string CurrentLogFile { get { return Path.Combine(LogDir, "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt"); } }
        #endregion

        #region Constructor
        private FileLogger() { base.WriteLogEntry += FileLogger_WriteLogEntry; }
        #endregion

        #region Functions
        /// <summary></summary>
        public static FileLogger GetInstance() { return _logger; }
        private void FileLogger_WriteLogEntry(object sender, WriteLogEntryEventArgs e) {
            bool mutexCreated;
            var m = new Mutex(true, "SizingServers.Log.FileLogger", out mutexCreated); //Handle multiple processes writing to the same file.

            if (mutexCreated || m.WaitOne())
                try {
                    if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);

                    using (var sw = new StreamWriter(CurrentLogFile, true)) sw.WriteLine(e.JSON);
                } finally {
                    m.ReleaseMutex();
                }
        }

        /// <summary>
        /// <para>Yield returns entries if the path exists. This pauses the logging to this file, still queued entries to log will be processed when this is done.</para>
        /// <para>If the path does not exist no entries will return and no exception will be thrown.</para>
        /// <para>Invalid entries will be ignored.</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entry> DeserializeCurrentLogFile() {
            foreach (var entries in DeserializeLogFile(CurrentLogFile))
                yield return entries;
        }
        /// <summary>
        /// <para>Yield returns entries if the path exists. If the path does not exist no entries will return and no exception will be thrown.</para>
        /// <para>Invalid entries will be ignored.</para>
        /// </summary>
        /// <param name="path">Path to log file.</param>
        /// <returns></returns>
        public IEnumerable<Entry> DeserializeLogFile(string path) {
            if (File.Exists(path)) {
                if (path == CurrentLogFile) PauseProcessingWriteLogEntriesWorkQueue();

                bool mutexCreated;
                var m = new Mutex(true, "SizingServers.Log.FileLogger", out mutexCreated); //Handle multiple processes writing to the same file.

                if (mutexCreated || m.WaitOne())
                    try {

                        int bracketOpenCount = 0, bracketClosedCount = 0; //For multiline entries
                        var entryBuilder = new StringBuilder();

                        using (var sr = new StreamReader(path))
                            while (sr.Peek() != -1)
                                foreach (Entry entry in GetEntries(sr.ReadLine().Trim(), entryBuilder, ref bracketOpenCount, ref bracketClosedCount))
                                    yield return entry;

                    } finally {
                        m.ReleaseMutex();
                    }

                ContinueProcessingWriteLogEntriesWorkQueue();
            }
        }
        #endregion
    }
}
