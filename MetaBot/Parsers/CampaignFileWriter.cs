using MetaBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MetaBot.Parsers
{
    public sealed class CampaignFileWriter : IFileWriter<Campaign>
    {
        public void Write(string filePath, IList<Campaign> campaigns, string countryCode, string totalSpent, string csvUrlPrefix, string title, string lastRun)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var readMe = new StringBuilder();

            readMe.AppendLine($"## {countryCode} - {title}");
            readMe.AppendLine($"**As at**: {lastRun}");
            readMe.AppendLine("");
            readMe.AppendLine($"**Total spent**: {totalSpent}");
            readMe.AppendLine("");

            readMe.AppendLine("|Page Name|Spent|Ads|Disclaimer|");
            readMe.AppendLine("|:---|---:|---:|:---|");

            var top100 = campaigns.Take(100);

            if (top100.Count() == 100)
            {
                top100 = campaigns.Where(c => c.spent > 0).Take(100);
            }

            foreach (Campaign campaign in top100)
            {
                var adsUrl = $"https://www.facebook.com/ads/library/?active_status=all&ad_type=political_and_issue_ads&country={countryCode}&view_all_page_id={campaign.pageId}&search_type=page&media_type=all";
                var pageUrl = $"https://www.facebook.com/{campaign.pageId}";
                var spent = campaign.spent.ToString("N");
                if (campaign.spent == 99)
                {
                    spent = "≤100";
                }
                readMe.Append($"|[{campaign.pageName}]({pageUrl})|{spent}|[{campaign.ads}]({adsUrl})|{campaign.disclaimer}|\r\n");
            }

            if (campaigns.Count > 100)
            {
                readMe.AppendLine($"|...||||");
            }

            readMe.AppendLine("");
            readMe.AppendLine($"View all in [{csvUrlPrefix}/{countryCode}.csv](../../MetaData/{csvUrlPrefix}/{countryCode}.csv)");

            File.WriteAllText(filePath, readMe.ToString());
        }
    }
}