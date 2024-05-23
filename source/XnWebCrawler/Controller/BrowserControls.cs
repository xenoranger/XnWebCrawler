using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;

namespace XnWebCrawler.Controllers
{
	public class BrowserControls
	{
		private string logMessage = "";
		private IWebDriver driver { get; set; }
		private string browserName { get; set; }
		private string driverFileName { get; set; }
		private string driverFilePath { get; set; }
		private string elementSelector { get; set; }
		private string locatorStrategy { get; set; }
		private By elementLocator { get; set; }
		public IWebElement webElement { get; set; }
		public List<IWebElement> webElements { get; set; }

		LogController logger = new LogController();

		public BrowserControls()
		{

		}

		public IWebDriver GetDriver()
		{
			return driver;
		}


		//  ---------------------------------------------------------------
		//  CREATING A SESSION
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region CreateSession

		/// <summary>
		/// <para>Returns a String for the filename of the WebDriver</para>
		/// <para>browserName = The name of the browser
		/// <br>---Gecko = FF or FireFox</br>
		/// <br>---Chrome = Google or Chrome</br>
		/// <br>---IE = IE or IExplore</br>
		/// <br>---MSEdge = Edge or MSEdge</br></para>
		/// </summary>
		/// <returns>String value for the driver's file name</returns>
		private void SetDriverFileName()
		{
			string funcName = MethodBase.GetCurrentMethod().Name;

			switch (browserName.ToLower())
			{
				case "ff":
				case "firefox":
					driverFileName = Constants.FIREFOX_MAC_DRIVER_NAME;
					break;
				case "chrome":
				case "google":
					driverFileName = Constants.CHROME_MAC_DRIVER_NAME;
					break;
				case "ie":
				case "iexplore":
					driverFileName = Constants.IE_MAC_DRIVER_NAME;
					break;
				case "edge":
				case "msedge":
					driverFileName = Constants.MSEDGE_MAC_DRIVER_NAME;
					break;
				default:
					string LogMsg = "The Browser Provided does not match an acceptable value.";
					logger.Write(LogMsg, funcName);
					throw new Exception(LogMsg);

			}

		}


		/// <summary>
		/// Returns a String for the full path to the Driver File
		/// <para>DriverFileName = The name of the Driver File (IE: Geckodriver.exe)</para>
		/// </summary>
		/// <param name="driverFileName"></param>
		/// <returns></returns>
		private void SetDriverFilePath()
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			bool isFoundDriverDirPath = false;
			string fullDriverFilePath;


			foreach (string driverDirPathItem in Constants.DEFAULT_MAC_DRIVER_DIRECTORIES)
			{
				if (Directory.Exists(driverDirPathItem))
				{
					isFoundDriverDirPath = true;
					logger.Write($"Driver Path Found:\t{driverFilePath}", funcName);
					fullDriverFilePath = driverDirPathItem + "/" + driverFileName;

//					RenameEdgeDriver(driverDirPathItem);

					if (File.Exists(fullDriverFilePath))
					{
						logger.Write($"Driver File Found:\t{fullDriverFilePath}", funcName);
						driverFilePath = driverDirPathItem + "/";
						return;
					}
				}
			}
			if (!isFoundDriverDirPath)
			{
				logMessage = "EXCEPTION:\n\tDriver Directory Cannot Be Found";
				logger.Write(logMessage, funcName);
				throw new Exception(logMessage);
			}

			logMessage = "EXCEPTION:\n\tDriver File Cannot Be Found";
			logger.Write(logMessage, funcName );
			throw new Exception(logMessage);
		}


