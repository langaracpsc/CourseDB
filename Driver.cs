using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;

namespace CourseDB
{
    public enum DriverType
    {
        Chromium,
        Gecko,
        WebKit,
        Phantom
    }

    public class Driver
    {
        protected IWebDriver DriverInstance;

        public DriverType Type;

        public bool IsHeadless; 
        

        protected static IDriverConfig GetDriverConfig(DriverType type)
        {
            switch (type)
            {
                case DriverType.Chromium:
                    return new ChromeConfig();
                case DriverType.Gecko:
                    return new FirefoxConfig();
                
                case DriverType.Phantom:
                    return new PhantomConfig();
            }

            return new ChromeConfig();
        }

        protected static IWebDriver GetDriver(DriverType type, bool headless = false)
        {
            switch (type)
            {
                case DriverType.Chromium:
                    ChromeOptions options = new ChromeOptions();
                    
                    if(headless)
                        options.AddArgument("headless");
                    
                    return new ChromeDriver(options);
                
                case DriverType.Gecko:
                    return new FirefoxDriver();
                
                case DriverType.WebKit:
                    return new SafariDriver();
            }

            return new ChromeDriver();
        }

        public void OpenURL(string url)
        {
            this.DriverInstance.Navigate().GoToUrl(url); 
        }

        public void Initialize()
        {
            new DriverManager().SetUpDriver(Driver.GetDriverConfig(this.Type));
            this.DriverInstance = Driver.GetDriver(this.Type, this.IsHeadless);
        }

        public Driver(DriverType type = DriverType.Chromium, bool isHeadless = false)
        {
            this.Type = type;
            this.IsHeadless = isHeadless;
            this.Initialize(); 
        }

        ~Driver()
        {
            this.DriverInstance.Quit();
        }
    }
}