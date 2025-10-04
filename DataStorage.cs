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
                if(!data.IsValid)
                {
                    Console.WriteLine("======   Próba zapisu nieprawidłowych danych   ======");
                    return false;
                }

                string directoryPath = GetDataTypePath(data.DataType);
                string fileName = GenerateFileName(data.DataType, data.Timestamp);
                string fullPath = Path.Combine(directoryPath, fileName);
                var linesToSave = DataParser.ConvertToSaveFormat(data);

                bool fileExists = File.Exists(fullPath);
                if (fileExists)
                {
                    File.AppendAllLines(fullPath, linesToSave);
                    Console.WriteLine($"Dodano {linesToSave.Count} wpisów do: {fullPath}");
                }
                else
                {
                    File.WriteAllLines(fullPath, linesToSave);
                    Console.WriteLine($"Utworzono nowy plik: {fullPath}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisu danych: {ex.Message}");
                return false;
            }
            return true;
        }

        




    }
}
