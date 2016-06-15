/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Windows.Forms;

namespace SizingServers.Log.WinForms {
    /// <summary>
    /// A winforms panel for displaying file log entries. You can also set the desired log level.
    /// </summary>
    public partial class FileLoggerPanel : Panel {
        private Timer _directoryWatcher;
        private DateTime _directoryLastWatched = DateTime.MaxValue;

        private string _previousSelectedLog;
        private FileLogger _logger;
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private List<object[]> _log = new List<object[]>();
        private HashSet<string> _raw = new HashSet<string>();

        /// <summary>
        /// Return JSON for the selected log entries.
        /// </summary>
        public string[] SelectedLogEntries {
            get {
                var rows = new List<DataGridViewRow>();
                foreach (DataGridViewCell cell in dgv.SelectedCells) {
                    DataGridViewRow row = cell.OwningRow;
                    if (!rows.Contains(row)) rows.Add(row);
                }

                var selectedLogEntries = new string[rows.Count];
                for (int i = 0; i != selectedLogEntries.Length; i++)
                    selectedLogEntries[i] = rows[i].Cells[9].Value as string;

                return selectedLogEntries;
            }
        }

        /// <summary>
        /// A winforms panel for displaying file log entries. You can also set the desired log level.
        /// </summary>
        public FileLoggerPanel() {
            InitializeComponent();
            _logger = Loggers.GetLogger<FileLogger>();
            cboLogLevel.SelectedIndex = (int)_logger.CurrentLevel;

            FetchLogFiles();
            LoadLog(false);

            _directoryWatcher = new Timer() { Interval = 2000 };
            _directoryWatcher.Tick += _directoryWatcher_Tick;
            _directoryWatcher.Start();

            cboLog.SelectedIndexChanged += cboLog_SelectedIndexChanged;
            cboLogLevel.SelectedIndexChanged += cboLogLevel_SelectedIndexChanged;
        }

        private void _directoryWatcher_Tick(object sender, EventArgs e) {
            if (Directory.Exists(_logger.LogDir)) {
                FetchLogFiles();
                string item = cboLog.SelectedItem as string;
                if (item == null) item = string.Empty;
                string selectedLog = Path.Combine(_logger.LogDir, item);
                LoadLog(item == string.Empty || !File.Exists(selectedLog) || _previousSelectedLog != selectedLog || File.GetCreationTimeUtc(selectedLog) > _directoryLastWatched);
                _previousSelectedLog = selectedLog;
            }
            _directoryLastWatched = DateTime.UtcNow;
        }

        /// <summary>
        /// </summary>
        public void SelectNewestLog() {
            if (cboLog.Items.Count != 0 && cboLog.SelectedIndex != 0)
                cboLog.SelectedIndex = 0;
        }

