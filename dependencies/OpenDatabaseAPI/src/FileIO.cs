using System.IO;
using OpenDatabase.Logs;

namespace OpenDatabase
{
	public class FileIO
	{
		/// <summary>
		/// Reads all the text from the given filepath
		/// </summary>
		/// <param name="path"> File path. </param>
		/// <returns> String read from the file. </returns>
		public static string ReadFile(string path)
		{
			string fileBuffer = null;

			StreamReader streamReader = null;

			try
			{
				if (!File.Exists(path))
					throw new FileNotFoundException();

				streamReader = new StreamReader(path);

				fileBuffer = streamReader.ReadToEnd();
			}	
			catch (FileNotFoundException)
			{
				Logger.ConsoleLog($"File {path} was not found.");
			}

			return fileBuffer;	
		}

		/// <summary>
		/// Pushes the provided string to the file at the provided path.
		/// </summary>
		/// <param name="str"> String to be pushed. </param>
		/// <param name="path"> File path. </param>
		public static void PushToFile(string str, string path)
		{
			StreamWriter streamWriter = null;

			try
			{	
				if (!File.Exists(path))
					throw new FileNotFoundException();

				streamWriter = new StreamWriter(path, true);

				streamWriter.WriteLine(str);
				streamWriter.Flush();
			}
			catch (FileNotFoundException)
			{
				Logger.ConsoleLog($"File {path} was not found.");

				return;			
			}
		}
	}
}
