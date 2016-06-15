/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Runtime.InteropServices;

namespace SizingServers.Log {
    /// <summary></summary>
    public enum Level {
        /// <summary></summary>
        Info = 0,
        /// <summary>
        /// <para>Use this when an error occures (thrown at runtime or self determined), but it is handled within the program and expected to happen.</para>
        /// <para>For instance, a connection could not be made to a database server and you inform the user of this.</para>
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Use this when a runtime exception occures, but the program or parts of it do not break. (You do a retry for instance). Stil it is not expected behaviour.
        /// </summary>
        Error = 2,
        /// <summary>
        /// <para>Use this when a runtime exception occures. When the program or parts of it break. This should be avoided at all costs.</para>
        /// <para>This is handled here in this class for you, if the application breaks, a log entry is written to file.</para>
        /// </summary>
        Fatal = 3
    }
    /// <summary></summary>
    public struct Entry {
        /// <summary>
        /// Should be a round-trip DateTimeOffset http://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Roundtrip.
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary></summary>
        public Level Level { get; set; }
        /// <summary></summary>
        public string Description { get; set; }
        /// <summary>
        /// The name of the member that called 'Logger.Log(...)'.
        /// </summary>
        public string Member { get; set; }
        /// <summary>
        /// The file where the caller member resides in.
        /// </summary>
        public string SourceFile { get; set; }
        /// <summary>
        /// The line in the source file where 'Logger.Log(...)' as called.
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// The parameters used in the calling member. This can be null.
        /// </summary>
        public string[] Parameters { get; set; }
        /// <summary></summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Only applicable for unhandled exceptions. Somewhat handy for when there is no stacktrace available, or you cannot trace into third-party assemblies.
        /// </summary>
        public ReadableWatsonBucketParameters ReadableWatsonBucketParameters { get; set; }
    }