        private void cboLog_SelectedIndexChanged(object sender, EventArgs e) { LoadLog(true); }
        private void btnClear_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to remove all the log files?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                if (Directory.Exists(_logger.LogDir)) {
                    string[] files = Directory.GetFiles(_logger.LogDir, "log*.txt", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                        try {
                            File.Delete(file);
                        } catch {
                            //Ignore if a log file fails to delete.
                        }
                }
                FetchLogFiles();
                LoadLog(true);
            }
        }
        private void cboLogLevel_SelectedIndexChanged(object sender, EventArgs e) {
            btnSet.Enabled = _logger.CurrentLevel != (Level)cboLogLevel.SelectedIndex;
            LoadLog(true);
        }
        private void dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            if (e.RowIndex < _log.Count) {
                object[] row = _log[e.RowIndex];
                if (e.ColumnIndex < row.Length)
                    e.Value = row[e.ColumnIndex];
            }
        }
        private void btnSet_Click(object sender, EventArgs e) {
            _logger.CurrentLevel = (Level)cboLogLevel.SelectedIndex;
        }

        private void FetchLogFiles() {
            var files = new List<string>();
            if (Directory.Exists(_logger.LogDir))
                foreach (string file in Directory.GetFiles(_logger.LogDir, "log*.txt", SearchOption.TopDirectoryOnly))
                    files.Add(Path.GetFileName(file));

            files.Sort(LogFileComparer.GetInstance());

            bool refresh = cboLog.Items.Count != files.Count;
            if (!refresh)
                for (int i = 0; i != files.Count; i++)
                    if (cboLog.Items[i] as string != files[i]) {
                        refresh = true;
                        break;
                    }

            if (refresh) {
                cboLog.SelectedIndexChanged -= cboLog_SelectedIndexChanged;

                string selectedItem = cboLog.SelectedItem as string;
                int selectedIndex = cboLog.SelectedIndex;

                cboLog.Items.Clear();
                cboLog.Items.AddRange(files.ToArray());

                if (selectedItem != null && files.Contains(selectedItem) && cboLog.Items.Contains(selectedItem))
                    cboLog.SelectedItem = selectedItem;
                else if (selectedIndex != -1 && selectedIndex < files.Count)
                    cboLog.SelectedIndex = selectedIndex;
                else if (files.Count != 0)
                    cboLog.SelectedIndex = 0;

                btnClear.Enabled = cboLog.Items.Count != 0;

                cboLog.SelectedIndexChanged += cboLog_SelectedIndexChanged;
            }
        }
        private void LoadLog(bool clear) {
            if (clear) {
                _log = new List<object[]>();
                _raw = new HashSet<string>();
                dgv.RowCount = 0;
            }
            if (cboLog.SelectedItem != null) {
                bool mutexCreated;
                var m = new System.Threading.Mutex(true, "SizingServers.Log.FileLogger", out mutexCreated); //Handle multiple processes writing to the same file.

                if (mutexCreated || m.WaitOne())
                    try {
                        string file = Path.Combine(_logger.LogDir, cboLog.SelectedItem as string);

                        if (File.Exists(file)) {
                            foreach (string raw in ReadEntriesReverse(file))
                                if (_raw.Contains(raw)) {
                                    break;
                                } else {
                                    _raw.Add(raw);
                                    Entry entry = JsonConvert.DeserializeObject<Entry>(raw, _jsonSerializerSettings);

                                    if ((int)entry.Level >= cboLogLevel.SelectedIndex) {
                                        var row = new object[10];
                                        row[0] = DateTimeOffset.Parse(entry.Timestamp).LocalDateTime.ToLongTimeString();
                                        row[1] = entry.Level;
                                        row[2] = entry.Description == null ? string.Empty : entry.Description;
                                        row[3] = entry.Member;
                                        row[4] = entry.SourceFile;
                                        row[5] = entry.Line;
                                        row[6] = entry.Parameters == null ? string.Empty : Combine(entry.Parameters);
                                        row[7] = entry.Exception == null ? string.Empty : entry.Exception.ToString();
                                        row[8] = entry.ReadableWatsonBucketParameters == null ? string.Empty : JsonConvert.SerializeObject(entry.ReadableWatsonBucketParameters, Formatting.None, _jsonSerializerSettings);
                                        row[9] = raw;

                                        _log.Add(row);
                                    }
                                }
                        }
                    } catch {
                        //Ignore
                        //(Exception ex) {
                        //_log = new List<object[]>();
                        //var row = new object[10];
                        //row[0] = string.Empty;
                        //row[1] = string.Empty;
                        //row[2] = "Failed loading the selected log file.";
                        //row[3] = string.Empty;
                        //row[4] = string.Empty;
                        //row[5] = string.Empty;
                        //row[6] = string.Empty;
                        //row[7] = ex.ToString();
                        //row[8] = string.Empty;
                        //row[9] = string.Empty;

                        //_log.Add(row);
                    } finally {
                        m.ReleaseMutex();
                    }
            }
            dgv.RowCount = _log.Count;

            FillCellView();
        }
        private IEnumerable<string> ReadEntriesReverse(string file) {
            //Overkill, I know, but I wanted to try something else.
            using (var mmf = MemoryMappedFile.CreateFromFile(file, FileMode.Open)) {
                using (var stream = mmf.CreateViewStream()) {
                    stream.Seek(0, SeekOrigin.End);

                    while (true) {
                        string entry = ReadPreviousEntry(stream);
                        if (entry == null) break;
                        yield return entry;
                    }

                }
            }
        }
        private string ReadPreviousEntry(Stream stream) {
            if (stream.Position == 0L) return null;

            int bracketClosedCount = 0, bracketOpenCount = 0;
            var chars = new List<char>();
            while (bracketClosedCount == 0 || bracketClosedCount != bracketOpenCount) {
                long position = stream.Seek(-1, SeekOrigin.Current);
                byte b = (byte)stream.ReadByte();
                stream.Position = position;

                if (b == '}') ++bracketClosedCount;
                else if (b == '{') ++bracketOpenCount;

                if (bracketClosedCount != 0)
                    chars.Insert(0, Convert.ToChar(b));

                if (position == 0L) break;
            }

            return new string(chars.ToArray());
        }
        private string Combine(string[] arr) {
            if (arr.Length != 0) {
                var sb = new StringBuilder();
                for (int i = 0; i != arr.Length - 1; i++) {
                    sb.Append(arr[i]);
                    sb.Append(", ");
                }
                sb.Append(arr[arr.Length - 1]);
                return sb.ToString();
            }
            return string.Empty;
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            FillCellView();
        }

        private void FillCellView() {
            if (dgv.SelectedCells.Count == 1) {
                dgv.CurrentCell = dgv.SelectedCells[0];
                rtxt.Text = dgv.CurrentCell.Value.ToString();
            } else {
                rtxt.Text = string.Empty;
            }
        }

        private void chkShowCellView_CheckedChanged(object sender, EventArgs e) {
            splitContainer.Panel2Collapsed = !chkShowCellView.Checked;
        }

        /// <summary>
        /// Refresh the child controls, because they are not drawn well (flat-style) when resized.
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnResize(EventArgs eventargs) {
            base.OnResize(eventargs);
            Refresh();
        }
        /// <summary>
        /// Z-A comparer
        /// </summary>
        private class LogFileComparer : IComparer<string> {
            private static LogFileComparer _logFileComparer;

            private LogFileComparer() { }

            /// <summary>
            /// Z-A comparer
            /// </summary>
            /// <returns></returns>
            public static LogFileComparer GetInstance() {
                if (_logFileComparer == null)
                    _logFileComparer = new LogFileComparer();
                return _logFileComparer;
            }

            public int Compare(string x, string y) {
                return ExtractDateTime(y).CompareTo(ExtractDateTime(x));
            }

            private DateTime ExtractDateTime(string s) {
                s = s.Substring(3, s.Length - 7);
                DateTime dt = new DateTime();
                int i = 0;
                if (int.TryParse(s.Substring(0, 4), out i)) {
                    dt = dt.AddYears(i);

                    if (int.TryParse(s.Substring(4, 2), out i)) {
                        dt = dt.AddMonths(i);

                        if (int.TryParse(s.Substring(6, 2), out i))
                            dt = dt.AddDays(i);
                        else
                            dt = new DateTime();

                    } else {
                        dt = new DateTime();
                    }
                }
                return dt;
            }
        }
    }
}
