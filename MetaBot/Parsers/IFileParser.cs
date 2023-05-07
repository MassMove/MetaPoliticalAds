using System.Collections.Generic;

namespace MetaBot.Parsers
{
    public interface IFileParser<T> where T : class, new()
    {
        List<T> Parse(string filePath);
        string GetCurrency(string filePath);
    }
}