using System.Diagnostics;
using System.Runtime.Serialization;
using HtmlAgilityPack;
using OpenDatabase;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V106.Network;
using OpenQA.Selenium.DevTools.V108.Audits;
using OpenQA.Selenium.DevTools.V108.CSS;
using OpenQA.Selenium.DevTools.V85.Overlay;

namespace CourseDB
{
    public class CourseScraper
    {
        protected HttpClient Client;

        public CourseManager Manager;
        
        protected string HTMLDump;

        public string BaseURL;

        protected Term _CourseTerm;
        
        public Term CourseTerm;
        
        public Dictionary<string, DateTime> LastSync;

        public HtmlDocument Document;
        
        public List<string[]> InnerStrings;

        public bool IsRunning;

        public void SetTerm(Term term)
        {
            this.CourseTerm = term;

            string path;

            this.BaseURL = $"https://swing.langara.bc.ca/prod/hzgkfcls.P_GetCrse?term_in={this.CourseTerm.ToString()}&sel_subj=dummy&sel_day=dummy&sel_schd=dummy&sel_insm=dummy&sel_camp=dummy&sel_levl=dummy&sel_sess=dummy&sel_instr=dummy&sel_ptrm=dummy&sel_attr=dummy&sel_dept=dummy&sel_crse=&sel_title=%25&sel_dept=%25&sel_ptrm=%25&sel_schd=%25&begin_hh=0&begin_mi=0&begin_ap=a&end_hh=0&end_mi=0&end_ap=a&sel_incl_restr=Y&sel_incl_preq=Y&SUB_BTN=Get+Courses";
            
            if (!File.Exists(path = $"dump_{term.ToString()}"))
            {
                this.FetchRawHTMLAsync();
                this.DumpHTML();
            }
                
            this.LoadHTML();
            this.Document.LoadHtml(this.HTMLDump);

            this.SyncDB();
        }
        
        protected void DumpHTML()
        {
            FileIO.Write(this.HTMLDump, $"dump_{this.CourseTerm.ToString()}");
        }

        protected void LoadHTML()
        {
            this.HTMLDump = FileIO.Read($"dump_{this.CourseTerm.ToString()}");
        }

        protected async Task FetchRawHTMLAsync()
        {
            this.HTMLDump = this.Client.GetAsync(this.BaseURL).Result.Content.ReadAsStringAsync().Result;
        }
        public List<string[]> GetInnerStrings(HtmlNode node)
        {
            HtmlNodeCollection collection = node.SelectNodes("//table/tr"),
                innerCollection;

            List<string[]> innerStrings = new List<string[]>();
            List<string> innerStringTemp = new List<string>();
            

            for (int x = 0; x < collection.Count; x++)
            {
                innerCollection = collection[x].SelectNodes("td");
                
                for (int y = 0; y < innerCollection.Count; y++)
                    innerStringTemp.Add(innerCollection[y] .InnerText);
                
                innerStrings.Add(innerStringTemp.ToArray());
                innerStringTemp.Clear();
            }

            return (this.InnerStrings = innerStrings);
        }
        
        protected static void PrintArray<T>(T[] array, string delimiter="\n")
        {
            for (int x = 0; x < array.Length; x++)
                Console.Write($"{x.ToString()}: {array[x]}{delimiter}");
        }

        protected bool IsRecord(string[] array) 
        {
            for (int x = 0; x < array.Length; x++)
                if (Tools.IsNumber(array[x]))
                    return true;
            return false;
        }

