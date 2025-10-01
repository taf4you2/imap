using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace imap_samemu
{
    public class ParsedData
    {
        public DataType DataType { get; set; }
        public DateTime Timestamp { get; set; }
        public List<double> NumericValues { get; set; } = new List<double>();
        public List<string> TimeValues { get; set; } = new List<string>();
        public bool IsValid { get; set; }
        public int ValueCount { get; set; }
    }

    public static class DataParser
    {
        private static string RemoveHtmlTags(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            string text = Regex.Replace(html, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            text = Regex.Replace(text, @"<[^>]+>", string.Empty);

            text = System.Net.WebUtility.HtmlDecode(text);

            return text;
        }

        public static ParsedData ParseEmailBody(string emailBody, DataType dataType, DateTime timestamp)
        {
            var result = new ParsedData
            {
                DataType = dataType,
                Timestamp = timestamp,
                IsValid = false
            };
            

            if (string.IsNullOrWhiteSpace(emailBody))
                return result;

            string cleanedBody = RemoveHtmlTags(emailBody);


            string[] lines = cleanedBody.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //Console.WriteLine(lines);


            List<double> numericValues = new List<double>();
            List<string> timeValues = new List<string>();

            bool parsingNumbers = true;

            foreach (string line in lines)
            {
                Console.WriteLine(line);
                string value = line.Trim();
                if (string.IsNullOrEmpty(value)) continue;

                if (int.TryParse(value, out int intValue))
                {
                    if (parsingNumbers)
                    {
                        numericValues.Add(intValue);
                    }
                    else
                    {
                        timeValues.Add(value);
                    }
                }
                else if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
                {
                    if (parsingNumbers)
                    {
                        numericValues.Add(doubleValue);
                    }
                    else
                    {
                        timeValues.Add(value);
                    }
                }
                else
                {
                    parsingNumbers = false;
                    timeValues.Add(value);
                }
            }

            result.NumericValues = numericValues;
            result.TimeValues = timeValues;
            result.ValueCount = Math.Min(numericValues.Count, timeValues.Count);
            result.IsValid = numericValues.Count > 0 && timeValues.Count > 0;

            return result;
        }

        public static List<string> ConvertToSaveFormat(ParsedData data)
        {
            var results = new List<string>();

            int count = Math.Min(data.NumericValues.Count, data.TimeValues.Count);

            for (int i = 0; i < count; i++)
            {
                string line = $"{data.NumericValues[i]} '{data.TimeValues[i]}'";
                results.Add(line);
            }

            return results;
        }
    }
}