using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using OpenDatabase.Logs;
using OpenDatabase.Json;
using OpenDatabaseAPI;

namespace OpenDatabase
{


	/// <summary>
	/// Handles database connectivity, data fetching and editing.		
	/// </summary>
	public class SQLServerDatabase : Database	
	{
		public static string DefaultDatabaseConfigurationFile = "DatabaseConfig.json";

		public DatabaseConfiguration DefaultDatabaseConfiguration;	//	DB config based on the provided config files

		public SqlConnection DefaultSqlConnection;	//	Global default SqlConnection based on DefaultDatabaseConfiguration

		/// <summary>
		/// Creates a connection to the specified database.
		/// </summary>
		/// <param name="dbname"> database name </param>
		/// <returns> SqlConnection instance storing the connection value. </returns>
		public override bool  Connect()
		{
			try
			{
				if (!this.DefaultDatabaseConfiguration.IsValid())
					throw new Exception("Invalid configuration provided.");

				if (this.DefaultSqlConnection == null)
					this.DefaultSqlConnection = new SqlConnection(this.DefaultDatabaseConfiguration.ConnectionString);

				this.DefaultSqlConnection.Open();
			}
			catch (Exception e)
			{
				Logger.Log($"SQL Sever connection error: {e.Message}");

				return false;
			}

			return true;
		}

		public override bool Disconnect()
		{
			try
			{
				this.DefaultSqlConnection.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return false;
			}

			return true;
		}


		public override bool ExecuteQuery(string query)
		{
			SqlCommand command = new SqlCommand(query, this.DefaultSqlConnection);
				
			this.Connect();
			command.ExecuteNonQuery();
			this.Disconnect();

			return true;
		}
	
		/// <summary>
		///	Gets the field names from a data record instance.
		/// </summary>
		/// <param name="record"> IDataRecord instance. </param>
		/// <returns> Array of fieldnames. </returns>
		private static string[] GetFieldNames(IDataRecord record)
		{
			Stack<string> fieldStack = new Stack<string>();
			
			int size = record.FieldCount;

			for (int x = (size - 1); x >= 0; x--)
				fieldStack.Push(record.GetName(x));	

			return fieldStack.ToArray();
		}

		/// <summary>
		///	Gets the field names from a data record instance.
		/// </summary>
		/// <param name="record"> IDataRecord instance. </param>
		/// <returns> Array of fieldnames. </returns>
		private static object[] GetValueNames(IDataRecord record)
		{
			Stack<object> fieldStack = new Stack<object>();
			
			int size = record.FieldCount;

			for (int x = (size - 1); x >= 0; x--)
				fieldStack.Push(record[x]);	

			return fieldStack.ToArray();
		}

		///	<summary>
		///	Fetches the data returned by executing the provided query.	
		///	</summary>
		///	<param name="query"> SQL Query. </param>
		///	<returns> Hashtable containing the fetched data. </returns>
		public Record[] FetchQueryData(string query)
		{
			Stack<Record> records = new Stack<Record>();

			SqlCommand command = null;

			SqlDataReader dataReader = null;
				
			command = new SqlCommand(query, this.DefaultSqlConnection);

			try
			{
				this.Connect();

				dataReader = command.ExecuteReader();

				while (dataReader.Read())
					records.Push(new Record(SQLServerDatabase.GetFieldNames(dataReader), SQLServerDatabase.GetValueNames(dataReader)));

				dataReader.Close();
				this.Disconnect();
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
			}

			return records.ToArray();
		}

		/// <summary>
		///	Inserts the provided record into provided table.
		/// </summary>
		/// protected<param name="recordHashtable"> Hashtable storing the record value. </param>
		/// <returns> Execution state. </returns>
		public bool InsertRecord(Hashtable recordHashtable, string tableName)
		{
			this.ExecuteQuery(QueryBuilder.GetInsertQuery(tableName, recordHashtable));

			return true;
		}

		/// <summary>
		///	Inserts the provided record into provided table.
		/// </summary>
		/// <param name="record"> Record to be inserted </param>
		/// <returns> Execution state. </returns>
		public override bool InsertRecord(Record record, string tableName)
		{
			try
			{
				this.ExecuteQuery(QueryBuilder.GetInsertQuery(tableName, record));
			}
			catch (SqlException)
			{
				Logger.Log("Insertion error occured, the provided record could be a duplicate.");
				
				return false;	
			}

			return true;
		}

		/// <summary>
		/// Replaces the database record with the provided ID with the provided record.
		/// </summary>
		/// <param name="ID"> Record ID. </param>
		/// <param name="record"> Record to be updated. </param>
		/// <param name="tableName"> Table name. </param>
		public override bool UpdateRecord(string ID, Record record, string tableName)
		{
			string query = $"UPDATE {tableName} SET {QueryBuilder.GetSetString(record)} WHERE ID='{ID}';";

			Logger.ConsoleLog(query);

			this.Connect();
			this.ExecuteQuery(query);
						
			return true;	
		}

		public override bool UpdateRecord(object id, Record record, string table)
		{
			try
			{
				this.ExecuteQuery(QueryBuilder.GetUpdateQuery(id, table, record));
			}
			catch (Exception e)
			{
				return false; 
			}
			
			return true; 
		}

		public int GetFieldCount(string tableName)
		{
			return Convert.ToInt32(this.FetchQueryData($"SELECT COUNT(*) FROM {tableName};")[0].Values[0]);
		}

		public SQLServerDatabase(DatabaseConfiguration databaseConfiguration) : base(databaseConfiguration)
		{
			this.DefaultSqlConnection = new SqlConnection(this.DefaultDatabaseConfiguration.ConnectionString);
		}
		
		public SQLServerDatabase(string configFilePath) : base(DatabaseConfiguration.LoadFromFile(configFilePath))
		{
			this.DefaultSqlConnection = new SqlConnection(this.DefaultDatabaseConfiguration.ConnectionString);
		}
	}
}
