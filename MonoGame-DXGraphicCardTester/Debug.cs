// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.IO;

namespace MonoGame_DXGraphicCardTester
{
	public class Info
	{
		public bool IsDev { get; set; }

		public string LogPathAndFileName { get; set; }
		public bool IsDebugFlagSet { get; set; }
	}

	public static class Debug
	{
		public delegate bool LogDecision(string message, Exception e);

		private static Info instance;

		public static Info Info
		{
			get => instance ?? (instance = new Info());
			set => instance = value;
		}

		public static event LogDecision OnLogDecision;

		/// <summary>
		///     Appends a value to an existing string. If the target is empty, just the value is appended.
		///     If the target contains a value, ', ' + value is appended.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string CommaAppend(this string target, string value)
		{
			if (target != "")
				target += ", ";
			return target + value;
		}

		public static void DLogIfNotEmpty(string commaSeparatedList, string text)
		{
			if (commaSeparatedList != "")
				DLog(text, commaSeparatedList);
		}

		public static void DLog(Exception e)
		{
			DLog(string.Empty, e);
		}

		public static void DLog(string message, params object[] args)
		{
			DLog(message, null, args);
		}

		public static void DLog(string message, Exception e, params object[] args)
		{
			var logIt = true;
			foreach (var @delegate in OnLogDecision.GetInvocationList())
			{
				var f = (LogDecision) @delegate;
				if (f(message, e)) continue;
				logIt = false;
				break;
			}

			if (instance.IsDebugFlagSet || logIt)
				Log(message, e, args);
#if DEBUG
			var sw = new StreamWriter(Console.OpenStandardOutput());
			Log(sw, message, e, false, args);
#endif
		}

		public static void Log(Exception e)
		{
			Log(string.Empty, e);
		}

		public static void Log(string message, params object[] args)
		{
			Log(message, null, true, args);
		}

		public static void Log(string message, Exception e, params object[] args)
		{
			var fs = File.Open(instance.LogPathAndFileName, FileMode.Append, FileAccess.Write);
			Log(new StreamWriter(fs), message, e, true, args);
		}

		public static string FormatDate(DateTime d)
		{
			return d.ToString("yyyy-MM-ddThh:mm:ss.fff");
		}

		public static string FormatDateForFile(DateTime d)
		{
			return d.ToString("yyyy-MM-ddThh-mm-ss_fff");
		}

		private static void Log(StreamWriter sw, string message, Exception e, bool isForFile, params object[] args)
		{
			using (sw)
			{
				sw.AutoFlush = true;
				if (isForFile)
					if (e != null)
						sw.WriteLine("### {0} ##################################", DateTime.Now);
					else
						sw.Write(FormatDate(DateTime.Now) + " ");
				if (message != null)
					if (args != null && args.Length > 0)
						sw.WriteLine(message, args);
					else
						sw.WriteLine(message);
				if (e == null) return;
				sw.WriteLine(e.ToString());
				sw.WriteLine(e.StackTrace);
			}
		}
	}
}