﻿using System.Text.RegularExpressions;
using OpenQA.Selenium;
using PriceTracker.Parser;
using PriceTracker.Services.Parser.Models;

namespace PriceTracker.Services.Parser;

public class MVideoParser : IParser
{
    private const string Pattern = @"\d+,?\d+";
    
    public async Task<ParseResult> ParseAsync(string url)
    {
        var driver = DriverConfig.GetConfiguredWebDriver();
        driver.Navigate().GoToUrl(url);
        var title = driver.FindElement(By.XPath(".//h1[@itemprop='name']")).Text;
        double price = 0;
        double cardPrice = 0;
        try
        {
            var findPrice = driver.FindElement(By.XPath(".//span[@class='price__sale-value ng-star-inserted']")).Text;
            findPrice = findPrice.Replace(" ", "");
            price = double.Parse(Regex.Match(findPrice, Pattern).Value);
            var findCardPrice = driver.FindElement(By.XPath(".//span[@class='price__main-value']")).Text;
            findCardPrice = findCardPrice.Replace(" ", "");
            cardPrice = double.Parse(Regex.Match(findCardPrice, Pattern).Value);
        }
        catch (Exception e)
        {
            var findCardPrice =
                driver.FindElement(By.XPath(".//span[@class='price__main-value']")).Text;
            findCardPrice = findCardPrice.Replace(" ", "");
            cardPrice = double.Parse(Regex.Match(findCardPrice, Pattern).Value);
            return new ParseResult(title, cardPrice, null);
        }
        finally
        {
            driver.Close();
        }

        return new ParseResult(title, price, cardPrice);
    }
}