using MetaBot.Parsers;
using System;
using System.IO;
using System.Linq;

namespace MetaBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is our world now");
            string readMe = "# Meta Political Ads\r\n\r\n";
            readMe += "Summary of the [Meta Political Ads Library](https://www.facebook.com/ads/library).\r\n\r\n";
            readMe += "Source data: [/MetaData](MetaData).\r\n\r\n";
            readMe += "Last run: " + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".\r\n\r\n";

            readMe += "|Country|Total Spent|Last Month|\r\n";
            readMe += "|:---|---:|---:|\r\n";

            var campaignFileParser = new CampaignFileParser();
            var campaignSummaryWriter = new CampaignFileWriter();

            foreach (var fileName in Directory.GetFiles("../../../../MetaData/Total"))
            {
                var fileInfo = new FileInfo(fileName);
                var countryCode = fileInfo.Name.Replace(".csv", "");
                var countryPath = $"../../../../Country/{countryCode}";

                Console.Write($"{countryCode}: ");

                if (!Directory.Exists(countryPath))
                {
                    Directory.CreateDirectory(countryPath);
                }

                var totalCampaigns = campaignFileParser.Parse(fileName);
                var currency = campaignFileParser.GetCurrency(fileName);
                var totalSpent = $"{totalCampaigns.Sum(c => c.spent).ToString("N")} {currency}";
                var totalUrl = $"Country/{countryCode}/Total.md";
                Console.Write($"{totalSpent,20}");
                campaignSummaryWriter.Write($"{countryPath}/Total.md", totalCampaigns, countryCode, totalSpent, "Total");

                var monthFileName = fileName.Replace("Total", "Month");
                var monthCampaigns = campaignFileParser.Parse(monthFileName);
                var monthSpent = $"{monthCampaigns.Sum(c => c.spent).ToString("N")} {currency}";
                var monthUrl = $"Country/{countryCode}/Month.md";
                Console.WriteLine($"{monthSpent,20}");
                campaignSummaryWriter.Write($"{countryPath}/Month.md", monthCampaigns, countryCode, monthSpent, "Month");

                readMe += $"|{countryCode}|[{totalSpent}]({totalUrl})|[{monthSpent}]({monthUrl})|\r\n";
            }

            File.WriteAllText("../../../../README.md", readMe);
        }
    }
}
