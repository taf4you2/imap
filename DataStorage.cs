using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imap_samemu
{
    public static class DataStorage
    {
        private static readonly string BaseDirectory = "data";

        private static string GetDataTypePath(DataType dataType)
        {
            string typeName = dataType.ToString().ToLower();
            return Path.Combine(BaseDirectory, typeName);
        }

        public static void InitializeStorage()
        {
            if (!Directory.Exists(BaseDirectory))
            {
                Directory.CreateDirectory(BaseDirectory);
                Console.WriteLine($"Utworzono katalog: {BaseDirectory}");
            }


            foreach (DataType dataType in Enum.GetValues<DataType>())
            {
                if (dataType == DataType.unknown) continue;

                string typePath = GetDataTypePath(dataType);
                if (!Directory.Exists(typePath))
                {
                    Directory.CreateDirectory(typePath);
                    Console.WriteLine($"Utworzono katalog: {dataType}");
                }
            }

        }

        private static string GenerateFileName(DataType dataType, DateTime timestamp)
        {
            string typeName = dataType.ToString().ToLower();
            string dateString = timestamp.ToString("yyyy-MM-dd");
            return $"{typeName}_{dateString}.txt";
        }

        public static bool SaveData(ParsedData data)
        {
            try
            {
                if (!data.IsValid)
                {
                    Console.WriteLine("====== Próba zapisu nieprawidłowych danych ======");
                    return false;
                }

                var groupedByDate = new Dictionary<string, List<(double value, string time)>>();

                int count = Math.Min(data.NumericValues.Count, data.TimeValues.Count);

                for (int i = 0; i < count; i++)
                {
                    string timeString = data.TimeValues[i];
                    double value = data.NumericValues[i];

                    DateTime? parsedDate = ParseTimeString(timeString);

                    if (parsedDate.HasValue)
                    {
                        string dateKey = parsedDate.Value.ToString("yyyy-MM-dd");

                        if (!groupedByDate.ContainsKey(dateKey))
                        {
                            groupedByDate[dateKey] = new List<(double, string)>();
                        }

                        groupedByDate[dateKey].Add((value, timeString));
                    }
                    else
                    {
                        Console.WriteLine($"Ostrzeżenie: Nie można sparsować daty z '{timeString}'");
                    }
                }

                int totalSaved = 0;
                foreach (var group in groupedByDate.OrderBy(x => x.Key))
                {
                    string dateKey = group.Key;
                    var entries = group.Value;

                    string directoryPath = GetDataTypePath(data.DataType);
                    string fileName = $"{data.DataType.ToString().ToLower()}_{dateKey}.txt";
                    string fullPath = Path.Combine(directoryPath, fileName);

                    var linesToSave = entries.Select(e => $"{e.value} '{e.time}'").ToList();

                    var uniqueLines = RemoveDuplicatesWithLastEntry(fullPath, linesToSave);

                    if (uniqueLines.Count > 0)
                    {
                        bool fileExists = File.Exists(fullPath);

                        File.AppendAllLines(fullPath, uniqueLines);

                        Console.WriteLine($"{(fileExists ? "Dodano" : "Utworzono")} {uniqueLines.Count} wpisów do {fileName} (data: {dateKey})");
                        totalSaved += uniqueLines.Count;
                    }
                    else
                    {
                        Console.WriteLine($"Pominięto {linesToSave.Count} duplikatów dla {fileName}");
                    }
                }

                Console.WriteLine($"Łącznie zapisano {totalSaved} wpisów w {groupedByDate.Count} plikach");
                return totalSaved > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisu danych: {ex.Message}");
                return false;
            }
        }

        private static DateTime? ParseTimeString(string timeString)
        {
            try
            {
                var parts = timeString.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 6)
                {
                    int day = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    int year = int.Parse(parts[2]);
                    int hour = int.Parse(parts[3]);
                    int minute = int.Parse(parts[4]);
                    int second = int.Parse(parts[5]);

                    return new DateTime(year, month, day, hour, minute, second);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ostrzeżenie przy sprawdzaniu duplikatów: {ex.Message}");
            }

            return null;
        }

        private static List<string> RemoveDuplicatesWithLastEntry(string filePath, List<string> newLines)
        {
            if (!File.Exists(filePath) || newLines.Count == 0)
            {
                return newLines;
            }

            try
            {
                var existingLines = File.ReadAllLines(filePath);
                if (existingLines.Length == 0)
                {
                    return newLines;
                }

                string lastExistingLine = existingLines[existingLines.Length - 1].Trim();

                var lastTimestamp = ExtractTimestampFromLine(lastExistingLine);

                if (lastTimestamp == null)
                {
                    return newLines;
                }

                var uniqueLines = new List<string>();

                foreach (var line in newLines)
                {
                    var currentTimestamp = ExtractTimestampFromLine(line);

                    if (currentTimestamp.HasValue && currentTimestamp.Value > lastTimestamp.Value)
                    {
                        uniqueLines.Add(line);
                    }
                }

                return uniqueLines;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ostrzeżenie przy sprawdzaniu duplikatów: {ex.Message}");
                return newLines;
            }
        }

        private static DateTime? ExtractTimestampFromLine(string line)
        {
            try
            {
                int startQuote = line.IndexOf('\'');
                int endQuote = line.LastIndexOf('\'');

                if (startQuote >= 0 && endQuote > startQuote)
                {
                    string timeString = line.Substring(startQuote + 1, endQuote - startQuote - 1);
                    return ParseTimeString(timeString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message}");
            }

            return null;
        }



    }
}
