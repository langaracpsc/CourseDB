namespace CourseDB;
public struct TimeDifference : IComparable<TimeDifference>
{
    public int Years; 
    public int Months;
    public int Days;
    public int Hours;
    public int Minutes;
    public int Seconds;
    
    public int CompareTo(TimeDifference difference)
    {
        int[,] timeBuffers = new int[2,6]
        {
            { this.Years, this.Months, this.Days, this.Hours, this.Minutes, this.Seconds },
            { difference.Years, difference.Months, difference.Days, difference.Hours, difference.Minutes, difference.Seconds  }
        };

        for (int x = 0; x < 6; x++)
            if (timeBuffers[0, x] < timeBuffers[1, x])
                return -1;
            else if (timeBuffers[0, x] > timeBuffers[1, x])
                return 1;
        
        return 0;
    }

    public TimeDifference(int years = 0, int months = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
    {
        this.Years = years;
        this.Months = months;
        this.Days = days;
        this.Hours = hours;
        this.Minutes = minutes;
        this.Seconds = seconds;
    }
} 
