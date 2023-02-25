using System.Diagnostics;
using HtmlAgilityPack;
using OpenDatabase;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V85.Overlay;

namespace CourseDB
{
    public class CourseScraper
    {
        protected HttpClient Client;

        protected CourseManager Manager;
        
        protected string HTMLDump;

        public string BaseURL;

        public Term CourseTerm;

        public DateTime LastSync;
        
        protected static string DumpCacheFile = "HtmlTableDump";

        protected HtmlDocument Document;

        protected void DumpHTML()
        {
            FileIO.Write(this.HTMLDump, CourseScraper.DumpCacheFile);
        }

        protected void LoadHTML()
        {
            this.HTMLDump = FileIO.Read(CourseScraper.DumpCacheFile);
        }

        protected async Task FetchRawHTMLAsync()
        {
            this.HTMLDump = this.Client.GetAsync(this.BaseURL).Result.Content.ReadAsStringAsync().Result;
        }
        protected List<string[]> GetInnerStrings(HtmlNode node)
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

            return innerStrings;
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

        protected Course ParseNode(string[] entries)
        {
            string[] splitArray = entries;

            if (splitArray.Length < 18 || !this.IsRecord(splitArray))
                return new Course();
            
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
                else if (splitArray[x] == "&nbsp;" || splitArray[x] == "-")
                    ints.Add(0) ;
            }

            double fees, credits;

            int index;

            Double.TryParse(splitArray[10].Substring(index = splitArray[10].IndexOf("$") + 1, splitArray[10].Length - index), out fees);
            Double.TryParse(splitArray[8], out credits);

            return new Course(this.CourseTerm.ToString(), 
                ints[0],
                    ints[1],
                    ints[3],
                    splitArray[17],
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
        
        protected Course[] ParseNodes()
        {
            List<string[]> strings = this.GetInnerStrings(this.Document.DocumentNode);
            
            Course[] courses = new Course[strings.Count];

            Course prev;
            
            
            for (int x = 0; x < strings.Count; x++)
                if (!(prev = this.ParseNode(strings[x])).IsNull())
                    courses[x] = prev;

            return courses;
        }

        public Course[] GetCourseList()
        {
            this.LoadHTML();
            
            if (this.HTMLDump == null)
            {
                this.FetchRawHTMLAsync();
                this.DumpHTML();
            }
    
            this.Document.LoadHtml(this.HTMLDump);

            return this.ParseNodes();
        }

        public CourseScraper(Term term, DatabaseConfiguration config)
        {
            this.CourseTerm = term;
            this.BaseURL = $"https://swing.langara.bc.ca/prod/hzgkfcls.P_GetCrse?term_in={this.CourseTerm.ToString()}&sel_subj=dummy&sel_day=dummy&sel_schd=dummy&sel_insm=dummy&sel_camp=dummy&sel_levl=dummy&sel_sess=dummy&sel_instr=dummy&sel_ptrm=dummy&sel_attr=dummy&sel_dept=dummy&sel_crse=&sel_title=%25&sel_dept=%25&sel_ptrm=%25&sel_schd=%25&begin_hh=0&begin_mi=0&begin_ap=a&end_hh=0&end_mi=0&end_ap=a&sel_incl_restr=Y&sel_incl_preq=Y&SUB_BTN=Get+Courses";
            this.Client = new HttpClient();
            this.Document = new HtmlDocument();

            this.Manager = new CourseManager(config);
        }
    }
}

 
  