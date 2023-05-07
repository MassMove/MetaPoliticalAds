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
        public void Write(string filePath, IList<Campaign> campaigns, string countryCode, string totalSpent)
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

            readMe.AppendLine($"## {countryCode}");
            readMe.AppendLine($"**Total spent**: {totalSpent}");
            readMe.AppendLine("");

            readMe.AppendLine("|Page Name|Spent|Ads|Disclaimer|");
            readMe.AppendLine("|:---|---:|---:|:---|");

            foreach (Campaign campaign in campaigns.Where(c => c.spent > 0))
            {
                var adsUrl = $"https://www.facebook.com/ads/library/?active_status=all&ad_type=political_and_issue_ads&country={countryCode}&view_all_page_id={campaign.pageId}&search_type=page&media_type=all";
                var pageUrl = $"https://www.facebook.com/{campaign.pageId}";
                readMe.Append($"|[{campaign.pageName}]({pageUrl})|{campaign.spent.ToString("N")}|[{campaign.ads}]({adsUrl})|{campaign.disclaimer}|\r\n");
            }

            File.WriteAllText(filePath, readMe.ToString());
        }
    }
}