/*
    Install-Package MailKit
    Install-Package MimeKit
 */
using imap_samemu;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.RegularExpressions;

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
                return;
            }

            /*
            === TO DO === nie przejmować się
            dorobic zapytanie jezeli danych sie nie uda wczytac czy chce wpisac jeszcze raz i czy chce je zapisac
            */

            Console.WriteLine("Dane zostały wczytane pomyślnie");

            await ReadGmailAsync(dane);

        }
        static async Task ReadGmailAsync(Json dane)
        {
            using var client = new ImapClient();

            // Połącz z Gmail
            Console.WriteLine("Łączenie z Gmail...");
            await client.ConnectAsync("imap.gmail.com", 993, true);

            Console.WriteLine("Logowanie...");
            await client.AuthenticateAsync(dane.Email, dane.Password);

            await client.Inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            Console.WriteLine($"Połączono!\n");

            await WczytajIWyswietlMaile(client);

            await client.DisconnectAsync(true);
        }

        static async Task WczytajIWyswietlMaile(ImapClient client)
        {
            var inbox = client.Inbox;

            Console.WriteLine($"Liczba wiadomości: {inbox.Count}");
            Console.WriteLine("Pobieranie nagłówków wiadomości...\n");

            // 10 ostatnich wiadomości
            int liczbaMaili = Math.Min(10, inbox.Count);

            for (int i = inbox.Count - liczbaMaili; i < inbox.Count; i++)
            {
                var wiadomosc = await inbox.GetMessageAsync(i);
                WyswietlMailBezTresci(wiadomosc, i + 1);
                Console.WriteLine(new string('-', 50));
            }
        }

        static void WyswietlMailBezTresci(MimeMessage wiadomosc, int numer)
        {
            Console.WriteLine($"Mail #{numer}");
            Console.WriteLine($"Od: {wiadomosc.From}");
            Console.WriteLine($"Temat: {wiadomosc.Subject ?? "(brak tematu)"}");
            Console.WriteLine($"Data: {wiadomosc.Date:yyyy-MM-dd HH:mm}");
            Console.WriteLine("=============\n");
        }

    }
}