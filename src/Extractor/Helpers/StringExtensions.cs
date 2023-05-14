using System;
using System.Linq;

namespace Extractor.Helpers
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            str = str.Replace("'", "");

            var wordsArray = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            var words = wordsArray.Select(word => char.ToUpper(word[0]) + word[1..])
                .ToArray();

            return string.Join(string.Empty, words);
        }
    }
}
