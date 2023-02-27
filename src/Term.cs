namespace CourseDB;

public enum TermType
{
    Spring,
    Summer,
    Fall
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

    protected static TermType GetCurrentTermType()
    {
        if (Tools.InRange(DateTime.Now.Month, 1, 5))
            return TermType.Spring;

        if (Tools.InRange(DateTime.Now.Month, 5, 9))
            return TermType.Summer;
        
        return TermType.Fall;
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
} 
