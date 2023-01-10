// See https:/i/aka.ms/new-console-template for more information
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V108.WebAudio;

namespace CourseDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Driver driver = new Driver(DriverType.Chromium, true);

            driver.OpenURL(args[0]);
        }
    }
}


