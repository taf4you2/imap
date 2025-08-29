/*
    Install-Package MailKit
    Install-Package MimeKit
 */
using imap_samemu;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace imap_samemu
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== Gmail Reader ===\n");
            Console.WriteLine("Wczytywanie pliku JSON");
            var dane = await Pomocnik.Wczytywanie();

            if (dane == null)
            {
                Console.WriteLine("Nie udało się wczytać danych. Program zostanie zakończony.");
            }

            Console.WriteLine("Dane zostały wczytane");
        }
    }
}