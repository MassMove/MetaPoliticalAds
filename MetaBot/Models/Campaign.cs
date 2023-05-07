using System.Collections.Generic;

namespace MetaBot.Models
{
    public sealed class Campaign
    {
        public long pageId { get; set; } = 0;
        public string pageName { get; set; } = "";
        public string disclaimer { get; set; } = "";
        public long spent { get; set; } = 0;
        public int ads { get; set; } = 0;
    }
}