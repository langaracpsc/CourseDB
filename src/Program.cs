// See https:/i/aka.ms/new-console-template for more information

using System.Globalization;
using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V108.WebAudio;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenDatabase;
using OpenQA.Selenium.DevTools.V106.Runtime;
using OpenQA.Selenium.DevTools.V107.Console;

namespace CourseDB
{
    class Program
    {
        public static string BaseURL = "";

        static List<string[]> GetInnerStrings(HtmlNode node)
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

        static void PrintArray<T>(T[] array, string delimiter="\n")
        {
            for (int x = 0; x < array.Length; x++)
                Console.Write($"{x.ToString()}: {array[x]}{delimiter}");
        }

        
        static Course[] ParseNodes(string[] nodes)
        {
            string[] splitArray;

            Course[] courses = new Course[nodes.Length];
                
            for (int x = 0; x < nodes.Length; x++)
            {
                try
                {
                    Console.WriteLine(courses[x].ToJson());
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            return courses;
        }
        static void Main(string[] args)
        {
            CourseScraper scaper = new CourseScraper(Term.GetCurrent(), DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));

            Course[] courses = scaper.GetCourseList();


            //FileIO.Write(JsonConvert.SerializeObject(courses), "courseDump.json");
            
            
            Console.WriteLine(JsonConvert.SerializeObject(courses[57].ToRecord()));
        }
    }
}



