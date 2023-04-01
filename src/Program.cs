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
            // CourseScraper scraper = new CourseScraper(Term.FromTermString(args[0]),
            //     DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));
            //
        
            // scraper.GetCourseList();
            // scraper.SyncDB();
            
            
            // scraper.Manager.CacheCourses(Term.FromTermString(args[0]));
        
            //scraper.CourseTerm = Term.FromTermString(args[0]);
        
            // Course[] courses = scraper.Manager.GetCoursesByTerm(Term.GetCurrent().ToString());
            //
            // Console.WriteLine(JsonConvert.SerializeObject(courses[1].Schedule)); 
            //
            // Console.WriteLine(scraper.Manager.CourseExists(courses[1]));//scraper.Manager.AddCourse(courses[x], false);
        
            // scraper.CourseTerm = Term.FromTermString("202320");
        
            //      scraper.SetTerm(Term.FromTermString(args[0]));


        //     Console.WriteLine(new KeyComparisonPair("Value", int.Parse(args[0]), Operator.Like).ToString());
        //     
        //     Console.WriteLine(new KeyComparisonPair("Value", "%pattern%", Operator.Like).ToString());
        // } 
    } 
}

