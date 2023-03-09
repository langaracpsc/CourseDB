using System.Data;
using System.Runtime.CompilerServices;

namespace CourseDB;

public enum TermType
{
    Spring,
    Summer,
    Fall
}

public class InvalidTermStringException : Exception
{
    public InvalidTermStringException() : base("Provided term string is invalid.")
    {
    }
}

/// <summary>
/// Stores a pair of an year and term
/// </summary>
public class Term
{
    public int Year;

    public TermType Type;

    public override string ToString()
    {
        return $"{this.Year}{((int)this.Type + 1) * 10}";
    }

    protected static TermType GetTermType(int term)
    {
        if (term > 30)
            return TermType.Fall;
        if (term < 10)
            return TermType.Summer;
        
        return (TermType)((int)(term / 10) - 1);
    }

    protected static TermType GetCurrentTermType()
    {
        if (Tools.InRange(DateTime.Now.Month, 1, 5))
            return TermType.Spring;

        if (Tools.InRange(DateTime.Now.Month, 5, 9))
            return TermType.Summer;
        
        return TermType.Fall;
    }

    public static Term FromTermString(string termString)
    {
        string[] divided = Tools.DivideString(termString, new int[]{ 4, 2 });

        return new Term(int.Parse(divided[0]), Term.GetTermType(int.Parse(divided[1])));
    }

    public static Term GetCurrent()
    {
        return new Term(DateTime.Now.Year, Term.GetCurrentTermType());
    }

    public Term(int year, TermType type)
    {
        this.Year = year;
        this.Type = type;
    }

    public Term(string str)
    {
        if (!Tools.InRange(str.Length, 0, 7))
            throw new InvalidTermStringException();

        string[] dividedString = Tools.DivideString(str, new int[] {4, 2});

        this.Type = Term.GetTermType(int.Parse(dividedString[1]));

        this.Year = int.Parse(dividedString[0]);
    }
} 
