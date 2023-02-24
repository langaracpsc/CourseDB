using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace OpenDatabaseAPI;

public class Table
{
    public string Name;
    
    public List<Field> Fields;

    public override string ToString()
    {
        string tableString = "(\n";

        int end = this.Fields.Count - 1;

        for (int x = 0; x < end; x++)
            tableString += $"{this.Fields[x].ToString()},\n";
        
        tableString += $"{this.Fields[end].ToString()})\n";

        return tableString;
    }

    public string GetCreateQuery()
    {
        return $"CREATE TABLE {this.Name}{this.ToString()}";
    }

    public Table(Field[] fields)
    {
        for (int x = 0; x < fields.Length; x++)
            this.Fields.Add(fields[x]);
    }
}