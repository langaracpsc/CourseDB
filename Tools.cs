using System.Collections.Immutable;
using System.Security.AccessControl;
using AngleSharp.Text;
using Npgsql.PostgresTypes;
using OpenQA.Selenium.DevTools.V85.CSS;

namespace CourseDB;

public class Tools
{
    public static bool IsElement<T>(T val, T[] array)
    {
        for (int x = 0; x < array.Length; x++)
            if (val.Equals(array[x]))
                return true;
        return false;
    }

    public static int LinearSearch<T>(T val, T[] array)
    {
        for (int x = 0; x < array.Length; x++)
            if (val.Equals(array[x]))
                return x;
        
        return -1;
    }

    public static int BinarySearch<T>(IComparable<T> value, T[] array, int start, int end)
    {   
        int mid  = (start + end) / 2;

        string str = null;
        
        if (start < end)
        {
            if (value.Equals(array[mid]))
                return mid;

            if (value.CompareTo(array[mid]) < 0)
                return Tools.BinarySearch<T>(value, array, mid + 1, end);
            
            if (value.CompareTo(array[mid]) > 0)
                return Tools.BinarySearch<T>(value, array, start, mid);
        }

        return -1;
    }

    public static string[] DivideString(string str, int part)
    {
        int parts;
        string[] divided = new string[parts = (str.Length / part)];
        
        for (int x = 0; x < parts; x++)
            divided[x] = str.Substring(x * part, part);
        
        return divided;
    }

    public static char[] AsciiRange(int start, int end)
    {
        List<char> ascii = new List<char>();
        
        for (int x = start; x <= end; x++)
            ascii.Add(Convert.ToChar(x));

        return ascii.ToArray(); 
    }

    public static bool IsNumber(string str)
    {
        char[] nums = Tools.AsciiRange(48, 56);
        
        for (int x = 0; x < str.Length; x++)
            if (Tools.LinearSearch(str[x], nums) == -1)
                return false;
                
        return true;
    }

    public static Time[] GetTimeRange(string str)
    {
        string[] splitString = str.Split('-');
        
        Console.WriteLine(splitString.Length);

        return new Time[]
        {
            Time.FromHourString(splitString[0]),
            Time.FromHourString(splitString[1])
        };
    }

    public static DayOfWeek[] GetDaysFromScheduleString(string str)
    {
        List<DayOfWeek> days = new List<DayOfWeek>();

        int index;
        
        for (int x = 0; x < str.Length; x++)
            if ((index = Tools.LinearSearch<char>(str[x], new char[]{ 'M', 'T', 'W', 'R', 'F' })) != -1)
                days.Add((DayOfWeek)(index + 1));
            
        return days.ToArray();
    }
}

