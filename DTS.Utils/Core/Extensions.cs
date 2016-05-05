using System;
using System.Collections.Generic;
using System.Linq;

namespace DTS.Utils.Core
{
    static class Extensions
    {
        public static string ToCamelCase(this string thisString)
        {
            return Char.ToLower(thisString[0]) + thisString.Substring(1);
        }


        public static string EverythingAfterLast(this string thisString, string last)
        {
            var index = thisString.LastIndexOf(last);

            if (index == -1)
            {
                return null;
            }

            return thisString.Substring(index + last.Length);
        }

        public static List<string> SplitAndTrim(this string thisString, string splitOn)
        {
            return thisString.Split(new[] { splitOn }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
        }
    }
}