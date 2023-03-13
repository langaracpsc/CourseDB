// See https:/i/aka.ms/new-console-template for more information

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenDatabase;
using OpenQA.Selenium.DevTools.V106.CSS;

namespace CourseDB
{
    class Program
    {
        // static void Main(string[] args)
        // {
        //     CourseScraper scraper = new CourseScraper(Term.GetCurrent(),
        //         DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));
        //     
        //     //scraper.CourseTerm = Term.FromTermString(args[0]);
        //     
        //     Course[] courses = scraper.Manager.GetCoursesByTerm(Term.GetCurrent().ToString());
        //     
        //     Console.WriteLine(JsonConvert.SerializeObject(courses[1].Schedule)); 
        //     
        //     
        //     
        //     Console.WriteLine(scraper.Manager.CourseExists(courses[1]));//scraper.Manager.AddCourse(courses[x], false);
        //     
        //
        //     scraper.SyncDB();
        // } 
    } 
}

