// See https:/i/aka.ms/new-console-template for more information

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenDatabase;

namespace CourseDB
{
    class Program
    {
        static void Main(string[] args)
        {
            CourseScraper scraper = new CourseScraper(Term.GetCurrent(),
                DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));
            
            Course[] courses = scraper.GetCourseList();
  
           scraper.SyncDB();
            
            // for (int x = 0; x < courses.Length; x++)
            //     if (courses[x] == null)
            //         Console.WriteLine($"courseNull");
           
           // Console.WriteLine(JsonConvert.SerializeObject(new Term(args[0])));
        } 
    } 
}
