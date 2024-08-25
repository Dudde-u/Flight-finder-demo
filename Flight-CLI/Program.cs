using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V125.LayerTree;
using OpenQA.Selenium.Firefox;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Xml.XPath;
using System.Reflection.Metadata;
using System.Net.NetworkInformation;
using Class_library;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

Console.Title = "Flight Finder";
Console.WriteLine("Welcome to the flight finder!");
Console.WriteLine("Currently supported airlines are RyanAir and Norwegian");
Console.WriteLine("Loading content...");
Console.WriteLine("Please wait...");

FirefoxOptions options = new FirefoxOptions();
options.ImplicitWaitTimeout = TimeSpan.FromSeconds(0.5);
options.PageLoadStrategy = PageLoadStrategy.Normal;
options.AddArgument("--headless");
options.AddArgument("--disable-gpu");
FirefoxDriver driver = new FirefoxDriver(options);

string origin = "";
string destination = "";
string dateAsString = "2024-11-11";

Console.Clear();
bool quit = false;


while (quit == false)
{
    Console.WriteLine("Type the IATA code for your destination airport");
    string destinationInput = Console.ReadLine();
    if (destinationInput.Length == 3)
    {
        destination = destinationInput;
    }
    else
    {
        Console.WriteLine("Invalid IATA code, using default destination RIX");
        destination = "RIX";
    }
    Console.WriteLine("Type the IATA code for your origin airport");
    string origininput = Console.ReadLine();
    if (origininput.Length == 3)
    {
        origin = origininput;
    }
    else
    {
        Console.WriteLine("Invalid IATA code, using default origin ARN");
        origin = "ARN";
    }
    Console.WriteLine("Type the date of your flight in the format YYYY-MM-DD");
    string dateInput = Console.ReadLine();
    if (dateInput.Length == 10)
    {
        dateAsString = dateInput;
    }
    else
    {
        Console.WriteLine("Invalid date, using default date 2024-11-11");
        dateAsString = "2024-11-11";
    }
    Console.WriteLine("Searching flights now.....");
    RyanAirQuery(destination, origin, dateAsString, driver);
    NorwegianQuery(destination, origin, dateAsString, driver);

    Console.WriteLine("Do you want to search for another flight? (Y/N)");
    if (Console.ReadLine().ToLower() == "n")
    {
        quit = true;
    }
    else
    {
        Console.Clear();

    }
 
}
driver.Quit();