		/// <summary>
		/// <para>Creates the IWebDriver session based on the browser selected</para>
		/// <para>browserName = The name of the browser
		/// <br>---Gecko = FF or FireFox</br>
		/// <br>---Chrome = Google or Chrome</br>
		/// <br>---IE = IE or IExplore</br>
		/// <br>---MSEdge = Edge or MSEdge</br></para>
		/// </summary>
		/// <param name="browserName"></param>
		/// <returns></returns>
		private IWebDriver CreateSession()
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			try
			{
				switch (browserName.ToLower())
				{
					case "ff":
						return new FirefoxDriver(driverFilePath);

					case "firefox":
						return new FirefoxDriver(driverFilePath);
					case "chrome":
						return new ChromeDriver(driverFilePath);
					case "google":
						return new ChromeDriver(driverFilePath);
					case "ie":
						return new InternetExplorerDriver(driverFilePath);
					case "iexplore":
						return new InternetExplorerDriver(driverFilePath);
					case "edge":
						return new EdgeDriver(driverFilePath);
					case "msedge":
						return new EdgeDriver(driverFilePath);
					default:
						string LogMsg = "Unable to Locate WebDriver";
						logger.Write(LogMsg, funcName);
						throw new Exception(LogMsg);
				}
			}
			catch (Exception e)
			{
				logMessage = $"Error while Attempting to Create Session\n{e}";
				logger.Write(logMessage, funcName);
				throw new Exception(logMessage);
			}
		}


		/// <summary>
		/// Instantiates the class and Opens the browser session 
		/// <para><br>inBrowserName = the name of the browser to open</br>
		/// <br>-Options:</br>
		/// <br>---FireFox, FF</br>
		/// <br>---Google, Chrome</br>
		/// <br>---IE, IExplore</br>
		/// <br>---Edge, MSEdge</br>
		/// </para>
		/// </summary>
		/// <param name="inBrowserName"></param>

		public BrowserControls(string inBrowserName)
		{
			string funcName = MethodBase.GetCurrentMethod().Name;

			logger.Write("Opening Browser", funcName);
			this.browserName = inBrowserName;

			try
			{
				SetDriverFileName();

				SetDriverFilePath();

				driver = CreateSession();

				driver.Manage().Window.Maximize();
			}
			catch (Exception e)
			{
				logMessage = $"Error while attempting to create the Browser Session\n{e}";
				logger.Write(logMessage, funcName);
				throw new Exception(logMessage);
			}
		}


		#endregion


		//  ---------------------------------------------------------------
		//  Navigating To pages
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------
		#region PageNavigation

		/// <summary>
		/// Navigates to the specified URL
		/// <para><br>goToURL = The URL to navigate to.</br>
		/// <br>retryAttempts = Number of Times to Retry loading the page (Default 0)</br>
		/// <br>waitInSec = The number of seconds to wait before reloading the page.(Default 20) </br></para>
		/// </summary>
		/// <param name="goToURL"></param>
		/// <param name="retryAttempts"></param>
		/// <param name="waitInSec"></param>
		public void NavTo(string goToURL, int retryAttempts = 0, int waitInSec = 20, bool hasHumanWait = true)
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			string ReadyState = "";
			int pageLoadTimeInSeconds = 0;
			DateTime startTime = DateTime.Now;

			driver.Url = goToURL;
			for (int retryCount = retryAttempts + 1; retryCount >= 0; retryCount--)
			{
				driver.Navigate();

				for (int i = 0; i < waitInSec; i++)
				{
					try
					{
						Thread.Sleep(1000);
						ReadyState = (string)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState;");
						logger.Write($"Ready State:\t{ReadyState}", funcName);
						if (ReadyState.ToLower() == "complete") break;
					}
					catch (Exception e)
					{
						string LogMsg = $"\nEXCEPTION HAS OCCURRRED\n\t{e}";
						logger.Write(LogMsg, funcName);
						throw new Exception(LogMsg);
					}
					pageLoadTimeInSeconds = i;
				}
				if (ReadyState.ToLower() == "complete") break;

			}

			if (ReadyState.ToLower() != "complete") throw new Exception($"\nEXCEPTION:\n\tFailed to Load Page {goToURL}");
			logger.Write($"Success - Navigated to:\t{driver.Url.ToString()}", funcName, TimeDiffToInt(startTime, DateTime.Now));

			//	if (hasHumanWait) { SimulateHumanWait(6, 10); }
		}


		/// <summary>
		/// Waits for the Page to be in a readyState
		/// </summary>
		/// <returns>True if Page is Ready</returns>
		public bool WaitForPageReady()
		{
			string jScript = "return document.readyState;";
			string checkValue = "";

			for (int i = 0;i < 1000; i++)
			{
				checkValue = ((IJavaScriptExecutor)driver).ExecuteScript($"{jScript}").ToString();
				if(checkValue == "complete") { return true; }

				Thread.Sleep(500);

			} 


			return false;

		}


		#endregion


		//  ---------------------------------------------------------------
		//  Find Elements (FE)
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region FindElements

		/// <summary>
		/// Sets the Locator and strategy to use.
		/// <para><br>-- inElementSelector = The selector for the element (IE: #id, "//a", etc)</br>
		/// <br>-- inLocatorStrategy = The Strategy to use to locate the element </br>
		/// <br>-- -- -- xpath (default)</br>
		/// <br>-- -- -- css / cssselector</br>
		/// <br>-- -- -- name</br>
		/// <br>-- -- -- id</br>
		/// </para>
		/// </summary>
		/// <param name="inElementSelector"></param>
		/// <param name="inLocatorStrategy"></param>

		public void SetLocator(string inElementSelector, string inLocatorStrategy = "xpath")
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			this.elementSelector = inElementSelector;
			locatorStrategy = inLocatorStrategy;
			switch (locatorStrategy.ToLower())
			{
				case "xpath":
					logMessage = $"Locator Strategy:\tXPATH\tElement:\t{inElementSelector}";
					elementLocator = By.XPath(inElementSelector);
					break;
				case "css":
				case "cssselector":
					logMessage = $"Locator Strategy:\tCssSelector\tElement:\t{inElementSelector}";
					elementLocator = By.CssSelector(inElementSelector);
					break;
				case "id":
					logMessage = $"Locator Strategy:\tID\tElement:\t{inElementSelector}";
					elementLocator = By.Id(inElementSelector);
					break;
				case "name":
					logMessage = $"Locator Strategy:\tName\tElement:\t{inElementSelector}";
					elementLocator = By.Name(inElementSelector);
					break;
				case "tagname":
					logMessage = $"Locator Strategy:\tTagName\tElement:\t{inElementSelector}";
					elementLocator = By.TagName(inElementSelector);
					break;
				case "classname":
					logMessage = $"Locator Strategy:\tClassName\tElement:\t{inElementSelector}";
					elementLocator = By.ClassName(inElementSelector);
					break;
				default:
					logMessage = $"EXCEPTION\tLOCATOR ERROR\nError:\tFE00001\n\tThe Locator Stragety Provided does not match a recognized strategy.\n\tLocator Strategy Provided:\t{inLocatorStrategy}\n\tLocator:\t{inElementSelector}";
					logger.Write(logMessage, funcName);
					throw new Exception(logMessage);

			}
			logger.Write(logMessage, funcName);
		}

		/// <summary>
		/// Locates a single Element on the page that matches the locator and returns an IWebElement object
		/// <para><br>-- inElementSelector = The locator for the element (IE: #id, "//a", etc)</br>
		/// <br>-- inLocatorStrategy = The Strategy to use to locate the element </br>
		/// <br>-- -- -- xpath (default)</br>
		/// <br>-- -- -- css / cssselector</br>
		/// <br>-- -- -- name</br>
		/// <br>-- -- -- id</br>
		/// <br>-- isRequired = Whether element is required. (Default: true)</br>
		/// <br>-- waitForElement = Wait for element to appear on page (true {default}/false)</br>
		/// <br>-- waitTimeSec = Number of seconds to wait for the element (default = 20)</br>
		/// </para>
		/// </summary>
		/// <param name="inElementSelector"></param>
		/// <param name="inLocatorStrategy"></param>
		/// <param name="isRequired"></param>
		/// <param name="waitForElement"></param>
		/// <param name="waitTimeSec"></param>
		/// <returns></returns>

		public void FindElement(string inElementSelector, string inLocatorStrategy = "xpath", bool isRequired = true, bool waitForElement = true, int waitTimeSec = 60)
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			webElement = null;
			DateTime startTime = DateTime.Now;

			SetLocator(inElementSelector, inLocatorStrategy);
			logger.Write($"Finding Element [{inElementSelector}]", funcName);

			int waitLoopCounter = (waitTimeSec < 1) ? 1 : waitTimeSec;
			for (int waitCount = waitLoopCounter; waitCount > 0; waitCount--)
			{
				Thread.Sleep(1000);
				try
				{
					webElement = driver.FindElement(elementLocator);
					break;
				}
				catch
				{
					logMessage = $"Locator not found:\t{inElementSelector}\t|\t{inLocatorStrategy}";

					logger.Write(logMessage, funcName);

				}
			}

			if (webElement == null)
			{
				logMessage = $"Unable to Locate Element:\t{this.elementSelector} using strategy {inLocatorStrategy}.";
				if (isRequired)
				{
					logger.Write(logMessage, funcName,TimeDiffToInt(startTime,DateTime.Now));
					throw new Exception(logMessage);

				}
				else
				{
					logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
				}
			}

		}


		/// <summary>
		/// Locates all Elements on the page that match the locator and returns a List of IWebElement objects
		/// <para><br>-- inElementSelector = The locator for the element (IE: #id, "//a", etc)</br>
		/// <br>-- inLocatorStrategy = The Strategy to use to locate the element </br>
		/// <br>-- -- -- xpath (default)</br>
		/// <br>-- -- -- css / cssselector</br>
		/// <br>-- -- -- name</br>
		/// <br>-- -- -- id</br>
		/// <br>-- isRequired = If element is required (Default true)</br>
		/// <br>-- waitForElement = Wait for element to appear on page (true {default}/false)</br>
		/// <br>-- waitTimeSec = Number of seconds to wait for the element (default = 20)</br>
		/// </para>
		/// </summary>
		/// <param name="inElementSelector"></param>
		/// <param name="inLocatorStrategy"></param>
		/// <param name="isRequired"></param>
		/// <param name="waitForElement"></param>
		/// <param name="waitTimeSec"></param>
		/// <returns></returns>

		public void FindElements(string inElementSelector, string inLocatorStrategy = "xpath", bool isRequired = true, bool waitForElement = true, int waitTimeSec = 60)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			webElements = new List<IWebElement>();
			DateTime startTime = DateTime.Now;

			SetLocator(inElementSelector, inLocatorStrategy);

			int waitLoopCounter = (waitTimeSec < 1) ? 1 : waitTimeSec;
			for (int waitCount = waitLoopCounter; waitCount > 0; waitCount--)
			{
				Thread.Sleep(1000);
				try
				{
					webElements = new List<IWebElement>(driver.FindElements(elementLocator));

					break;
				}
				catch
				{
					logMessage = $"Locator not found:\t{inElementSelector}\t|\t{inLocatorStrategy}";
					logger.Write(logMessage, funcName);

				}
			}

			if (webElements.Count == 0)
			{
				logMessage = $"Unable to Locate Element:\t{inElementSelector} using strategy {inLocatorStrategy}.";
				if (isRequired)
				{
					logger.Write(logMessage, funcName);
					throw new Exception(logMessage);
				}
				else
				{
					logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
				}
			}
			else
			{
				string ListOfElements = "\n";
				foreach (var element in webElements)
				{
					ListOfElements += $"|{element.ToString()}|\n";
				}
				logMessage = $"Elements Found:\t{webElements.Count.ToString()}\n\t{ListOfElements}";
				logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
			}

		}

		#endregion



		//  ---------------------------------------------------------------
		//  Element Interactions (FE)
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region ElementInteractions

		/// <summary>
		/// Clicks on the Element
		/// <para>hasHumanWait = whether to simulate human hesitancy after a click (default: true)</para>
		/// </summary>
		/// <param name="hasHumanWait"></param>
		public void Click(bool hasHumanWait = true)
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			DateTime startTime = DateTime.Now;

			try
			{
				logger.Write("Clicking on Element.", funcName, TimeDiffToInt(startTime, DateTime.Now));
				webElement.Click();
			}
			catch (Exception e)
			{
				logMessage = $"ERROR:\tCannot Click with Selenium.\n{e}";
				logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
				throw new Exception(logMessage);

			}
