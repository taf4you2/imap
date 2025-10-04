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
            var data = await Helper.Loading();

            if (data == null)
            {
                Console.WriteLine("Nie udało się wczytać danych. Program zostanie zakończony.");
                return;
            }
            DataStorage.InitializeStorage();


            //Console.WriteLine("Dane zostały wczytane pomyślnie");
            await ReadGmailAsync(data);
        }

        static async Task ReadGmailAsync(Json data)
        {
            using var client = new ImapClient();

            Console.WriteLine("Łączenie z Gmail...");
            await client.ConnectAsync("imap.gmail.com", 993, true);

            Console.WriteLine("Logowanie...");
            await client.AuthenticateAsync(data.Email, data.Password);

            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            Console.Clear();

            UpperPart(data.Email);

            await client.Inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            var messageCounter = new MessageCounter();

            int totalMessages = client.Inbox.Count;
            Console.WriteLine($"Całkowita liczba wiadomości w skrzynce: {totalMessages}");

            await ProcessMessages(client, messageCounter, totalMessages);

            Console.WriteLine("Zakończono przetwarzanie.");
        }
        static async Task ProcessMessages(ImapClient client, MessageCounter counter, int totalMessages)
        {
            Console.WriteLine($"\nSprawdzanie wiadomości...");

            int newMessagesProcessed = 0;
            string lastProcessedId = null;
            bool foundLastProcessed = false;

            // Przeglądaj od najnowszej do najstarszej
            for (int i = totalMessages - 1; i >= 0; i--)
            {
                try
                {
                    var message = await client.Inbox.GetMessageAsync(i);
                    string messageId = message.MessageId ?? $"unknown_{i}";

                    // Jeśli znaleźliśmy ostatnio przetworzoną wiadomość, kończymy
                    if (!counter.ShouldProcess(messageId))
                    {
                        foundLastProcessed = true;
                        Console.WriteLine($"Znaleziono ostatnio przetworzoną wiadomość. Zatrzymano.");
                        break;
                    }

                    DisplayMailWithoutContent(message, i + 1);
                    Console.WriteLine();

                    string subject = message.Subject ?? "";
                    var pattern = EmailPatternRecognizer.RecognizePattern(subject);

                    if (pattern.IsValid)
                    {
                        Console.WriteLine($"\n Rozpoznano: {EmailPatternRecognizer.GetDataTypeString(pattern.DataType)} z {pattern.DateTime:yyyy-MM-dd HH:mm}");

                        string messageBody = message.TextBody ?? message.HtmlBody ?? "";
                        var parsedData = DataParser.ParseEmailBody(messageBody, pattern.DataType, pattern.DateTime);

                        bool saved = DataStorage.SaveData(parsedData);

                        if (!parsedData.IsValid)
                        {
                            Console.WriteLine("Nie udało się sparsować danych z treści emaila");
                        }
                    }

                    // Zapamiętaj ID pierwszej przetworzonej wiadomości (najnowszej)
                    if (lastProcessedId == null)
                    {
                        lastProcessedId = messageId;
                    }

                    newMessagesProcessed++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd przy przetwarzaniu wiadomości nr {i}: {ex.Message}");
                }
            }

            // Zapisz ID najnowszej przetworzonej wiadomości
            if (lastProcessedId != null)
            {
                counter.UpdateLastProcessed(lastProcessedId);
            }

            Console.WriteLine($"\nPrzetworzono {newMessagesProcessed} nowych wiadomości");
        }
        static void DisplayMailWithoutContent(MimeMessage wiadomosc, int numer)
        {
            Console.WriteLine($"Mail #{numer}");
            Console.WriteLine($"Od: {wiadomosc.From}");
            Console.WriteLine($"Temat: {wiadomosc.Subject ?? "(brak tematu)"}");
            Console.WriteLine($"Data: {wiadomosc.Date:yyyy-MM-dd HH:mm}");
        }

        static void UpperPart(string email)
        {
            Console.WriteLine("=====================");
            Console.WriteLine($"Jesteś zalogowany do:\n{email}");
            Console.WriteLine("=====================\n");
        }

    }
}