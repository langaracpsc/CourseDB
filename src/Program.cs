// See https:/i/aka.ms/new-console-template for more information

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenDatabase;
using OpenQA.Selenium.DevTools.V106.CSS;
using OpenQA.Selenium.DevTools.V106.Runtime;

namespace CourseDB
{
    // class Program
    // {
    //      static void Main(string[] args)
    //      {
    //         CourseScraper scraper = new CourseScraper(Term.FromTermString(args[0]),
    //             DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));
    //         //
    //         //
    //         scraper.GetCourseList();
    //         //  scraper.SyncDB();
    //         //
    //         //
    //         //  scraper.Manager.CacheCourses(Term.FromTermString(args[0]));
    //         //
    //         // scraper.CourseTerm = Term.FromTermString(args[0]);
    //         //
    //         //  Course[] courses = scraper.Manager.GetCoursesByTerm(Term.GetCurrent().ToString());
    //         //
    //         //  Console.WriteLine(JsonConvert.SerializeObject(courses[1].Schedule)); 
    //         //
    //         //  Console.WriteLine(scraper.Manager.CourseExists(courses[1]));//scraper.Manager.AddCourse(courses[x], false);
    //         //
    //         //  scraper.CourseTerm = Term.FromTermString("202320");
    //         //
    //         //       scraper.SetTerm(Term.FromTermString(args[0]));
    //         //
    //      
    //         Dictionary<string, object> queryMap = new Dictionary<string, object>();
    //             
    //         //queryMap.Add("Term", "20232%");
    //         queryMap.Add("CourseNumber", 1050);
    //         //queryMap.Add("Subj", "CPS%");
    //         
    //         // Console.Error.Errorwnew KeyComparisonPair("someInt",  11, Operator.Like)
    //     
    //         Console.WriteLine(JsonConvert.SerializeObject(scraper.Manager.GetCoursesByQueryMatch(queryMap)));
    //         
    //         Console.WriteLine(new KeyComparisonPair("Value", 5, Operator.Like));    
    //     } 
    // } 
}

