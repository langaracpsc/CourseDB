using System; 
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Text.Json.Serialization;
using OpenDatabase;
using OpenDatabaseAPI;
using Newtonsoft.Json;
using OpenQA.Selenium.DevTools.V106.BackgroundService;
using OpenQA.Selenium.DevTools.V106.Overlay;
using OpenQA.Selenium.DevTools.V108.Audits;
using RecordingStateChangedEventArgs = OpenQA.Selenium.DevTools.V107.BackgroundService.RecordingStateChangedEventArgs;
using SetShowFPSCounterCommandResponse = OpenQA.Selenium.DevTools.V108.Overlay.SetShowFPSCounterCommandResponse;

namespace CourseDB
{
    public class Course
    {
        public int ID;
        
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

        public override bool Equals(object? obj)
        {
            Course course = obj as Course;
            
            return (this.Term == course.Term &&
                    this.Seats == course.Seats && 
                    this.Waitlist == course.Waitlist &&
                    this.CRN == course.Waitlist &&
                    this.Location == course.Location && 
                    this.Subject == course.Subject &&
                    this.Section == course.Section &&
                    this.Credits == course.Credits &&
                    this.Title == course.Title && 
                    this.Fees == course.Fees &&
                    this.RptLimit == course.RptLimit &&
                    this.Type == course.Type &&
                    this.Instructor == course.Instructor &&
                    this.Schedule == course.Schedule &&
                    this.StartTime == course.StartTime &&
                    this.EndTime == course.EndTime);
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

        public Course(Record record)
        {
            string[] valueStrings = Tools.CastArray<string>(record.Values);
            
            this.Term = valueStrings[0];
            this.Seats = int.Parse(valueStrings[1]);
            this.Waitlist = int.Parse(valueStrings[2]);
            this.CRN = int.Parse(valueStrings[3]);

            this.Location = valueStrings[4];
            this.Subject = valueStrings[5];

            this.CourseNumber = int.Parse(valueStrings[6]);
            this.Section = valueStrings[7];
            this.Credits = int.Parse(valueStrings[8]);
            this.Title = valueStrings[9];
            this.Fees = int.Parse(valueStrings[10]);

            this.RptLimit = int.Parse(valueStrings[11]);
            this.Type = valueStrings[12];
            this.Instructor = valueStrings[13];
            this.Schedule = Tools.GetDaysFromDayString(valueStrings[14]);
            this.StartTime = Time.FromSQLString(valueStrings[15]);
            this.EndTime = Time.FromSQLString(valueStrings[15]);
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

        public int SearchCourse(Course course)
        {
            for (int x = 0; x < this.Courses.Count ; x++)
                if (this.Courses[x] == course)
                    return x;
            return -1;
        }

        public bool CourseExists(Course course)
        {
            return (this.SearchCourse(course) != -1);
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

        public Course[] GetCoursesByTerm(string term)
        {
            Record[] records = this.Database.FetchQueryData($"SELECT * FROM Courses WHERE Term={term.ToString()}", "Courses");

            Course[] courses = new Course[records.Length];

            for (int x = 0; x < courses.Length; x++)
                courses[x] = new Course(records[x]); 
            
            return courses;
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
 
 