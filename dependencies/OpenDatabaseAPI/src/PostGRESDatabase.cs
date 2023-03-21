using System.Collections;
using System.Collections.Generic;

using OpenDatabase;
using OpenDatabase.Logs;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Npgsql.Schema;

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
        /// <summary>
        /// Extracts the records from the reader.
        
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected Record[] GetRecordsFromReader(NpgsqlDataReader reader, string[] fields)
        {
            List<Record> recordStack = new List<Record>();

            Record tempRecord = new Record();
            
            try
            {
                // Console.WriteLine($"Rows:{reader.Rows}");
                while (reader.Read())
                {
                    int fieldCount = reader.FieldCount;

                    tempRecord = new Record(new string[fieldCount], new object[fieldCount]);

                    Type fieldType = typeof(string);

                    tempRecord.Keys = fields;
                    
                    for (int x = 0; x < fieldCount; x++)
                    {
                        if ((fieldType = reader.GetFieldType(x)) == typeof(int))
                            tempRecord.Values[x] = reader.GetInt32(x);
                       
                        else if ((fieldType = reader.GetFieldType(x)) == typeof(double))
                            tempRecord.Values[x] = reader.GetDouble(x);
                        
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

        public string[] GetTableFieldNames(NpgsqlDataReader reader)
        {
            string[] fields = null;

            ReadOnlyCollection<NpgsqlDbColumn> columns;

            try
            {
                fields = new string[reader.FieldCount];

                columns = reader.GetColumnSchema();

                for (int x = 0; x < reader.FieldCount; x++)
                    fields[x] = columns[x].ColumnName;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured while fetching field names. Exception: {e.Message}");
            }

            return fields;
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

        public override bool InsertRecord(Record record, string table)
        {
            try
            {
                string query;
                this.ExecuteQuery(query = QueryBuilder.GetInsertQuery(table, record)); 
                // Console.WriteLine(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            
            return true;
        }
        
        public override bool InsertRecord(Record record, string table, bool specify)
        {
            try
            {
                string query;
                this.ExecuteQuery(query = QueryBuilder.GetInsertQuery(table, record, specify)); 
                // Console.WriteLine(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            
            return true;
        }

        
        public override Record[] FetchQueryData(string query, string tableName)
        {
            NpgsqlCommand command = null;

            Record[] fetchedRecords = null;

            NpgsqlDataReader reader = null;

            string[] fields;
            
            try
            {
                command = new NpgsqlCommand(query, this.Connection);
                
                reader = command.ExecuteReader();
                fields = this.GetTableFieldNames(reader);
                
                fetchedRecords = this.GetRecordsFromReader(reader, fields);

                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, true);
            }

            return fetchedRecords;
        }
        
        public PostGRESDatabase(DatabaseConfiguration configuration) : base(configuration)
        {
        }
    }
}