        public Course ParseNode(string[] splitArray)
        {
            if (splitArray.Length < 18 || !this.IsRecord(splitArray))
                return null;

            Time[] timeRange = Tools.GetTimeRange(splitArray[14]);

            List<int> ints = new List<int>();

            int temp;

            splitArray = Tools.BulkReplace(splitArray, "&nbsp;", " "); // removes all the nulls from entries

            for (int x = 0; x < splitArray.Length; x++) // Isolates the integers into one array.
            {
                if (Tools.IsNumber(splitArray[x]))
                {
                    Int32.TryParse(splitArray[x], out temp);
                    ints.Add(temp);
                }
                else if (splitArray[x] == "&nbsp;" || splitArray[x] == "-" || splitArray[x] == " " || splitArray[x] == "N/A")
                    ints.Add(0);
                
                else if (splitArray[x] == "Cancel")
                    ints.Add(-1);
            }

            double fees, credits;

            int index;

            Double.TryParse(
                splitArray[10].Substring(index = splitArray[10].IndexOf("$") + 1, splitArray[10].Length - index),
                out fees);
            Double.TryParse(splitArray[8], out credits);

            Course retCourse = new Course();
    
            
                if (ints[4] > 10000) // inconsistent record exception
                {
                Console.WriteLine($"fees: {fees}");
                    retCourse = new Course(this.CourseTerm.ToString(),
                        ints[1],
                        ints[2],
                        ints[4],
                        (splitArray[17] == " ") ? "TBA" : splitArray[17],
                        splitArray[5],
                        ints[5],
                        splitArray[7],
                        credits,
                        splitArray[9],
                        fees,
                        ints[6],
                        splitArray[12],
                        Tools.GetDaysFromScheduleString(splitArray[13]),
                        timeRange[0],
                        timeRange[1],
                        splitArray[18]
                    );
                }
                // else if (ints.Count < 7)
                // {
                //     
                // }
                else // normal behavior
                {
                    retCourse = new Course(this.CourseTerm.ToString(), ints[0],
                        ints[1],
                        ints[3],
                        (splitArray[17] == " ") ? "TBA" : splitArray[17],
                        splitArray[5],
                        ints[4],
                        splitArray[7],
                        credits,
                        splitArray[9],
                        fees,
                        ints[5],
                        splitArray[12],
                        Tools.GetDaysFromScheduleString(splitArray[13]),
                        timeRange[0],
                        timeRange[1],
                        splitArray[18]
                    );
                }


            return retCourse;
        }
        
        protected Course[] ParseNodes()
        {
            List<string[]> strings = this.GetInnerStrings(this.Document.DocumentNode);
            
            List<Course>  courses = new List<Course>();

            Course prev;

            for (int x = 0; x < strings.Count; x++)
                if ((prev = this.ParseNode(strings[x])) != null)
                    courses.Add(prev);

            return courses.ToArray();
        }

        public Course[] GetCourseList()
        {
            this.LoadHTML();
            
            if (this.HTMLDump == null)
            {
                this.FetchRawHTMLAsync();
                this.DumpHTML();
                this.LoadHTML();
            }
    
            this.Document.LoadHtml(this.HTMLDump);

            return this.ParseNodes();
        }
        
        public void SyncDB(bool update = true)
        {
            Course[] courses = this.GetCourseList();

            for (int x = 0; x < courses.Length; x++)
                if (courses[x] != null)
                    this.Manager.AddCourse(courses[x], false);

            if (update)
                this.Manager.UpdateDB();
            
            this.LastSync[this.CourseTerm.ToString()] = DateTime.Now;
        }

        public async Task Run()
        {
            this.IsRunning = true;
            await Task.Run(()=> {
                while (this.IsRunning)
                {
                    if (this.LastSync[this.CourseTerm.ToString()].CompareTo(DateTime.Now) <= 0)
                        this.SyncDB();
                }
            });
        }

        public void Terminate()
        {
            this.IsRunning = false;
        }

        public CourseScraper(Term term, DatabaseConfiguration config)
        {
            this.IsRunning = false;
            this.CourseTerm =  term;
            this.BaseURL = $"https://swing.langara.bc.ca/prod/hzgkfcls.P_GetCrse?term_in={this.CourseTerm.ToString()}&sel_subj=dummy&sel_day=dummy&sel_schd=dummy&sel_insm=dummy&sel_camp=dummy&sel_levl=dummy&sel_sess=dummy&sel_instr=dummy&sel_ptrm=dummy&sel_attr=dummy&sel_dept=dummy&sel_crse=&sel_title=%25&sel_dept=%25&sel_ptrm=%25&sel_schd=%25&begin_hh=0&begin_mi=0&begin_ap=a&end_hh=0&end_mi=0&end_ap=a&sel_incl_restr=Y&sel_incl_preq=Y&SUB_BTN=Get+Courses";
            this.Client = new HttpClient();
            this.Document = new HtmlDocument();

            this.Manager = new CourseManager(config);
            this.LastSync = new Dictionary<string,DateTime>();
        }
    }
}

 
  