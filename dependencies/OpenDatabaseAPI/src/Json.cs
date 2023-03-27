using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenDatabase.Logs; 

namespace OpenDatabase.Json
{
	public class Json
	{
		/// <summary>
		/// Deserializes the Json dumped in provided file to a Hashtable 
		/// </summary>
		/// <param name="path"> File path. </param>
		/// <returns> Hashtable generated from the Json in the file </returns>
		public static Hashtable GetJsonHashtable(string path)
		{	
			Hashtable jsonHash = new Hashtable();

			string jsonStringDump = null;

			try
			{
				jsonStringDump = FileIO.ReadFile(path); 			
					
				jsonHash = JsonConvert.DeserializeObject<Hashtable>(jsonStringDump);	
			}
			catch (FileNotFoundException)
			{
				Logger.Log($"File {path} not found.");
			}	

			return jsonHash;	
		}

		/// <summary>
		/// Converts the provided Json 
		/// </summary>
		/// <param name="jsonString"> JSON string. </param>	
		/// <returns> JSON string deserialized into a Hashtable. </returns>
		public static Hashtable JsonToHashtable(string jsonString)
		{
			return JsonConvert.DeserializeObject<Hashtable>(jsonString);	
		}
	}
}

