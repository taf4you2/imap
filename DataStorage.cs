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

        public static void InicializeStorage()
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






    }
}
