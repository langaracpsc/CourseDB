using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDatabaseAPI;

public enum FieldType
{
    Bool,
    Int,
    Float,
    Char,
    VarChar
}

public enum Flag
{
    NotNull,
    PrimaryKey
}

public struct Field
{
    public string Name;
    public FieldType Type;

    public int Size;
    
    public List<Flag> Flags;

    public static string[] FieldTypeStrings = new string[] {
        "BOOL",
        "INT",
        "FLOAT",
        "DOUBLE",
        "CHAR",
        "VARCHAR"
    };

    public static string[] FlagStrings = new string[] {
        "NOT NULL",
        "PRIMARY KEY"
    };

    public void AddFlag(Flag flag)
    {
        this.Flags.Add(flag);
    }

    public override string ToString()
    { 
        string size = (this.Size > 0) ? Convert.ToString(this.Size) : "";

        string flagString = null;

        for (int x = 0; x < this.Flags.Count; x++)
            flagString += $"{Field.FlagStrings[x]}";

        return $"{this.Name} {Field.FieldTypeStrings[(int)this.Type]} {Size} {flagString}";
    }

    public Field(string name, FieldType type, Flag[] flags, int size = 0)
    {
        this.Name = name;
        this.Type = type;
        this.Size = size;

        this.Flags = new List<Flag>();

        for (int x = 0; x < flags.Length; x++)
            this.Flags.Add(flags[x]);
        
    }
} 
