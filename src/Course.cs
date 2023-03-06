using System; 
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
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

        public string Term;

        public int CourseNumber;

        public string Section;

        public double Credits;

        public string Title;

        public double Fees;

        public int RptLimit;

        public string Type;

        public string Instructor;

        public DayOfWeek[] Schedule;

        public Time StartTime;

        public Time EndTime;

        public Course FromRawString(string str)
        {
            return new Course();
        }
        
        public Record ToRecord()
        {
            return new Record(new string[]
            {
                "Term",
                "Seats",
                "Waitlist",
                "CRN",
                "Room",
                "Subj",
                "CourseNumber",
                "Section",
                "Credits",
                "Title",
                "Fees",
                "RptLimit",
                "CourseType",
                "Instructor",
                "Schedule",
                "StartTime",
                "EndTime"
            }, new object[]
            {
                this.Term.ToString(),
                this.Seats,
                this.Waitlist,
                this.CRN,
                this.Location,
                this.Subject,
                this.CourseNumber,
                this.Section,
                this.Credits,
                this.Title,
                this.Fees,
                this.RptLimit,
                this.Type,
                this.Instructor,
                Tools.GetDaysString(this.Schedule),
                this.StartTime.ToString(),
                this.EndTime.ToString()
            });
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool IsNull()
        {
            return (this.Term == null &&
                    this.Seats == 0 &&
                    this.Waitlist == 0 &&
                    this.CRN == null &&
                    this.Location == null &&
                    this.Subject == null &&
                    this.CourseNumber == 0 &&
                    this.Section == null &&
                    this.Credits == 0 &&
                    this.Title == null &&
                    this.Fees == 0 &&
                    this.RptLimit == 0 &&
                    this.Type == null &&
                    this.Instructor == null &&
                    this.Schedule == null &&
                    this.StartTime == null &&
                    this.EndTime == null);
        }

    public Course(string term = null, int seats = 0, int waitlist = 0, int crn = 0, string location = null, string subject = null,
            int courseNumber = 0, string section = null, double credits = 0, string title = null, double fees = 0, int rptLimit = 0,
            string type = null, DayOfWeek[] schedule = null, Time startTime = null,
            Time endTime = null, string instructor = null)
        {
            this.Term = term;
            this.Seats = seats;
            this.Waitlist = waitlist;
            this.CRN = crn;
            this.Schedule = schedule;
            this.Location = location;
            this.Subject = subject;
            this.CourseNumber = courseNumber;
            this.Section = section;
            this.Credits = credits;
            this.Title = title;
            this.RptLimit = rptLimit;
            this.Type = type;
            this.Fees = fees;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Instructor = instructor;
            //Console.WriteLine(this.IsNull());
        }
    }

    public class CourseManager
    {
        public List<Course> Courses;

        public PostGRESDatabase Database;

        public DatabaseConfiguration DatabaseConfig;

        /// <summary>
        /// Syncs the courses in the cache with the ones in the DB 
        /// </summary>
        public void UpdateDB()
        {
            //if (this.Database.FetchQueryData("SELECT * FROM Courses;", "Courses").Length == 0)
                for (int x = 0; x < this.Courses.Count; x++)
                    try
                    {
                        this.Database.InsertRecord(this.Courses[x].ToRecord(), "Courses", true);
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine($"Null at {x}");
                    }
                // else
            //     for (int x = 0; x < this.Courses.Count; x++)
            //         this.Database.UpdateRecord(this.Courses[x].CRN.ToString(), this.Courses[x].ToRecord(), "");
        }
           
        /// <summary>
        /// Adds the provided course the cache and optionally updates the DB.
        /// </summary>
        /// <param name="course">Course to be added.</param>
        /// <param name="updateDB">Represents if the DB should be updated.</param>
        public void AddCourse(Course course, bool updateDB = true)
        {
            this.Courses.Add(course);
            
            if (updateDB)
                this.UpdateDB();
        }
        
        public CourseManager(DatabaseConfiguration databaseConfig)
        {
            this.Courses = new List<Course>(); 
            this.DatabaseConfig = databaseConfig;
            this.Database = new PostGRESDatabase(this.DatabaseConfig);
            this.Database.Connect();
        }
    }
}
 
 