using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria
{
	public enum LogType
	{
		Trace,
		Debug,
		Info,
		Warning,
		Error,
		Fatal,
	}

	public static class AriaLog
	{
		static int LogFlag;
		public static IAriaLogger Logger { get; set; }

		static AriaLog()
		{
			//デフォルトは全出力
			LogFlag = -1;

			//アプリ全体でキャッチしてない例外をログに出力するようにしておく
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
		}

		static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception exception)
				Fatal(sender, exception);
		}

		static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			if (e.Exception != null)
				Fatal(sender, e.Exception);
		}

		static void AddFlag(LogType logType)
			=> LogFlag |= (1 << (int)logType);
		static void RemoveFlag(LogType logType)
			=> LogFlag &= ~(1 << (int)logType);
		static bool HasFlag(LogType logType)
			=> (LogFlag & (1 << (int)logType)) == (1 << (int)logType);

		public static void Trace(string message)
			=> Output(LogType.Trace, message);
		public static void Debug(string message)
			=> Output(LogType.Debug, message);
		public static void Info(string message)
			=> Output(LogType.Info, message);
		public static void Warning(string message)
			=> Output(LogType.Warning, message);
		public static void Error(string message)
			=> Output(LogType.Error, message);
		public static void Error(object sender, Exception exception)
			=> Output(LogType.Error, sender, exception);
		public static void Fatal(string message)
			=> Output(LogType.Fatal, message);
		public static void Fatal(object sender, Exception exception)
			=> Output(LogType.Fatal, sender, exception);

		static void Output(LogType logType, string message)
		{
			if (Logger == null)
				return;
			switch (logType) {
				case LogType.Trace:
					if (HasFlag(LogType.Trace))
						Logger.Trace(message);
					break;
				case LogType.Debug:
					if (HasFlag(LogType.Debug))
						Logger.Debug(message);
					break;
				case LogType.Info:
					if (HasFlag(LogType.Info))
						Logger.Info(message);
					break;
				case LogType.Warning:
					if (HasFlag(LogType.Warning))
						Logger.Warning(message);
					break;
				case LogType.Error:
					if (HasFlag(LogType.Error))
						Logger.Error(message);
					break;
				case LogType.Fatal:
					if (HasFlag(LogType.Fatal))
						Logger.Fatal(message);
					break;
			}
		}
		static void Output(LogType logType, object sender, Exception exception)
		{
			if (Logger == null)
				return;
			switch (logType) {
				case LogType.Error:
					if (HasFlag(LogType.Error))
						Logger.Error(sender, exception);
					break;
				case LogType.Fatal:
					if (HasFlag(LogType.Fatal))
						Logger.Fatal(sender, exception);
					break;
			}
		}
	}
}
