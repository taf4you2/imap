
namespace imap_samemu
{
    internal class MessageCounter
    {
        private readonly string _CounterFilePath;

        private int LastCount;

        public MessageCounter (string CounterFilePath = "messege_counter.txt")
        {
            _CounterFilePath = CounterFilePath;
            LoadCount();
        }

        public int CurrentCount => LastCount;

        private void LoadCount()
        {
            try
            {
                if (File.Exists(_CounterFilePath))
                {
                    string       content = File.ReadAllText(_CounterFilePath).Trim();

                    if (int.TryParse(content, out int count))
                    {
                        LastCount = count;
                        Console.WriteLine($"Wczytano ostatnią liczbę wiadomości: {LastCount}");
                    }
                    else
                    {
                        LastCount = 0;
                        Console.WriteLine("Niepoprawny format w pliku licznika. Ustawiono na 0.");
                    }
                }
                else
                {
                    LastCount = 0;
                    Console.WriteLine("Plik licznika nie istnieje. Ustawiono na 0.");
                }
            }
            catch (Exception ex)
            {
                LastCount = 0;
                Console.WriteLine($"Błąd podczas wczytywania licznika: {ex.Message}. Ustawiono na 0.");
            }
        }

        public void SaveCount(int CurrentCount)
        {
            try
            {

                /*
                ========================================
                pamietac o tym
                ========================================
                */

                CurrentCount = 0;
                File.WriteAllText(_CounterFilePath, CurrentCount.ToString());
                Console.WriteLine($"Zapisano nową liczbę wiadomości: {CurrentCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisywania licznika: {ex.Message}");
            }
        }

        public int GetNewMessagesCount(int currentTotalCount)
        {
            int newMessages = currentTotalCount - LastCount;
            return Math.Max(0, newMessages); // Nie może być ujemna
        }

        public void UpdateProcessedCount(int currentTotalCount)
        {
            LastCount = currentTotalCount;
            SaveCount(currentTotalCount);
        }

        public bool HasNewMessages(int currentTotalCount)
        {
            return GetNewMessagesCount(currentTotalCount) > 0;
        }

        public void Reset()
        {
            LastCount = 0;
            SaveCount(0);
            Console.WriteLine("Licznik został zresetowany.");
        }

    }
}
