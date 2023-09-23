#if UNITY_2017_1_OR_NEWER
using System;
using System.Text;
using UnityEngine;

namespace Aria
{
	public class AriaUnityLogger : IAriaLogger
	{
		[RuntimeInitializeOnLoadMethod]
		static void RuntimeInitialize()
		{
			AriaLog.Logger = new AriaUnityLogger();
		}

		readonly string m_FilePath;
		DateTime m_LastLogDate;
		System.Globalization.CultureInfo m_CultureInfo;

		object m_WriteLock;

		static DateTime Time
			=> DateTime.UtcNow;

		public AriaUnityLogger()
		{
			m_WriteLock = new object();

			var time = Time;
			m_LastLogDate = time.Date;
			m_FilePath = "Logs/" + time.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
			m_CultureInfo = System.Globalization.CultureInfo.CurrentCulture;
			OutputSystemInfo();
			OutputDeviceInfo();
		}

		void OutputSystemInfo()
		{
			var timezone = TimeZoneInfo.Local;

			var time = Time;
			var builder = new StringBuilder()
				.AppendLine().AppendFormat("Unity: {0} {1} {2}", Application.unityVersion, Application.genuine, Application.genuineCheckAvailable)
				.AppendLine().AppendFormat("InstallMode: {0}", Application.installMode)
				.AppendLine().AppendFormat("Installer: {0}", Application.installerName)

				.AppendLine().AppendFormat("Identifier: {0}", Application.identifier)
				.AppendLine().AppendFormat("Version: {0} ({1})", Application.version, Application.buildGUID)

				.AppendLine().AppendFormat("Languate: {0}", Application.systemLanguage)
				.AppendLine().AppendFormat("Culture: {0}", m_CultureInfo.Name)
				.AppendLine().AppendFormat("Time: {0:u}", time)
				.AppendLine().AppendFormat("TimeZone: {0}", timezone.DisplayName)
				.AppendLine();

			Info(builder.ToString());
		}

		void OutputDeviceInfo()
		{
			var builder = new StringBuilder()
				.AppendLine().AppendFormat("Device: {0} {1} {2}", SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType)
				.AppendLine().AppendFormat("OS: {0}", SystemInfo.operatingSystem)
				.AppendLine().AppendFormat("CPU: {0} {1}.{2:00}GHz x {3}", SystemInfo.processorType, SystemInfo.processorFrequency / 1000, SystemInfo.processorFrequency % 1000 / 10, SystemInfo.processorCount)
				.AppendLine().AppendFormat("RAM: {0:#,0}MB", SystemInfo.systemMemorySize)
				.AppendLine().AppendFormat("GPU: {0} {1} ({2})", SystemInfo.graphicsDeviceVendor, SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceID)
				.AppendLine().AppendFormat("VRAM: {0:#,0}MB", SystemInfo.graphicsMemorySize)
				.AppendLine().AppendFormat("Graphic: {0}", SystemInfo.graphicsDeviceVersion)
				.AppendLine().AppendFormat("Shader: {0}", SystemInfo.graphicsShaderLevel)
				.AppendLine();
			Info(builder.ToString());
		}

		//ファイルに吐き出す
		void WriteFile(string message)
		{
			lock (m_WriteLock) {
				using var writer = new System.IO.StreamWriter(m_FilePath, true);

				var builder = new StringBuilder();
				var time = Time;
				if (m_LastLogDate != time.Date) {
					m_LastLogDate = time.Date;
					writer.WriteLine();
					writer.WriteLine("------------------------------------------");
					writer.WriteLine();
					writer.WriteLine("Change Date: {0:u}", m_LastLogDate);
					writer.WriteLine();
					writer.WriteLine("------------------------------------------");
				}

				builder.AppendFormat("[{0:u}]: {1}", time, message).AppendLine();

				//string[] lines = message.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
				writer.Write(builder.ToString());
			}
		}

		public void Trace(string message)
		{
			string logText = "[Trace] " + message;
			UnityEngine.Debug.Log(logText);
			WriteFile(logText);
		}

		public void Debug(string message)
		{
			string logText = "[Debug] " + message;
			UnityEngine.Debug.Log(logText);
			WriteFile(logText);
		}

		public void Info(string message)
		{
			string logText = "[Info] " + message;
			UnityEngine.Debug.Log(logText);
			WriteFile(logText);
		}

		public void Warning(string message)
		{
			string logText = "[Warning] " + message;
			UnityEngine.Debug.LogWarning(logText);
			WriteFile(logText);
		}

		public void Error(string message)
		{
			string logText = "[Error] " + message;
			UnityEngine.Debug.LogError(logText);
			WriteFile(logText);
		}
		public void Error(object sender, Exception exception)
		{
			string logText = "[Error] " + exception.Message;
			UnityEngine.Debug.LogError(logText);
			WriteFile(logText);
		}

		public void Fatal(string message)
		{
			string logText = "[Fatal] " + message;
			UnityEngine.Debug.LogError(logText);
			WriteFile(logText);
		}
		public void Fatal(object sender, Exception exception)
		{
			var builder = new StringBuilder()
				.Append("[Fatal] ");
			if (sender != null)
				builder.Append(sender);
			if (sender != null && exception != null)
				builder.Append(": ");
			if (exception != null)
				builder.Append(exception);
			string logText = builder.ToString();
			UnityEngine.Debug.LogError(logText);
			WriteFile(logText);
		}
	}
}
#endif