static void RyanAirQuery(string destination, string origin, string dateAsString, FirefoxDriver firefoxDriver)
{


    try
    {
        firefoxDriver.Navigate().GoToUrl(@"https://www.ryanair.com/gb/en/trip/flights/select?adults=1&teens=0&children=0&infants=0&dateOut=" + dateAsString + "&isConnectedFlight=false&isReturn=false&discount=0&originIata=" + origin + "&destinationIata=" + destination + "&tpAdults=1&tpTeens=0&tpChildren=0&tpInfants=0&tpStartDate=2024-10-11&tpDiscount=0&tpOriginIata=" + origin + "&tpDestinationIat<a=" + destination);

        Thread.Sleep(500);
        firefoxDriver.FindElement(By.CssSelector("button.cookie-popup-with-overlay__button-settings:nth-child(2)")).Click();
        List<IWebElement> flights = new List<IWebElement>();
        Thread.Sleep(1000);
        for (int i = 1; i < 10; i++)
        {
            IWebElement element;
            int Errors = 0;
            try
            {
                element = firefoxDriver.FindElement(By.XPath("/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new[" + i + "]/div/div"));
                if (element != null)
                {
                    flights.Add(element);
                }

            }
            catch
            {
                if (i == 1)
                {
                    Errors++;

                    if (Errors > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }


        }

        Console.WriteLine("Ryanair Flights found: " + flights.Count);

        foreach (IWebElement flight in flights)
        {
            string[] flightInfo = flight.Text.Split("\r\n");
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Departs " + flightInfo[2] + " at " + flightInfo[1] + ", and arrives in " + flightInfo[6] + " at " + flightInfo[5]);
            Console.WriteLine();
            Console.WriteLine("Flight time is: " + flightInfo[4]);
            Console.WriteLine();
            Console.WriteLine("Price: " + flightInfo[flightInfo.Length - 2]);
            Console.WriteLine();
            Console.WriteLine(flightInfo[0]);
            Console.WriteLine("Link to website:");
            Console.WriteLine(firefoxDriver.Url);
            Console.WriteLine("--------------------------------------------------------");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("No flights found for Ryanair");
    }
}
// https://www.norwegian.com/en/low-fare-calendar/?D_City=VIE&A_City=ARN&TripType=1&D_Day=24&D_Month=202408&D_SelectedDay=24&R_Day=24&R_Month=202408&R_SelectedDay=24&CurrencyCode=EUR&message=noFlightsOnDate&processid=80586

//https://www.norwegian.com/en/ipc/availability/avaday?AdultCount=1&A_City=ARN&D_City=VIE&D_Month=202408&D_Day=30&IncludeTransit=true&TripType=1&CurrencyCode=EUR&AgreementCodeFK=undefined&dFare=242

static void NorwegianQuery(string destination, string origin, string dateAsString, WebDriver driver)
{
    string monthdate = dateAsString.Substring(0, 7);
    monthdate = monthdate.Replace("-", "");
    string daydate = dateAsString.Substring(8, 2);
    daydate = daydate.Replace("-", "");
    string url = @"https://www.norwegian.com/en/ipc/availability/avaday?AdultCount=1&A_City="+destination+"&D_City="+origin+"&D_Month="+monthdate+"&D_Day="+daydate+"&IncludeTransit=true&TripType=1&CurrencyCode=EUR&AgreementCodeFK=undefined&dFare=1000";
    driver.Navigate().GoToUrl(url);
   

    for (int i = 0; i < 4; i++)
    {
      
        try
        {
            driver.FindElement(By.CssSelector("button.button:nth-child(2)")).Click();
            break;
        }
        catch (Exception ex) { }
    }

    try
    {
        IWebElement priceElement = driver.FindElement(By.CssSelector("tr.oddrow:nth-child(1) > td:nth-child(5) > div:nth-child(1) > label:nth-child(1)"));
        IWebElement TimeElement = driver.FindElement(By.CssSelector("tr.oddrow:nth-child(2) > td:nth-child(3) > div:nth-child(1)"));
        IWebElement leavingTime = driver.FindElement(By.CssSelector("tr.oddrow:nth-child(1) > td:nth-child(1) > div:nth-child(1)"));
        IWebElement arrivalTime = driver.FindElement(By.CssSelector("tr.oddrow:nth-child(1) > td:nth-child(2) > div:nth-child(1)"));
   

        Console.WriteLine("--------------------------------------------------------");
        Console.WriteLine("Departs " + origin + " at " + leavingTime.Text + ", and arrives in " + destination + " at " + arrivalTime.Text);
        Console.WriteLine();
        Console.WriteLine("Flight time is: " + TimeElement.Text.Split(':')[1]);
        Console.WriteLine();
        Console.WriteLine("Price: " + priceElement.Text + " (EUR) ");
        Console.WriteLine();
        Console.WriteLine("Link to website:");
        Console.WriteLine(driver.Url);
        Console.WriteLine("--------------------------------------------------------");

    }
    catch (Exception ex) 
    {
    Console.WriteLine("No flights found for Norwegian");

    }




}
static void LuftHansaQuery(string destination, string origin, string dateAsString, WebDriver driver) //quite a lot of work to get this to work
{

    driver.Navigate().GoToUrl("https://www.lufthansa.com/se/en/homepage");
    driver.FindElement(By.CssSelector("#cm-acceptNone")).Click();

    for (int i = 0; i < 5; i++)
    {
        try
        {
            driver.FindElement(By.CssSelector(".lh-close")).Click();

            break;
        }
        catch (Exception)
        {
            Console.WriteLine("Issue");
        }
    }
    Thread.Sleep(500);
    IWebElement departureTbx = driver.FindElement(By.Name("flightQuery.flightSegments[0].originCode"));

    IWebElement arrivalTbx = driver.FindElement(By.Name("flightQuery.flightSegments[0].destinationCode"));
    IWebElement datebox = driver.FindElement(By.XPath("/html/body/div[2]/div[4]/div/div/div[2]/div/div/div[2]/div[1]/div/section/div[2]/div[1]/div/div/form/div[2]/div[2]/div/div[1]"));

    departureTbx.Clear();
    arrivalTbx.Clear();
    departureTbx.SendKeys(origin);
    arrivalTbx.SendKeys(destination);
    datebox.Click();

}
