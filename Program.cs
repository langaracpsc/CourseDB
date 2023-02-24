// See https:/i/aka.ms/new-console-template for more information

using System.Globalization;
using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V108.WebAudio;
using HtmlAgilityPack;
using Newtonsoft.Json;
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
                    innerStringTemp.Add(innerCollection[y].InnerText);
                
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

        static Course ParseNode(string[] entries)
        {

            //      Console.WriteLine(node);
            string[] splitArray = entries; // node.Split("\n");
            Program.PrintArray<string>(splitArray);

            Time[] timeRange = Tools.GetTimeRange(splitArray[14]);

            int[] ints = new int[6];

            //splitArray = Tools.BulkReplace(splitArray, "&nbsp", " ");

            for (int x = 0, count = 0; x < splitArray.Length; x++)
            {
                //if (x == 7) 
                    //continue;
                
                if (Tools.IsNumber(splitArray[x]))
                    Int32.TryParse(splitArray[x], out ints[count++]);
            }

            Program.PrintArray<int>(ints);
            double fees, credits;

            int index;

            Console.WriteLine($"10: {splitArray[10]}; index:{splitArray[10].IndexOf('$')}");
            Double.TryParse(splitArray[10].Substring(index = splitArray[10].IndexOf("$") + 1, splitArray[10].Length - index), out fees);
            Double.TryParse(splitArray[8], out credits);
            
            
            return new Course(Term.GetCurrent().ToString(), ints[0],
                    ints[1],
                    ints[2],
                    splitArray[17],
                    splitArray[6],
                    ints[3],
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
        
        static Course[] ParseNodes(string[] nodes)
        {
            string[] splitArray;

            Course[] courses = new Course[nodes.Length];
                
            for (int x = 0; x < nodes.Length; x++)
            {
                //splitArray = nodes[x].Split("\n");
                try
                {
                    //courses[x] = ParseNode(nodes[x]);
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
            // CourseScraper scaper = new CourseScraper("202310");
            //scaper.GetCourseList();
            HtmlDocument document = new HtmlDocument();
            //
            string dump = FileIO.Read("HtmlTableDump");
            //
            document.LoadHtml(dump);
            List<string[]> htmlStrings = Program.GetInnerStrings(document.DocumentNode);
            //string jsonString = JsonConvert.SerializeObject(Program.ParseNodes(htmlStrings));

          
           // FileIO.Write(jsonString, "courseDump.json");
            
            Console.WriteLine(Program.ParseNode(htmlStrings[Convert.ToInt32(args[0])]).ToJson());

            //Console.WriteLine(Tools.ReplaceSubString(args[0], args[1], args[2]));
         }
    }
}
