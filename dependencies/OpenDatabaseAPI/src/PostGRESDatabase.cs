using System.Collections;
using System.Collections.Generic;

using OpenDatabase;
using OpenDatabase.Logs;
using Npgsql;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace OpenDatabaseAPI
{
    public class PostGRESDatabase : Database
    {
        public NpgsqlConnection Connection = new NpgsqlConnection();

        /// <summary> 
        /// Configures and connects to the database server.
        /// </summary>
        /// <returns> Execution status. </returns>
        public override bool Connect()
        {
            if (!this.Configuration.IsValid())
                this.Configuration = DatabaseConfiguration.LoadFromFile(DatabaseConfiguration.DefaultDatabaseConfigFile);
            
            try
            {
                this.Connection = new NpgsqlConnection(this.Configuration.GetConnectionString(SQLClientType.PostGRES));
                    
                Logger.Log(this.Configuration.GetConnectionString(SQLClientType.PostGRES));

                this.Connection.Open();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, true);

                return false;
            }

            return true;
        }

        protected Record[] GetRecordsFromReader(NpgsqlDataReader reader, string[] fields)
        {
            List<Record> recordStack = new List<Record>();

            Record tempRecord = new Record();

            try
            {
                while (reader.Read())
                {
                    int fieldCount = reader.FieldCount;

                    tempRecord = new Record(new string[fieldCount], new object[fieldCount]);

                    Type fieldType = typeof(string);

                    for (int x = 0; x < fieldCount; x++)
                    {
                        tempRecord.Keys[x] = fields[x];

                        if ((fieldType = reader.GetFieldType(x)) == typeof(int))
                            tempRecord.Values[x] = reader.GetInt32(x);
                        else
                            tempRecord.Values[x] = reader.GetString(x);
                    }

                    recordStack.Add(tempRecord);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, true);
            }

            return recordStack.ToArray();
        }

        public string[] GetTableFieldNames(string tableName)
        {
            List<string> fields = new List<string>();

            NpgsqlCommand command = null;

            NpgsqlDataReader reader = null;

            try
            {
                command = new NpgsqlCommand($"SELECT column_name FROM INFORMATION_SCHEMA. COLUMNS WHERE TABLE_NAME = '{tableName}';", this.Connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                    fields.Add(reader.GetString(0));

                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, true);
            }

            return fields.ToArray();
        }

        public override bool ExecuteQuery(string query)
        {
            NpgsqlCommand command = null;

            try
            {
                command = new NpgsqlCommand(query, this.Connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, true);

                return false;
            }

            return true;
        }

        public int GetRecordCount(string tableName)
        {
            int count = 0;

            NpgsqlCommand command = null;

            NpgsqlDataReader reader = null;

            try
            {
                command = new NpgsqlCommand($"SELECT * FROM {tableName}", this.Connection);

                reader = command.ExecuteReader();

                for (; reader.Read(); count++) ;

                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, true);

                return count;
            }

            return count;
        }

        public override Record[] FetchQueryData(string query, string tableName)
        {
            NpgsqlCommand command = null;

            Record[] fetchedRecords = null;

            NpgsqlDataReader reader = null;

            // try
            // {
            command = new NpgsqlCommand(query, this.Connection);

            string[] fields = this.GetTableFieldNames(tableName);

            reader = command.ExecuteReader();

            fetchedRecords = this.GetRecordsFromReader(reader, fields);

            reader.Close();
            // }
            // catch (Exception e)
            // {
            //     Logger.Log(e.Message, true);
            // }

            return fetchedRecords;
        }
        public PostGRESDatabase(DatabaseConfiguration configuration) : base(configuration)
        {
        }
    }
}

