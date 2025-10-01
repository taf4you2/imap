using System;
using System.Text.RegularExpressions;

namespace imap_samemu
{
    public enum DataType
    {
        //hrv,
        //temperatura,
        //sen,
        //oddech,
        tetno,
        //unknown
    }

    public class EmailPattern
    {
        public DataType DataType { get; set; }
        public DateTime DateTime { get; set; }
        public string OrginalSubject { get; set; }
        public bool IsValid { get; set; }
    }

    public static class EmailPatternRecognizer
    {
        private static readonly string[] MonthNames = {
            "sty", "lut", "mar", "kwi", "maj", "cze",
            "lip", "sie", "wrz", "paü", "lis", "gru"
        };

        private static readonly string DateTimePattern = @"(\d{1,2})\s+(\w{3})\s+(\d{4})\s+o\s+(\d{1,2}):(\d{2})";

        public static EmailPattern RecognizePattern(string subject)
        {
            var result = new EmailPattern
            {
                OrginalSubject = subject,
                //DataType = DataType.unknown,
                IsValid = false
            };

            if (string.IsNullOrEmpty(subject))
                return result;

            string subjectLower = subject.ToLower();

            foreach (DataType dataType in Enum.GetValues<DataType>())
            {
                //if (dataType == DataType.unknown) continue;

                string enumName = dataType.ToString().ToLower();

                if (subjectLower.StartsWith(enumName))
                {
                    result.DataType = dataType;
                    break;
                }
            }

            var match = Regex.Match(subject, DateTimePattern);
            if (match.Success)
            {
                try
                {
                    int day = int.Parse(match.Groups[1].Value);
                    string monthStr = match.Groups[2].Value.ToLower();
                    int year = int.Parse(match.Groups[3].Value);
                    int hour = int.Parse(match.Groups[4].Value);
                    int minute = int.Parse(match.Groups[5].Value);

                    int month = GetMonthNumber(monthStr);
                    if (month == -1)
                        return result;

                    result.DateTime = new DateTime(year, month, day, hour, minute, 0);
                    result.IsValid = true;
                }
                catch (Exception)
                {
                    result.IsValid = false;
                }
            }

            return result;
        }

        private static int GetMonthNumber(string monthAbbreviation)
        {
            for (int i = 0; i < MonthNames.Length; i++)
            {
                if (MonthNames[i].Equals(monthAbbreviation, StringComparison.OrdinalIgnoreCase))
                {
                    return i + 1;
                }
            }
            return -1;
        }

        public static bool IsRecognizedDataType(string subject)
        {
            var pattern = RecognizePattern(subject);
            return pattern.IsValid; //&& pattern.DataType != DataType.unknown;
        }

        public static string GetDataTypeString(DataType dataType)
        {
            return dataType switch
            {
                //DataType.hrv => "hrv",
                //DataType.temperatura => "temperatura",
                //DataType.sen => "sen",
                //DataType.oddech => "oddech",
                DataType.tetno => "tetno",
                //_ => "unknown"
            };
        }
    }
}