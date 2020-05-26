using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OTITO.Web.Models.Slug
{
    public static class Slug
    {

        public static string ToUrlSlug(string value)
        {

            //First to lower case
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
        public static string GetSlug(string value)
        {

            string[] split = value.Split(new char[] { '-' });
            return value.Substring(0, (value.Length - split[split.Length-1].Length)-1);
        }
        public static string ToNewGuid()
        {
            Guid id = Guid.NewGuid();
            string val = id.ToString();
            string[] split = val.Split(new char[] { '-' });
            return split[0];
        }



    }
}
