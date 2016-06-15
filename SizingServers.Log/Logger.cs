/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using Newtonsoft.Json;
using SizingServers.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SizingServers.Log {
    /// <summary>
    /// <para>This is my take on an application logger for any 64 bit .Net 4.5 (and up) Windows desktop (maybe other app types, untested) app. (Yes, I know there is Log4Net, this was a fun little project, and well suited for my needs)</para>
    /// <para></para>
    /// <para>It subscribes to AppDomain.CurrentDomain.UnhandledException, Meaning that unhandled (app breaking) exceptions should always get logged for the current application domain.</para>
    /// <para>Corrupted state exceptions are also taken care of. BUT only if this logger is set to used, using the Loggers.SetUseLogger function!</para>
    /// <para></para>
    /// <para>Last but not least, read this: http://www.codeproject.com/Articles/9538/Exception-Handling-Best-Practices-in-NET. </para>
    /// <para>HResult: http://en.wikipedia.org/wiki/HRESULT </para>
    /// </summary>
    public abstract class Logger {

        #region Events
        /// <summary>
        /// <para>You must suscribe to this in your own logger, done like this because all boilerplate code is done here.</para>
        /// <para>If I used an abstract function you had to do this yourself.</para>
        /// <para>In the handler you do the actual writing of the entry (to file, to mail, to database, ...).</para>
        /// </summary>
        protected event EventHandler<WriteLogEntryEventArgs> WriteLogEntry;
        /// <summary>
        /// <para>The logger handles log write exceptions by always writing them to the debug console.</para>
        /// <para>This is done like this to not break the program when an error occures in a 'Log Write' member, obviously.</para>
        /// <para>Subscribe to this event in the constructor of the upper class to get a notification of such an exception. (This is highly encouraged!)</para>
        /// <para>The exceptions occuring in the 'Read' functions will be thrown normally. Since you will do something with the read data, you can as well handle the exception directly.</para>
        /// <para>If you want to update the GUI in the handler, do not forget to synchronize to the main thread by using for example SizingServers.Util.SynchronizationContextWrapper.</para>
        /// </summary>
        public event EventHandler<ErrorEventArgs> LogWriteException;
        /// <summary>
        /// <para>Suscribe to this to for instance list the log entries on the GUI.</para>
        /// <para>Do not forget to synchronize to the main thread by using for example SizingServers.Util.SynchronizationContextWrapper.</para>
        /// </summary>
        public event EventHandler<WriteLogEntryEventArgs> LogEntryWritten;
        #endregion

        #region Fields
        private BackgroundWorkQueue _backgroundWorkQueue = new BackgroundWorkQueue();
        //The work for the queue.
        private Action<Level, string, Exception, object[], string, string, int, bool, ReadableWatsonBucketParameters> _logEntryWriteCallback;

        //We want a compact output.
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        #endregion

        #region Properties
        /// <summary>
        /// <para>Log only entries with the given and higher levels. For instance: CurrentLevel Warning will not log entries with Level Info.</para>
        /// <para>But will log everything above that.</para>
        /// <para>This is handy to be able to set the level in the gui of the app without having to change the code.</para>
        /// </summary>
        public Level CurrentLevel { get; set; }
        /// <summary>
        /// <para>When a entry is written it can also be logged to the Visual Studio debug console when this property is set to true. (category: LOGGER)</para>
        /// <para>When the logger cannot write an entry after 3 tries, the log exception will be witten to the Visual Studio debug console.</para>
        /// </summary>
        public bool LogToDebug { get; set; }
        /// <summary>
        /// <para>If this is set to true, the 'CurrentLevel' equals Level.Info and this logger is set to used, using the Loggers.SetUseLogger function, first chance exceptions (also handled exceptions in and by the .Net framework) will be logged.</para>
        /// <para>This is handy to find the source of strange behaviour but should typically not be used in production (way to much logging for exceptions that are handled anyway).</para>
        /// </summary>
        public bool LogFirstChanceExceptions { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// <para>This is my take on an application logger for any 64 bit .Net 4.5 (and up) Windows desktop (maybe other app types, untested) app. (Yes, I know there is Log4Net, this was a fun little project, and well suited for my needs)</para>
        /// <para></para>
        /// <para>It subscribes to AppDomain.CurrentDomain.UnhandledException, Meaning that unhandled (app breaking) exceptions should always get logged for the current application domain.</para>
        /// <para>Corrupted state exceptions are also taken care of. BUT only if this logger is set to used, using the Loggers.SetUseLogger function!</para>
        /// <para></para>
        /// <para>Last but not least, read this: http://www.codeproject.com/Articles/9538/Exception-Handling-Best-Practices-in-NET. </para>
        /// </summary>
        public Logger() {
            CurrentLevel = Level.Warning;
            LogToDebug = true;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            _logEntryWriteCallback = LogEntryWriteCallback;
        }

        #endregion

        #region Write
        //Must be security critical instead of security transparant to be able to handle corrupted state events.
        //I think I got it right http://msdn.microsoft.com/en-us/library/system.appdomain.unhandledexception.aspx.
        [SecurityCritical, HandleProcessCorruptedStateExceptions]
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (!Loggers.IsUsed(this)) return;

            //Can be safely cast to Exception if RuntimeCompatibilityAttribute(WrapNonException=true) has been applied to your assembly,
            //which is automatically done for you since .net 2.0. 'Non exceptions' will be wrapped in RuntimeWrappedException.
            var ex = e.ExceptionObject as Exception;
            var frame = GetApplicableFrame(ex);
            string sourceFile = frame.GetFileName();

            string description = "An unhandled exception occured and killed the application!";
            if (sourceFile == null) description += " SourceFile and Line are not available because the application does not have (up-to-date) .pdb files included in the folder holding the binaries where the exception occured.";

            //Do not notify in write exceptions, they will not be handled anyways.
            LogWriteException = null;
            //_backgroundWorkQueue is not used because its background thread gets disposed if the application halts. However, we dispose it first so there is absolutely no chance that LogEntryWriteCallback is called by 2 threads at the same time.
            _backgroundWorkQueue.Dispose();

            //Create a readable watson buckets. (_watsonBuckets holds a byte array.)
            //Watson buckets are somewhat handy for when there is no stacktrace available, or you cannot trace into third-party assemblies.
            //Only applicable with unhandled exceptions.
            //I did not simply replace the value for _watsonBuckets in the exception, because the Windows Error Report tool would not work: 
            //You can get the path to the minidump there. The minidump can be analyzed using a tool like WinDbg.
            //Downside: The output contains a serialized byte array.
            //http://msdn.microsoft.com/en-us/library/ms404466(v=VS.110).aspx
            //http://mrpfister.com/programming/demystifying-clr20r3-error-messages/
            //http://mrpfister.com/programming/opcodes-msil-and-reflecting-net-code/
            ReadableWatsonBucketParameters readableWatsonBucketParameters = null;
            FieldInfo watsonBucketsField = ex.GetType().GetField("_watsonBuckets", BindingFlags.Instance | BindingFlags.NonPublic);
            if (watsonBucketsField != null) {
                byte[] arr = watsonBucketsField.GetValue(ex) as byte[];
                if (arr != null) {
                    var str = MarshalBytesToStruct<BucketParameters>(arr);
                    readableWatsonBucketParameters = new ReadableWatsonBucketParameters(str);
                }
            }

            LogEntryWriteCallback(Level.Fatal, description, ex, new object[] { sender }, frame.GetMethod().Name, sourceFile, frame.GetFileLineNumber(), LogToDebug, readableWatsonBucketParameters);
        }
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        private T MarshalBytesToStruct<T>(byte[] arr) where T : struct {
            T str = Activator.CreateInstance<T>();
            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);
            str = (T)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);
            return str;
        }
        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e) {
            if (!Loggers.IsUsed(this)) return;

            if (LogFirstChanceExceptions && CurrentLevel == Level.Info) {
                var frame = GetApplicableFrame(e.Exception);
                string sourceFile = frame.GetFileName();

                string description = "First chance exception.";
                if (sourceFile == null) description += " SourceFile and Line are not available because the application does not have (up-to-date) .pdb files included in the folder holding the binaries where the exception occured.";

                _backgroundWorkQueue.EnqueueWorkItem(_logEntryWriteCallback, Level.Info, description, e.Exception, new object[] { sender }, frame.GetMethod().Name, sourceFile, frame.GetFileLineNumber(), false, null);
            }
        }
        /// <summary>
        /// Gets the frame that has debugging info or the first (last happened).
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private StackFrame GetApplicableFrame(Exception ex) {
            var trace = new StackTrace(ex, true);
            foreach (var candidate in trace.GetFrames())
                if (candidate.GetFileLineNumber() != 0) return candidate;
            return trace.GetFrame(0); ;
        }

        /// <summary>
        /// <para>Pauses the processing of the work queue and blocks until the processing is effectively paused.</para>
        /// <para>Continue this processing calling 'ContinueProcessingWorkQueue()'.</para>
        /// <para>Handy for instance when you want to read from a file that is being written to.</para>
        /// </summary>
        protected void PauseProcessingWriteLogEntriesWorkQueue() { if (_backgroundWorkQueue != null) _backgroundWorkQueue.PauseProcessingWorkQueue(); }
        /// <summary>
        /// Resumes the processing of the work queue.
        /// </summary>
        protected void ContinueProcessingWriteLogEntriesWorkQueue() { if (_backgroundWorkQueue != null) _backgroundWorkQueue.ContinueProcessingWorkQueue(); }

        /// <summary>
        /// <para>Logs using Level.Info. Use another 'Log' function to log exceptions.</para>
        /// <para>When the logger cannot write an entry after 3 tries, the log exception will always be witten to the Visual Studio debug console. (category: LOGGER)</para>
        /// </summary>
        /// <param name="description">A free to choose message.</param>
        /// <param name="member">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="sourceFile">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="line">Do not fill this in, this will be done automatically at runtime.</param>
        public void Log(string description, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
            Log(Level.Info, description, null, null, member, sourceFile, line);
        }
        /// <summary>
        /// <para>Logs using Level.Info. Use another 'Log' function to log exceptions.</para>
        /// <para>When the logger cannot write an entry after 3 tries, the log exception will always be witten to the Visual Studio debug console. (category: LOGGER)</para>
        /// </summary>
        /// <param name="description">A free to choose message.</param>
        /// <param name="parameters">The parameters used in the calling member. This can be null.</param>
        /// <param name="member">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="sourceFile">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="line">Do not fill this in, this will be done automatically at runtime.</param>
        public void Log(string description, object[] parameters, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
            Log(Level.Info, description, null, parameters, member, sourceFile, line);
        }

        /// <summary>
        /// When the logger cannot write an entry after 3 tries, the log exception will always be witten to the Visual Studio debug console. (category: LOGGER)
        /// </summary>
        /// <param name="level"></param>
        /// <param name="description">A free to choose message.</param>
        /// <param name="member">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="sourceFile">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="line">Do not fill this in, this will be done automatically at runtime.</param>
        public void Log(Level level, string description, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
            Log(level, description, null, null, member, sourceFile, line);
        }
        /// <summary>
        /// When the logger cannot write an entry after 3 tries, the log exception will always be witten to the Visual Studio debug console. (category: LOGGER)
        /// </summary>
        /// <param name="level"></param>
        /// <param name="description">A free to choose message.</param>
        /// <param name="exception">Should only be given for a Logger.Lever higher than Info.</param>
        /// <param name="member">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="sourceFile">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="line">Do not fill this in, this will be done automatically at runtime.</param>
        public void Log(Level level, string description, Exception exception, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
            Log(level, description, exception, null, member, sourceFile, line);
        }
        /// <summary>
        /// When the logger cannot write an entry after 3 tries, the log exception will always be witten to the Visual Studio debug console. (category: LOGGER)
        /// </summary>
        /// <param name="level"></param>
        /// <param name="description">A free to choose message.</param>
        /// <param name="exception">Should only be given for a Logger.Lever higher than Info.</param>
        /// <param name="parameters">The parameters used in the calling member. This can be null.</param>
        /// <param name="member">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="sourceFile">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="line">Do not fill this in, this will be done automatically at runtime.</param>
        public void Log(Level level, string description, Exception exception, object[] parameters, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
            if (level >= CurrentLevel)
                _backgroundWorkQueue.EnqueueWorkItem(_logEntryWriteCallback, level, description, exception, parameters, member, sourceFile, line, LogToDebug, null);
        }

        private void LogEntryWriteCallback(Level level, string description, Exception exception, object[] parameters, string member, string sourceFile, int line, bool logToDebug, ReadableWatsonBucketParameters readableWatsonBucketParameters) {
            if (level < CurrentLevel) return;

            //We are working around the problem that stuff closely linked to the gui /unknown stuff in another program cannot be serialized or deserialized.
            string[] parameterStrings = null;
            if (parameters != null) {
                parameterStrings = new string[parameters.Length];
                for (int i = 0; i != parameters.Length; i++) {
                    object parameter = parameters[i];
                    if (parameter != null)
                        parameterStrings[i] = parameter.ToString();
                }
            }

            var entry = new Entry { Timestamp = DateTimeOffset.Now.ToString("o"), Level = level, Description = description, Exception = exception, ReadableWatsonBucketParameters = readableWatsonBucketParameters, Parameters = parameterStrings, Member = member, SourceFile = sourceFile, Line = line };
            string json = JsonConvert.SerializeObject(entry, Formatting.None, _jsonSerializerSettings);

            if (logToDebug) Debug.WriteLine(json, "LOGGER");

            InvokeWriteLogEntry(entry, json); // Also handles write exceptions and invokes LogEntryWritten.
        }

        /// <summary>
        /// <para>There are max 3 retries.</para>
        /// <para>You should not do your own error handling in the upper class. This is done for you. If you want to do that anyway, throw the exception, so the flow is respected.</para>
        /// <para>You should suscribe to LogWriteException.</para>
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="json"></param>
        private void InvokeWriteLogEntry(Entry entry, string json) {
            Exception logException = null;

            if (WriteLogEntry == null)
                logException = new Exception("You must suscribe to WriteLogEntry in the upper class!\nThere you do the actual writing of the entry (to file, to mail, to database, ...).");
            else
                for (int tryCount = 0; tryCount != 3; tryCount++)
                    try {
                        var ea = new WriteLogEntryEventArgs(entry, json);
                        WriteLogEntry.Invoke(this, ea);

                        if (LogEntryWritten != null) LogEntryWritten(this, ea);
                        return;
                    } catch (Exception ex) {
                        logException = ex;
                        Thread.Sleep(100); // Some write stuff (like to file) possibly needs a bit of a timout before trying again. 
                    }

            //Logs the exception to Debug, regardless the settings. Invokes LogWriteException if it has handlers in its invocationlist.
            InvokeLogWriteException(LogWriteExceptionToDebug(json, logException, null));
        }
        /// <summary></summary>
        /// <param name="json"></param>
        /// <param name="exception"></param>
        /// <param name="parameters">The parameters used in the calling member. This can be null.</param>
        /// <param name="member">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="sourceFile">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="line">Do not fill this in, this will be done automatically at runtime.</param>
        /// <returns>A new entry encapsulating the given json.</returns>
        private string LogWriteExceptionToDebug(string json, Exception exception, string[] parameters, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
            var entry = new Entry { Timestamp = DateTimeOffset.Now.ToString("o"), Level = Level.Fatal, Description = "Failed writing log entry: " + json, Exception = exception, Parameters = parameters, Member = member, SourceFile = sourceFile, Line = line };
            json = JsonConvert.SerializeObject(entry, Formatting.None, _jsonSerializerSettings);
            Debug.WriteLine(json, "LOGGER");
            return json;
        }
        private void InvokeLogWriteException(string json) {
            if (LogWriteException != null) {
                var invocationList = LogWriteException.GetInvocationList();
                var eventArgs = new ErrorEventArgs(new Exception(json));
                Parallel.For(0, invocationList.Length, (i) => (invocationList[i] as EventHandler<ErrorEventArgs>).Invoke(this, eventArgs));
            }
        }

        /// <summary>
        /// Wait untill all log entries are written. Do this before the application exits (if need be).
        /// </summary>
        public void Flush() {
            _backgroundWorkQueue.Flush();
        }
        #endregion

        #region Read
        /// <summary>
        /// <para>Yield returns entries. If log == null an exception will be thrown.</para>
        /// <para>Invalid entries will be ignored.</para>
        /// </summary>
        /// <param name="log">A string containing one or more log entries.</param>
        /// <returns></returns>
        public static IEnumerable<Entry> DeserializeLog(string log) {
            if (log == null) throw new NullReferenceException("log");

            int bracketOpenCount = 0, bracketClosedCount = 0; //For multiline entries
            var entryBuilder = new StringBuilder();

            foreach (Entry entry in GetEntries(log, entryBuilder, ref bracketOpenCount, ref bracketClosedCount))
                yield return entry;
        }
        /// <summary>
        /// <para>Build one or more entries and clears the StringBuilder if appropriate or appends to the StringBuilder if the entry is not complete.</para>
        /// <para>Entries can be spread over multiple lines, so the stringbuilder can contain partial entries.</para>
        /// <para>Faulty data is ignored.</para>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="entryBuilder"></param>
        /// <param name="bracketOpenCount"></param>
        /// <param name="bracketClosedCount"></param>
        /// <returns></returns>
        protected static List<Entry> GetEntries(string text, StringBuilder entryBuilder, ref int bracketOpenCount, ref int bracketClosedCount) {
            var entries = new List<Entry>();
            foreach (string line in text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                for (int i = 0; i != line.Length; i++) {
                    char c = line[i];
                    if (c == '{') ++bracketOpenCount; else if (c == '}') ++bracketClosedCount;

                    // Ignore gibberish
                    if (bracketOpenCount != 0) entryBuilder.Append(c);

                    if (bracketOpenCount == bracketClosedCount) {
                        //Ignore invalid entries
                        try { entries.Add(JsonConvert.DeserializeObject<Entry>(entryBuilder.ToString(), _jsonSerializerSettings)); } catch { }
                        bracketOpenCount = bracketClosedCount = 0;
                        entryBuilder.Clear();
                    }
                }
            return entries;
        }
        #endregion

    }
}
