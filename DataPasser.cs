using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public static ParsedData ParsedEmailBody(string emailBody, DataType dataType, DateTime timestamp)
        {
            var result = new ParsedData()
            {
                DataType = dataType,
                Timestamp = timestamp,
                IsValid = false
            };

            if (string.IsNullOrEmpty(emailBody))
            {
                return result;
            }


            string[] lines = emailBody.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            List<double> numericValues = new List<double>();
            List<string> timeValues = new List<string>();

            bool parsingNumbers = true;

            foreach (string line in lines)
            {
                string value = line.Trim();

                if (string.IsNullOrEmpty(value))
                    continue;

                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
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
            result.IsValid = numericValues.Count > 0;

            return result;
        }
        public static List<string> ConvertToSaveFormat(ParsedData data)
        {
            var results = new List<string>();

            int count = Math.Min(data.NumericValues.Count, data.TimeValues.Count);

            for(int i = 0; i < count; ++i)
            {
                string line = $"{data.NumericValues[i]} {data.TimeValues[i]}";
                results.Add(line);
            }
            return results;
        }

        public static void PrintSummary(ParsedData data)
        {
            Console.WriteLine($"  Typ danych: {EmailPatternRecognizer.GetDataTypeString(data.DataType)}");
            Console.WriteLine($"  Timestamp: {data.Timestamp:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"  Liczba wartości numerycznych: {data.NumericValues.Count}");
            Console.WriteLine($"  Liczba znaczników czasu: {data.TimeValues.Count}");
            Console.WriteLine($"  Dopasowanych par: {data.ValueCount}");

            if (data.NumericValues.Count > 0)
            {
                Console.WriteLine($"  Zakres wartości: {data.NumericValues.Min():F2} - {data.NumericValues.Max():F2}");
                Console.WriteLine($"  Średnia: {data.NumericValues.Average():F2}");
            }
        }



    }

}