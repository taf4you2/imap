using System.Text.Json;

namespace imap_samemu
{
    public static class Pomocnik
    {
        private static readonly string SciezkaDoJson = "C:\\Users\\wojtek\\Desktop\\imap\\pass.json";

        public static async Task<Json> Wczytywanie()
        {
            if (!File.Exists(SciezkaDoJson))
            {
                Console.WriteLine("Plik JSON nie istnieje!");
                return null;
            }

            try
            {
                string zawartosc = await File.ReadAllTextAsync(SciezkaDoJson);
                var rozpakowane = JsonSerializer.Deserialize<Json>(zawartosc);

                if (string.IsNullOrWhiteSpace(rozpakowane?.Email) ||
                    string.IsNullOrWhiteSpace(rozpakowane?.Password))
                {
                    Console.WriteLine("Vrak wymaganych danych");
                    return null;
                }

                Console.WriteLine($"Email: {rozpakowane.Email}");
                Console.Write("Password: ");
                for (int i = 0; i < rozpakowane.Password.Length; i++)
                    Console.Write("*");
                Console.WriteLine();

                return rozpakowane;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wczytywania pliku: {ex.Message}");
                return null;
            }
        }
    }
}