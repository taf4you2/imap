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
            var dane = await Helper.Wczytywanie();

            if (dane == null)
            {
                Console.WriteLine("Nie udało się wczytać danych. Program zostanie zakończony.");
                return;
            }

            Console.WriteLine("Dane zostały wczytane pomyślnie");
            await ReadGmailAsync(dane);
        }

        static async Task ReadGmailAsync(Json dane)
        {
            using var client = new ImapClient();

            Console.WriteLine("Łączenie z Gmail...");
            await client.ConnectAsync("imap.gmail.com", 993, true);

            Console.WriteLine("Logowanie...");
            await client.AuthenticateAsync(dane.Email, dane.Password);

            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            Console.Clear();

            GornaCzesc(dane.Email);

            await client.Inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            var messageCounter = new MessageCounter();

            int currentTotalMessages = client.Inbox.Count;
            Console.WriteLine($"Całkowita liczba wiadomości w skrzynce: {currentTotalMessages}");

            if (messageCounter.HasNewMessages(currentTotalMessages))
            {
                int newMessagesCount = messageCounter.GetNewMessagesCount(currentTotalMessages);
                Console.WriteLine($"Nowych wiadomości do przetworzenia: {newMessagesCount}");

                await ProcessNewMessages(client, messageCounter.CurrentCount, currentTotalMessages);

                messageCounter.UpdateProcessedCount(currentTotalMessages);
            }
            else
            {
                Console.WriteLine("Brak nowych wiadomości do przetworzenia.");
            }

            Console.WriteLine("Zakończono przetwarzanie.");
        }

        static async Task ProcessNewMessages(ImapClient client, int lastProcessed, int currentTotal)
        {
            Console.WriteLine($"\nPrzetwarzanie wiadomości od {lastProcessed + 1} do {currentTotal}...");

            for (int i = lastProcessed; i < currentTotal; i++)
            {
                try
                {
                    var message = await client.Inbox.GetMessageAsync(i);
                    string subject = message.Subject ?? "";

                    var pattern = EmailPatternRecognizer.RecognizePattern(subject);

                    if (pattern.IsValid)
                    {
                        Console.WriteLine($"Rozpoznano: {EmailPatternRecognizer.GetDataTypeString(pattern.DataType)} z {pattern.DateTime:yyyy-MM-dd HH:mm}");

                        string messageBody = message.TextBody ?? message.HtmlBody ?? "";

                        Console.WriteLine($"Treść do przetworzenia: {messageBody.Length} znaków");
                    }
                    else if (pattern.DataType != DataType.unknown)
                    {
                        Console.WriteLine($"Rozpoznano typ {EmailPatternRecognizer.GetDataTypeString(pattern.DataType)}, ale błędna data w: '{subject}'");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Błąd przy przetwarzaniu wiadomości #{i + 1}: {ex.Message}");
                }
            }
        }

        static void WyswietlMailBezTresci(MimeMessage wiadomosc, int numer)
        {
            Console.WriteLine($"Mail #{numer}");
            Console.WriteLine($"Od: {wiadomosc.From}");
            Console.WriteLine($"Temat: {wiadomosc.Subject ?? "(brak tematu)"}");
            Console.WriteLine($"Data: {wiadomosc.Date:yyyy-MM-dd HH:mm}");
        }

        static void GornaCzesc(string email)
        {
            Console.WriteLine("=====================");
            Console.WriteLine($"Jesteś zalogowany do:\n{email}");
            Console.WriteLine("=====================\n");
        }

    }
}