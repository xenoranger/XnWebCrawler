using System;
namespace XnWebCrawler
{
	public class Constants
	{


        // ----------------------------------------------------------------------
        // DRIVER CONSTANTS
        // ----------------------------------------------------------------------


        // Windows
        public const string FIREFOX_WIN_DRIVER_NAME = "geckodriver.exe";
        public const string CHROME_WIN_DRIVER_NAME = "chromedriver.exe";
        public const string IE_WIN_DRIVER_NAME = "IEDriverServer.exe";
        public const string MSEDGE_WIN_DRIVER_NAME = "MicrosoftWebDriver.exe";
        public const string MSEDGE_WIN_DRIVER_NAME_LEGACY = "msedgedriver.exe";
        public static readonly string[] DEFAULT_WIN_DRIVER_DIRECTORIES =  { "C:/ProgramData/SeleniumDrivers",
                                                                        "./Drivers",
                                                                        "../../../Drivers",
                                                                        "../../Drivers",
                                                                        "../Drivers"
        };

        // MAC
        public const string FIREFOX_MAC_DRIVER_NAME = "geckodriver";
        public const string CHROME_MAC_DRIVER_NAME = "chromedriver";
        public const string IE_MAC_DRIVER_NAME = "IEDriverServer";
        public const string MSEDGE_MAC_DRIVER_NAME = "MicrosoftWebDriver";
        public const string MSEDGE_MAC_DRIVER_NAME_LEGACY = "msedgedriver";
        public static readonly string[] DEFAULT_MAC_DRIVER_DIRECTORIES =  { "~/",
                                                                        "./Webdriver/Mac64",
                                                                        "../../../Webdriver/Mac64",
                                                                        "../../Webdriver/Mac64",
                                                                        "../Webdriver/Mac64"
        };


        //  Linux
        public const string FIREFOX_LIN_DRIVER_NAME = "geckodriver";
        public const string CHROME_LIN_DRIVER_NAME = "chromedriver";
        public const string IE_LIN_DRIVER_NAME = "IEDriverServer";
        public const string MSEDGE_LIN_DRIVER_NAME = "MicrosoftWebDriver";
        public const string MSEDGE_LIN_DRIVER_NAME_LEGACY = "msedgedriver";
        public static readonly string[] DEFAULT_LIN_DRIVER_DIRECTORIES =  { "~/",
                                                                        "./Webdriver/Linux",
                                                                        "../../../Webdriver/Linux",
                                                                        "../../Webdriver/Linux",
                                                                        "../Webdriver/Linux"
        };




        // ----------------------------------------------------------------------
        // WAITING CONSTANTS
        // ----------------------------------------------------------------------
        public const int DEFAULT_MINIMUM_WAIT_TIME = 3;
        public const int DEFAULT_MAXIMUM_WAIT_TIME = 7;


        // ----------------------------------------------------------------------
        // APP SETTINGS LOCATION
        // ----------------------------------------------------------------------
        public static readonly string[] APP_SETTINGS_PATH =
        {
                    "./",
                    "../",
                    "../../",
                    "~/",
                    "~/Development/GIT/insight/InsightWeb/XnWebCrawler/",
                    "/Users/carlosgarcia/Development/GIT/insight/InsightWeb/XnWebCrawler" //appsettings.json
        };



    }
}
