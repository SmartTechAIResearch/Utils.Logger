/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace SizingServers.Log {
    /// <summary>
    /// The most basic implementation of Logger. Suscribe to the WriteLogEntry event to handle log entries written yourself.
    /// </summary>
    public class SimpleLogger : Logger {
        private static SimpleLogger _logger = new SimpleLogger();

        private SimpleLogger() { base.WriteLogEntry += SimpleLogger_WriteLogEntry; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static SimpleLogger GetInstance() { return _logger; }

        private void SimpleLogger_WriteLogEntry(object sender, WriteLogEntryEventArgs e) {
            //Do nothing.
        }
    }
}
