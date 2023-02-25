using OpenQA.Selenium.DevTools.V106.Profiler;
using OpenQA.Selenium.DevTools.V108.Overlay;

namespace CourseDB;

public class Time : IComparable<Time>
{
    public int Hour;
    public int Minute;
    public int Second;

    public static Time FromHourString(string timeString)
    {
        string[] split = Tools.DivideString(timeString, 2);

        if (split.Length < 2)
            return new Time();
        
        for (int x = 0; x < split.Length; x++)
            if (split[x] == "00")
                split[x] = "0";
    
        return new Time(Convert.ToInt32(split[0]), Convert.ToInt32(split[1])); 
    }

    public static Time FromSQLString(string sqlString)
    {
        string[] splitStrings;
        
        return new Time(Convert.ToInt32((splitStrings = sqlString.Split(':'))[0]), 
            Convert.ToInt32(splitStrings[1]),
            Convert.ToInt32(splitStrings[2]));
    } 

    public int CompareTo(Time time)
    {
        int[,] timeCompArray = new int[,] {
            {
                time.Hour, time.Minute, time.Second
            },
            {
                this.Hour, this.Minute, this.Second 
            }
        };

        for (int x = 0; x < 3; x++)
            if (timeCompArray[0, x] > timeCompArray[1, x])
                return 1;
        
        return 0; 
    }

    public override string ToString()
    {
        return $"{this.Hour}:{this.Minute}:{this.Second}";
    }

    public Time(int hour = 0, int minute = 0, int second = 0)
    {
        this.Hour = hour;
        this.Minute = minute;
        this.Second = second;
    }
}