    /// <summary>
    /// <para>BucketParameters Structure to get watson buckets back from CLR. Applicable to dr. watson (pre-Vista) and WER (Windows Error Reporting, Vista and newer).</para>
    /// <para>http://msdn.microsoft.com/en-us/library/ms404466(v=VS.110).aspx</para>
    /// <para>http://mrpfister.com/programming/demystifying-clr20r3-error-messages/</para>
    /// <para>http://mrpfister.com/programming/opcodes-msil-and-reflecting-net-code/</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct BucketParameters {
        /// <summary>
        /// True if the rest of the structure is valid, otherwise false.
        /// </summary>
        public int fInited;
        /// <summary>
        /// Always CLR20r3, I think. Stands for common language runtime 2.0 revision 3?
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string pszEventTypeName;
        /// <summary>
        /// Application filename.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p1;
        /// <summary>
        /// Application version.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p2;
        /// <summary>
        /// Time in seconds, since epoch (01/01/1970), when the application was build. Hex
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p3;
        /// <summary>
        /// Assembly / module name. An assembly can contain multiple modules in theory, however, MSBuild does not support this.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p4;
        /// <summary>
        /// Assembly / module version.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p5;
        /// <summary>
        /// Time in seconds, since epoch (01/01/1970), when the assembly / module was build. Hex
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p6;
        /// <summary>
        /// <para>Method identifier. Method identifiers are usaly prefixed with 0x6000. You should be able to locate the faulty method using Microsoft's IL Dissasembler (part of Visual Studio SDK tools).</para>
        /// <para>To find the method where the error occurred:</para>
        /// <para>  Go to File > Open and select the .DLL associated with P4 (with version P5)</para>
        /// <para>  Go to View > Meta Info > Show!</para>
        /// <para>  Go to Find, and type in 06000 and the P7 value.</para>
        /// <para>Hex.</para>
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p7;
        /// <summary>
        /// IL code offset: the offset of the IL code in the compiled module.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p8;
        /// <summary>
        /// Error message. Hashed if not enough place, this maximum is unknown. Can be the exception name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p9;
        /// <summary>
        /// No idea what this is, this is always an empty string (please correct me if I am wrong). I guess it was added, back in the day, for when they would ever need an extra entry.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
        public string p10;
    }

    /// <summary>
    /// Readable watson bucket parameters from the clr.
    /// </summary>
    public sealed class ReadableWatsonBucketParameters {
        /// <summary>
        /// <para>BucketParameters Structure to get watson buckets back from CLR. Applicable to dr. watson (pre-Vista) and WER (Windows Error Reporting, Vista and newer).</para>
        /// <para>http://msdn.microsoft.com/en-us/library/ms404466(v=VS.110).aspx</para>
        /// <para>http://mrpfister.com/programming/demystifying-clr20r3-error-messages/</para>
        /// <para>http://mrpfister.com/programming/opcodes-msil-and-reflecting-net-code/</para>
        /// </summary>
        public BucketParameters BucketParameters { get; set; }
        /// <summary>
        /// BucketParameters.p1
        /// </summary>
        public string ApplicationFilename { get; set; }
        /// <summary>
        /// BucketParameters.p2
        /// </summary>
        public string ApplicationVersion { get; set; }
        /// <summary>
        /// <para>BucketParameters.p3</para>
        /// <para>Should be a round-trip DateTimeOffset http://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Roundtrip.</para>
        /// </summary>
        public string ApplicationBuildTimestamp { get; set; }
        /// <summary>
        /// BucketParameters.p4
        /// </summary>
        public string AssemblyOrModuleName { get; set; }
        /// <summary>
        /// BucketParameters.p5
        /// </summary>
        public string AssemblyOrModuleVersion { get; set; }
        /// <summary>
        /// BucketParameters.p6
        /// Should be a round-trip DateTimeOffset http://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Roundtrip.
        /// </summary>
        public string AssemblyOrModuleBuildTimestamp { get; set; }
        /// <summary>
        /// <para>BucketParameters.p7</para>
        /// <para>Method identifiers are usaly prefixed with 0x6000. You should be able to locate the faulty method using Microsoft's IL Dissasembler (part of Visual Studio SDK tools).</para>
        /// <para>To find the method where the error occurred:</para>
        /// <para>  Go to File > Open and select the .DLL associated with P4 (with version P5)</para>
        /// <para>  Go to View > Meta Info > Show!</para>
        /// <para>  Go to Find, and type in 06000 and the P7 value.</para>
        /// </summary>
        public int MethodIdentifier { get; set; }
        /// <summary>
        /// <para>BucketParameters.p8</para>
        /// <para>The offset of the IL code in the compiled module.</para>
        /// </summary>
        public int ILCodeOffset { get; set; }
        /// <summary>
        /// <para>BucketParameters.p9</para>
        /// <para>Hashed if not enough place, this maximum is unknown. Can be the exception name.</para>
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Use the other constructor to auto-populate all properties.
        /// </summary>
        public ReadableWatsonBucketParameters() { 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketParameters">Auto-populates the properties.</param>
        public ReadableWatsonBucketParameters(BucketParameters bucketParameters) {
            BucketParameters = bucketParameters;
            ApplicationFilename = BucketParameters.p1;
            ApplicationVersion = BucketParameters.p2;

            double secondsFromEpoch = Convert.ToDouble(long.Parse(BucketParameters.p3, System.Globalization.NumberStyles.HexNumber));
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(secondsFromEpoch);
            DateTimeOffset dtOffset = new DateTimeOffset(dt).ToLocalTime();
            
            ApplicationBuildTimestamp = dtOffset.ToString("o");

            AssemblyOrModuleName = BucketParameters.p4;
            AssemblyOrModuleVersion = BucketParameters.p5;

            secondsFromEpoch = Convert.ToDouble(long.Parse(BucketParameters.p6, System.Globalization.NumberStyles.HexNumber));
            dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(secondsFromEpoch);
            dtOffset = new DateTimeOffset(dt).ToLocalTime();

            AssemblyOrModuleBuildTimestamp = dtOffset.ToString("o");

            MethodIdentifier = int.Parse(BucketParameters.p7, System.Globalization.NumberStyles.HexNumber);
            ILCodeOffset = int.Parse(BucketParameters.p8, System.Globalization.NumberStyles.HexNumber);

            ErrorMessage = bucketParameters.p9;
        }
    }

}
