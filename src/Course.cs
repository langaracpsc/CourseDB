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

        public Course(string term = null, int seats = 0, int waitlist = 0, int crn = 0, string location = null,
            string subject = null,
            int courseNumber = 0, string section = null, double credits = 0, string title = null, double fees = 0,
            int rptLimit = 0,
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
            this.Term = record.Values[1].ToString();
            this.Seats = int.Parse(record.Values[2].ToString());
            this.Waitlist = int.Parse(record.Values[3].ToString());
            this.CRN = int.Parse(record.Values[4].ToString());
            this.Location = record.Values[5].ToString();
            this.Subject = record.Values[6].ToString();
            this.CourseNumber = int.Parse(record.Values[7].ToString());
            this.Section = record.Values[8].ToString();
            this.Credits = double.Parse(record.Values[9].ToString());
            this.Title = record.Values[10].ToString();
            this.Fees = double.Parse(record.Values[11].ToString());
            this.RptLimit = int.Parse(record.Values[12].ToString());
            this.Type = record.Values[13].ToString();
            this.Instructor = record.Values[14].ToString();
            this.Schedule = Tools.GetDaysFromDayString(record.Values[15].ToString());
            this.StartTime = Time.FromSQLString(record.Values[16].ToString());
            this.EndTime = Time.FromSQLString(record.Values[17].ToString());
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
            for (int x = 0; x < this.Courses.Count; x++)
                try
                {
                    if (!this.CourseExists(this.Courses[x]))
                        this.Database.InsertRecord(this.Courses[x].ToRecord(), "Courses", true);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                }
        }

        public int SearchCourse(Course course)
        {
            for (int x = 0; x < this.Courses.Count; x++)
                if (this.Courses[x] == course)
                    return x;
            return -1;
        }

        public bool CourseExists(Course course)
        {
            string str;

            return (this.Database.FetchQueryData($"SELECT * FROM Courses WHERE {(str = Tools.ReplaceSubString(course.ToRecord().ToString(), ", ", " AND ")).Substring(1, str.Length - 2)}", "Courses").Length != 0);
        }

        /// <summary>
        /// Adds the provided course the cache and optionally updates the DB.
        /// </summary>:
        /// <param name="course">Course to be added.</param>
        /// <param name="updateDB">Represents if the DB should be updated.</param>
        public void AddCourse(Course course, bool updateDB = true)
        {
            if (!this.CourseExists(course))
            {
                this.Courses.Add(course);
               
                if (updateDB)
                    this.UpdateDB();
            }
        }

        public void CacheCourses(Term term)
        {
            OpenDatabase.Record[] courseRecords = this.Database.FetchQueryData($"SELECT * FROM Courses WHERE Term={term.ToString()}", "Courses");

            for (int x = 0; x < courseRecords.Length; x++)
                this.Courses.Add(new Course(courseRecords[x]));
        }

        public Course[] GetCoursesByTerm(string term)
        {
            Record[] records = this.Database.FetchQueryData($"SELECT * FROM Courses WHERE Term=\'{term}\'", "Courses");

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


