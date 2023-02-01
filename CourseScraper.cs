using System.Diagnostics;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V85.Overlay;

namespace CourseDB
{
    public class CourseScraper
    {
        protected HttpClient Client;

        protected string HTMLDump;

        public string BaseURL;

        public string Term;
        
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

        public Course[] GetCourseList()
        {
            this.LoadHTML();
            
            if (this.HTMLDump == null)
            {
                this.FetchRawHTMLAsync();
                this.DumpHTML();
            }
    
            this.Document.LoadHtml(this.HTMLDump);

            Console.WriteLine(this.HTMLDump);
            
            HtmlNodeCollection nodes = this.Document.DocumentNode.SelectNodes("//table/tr");

            for (int x = 0; x < nodes.Count; x++)
                Console.WriteLine(nodes[x].InnerText);
                
            return null;
        }

        public CourseScraper(string term)
        {
            this.BaseURL =
                "https://swing.langara.bc.ca/prod/hzgkfcls.P_GetCrse?term_in=202310&sel_subj=dummy&sel_day=dummy&sel_schd=dummy&sel_insm=dummy&sel_camp=dummy&sel_levl=dummy&sel_sess=dummy&sel_instr=dummy&sel_ptrm=dummy&sel_attr=dummy&sel_dept=dummy&sel_crse=&sel_title=%25&sel_dept=%25&sel_ptrm=%25&sel_schd=%25&begin_hh=0&begin_mi=0&begin_ap=a&end_hh=0&end_mi=0&end_ap=a&sel_incl_restr=Y&sel_incl_preq=Y&SUB_BTN=Get+Courses";
            this.Client = new HttpClient();
            this.Document = new HtmlDocument();
        }
    }
}
