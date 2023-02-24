using System;
using System.IO;

namespace OpenDatabase.Logs 
{
	///<summary>
	///	Handles the API logs.
	///</summary>
	public class Logger
	{
		public static string DefaultLogFilePath = ".logs";
		
		private static uint LogCount = 0;

		/// <summary>
		/// Logs a message to the DefaultLogFilePath.
		/// </summary>
		/// <param name="message"></param>
		public static void Log(string message)
		{
			if (!File.Exists(Logger.DefaultLogFilePath))
				File.CreateText(Logger.DefaultLogFilePath);

			FileIO.PushToFile(message, Logger.DefaultLogFilePath);
		}

		/// <summary>
		/// Logs the provided messag 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="logFilePath"></param>		
		public static void Log(string message, string logFilePath)
		{
			FileIO.PushToFile(message, logFilePath);
		}

		/// <summary>
		/// Logs the provided message to the DefaultLogFile and also prints it to the console
		/// </summary>
		/// <param name="message"></param>
		/// <param name="print"></param>
		public static void Log(string message, bool print)
		{
			if (print)
				Logger.ConsoleLog(message);

			FileIO.PushToFile(message, Logger.DefaultLogFilePath);
		}
		
		/// <summary>
		/// Prints the provided log message to the console.
		/// </summary>
		/// <param name="message"></param>
		public static void ConsoleLog(string message)
		{
			Console.WriteLine($"Debug {Logger.LogCount++}: {message}");
		}
	}	
}

