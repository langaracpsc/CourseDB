using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using OpenDatabase.Logs;

namespace OpenDatabase
{
	public enum SQLClientType
	{
		Unknown = -1,
		PostGRES,
		SQLServer
	}

	///<summary>
	///	Stores the Database configuration to be used by the SQL server.
	///</summary>
	public class DatabaseConfiguration
	{
		public string HostName;	//	Name of the database host server

		public string DatabaseName;	//	Name of the DB to connect to.

		public string UserID;	//	Database user's ID.

		public string Password;	//	Database users's password.

		public bool IntegratedSecurity;		//	Integrated security toggle

		public SQLClientType Client;
		
        public static string DefaultDatabaseConfigFile = "DatabaseConfig.json"; 
		public string ConnectionString { get { return this.GetConnectionString(this.Client); } set { }  }	//	SQL connection string.

		protected static string[] ConnectionStrings = new string[] {
			"Host={0};Database={1};UserName={2};Password={3};",
			"Server={0};Database={1}; User Id={2};Password={3};"
		};

		public static string[] SQLClientStrings = new string[]
		{
			"PostGRES",
			"SQLServer"
		};

		public static SQLClientType GetClientType(string clientTypeStr)
		{
			for (int x = 0; x < DatabaseConfiguration.ConnectionStrings.Length; x++)
				if (DatabaseConfiguration.ConnectionStrings[x] == clientTypeStr)
					return (SQLClientType)x;
			
			return SQLClientType.Unknown;
		}

		public bool IsValid()
		{
			return (this.HostName != null &&
					this.DatabaseName != null &&
					this.UserID != null &&
					this.Password != null);	
		}

		public string GetConnectionString(SQLClientType type)
		{	
			string integratedSecurity = (this.IntegratedSecurity) ? "True" : "False";

			return String.Format(DatabaseConfiguration.ConnectionStrings[(int)type], this.HostName, this.DatabaseName, this.UserID, this.Password);
		}

		public static DatabaseConfiguration LoadFromFile(string file)
		{
			DatabaseConfiguration configuration = null; //new DatabaseConfiguration(file);

			try
			{
				configuration = new DatabaseConfiguration(Json.Json.GetJsonHashtable(file));
			}
			catch (FileNotFoundException e)
			{
				Logger.Log(e.Message);
			}
			
			return configuration;
		}

		public DatabaseConfiguration(string hostName, string databaseName, string userID, string password)
		{
			this.UserID = userID;
			this.Password = password;
			this.HostName = hostName;
			this.DatabaseName = databaseName;
			this.IntegratedSecurity = true;
		}

		public DatabaseConfiguration(string hostName, string databaseName, string userID, string password, SQLClientType type, bool integratedSecurity)
		{
			this.UserID = userID;
			this.Password = password;
			this.HostName = hostName;
			this.DatabaseName = databaseName;
			this.Client = type;
			this.IntegratedSecurity = integratedSecurity;
		}

		public DatabaseConfiguration(string configFilePath)
		{
			Hashtable configHash = Json.Json.GetJsonHashtable(configFilePath);

			try
			{
				this.UserID = configHash["UserID"].ToString();
				this.Password = configHash["Password"].ToString();
				this.HostName = configHash["HostName"].ToString();
				this.DatabaseName = configHash["DatabaseName"].ToString();
				this.Client = DatabaseConfiguration.GetClientType(configHash["SQLClient"].ToString());
				this.IntegratedSecurity = (configHash["IntegratedSecurity"].ToString() == "True") ? true : false;
			}
			catch (NullReferenceException)
			{
				Logger.Log($"Invalid configuration provided.");

				this.UserID = null;
				this.Password = null;
				this.HostName = null;
				this.DatabaseName = null;
				this.IntegratedSecurity = true;
			}
		}
		

		public DatabaseConfiguration(Hashtable configHash)
		{
			this.UserID = configHash["UserID"].ToString();
			this.Password = configHash["Password"].ToString();
			this.HostName = configHash["HostName"].ToString();
			this.DatabaseName = configHash["DatabaseName"].ToString();
			this.IntegratedSecurity = (configHash["IntegratedSecurity"].ToString() == "True") ? true : false;
		}
	}


    public abstract class Database
    {
        public DatabaseConfiguration Configuration;

        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }

        public virtual bool InsertRecord(Record record, string table)
        {
            return true;
        }

        public virtual bool UpdateRecord(string id, Record record, string table)
        {
            return true;
        }

        public virtual Record[] FetchQueryData(string query, string tableName)
        {
            return null;
        }

        public virtual bool ExecuteQuery(string query)
        {
            return true;
        }

        public Database(DatabaseConfiguration configuration)
        {
            this.Configuration = configuration;
        }
    }
} 
