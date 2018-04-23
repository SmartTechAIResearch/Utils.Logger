/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace SizingServers.Log {
    /// <summary> 
    /// <para>This is my take on an application logger for any 64 bit .Net 4.5 (and up) Windows desktop (maybe other app types, untested) app. (Yes, I know there is Log4Net, this was a fun little project, and well suited for my needs)</para>
    /// <para></para>
    /// <para>All the available loggers are kept here.</para>
    /// <para></para>
    /// <para>They subscribes to AppDomain.CurrentDomain.UnhandledException, Meaning that unhandled (app breaking) exceptions should always get logged for the current application domain.</para>
    /// <para>Corrupted state exceptions are also taken care of.</para>
    /// <para></para>
    /// <para>Last but not least, read this: http://www.codeproject.com/Articles/9538/Exception-Handling-Best-Practices-in-NET. </para>
    /// <para></para>
    /// <para>You can choose which loggers you want to use when logging (default only FileLogger and SimpleLogger), using the SetUseLogger function.</para>
    /// <para>For all 'used' loggers the Log function is called when calling a Log function here. (in parallel)</para>
    /// <para>You can if you like use a logger directly to write, but via here is the preferred method.</para>
    /// <para></para>
    /// <para>For read actions and setting properties, get the logger you want using the GetLogger function.</para>
    /// </summary>
    public static class Loggers {

        #region Fields
        private static Dictionary<Logger, bool> _availableLoggers = new Dictionary<Logger, bool>(); // value = used or not for write actions.
        #endregion

        #region Properties
        /// <summary>
        /// Yield returns all available loggers.
        /// </summary>
        public static IEnumerable<Logger> AvailableLoggers {
            get { foreach (var logger in _availableLoggers.Keys) yield return logger; }
        }

        /// <summary>
        /// Yield returns all used loggers for write actions.
        /// </summary>
        public static IEnumerable<Logger> UsedLoggers {
            get {
                foreach (var logger in _availableLoggers.Keys)
                    if (_availableLoggers[logger]) yield return logger;
            }
        }
        #endregion

        #region Constructor
        static Loggers() {
            _availableLoggers.Add(FileLogger.GetInstance(), true);
            _availableLoggers.Add(MailLogger.GetInstance(), false);
            _availableLoggers.Add(SimpleLogger.GetInstance(), true);
        }
        #endregion

        #region Function
        /// <summary>
        /// <para>Specify the loggers you want to use.</para>
        /// <para>At least one logger should be used!</para>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="use"></param>
        public static void SetUseLogger(Logger logger, bool use) {
            _availableLoggers[logger] = use;
            int used = 0;
            foreach (var usedLogger in UsedLoggers) ++used;
            if (used == 0) throw new Exception("At least one logger should be used!");
        }

        /// <summary>
        /// <para>Get a specific logger. To write, use the functions here and set the loggers you want to use (SetUseLogger).</para>
        /// <para>To read, use the functions specified in the logger.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The logger or null if not found.</returns>
        public static T GetLogger<T>() where T : Logger {
            var type = typeof(T);
            foreach (var logger in AvailableLoggers)
                if (logger.GetType() == type) return (T)logger;
            return null;
        }

        /// <summary></summary>
        public static bool IsUsed(Logger logger) {
            foreach (var candidate in UsedLoggers)
                if (candidate == logger) return true;
            return false;
        }

        /// <summary>
        /// <para>Logs using Level.Info. Use another 'Log' function to log exceptions.</para>
        /// <para>When the logger cannot write an entry after 3 tries, the log exception will always be witten to the Visual Studio debug console. (category: LOGGER)</para>
        /// </summary>
        /// <param name="description">A free to choose message.</param>
        /// <param name="member">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="sourceFile">Do not fill this in, this will be done automatically at runtime.</param>
        /// <param name="line">Do not fill this in, this will be done automatically at runtime.</param>
        public static void Log(string description, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
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
        public static void Log(string description, object[] parameters, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
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
        public static void Log(Level level, string description, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
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
        public static void Log(Level level, string description, Exception exception, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
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
        public static void Log(Level level, string description, Exception exception, object[] parameters, [CallerMemberName] string member = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int line = -1) {
            Parallel.ForEach(_availableLoggers, (kvp) => {
                if (kvp.Value) kvp.Key.Log(level, description, exception, parameters, member, sourceFile, line);
            });
        }

        /// <summary>
        /// Wait untill all log entries are written. Do this before the application exits (if need be).
        /// </summary>
        public static void Flush() {
            foreach (var kvp in _availableLoggers)
                if (kvp.Value)
                    kvp.Key.Flush();
        }
        #endregion
    }
}