//			if (hasHumanWait) { SimulateHumanWait(); }

		}


		/// <summary>
		/// Gets Element Attributes (IE: InnerText, InnerHTML)
		/// <para><br>--- attribute2Get = the attribute to get.</br>
		/// <br>--- --- innerHTML</br>
		/// <br>--- --- innerText</br>
		/// <br>--- --- Others as allowed by Selenium WebDriver</br></para>
		/// </summary>
		/// <param name="attribute2Get"></param>
		/// <returns></returns>
		public string GetAttribute(string attribute2Get = "innerText")
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			string outValue = "";
			string outMsg = "";
			DateTime startTime = DateTime.Now;

			try
			{
				switch (attribute2Get.ToLower())
				{
					case "innertext":
					case "text":
						outValue = webElement.GetAttribute("innerText");
						break;
					case "innerhtml":
					case "html":
						outValue = webElement.GetAttribute("innerHTML");
						break;

					default:
						outValue = webElement.GetAttribute(attribute2Get);
						break;
				}
			}
			catch (Exception e)
			{
				logMessage = $"ERROR:\tThe attribute provided [{attribute2Get}] does not match a valid type for element [{webElement}]. \n{e}";
				logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
				throw new Exception(logMessage);
			}
			outMsg = $"Attribute Returned: {outValue}";
			logger.Write(outMsg, funcName, TimeDiffToInt(startTime, DateTime.Now));
			return outValue;

		}


		/// <summary>
		/// GetProperty retrieves the property from an element.
		/// <para>- property2Get = the Property to get. (Default: Value)</para>
		/// </summary>
		/// <param name="property2Get"></param>
		/// <returns></returns>
		public string GetProperty(string property2Get = "value")
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			string outValue = "";
			string outMsg = "";
			DateTime startTime = DateTime.Now;

			try
			{
				outValue = webElement.GetDomProperty(property2Get);
			}
			catch (Exception e)
			{
				logMessage = $"ERROR:\tThe attribute provided [{property2Get}] does not match a valid type for element [{webElement}] . \n{e}";
				logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
				throw new Exception(logMessage);
			}
			outMsg = $"Property Found: {outValue}";
			logger.Write(outMsg, funcName, TimeDiffToInt(startTime, DateTime.Now));
			return outValue;

		}


		/// <summary>
		/// Sends keys to the element
		/// <para>sendValue = the text to send
		/// <br>doTrim = Trim all beginning and ending white space as well as removing double spaces</br></para>
		/// </summary>
		/// <param name="sendValue"></param>
		/// <param name="doTrim"></param>
		public void SendKeys(string sendValue, bool doTrim = true)
		{
			string funcName = MethodBase.GetCurrentMethod().Name;

			if (doTrim)
			{
				// Remove Double Spacing
				sendValue = Regex.Replace(sendValue, "\\s+", " ");
				// Left Trim/Right Trim
				sendValue = sendValue.Trim();
			}

			try
			{
				webElement.SendKeys(sendValue);
			}
			catch (Exception e)
			{
				logMessage = $"Unable to Send text [{sendValue}] to Element [{webElement}]\n{e}";
				logger.Write(logMessage, funcName);
				throw new Exception(logMessage);
			}

		}

		/// <summary>
		/// Set the Attribute for an Element
		/// </summary>
		/// <param name="attribute2Set"></param>
		/// <param name="value2Set"></param>
		public void SetAttribute(string attribute2Set, string value2Set)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			string jScript = "";

			try
			{

				logger.Write($"JavaScript to Run:\n{jScript}", funcName);

				var testItem = ((IJavaScriptExecutor)driver).ExecuteScript(jScript);
			}
			catch (Exception e)
			{
				logMessage = $"Failed to Run JavaScript:\n{jScript}\n{e}";
				logger.Write(logMessage, funcName);
				throw new Exception(logMessage);

			}

		}



		/// <summary>
		/// Generates the JavaScript locator script 
		/// </summary>
		/// <returns></returns>
		public string LocateByJS()
		{
			//			throw new Exception("Not Yet Implemented");
			string funcName = MethodBase.GetCurrentMethod().Name;
			string outMsg = "";
			DateTime startTime = DateTime.Now;
			string jsOutString = "";
			switch (locatorStrategy.ToLower())
			{
				case ("xpath"):
					jsOutString = $"document.evaluate(\"{elementSelector}\", document,null, XPathResult.ANY_TYPE,null).FIRST_ORDERED_NODE_TYPE";
					break;
				case ("css"):
				case ("cssselector"):
					jsOutString = $"document.querySelector(\"{elementSelector}\");";
					break;
				case ("id"):
					jsOutString = $"document.getElementById(\"{elementSelector}\");";
					break;
				case ("name"):
					jsOutString = $"document.getElementsByName(\"{elementSelector}\");";
					break;
				case ("classname"):
					jsOutString = $"document.getElementsByClassName(\"{elementSelector}\");";
					break;
				case ("tagname"):
					jsOutString = $"document.getElementsByTagName(\"{elementSelector}\");";
					break;

				default:
					logMessage = "Locator Strategy does not match viable options. ";
					logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
					throw new Exception(logMessage);

			}

			if (jsOutString == "")
			{
				logMessage = $"Location Strategy [{locatorStrategy}] not supported.";
				logger.Write(logMessage, funcName, TimeDiffToInt(startTime, DateTime.Now));
				throw new Exception(logMessage);
			}

			outMsg = $"Found: {elementSelector}";
			logger.Write(outMsg, funcName, TimeDiffToInt(startTime, DateTime.Now));
			return jsOutString;
		}


		#endregion



		//  ---------------------------------------------------------------
		//  JavaScript Functions
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region JavaScript

		public void RunJS(string javascriptCode)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			try
			{
				((IJavaScriptExecutor)driver).ExecuteScript($"{javascriptCode}");
			}
			catch(Exception e)
			{
				logger.Write(e.Message, funcName);

			}
		}

		public string RunJsStringOut(string javascriptCode)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			try
			{
				return (string)((IJavaScriptExecutor)driver).ExecuteScript($"{javascriptCode}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Thread.Sleep(5000);
				throw new Exception(e.Message);

			}
		}

		/// <summary>
		/// Uses JavaScript to find and click on an element <br />
		/// Should click elements that normal Selenium Click does not <br />
		/// Does not always return errors when click fails (JS limitation)
		/// </summary>
		/// <param name="xpath">Xpath to the element</param>
		public void ClickJS(string xpath)
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			string jScript = "";

			jScript += "function clickJS(xpath) \n";
			jScript += "{  \n";
			jScript += "	try \n";
			jScript += "	{ \n";
			jScript += "		let element = document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE).singleNodeValue; \n";
			jScript += "		element.click(); \n";
			jScript += "	} \n";
			jScript += "	catch (e) \n";
			jScript += "	{ \n";
			jScript += "		return e; \n";
			jScript += "	} \n";
			jScript += "	return 'SUCCESS'; \n";
			jScript += "} \n";
			jScript += $"return clickJS(`{xpath}`);";

			string returnValue = "";
			try
			{

				returnValue = ((IJavaScriptExecutor)driver).ExecuteScript($"{jScript}").ToString();
				if (returnValue == "SUCCESS")
				{
					logger.Write($"{returnValue} Click on [{xpath}]", funcName);
				}
				else
				{
					logger.Write($"Failed to click on [{xpath}]\n\t{returnValue}", funcName);
				}

			}
			catch (Exception e)
			{
				logger.Write(e.Message, funcName);
			}


		}


		#endregion





		//  ---------------------------------------------------------------
		//  JQX Element Functions
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region Jqx Element Functions


		public void GetJqxGridColumn(string gridId,int columnNum = 1)
		{
			string elementXpath = $"//div[@id='{gridId}']//div[@role='row']/div[@columnindex={columnNum.ToString()}]/div";

			FindElements(elementXpath);

		}

		/// <summary>
		/// Reads a jqxGird then converts the data into a Dictionary.
		/// </summary>
		/// <param name="gridId">The Element ID for the jqxGrid</param>
		/// <param name="debug">Determines whether Javascript will be sent to SQL</param>
		/// <returns>JSON from the Grid as Dictionary (int,dynamic)</returns>
		public Dictionary<int,dynamic> JqxGridData(string gridId,bool debug = false)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			Dictionary<int,dynamic> output = new Dictionary<int, dynamic>();
			logger.Write($"Getting Grid Data for {gridId}",funcName);
			Console.WriteLine($"Getting Grid Data for {gridId}");
			string jscript= "";

			// Javascript to be run 
			jscript = "function getJqxTableData(gridId)	\n	{	\n";
			jscript += "	let xpath = `//div[@id='${gridId}']//div[@role='columnheader']//span`;	\n";
			jscript += "	let allHeaders = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE);	\n";
			jscript += "	let columnCount = allHeaders.snapshotLength;	\n	";
			jscript += "	xpath = `//div[@id='${gridId}']//div[@role='columnheader'][not(contains(@style,'display'))]//span` \n ";
			jscript += "	let visibleHeaders = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE);	\n	";
			jscript += "	let visibleHeaderItem = 0;	\n ";
			jscript += "	let tableHeaders = [];	\n ";
			jscript += "	for (i = 0; i < columnCount; i++)	\n	";
			jscript += "	{	\n	";
			jscript += "		let currentHeader = allHeaders.snapshotItem(i);	\n	";
			jscript += "		let currentVisible = visibleHeaders.snapshotItem(visibleHeaderItem);	\n ";
			jscript += "		if (currentHeader == currentVisible)	\n ";
			jscript += "		{	\n	";
			jscript += "			tableHeaders.push({ colTitle: currentVisible.innerText, colNum: i });	\n	";
			jscript += "			visibleHeaderItem++;	\n	";
			jscript += "		}	\n	";
			jscript += "	}	\n	";
			jscript += "	xpath = `//div[@id='${gridId}']//div[@role='row']/div/div`;	\n	";
			jscript += "	tableRaw = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE);	\n	";
			jscript += "	let rowCount = tableRaw.snapshotLength;	\n	";
			jscript += "	let resultsJson = new Object();	\n	";
			jscript += "	for (i = 0; i < rowCount; i++)	\n	";
			jscript += "	{	\n	";
			jscript += "	let rowJson = new Object();	\n  ";

			jscript += "        for (i2 = 0; i2 < tableHeaders.length; i2++) {	\n	";
			jscript += "            let curColumn = tableHeaders[i2]['colNum'];	\n	";
			jscript += "            let colTitle = tableHeaders[i2]['colTitle'];	\n	";
			jscript += "            xpath = `//div[@id='${gridId}']//div[@id='row${i}${gridId}']/div[@columnindex=${curColumn}]/div`;	\n	";
			jscript += "            let curElement = document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE).singleNodeValue;	\n	";
			jscript += "            try {	\n	";
			jscript += "                if (!curElement.innerText) {	\n	";
			jscript += "                    break;	\n	";
			jscript += "                }	\n	";
			jscript += "                rowJson[colTitle] = curElement.innerText;	\n	";
			jscript += "            }	\n	";
			jscript += "            catch (e) {	\n	";
			jscript += "                break;	\n	";
			jscript += "            }	\n	";
			jscript += "            if (rowJson[tableHeaders[0]['colTitle']]) {	\n	";
			jscript += "                resultsJson[i] = rowJson;	\n	";
			jscript += "            }	\n	";
			jscript += "        }	\n	";
			jscript += "    }	\n	";
			jscript += "    return resultsJson;	\n	";
			jscript += "}	\n	";

			jscript += $"return JSON.stringify(getJqxTableData('{gridId}'));";

			if (debug)
			{
				logger.Write($"Javascript\n\n{jscript}", funcName);
			}
			
			try
			{
				string resultsJson = ((IJavaScriptExecutor)driver).ExecuteScript($"{jscript}").ToString();
				output = JsonConvert.DeserializeObject<Dictionary<int,dynamic>>(resultsJson);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw new Exception($"Func: {funcName}({gridId}) || An error occurred while extracting data from a jqxGrid.\n{e.Message}");
			}


			return output;

		}


		/// <summary>
		/// Reads a jqxDropdown then converts the data into a String
		/// </summary>
		/// <param name="ddmsId">The Element ID for the jqx Drop Down</param>
		/// <param name="debug">Determines wehther Javascript will be sent to SQL</param>
		/// <returns>JSON from the Grid as String </returns>
		public string JqxDDMSData(string ddmsId,bool debug = false)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			string output = "";
			string jScript = "";

			jScript += "function getJqxDropDownData(ddmsId)	\n";
			jScript += "{ \n ";
			jScript += "	let xpath = `//select[@id='${ddmsId}']/option[@selected]`; \n ";
			jScript += "	let returnValue = []; \n ";
			jScript += "	let elements = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE); \n ";
			jScript += "	for (i = 0; i < elements.snapshotLength; i++) \n ";
			jScript += "	{ \n ";
			jScript += "		returnValue.push(elements.snapshotItem(i).innerText); \n ";
			jScript += "	} \n ";
			jScript += "	return returnValue.toString(); \n ";
			jScript += "} \n ";

			jScript += $"return JSON.stringify( getJqxDropDownData('{ddmsId}') )";

			if (debug)
			{
				logger.Write($"Javascript\n\n{jScript}", funcName);
			}

			try
			{
				output = ((IJavaScriptExecutor)driver).ExecuteScript($"{jScript}").ToString();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw new Exception($"Func: {funcName}({ddmsId}) || An error occurred while extracting data from a jqxGrid.\n{e.Message}");
			}

			return output;

		}

		/// <summary>
		/// Clicks the Option from a JQX Dropdown (Multi-Selcr or Single)
		/// </summary>
		/// <param name="ddmsId">The Element ID for the Select Element</param>
		/// <param name="options">A Comma Separated list of values to select from the Drop Down</param>
		public void ClickJqxDDMSOptions(string ddmsId, string options)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;

			try
			{
				string selectXpath = $"//select[@id='{ddmsId}']";
				selectXpath = $"//select[@id='{ddmsId}']/following-sibling::button";

				this.FindElement(selectXpath);
//				this.ClickJS(selectXpath);
				this.Click();

				Thread.Sleep(250);

				var optionValues = options.Trim().Replace("  ", " ").Split(",");

				foreach (var opt in optionValues)
				{
					string optionXpath = $"//select[@id='{ddmsId}']//option[contains(text(),'{opt}')]";
					optionXpath = $"//li[@class='optLookup']//input[contains(@name,'multiselect')][contains(@name,'{ddmsId}')]/following-sibling::span[contains(text(),'{opt}')]";


					this.FindElement(optionXpath);
//					this.ClickJS(optionXpath);
					this.Click();
					Thread.Sleep(250);
				}
			}
			catch(Exception e)
			{
				logger.Write(e.Message, funcName);
			}
		}


		#endregion



		//  ---------------------------------------------------------------
		//  InSight Specific Functions
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region Insight Specific

		/// <summary>
		/// Checks the InSight Error Message for specified text
		/// </summary>
		/// <param name="expectedMsgText">The expected text in the message</param>
		/// <returns></returns>
		public bool IsValidErrorMsg(string expectedMsgText)
		{
			string validationText = "";
			string javaScript = "";

			javaScript += " var xpath = `//div[contains(@class,'alert-error')]`; ";
			javaScript += " var element = document.evaluate(xpath,document,null,XPathResult.FIRST_ORDERED_NODE_TYPE).singleNodeValue; ";
			javaScript += " return element.textContent; ";

			validationText = RunJsStringOut(javaScript);

			if (validationText.Contains(expectedMsgText))
			{
				return true;
			}
			return false;

		}


		#endregion



		//  ---------------------------------------------------------------
		//  General Functions
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------
		#region General Functions

		/// <summary>
		/// Returns the time differences in seconds
		/// </summary>
		/// <param name="startTime">The Start Time</param>
		/// <param name="endTime">The End Time</param>
		/// <returns></returns>
		public int TimeDiffToInt(DateTime startTime, DateTime endTime)
		{
			int result;

			result = endTime.Subtract(startTime).Seconds + endTime.Subtract(startTime).Minutes * 60 + endTime.Subtract(startTime).Hours * 3600 + endTime.Subtract(startTime).Hours * 3600 * 24;

			return result;
		}


		#endregion



	}
}
 