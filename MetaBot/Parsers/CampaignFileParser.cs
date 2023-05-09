using Microsoft.VisualBasic.FileIO;
using MetaBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetaBot.Parsers
{
    public sealed class CampaignFileParser : IFileParser<Campaign>
    {
        public List<Campaign> Parse(string filePath)
        {
            var campaigns = new List<Campaign>();
            try
            {
                using (var parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.ReadFields(); // skip header

                    while (!parser.EndOfData)
                    {
                        var fields = parser.ReadFields();
                        var campaign = ParseCampaign(fields);
                        campaigns.Add(campaign);
                    }
                }

                return campaigns.ToList();
            }
            catch (IOException)
            {
                return default;
            }
        }

        public string GetCurrency(string filePath)
        {
            using (var parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                var columns = parser.ReadFields(); // skip header
                var currency = columns[3].Split('(', ')')[1];
                return currency;
            }
        }

        private static Campaign ParseCampaign(string[] fields)
        {
            var campaign = new Campaign();

            long pageId;
            long.TryParse(fields[0], out pageId);
            campaign.pageId = pageId;

            campaign.pageName = fields[1];
            campaign.disclaimer = fields[2];

            long spent;
            long.TryParse(fields[3], out spent);
            if (spent == 0 && fields[3].Contains("100"))
            {
                spent = 99;
            }
            campaign.spent = spent;

            int ads;
            int.TryParse(fields[4], out ads);
            campaign.ads = ads;

            return campaign;
        }
    }
}