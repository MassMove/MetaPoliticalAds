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

            readMe += "|Country|Spent|\r\n";
            readMe += "|:---|---:|\r\n";

            var campaignFileParser = new CampaignFileParser();
            var campaignSummaryWriter = new CampaignFileWriter();

            foreach (var fileName in Directory.GetFiles("../../../../MetaData"))
            {
                var fileInfo = new FileInfo(fileName);
                var countryCode = fileInfo.Name.Replace(".csv", "");
                var countryPath = $"../../../../Country/{countryCode}";

                Console.Write($"{countryCode}: ");

                if (!Directory.Exists(countryPath))
                {
                    Directory.CreateDirectory(countryPath);
                }

                var campaigns = campaignFileParser.Parse(fileName);
                var currency = campaignFileParser.GetCurrency(fileName);
                var totalSpent = $"{campaigns.Sum(c => c.spent).ToString("N")} {currency}";
                Console.WriteLine($"{totalSpent, 20}");

                campaignSummaryWriter.Write($"{countryPath}/README.md", campaigns, countryCode, totalSpent);

                var countryUrl = $"Country/{countryCode}/README.md";
                readMe += $"|[{countryCode}]({countryUrl})|{totalSpent}|\r\n";
            }

            File.WriteAllText("../../../../README.md", readMe);
        }
    }
}
