using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using AngleSharp.Text;
using Newtonsoft.Json;
using Npgsql.PostgresTypes;
using OpenQA.Selenium.DevTools.V85.CSS;

namespace CourseDB;

public class Tools
{
    public static bool InRange(long val, long min, long max)
    {
        return (val >= min && val <  max);
    }
 
    
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

    public static int AddArray(int[] ints)
    {
        int sum = 0;
        
        for (int x = 0; x < ints.Length; x++)
            sum += ints[x];
        
        return sum;
    }

    public static T[] CastArray<T>(object[] objects)
    {
        T[] casted = new T[objects.Length];

        for (int x = 0; x < objects.Length; x++)
            casted[x] = (T)objects[x];
            
        return casted;
    }

    /// <summary>
    /// Divides the provided string in the given ratio.
    /// </summary>
    /// <param name="str">String tobe divided.</param>
    /// <param name="parts">Ratio to divide the string in.</param>
    /// <returns> Divided string. </returns>
    public static string[] DivideString(string str, int[] parts)
    {
        string[] divided;

        if (Tools.AddArray(parts) > str.Length)
            return null;

        divided = new string[parts.Length];

        for (int x = 0, prev = 0; x < parts.Length; prev += parts[x], x++)
            divided[x] = str.Substring(prev, parts[x]);
        
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
        char[] nums = Tools.AsciiRange(48, 57);
        
        for (int x = 0; x < str.Length; x++)
            if (Tools.LinearSearch(str[x], nums) == -1)
                return false;
                
        return true;
    }

    
    public static Time[] GetTimeRange(string str)
    {
        string[] splitString = str.Split('-'); 
        
        //Console.WriteLine(splitString.Length);

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

    
    /// <summary>
    /// Replaces the specified substring with the provided delimiter. 
    /// </summary>
    /// <param name="str">Source string.</param>
    /// <param name="subStr">Substring to be replaced.</param>
    /// <param name="delimiter">Substring to replace with.</param>
    /// <returns>Modified string.</returns>
    public static string ReplaceSubString(string str, string subStr, string delimiter)
    {
        string altString = null;

        int x;
        
        if (str == subStr)
            return delimiter;
        
        for (x = 0; x < str.Length - subStr.Length; x++)
        {
            if (str.Substring(x, subStr.Length) == subStr)
            {
                altString += delimiter;
                x += subStr.Length;
            }
            
            altString += str[x];
        }

        for (; x < str.Length; x++)
            altString += str[x];

        return altString;
    }

    public static List<T> RemoveNulls<T>(List<T> objects)
    {
        List<T> nonNulls = new List<T>(); 
       
        for (int x = 0; x < objects.Count; x++)
            if (objects[x] != null)
                nonNulls.Add(objects[x]);

        return nonNulls;
    }

    public static Course[] GetCoursesFromObjects(object[] objects)
    {
        Course[] courses = new Course[objects.Length];

        for (int x = 0; x < objects.Length; x++)
            courses[x] = objects[x] as Course;
        return courses;
    }

    /// <summary>
    /// Replaces the substring in the provided strings with a delimiter.
    /// </summary>
    /// <param name="strings">Strings to be modified.</param>
    /// <param name="subStr">Substring to be replaced.</param>
    /// <param name="delimiter">Substring to replace with.</param>
    /// <returns>Modified strings.</returns>
    public static string[] BulkReplace(string[] strings, string subStr, string delimiter)
    {
        for (int x = 0; x < strings.Length; x++)
            strings[x] = Tools.ReplaceSubString(strings[x], subStr, delimiter);

        return strings;
    }

    public static string GetDaysString(DayOfWeek[] days)
    {
        string dayString = null;

            for (int x = 0; x < days.Length; x++)
            dayString += Convert.ToString((int)days[x]);

        return dayString;
    }

    
    /// <summary>
    ///  Converts the provided SQL day string to a DayOfWeek array.
    /// </summary>
    /// <param name="days"> day string </param>
    /// <returns></returns>
    public static DayOfWeek[] GetDaysFromDayString(string days)
    {
        List<DayOfWeek> dayStack = new List<DayOfWeek>();
        
        DayOfWeek day = 0;

        for (int x = 0; x < days.Length; x++)
            if (!dayStack.Contains(day = (DayOfWeek)(days[x] -  48)))
                dayStack.Add(day);

        return dayStack.ToArray();
    }

    public static void ListSwap<T>(ref List<T> list, int index, int index1)
    {
        T temp = list[index];
        list[index] = list[index1];
        list[index1] = temp; 
    }
}   
  
  