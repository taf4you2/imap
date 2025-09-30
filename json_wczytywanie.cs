using System.Text.Json;

namespace imap_samemu
{
    public static class Helper
    {
        private static readonly string PathToJson = "pass.json";

        public static async Task<Json> Loading()
        {
            if (!File.Exists(PathToJson))
            {
                Console.WriteLine("Plik JSON nie istnieje!");
                return null;
            }

            try
            {
                string contents = await File.ReadAllTextAsync(PathToJson);
                var unpacked = JsonSerializer.Deserialize<Json>(contents);

                if (string.IsNullOrWhiteSpace(unpacked?.Email) || string.IsNullOrWhiteSpace(unpacked?.Password))
                {
                    Console.WriteLine("Brak wymaganych danych");
                    return null;
                }

                Console.WriteLine($"Email: {unpacked.Email}");
                Console.Write("Password: ");
                Console.WriteLine(new string('*', unpacked.Password.Length));

                /*
                === TO DO === nie przejmować się
                trzeba pytac na przyszlosc czy chce uzywac danych dla tego konta 
                */


                return unpacked;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wczytywania pliku: {ex.Message}");
                return null;
            }
        }
    }
}