using System; 
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using OpenDatabase;
using OpenDatabaseAPI;
using Newtonsoft.Json;
using OpenQA.Selenium.DevTools.V106.Overlay;

namespace CourseDB
{
    public class Course
    {
        public int Seats;

        public int Waitlist;

        public int CRN;

        public string Location;

        public string Subject;

        public int CourseNumber;

        public string Section;

        public double Credits;

        public string Title;

        public double Fees;

        public int RptLimit;

        public string Type;

        public string Instructor;

        public string Room;
        
        public DayOfWeek[] Days;

        public Time StartTime;

        public Time EndTime;

        public Course FromRawString(string str)
        {
            return new Course();
        }

        public Record ToRecord()
        {
            return new Record(new string[] {
                
            }, new object[] {
                
            });
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Course(int seats = 0, int waitlist = 0, int crn = 0, string location = null, string subject = null,
            int courseNumber = 0, string section = null, double credits = 0, string title = null, double fees = 0, int rptLimit = 0,
            string type = null, DayOfWeek[] days = null, Time startTime = null,
            Time endTime = null, string room = null, string instructor = null)
        {
            this.Seats = seats;
            this.Waitlist = waitlist;
            this.CRN = crn;
            this.Location = location;
            this.Subject = subject;
            this.CourseNumber = courseNumber;
            this.Section = section;
            this.Credits = credits;
            this.Title = title;
            this.Days = days;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Instructor = instructor;
        }
    }

    public class CourseManager
    {
        public List<Course> Courses;

        public PostGRESDatabase Database;

        //public DatabaseConfiguration DatabaseConfig;

        public void UpdateCache()
        {
        }

        public void AddCourse(Course course, bool updateDB = true)
        {
            this.Courses.Add(course);
        }
    }
}
