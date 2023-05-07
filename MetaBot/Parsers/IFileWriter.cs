using System;
using System.Collections.Generic;
using System.Text;

namespace MetaBot.Parsers
{
    public interface IFileWriter<T>
    {
        void Write(string filePath, IList<T> items, string countryCode, string totalSpent, string csvUrlPrefix);
    }
}