using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace OpenDatabase
{
	public enum Operator
	{
		Less,
		More,
		Equal,
		LessEqual,
		MoreEqual,
		Like
	}

	public class ComparisonPair<T>
	{
		protected static string[] OperatorChars = new string[] {
			"<",
			">",
			"=",
			"<=",
			">",
			"LIKE"
		};
		
		T Left,
	  		Right;

		public Operator ComparisionOperator;
		
		protected string OperatorChar; 
		
		public override string ToString()
		{
			string quote = (typeof(T) == typeof(string)) ? "\'" : null;
		
			return $"{quote}{Convert.ToString(this.Left)}{quote}{this.OperatorChar}{quote}{Convert.ToString(this.Right)}{quote}";
		}

		public ComparisonPair(T left, T right, Operator comparisonOp)
		{
			this.ComparisionOperator = comparisonOp;
			this.Left = left;
			this.Right = right;
			this.OperatorChar = ComparisonPair<T>.OperatorChars[(int)this.ComparisionOperator];
		}
	}

	public class KeyComparisonPair
	{
		protected static string[] OperatorChars = new string[] {
			"<",
			">",
			"=",
			"<=",
			">",
			"LIKE"
		};

		public string Key;

		public object Value;
		
		public Operator ComparisionOperator;
		
		protected string OperatorChar; 
		
		public override string ToString()
		{
			string? quote = (this.Value is string) ? "\'" : null;

			if ((QueryBuilder.IsNumeric(this.Value)) && (ComparisionOperator == Operator.Like))
			{
				return $"{$"CAST({this.Key} as TEXT)"} {this.OperatorChar} \'{this.Value}{quote}%\'";
			}

			return $"{this.Key} {this.OperatorChar} {quote}{this.Value}{quote}";
		}

		public KeyComparisonPair(string key, object value, Operator comparisonOp)
		{
			this.ComparisionOperator = comparisonOp;
			this.Key = key;
			this.Value = value;
			this.OperatorChar = KeyComparisonPair.OperatorChars[(int)this.ComparisionOperator];
		}
	}
	
	public class Condition<T>
	{
		public List<ComparisonPair<T>>[] Comparisons;

		public Condition<T> And(ComparisonPair<T> comparisonPair)
		{
			this.Comparisons[0].Add(comparisonPair);
			
			return this;
		}

		public Condition<T> Or(ComparisonPair<T> comparisonPair)
		{
			this.Comparisons[1].Add(comparisonPair);
			
			return this;
		}

		public override string ToString() 
		{
			string conditionString = null;
			
			for (int x = 0; x < this.Comparisons[0].Count - 1; x++)
				conditionString += $"{this.Comparisons[0][x].ToString()} AND ";
		
			if (this.Comparisons[0].Count != 0)
				conditionString += $"{this.Comparisons[0][this.Comparisons[0].Count - 1].ToString()} ";
			
			for (int x = 0; x < this.Comparisons[1].Count - 1; x++)
				conditionString += $"{this.Comparisons[1][x].ToString()} OR";
		
			if (this.Comparisons[1].Count != 0) 
				conditionString += $"OR {this.Comparisons[1][this.Comparisons[1].Count - 1].ToString()} ";
			
			return conditionString;
		}
		

		public Condition()
		{
			this.Comparisons = new List<ComparisonPair<T>>[2]
			{
				new List<ComparisonPair<T>>(),
				new List<ComparisonPair<T>>()
			};
		}
	}
	
	
	public class KeyPairCondition
	{
		public List<KeyComparisonPair>[] Comparisons;

		public KeyPairCondition And(KeyComparisonPair comparisonPair)
		{
			this.Comparisons[0].Add(comparisonPair);
			
			return this;
		}

		public KeyPairCondition Or(KeyComparisonPair comparisonPair)
		{
			this. Comparisons[1].Add(comparisonPair);
			
			return this;
		}

		public override string ToString() 
		{
			string conditionString = null;
			
			for (int x = 0; x < this.Comparisons[0].Count - 1; x++)
				conditionString += $"{this.Comparisons[0][x].ToString()} AND ";
		
			if (this.Comparisons[0].Count != 0)
				conditionString += $"{this.Comparisons[0][this.Comparisons[0].Count - 1].ToString()} ";
			
			for (int x = 0; x < this.Comparisons[1].Count - 1; x++)
				conditionString += $"{this.Comparisons[1][x].ToString()} OR";
		
			if (this.Comparisons[1].Count != 0) 
				conditionString += $"OR {this.Comparisons[1][this.Comparisons[1].Count - 1].ToString()} ";
			
			return conditionString;
		}

		public KeyPairCondition()
		{
			this.Comparisons = new List<KeyComparisonPair>[2]
			{
				new List<KeyComparisonPair>(),
				new List<KeyComparisonPair>()
			};
		}
	}
	

	public class QueryBuilder 
	{
		public enum Command
		{
			Insert,
			Update,
			Alter,
			Drop
		}

		public enum SubCommand
		{
			Set,
			Where		
		}

		public static string[] CommandStrings = new string[] {
			"INSERT",
			"UPDATE",
			"ALTER",
			"DROP"
		};

		public static string[] SubCommandStrings = new string[]
		{
			"SET",
			"WHERE"
		};

		public static bool IsNumeric(object obj)
		{
			switch (Type.GetTypeCode(obj.GetType()))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
			}
			return false;
		}

		public static string GetValueString<T>(T value)
		{
			string valueString = null;

			if (value == null)
				return "null";

			Type valueType = value.GetType();
			
			 if (valueType == typeof(int))
				valueString = Convert.ToString(value);
			 else if (valueType == typeof(char))
				 valueString = $"\'{Convert.ToString(value)}\'";
			
			 else if (valueType == typeof(string))
			 {
				 string converted = Convert.ToString(value), altered = null;

				 string[] split = converted.Split("'");
				 
				 for (int x = 0; x < converted.Length; x++)
					 if (converted[x] == '\'')
						 altered += "\'\'";
					 else
						 altered += converted[x];

				 
				 valueString = $"\'{altered}\'";
			 }
			 else if (valueType == typeof(bool))
				valueString = (value.Equals(true)) ? "TRUE" : "FALSE";
			 
			 else if (valueType == typeof(double))
				 valueString = value.ToString();
			 else;

			 return valueString;
		}

		public static string GetRecordTuple(Record record)
		{
			string tuple = "(";

			int size = record.Keys.Length - 1;

			for (int x = 0; x < size; x++)
				tuple += $"{record.Keys[x]}={QueryBuilder.GetValueString(record.Values[x])}, ";

			tuple += $"{record.Keys[size]}={QueryBuilder.GetValueString(record.Values[size])})";

			return tuple;
		}

		public static string GetValueFunctionString(Hashtable data)
		{
			int size = data.Keys.Count;
	
			string valueFunctionString = "VALUES(";
		
			string[] keys = new string[size];
			object[] values = new object[size]; 
			
			data.Keys.CopyTo(keys, 0);
			data.Values.CopyTo(values, 0);
			
			for (int x = 0; x < size - 1; x++)
				valueFunctionString += $"{QueryBuilder.GetValueString(values[x])}, "; 
			
			valueFunctionString += $"{QueryBuilder.GetValueString(values[size])})"; 

			return valueFunctionString;
		}

		public static string GetValueFunctionString(Record data)
		{
			int size = data.Values.Length;
	
			string valueFunctionString = "VALUES(";

			for (int x = 0; x < size - 1; x++)
				valueFunctionString += $"{QueryBuilder.GetValueString(data.Values[x])}, ";
			valueFunctionString += $"{QueryBuilder.GetValueString(data.Values[size - 1])})";

			return valueFunctionString;
		}
		
		/// <summary>
		///	Gets the SQL SET function string. 
		/// </summary>
		/// <param name="record"> Record instance </param>
		/// <returns></returns>
		public static string GetSetString(Record record)
		{
			string setString = "SET ";
			
			int size = record.Keys.Length;
	
			for (int x = 0; x < size - 1; x++)
				setString += $"{record.Keys[x]}={QueryBuilder.GetValueString(record.Values[x])}, ";

			setString += $"{record.Keys[size - 1]}={QueryBuilder.GetValueString(record.Values[size - 1])}";
			
			return setString;
		}


		public static string GetColumnTuple(string[] strings)
		{
			string tuple = $"(";

			for (int x = 0; x < strings.Length - 1; x++)
				tuple += $"{strings[x]},";
			tuple += $"{strings[strings.Length - 1]})";

			return tuple;
		}

		public static string GetInsertQuery(string tableName, Hashtable data)
		{
			string queryString = null;

			queryString  = $"{QueryBuilder.CommandStrings[(int)QueryBuilder.Command.Insert]} INTO {tableName} {QueryBuilder.GetValueFunctionString(data)};";

			return queryString;
		}

		public static string GetInsertQuery(string tableName, Record data, bool specify = false)
		{
			return $"{QueryBuilder.CommandStrings[(int)QueryBuilder.Command.Insert]} INTO {tableName} {((specify) ? QueryBuilder.GetColumnTuple(data.Keys) : null)} {QueryBuilder.GetValueFunctionString(data)};";
		}
		
		/// <summary>
		/// Generates a query for updating the given records in the table.
		/// </summary>
		/// <param name="id">Primary key value.</param>
		/// <param name="tableName">Table name. </param>
		/// <param name="data"> Record values to be updated. </param>
		/// <returns> Generated query. </returns>
		public static string GetUpdateQuery(object id, string tableName, Record data) 
		{
			return $"UPDATE {tableName} {QueryBuilder.GetSetString(data)} WHERE ID={QueryBuilder.GetValueString(id)}";
		}
	}

}