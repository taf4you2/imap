namespace imap_samemu
{
    internal class MessageCounter
    {
        private readonly string _CounterFilePath;
        private string _lastProcessedMessageId;

        public MessageCounter(string CounterFilePath = "message_counter.txt")
        {
            _CounterFilePath = CounterFilePath;
            LoadLastProcessedId();
        }

        private void LoadLastProcessedId()
        {
            try
            {
                if (File.Exists(_CounterFilePath))
                {
                    _lastProcessedMessageId = File.ReadAllText(_CounterFilePath).Trim();
                    Console.WriteLine($"Wczytano ostatnio przetworzone ID: {_lastProcessedMessageId}");
                }
                else
                {
                    _lastProcessedMessageId = null;
                    Console.WriteLine("Plik licznika nie istnieje. Pierwszy start.");
                }
            }
            catch (Exception ex)
            {
                _lastProcessedMessageId = null;
                Console.WriteLine($"Błąd podczas wczytywania licznika: {ex.Message}");
            }
        }

        private void SaveLastProcessedId(string messageId)
        {
            try
            {
                File.WriteAllText(_CounterFilePath, messageId);
                Console.WriteLine($"Zapisano ostatnie ID: {messageId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisywania licznika: {ex.Message}");
            }
        }

        public bool ShouldProcess(string messageId)
        {
            if (string.IsNullOrEmpty(_lastProcessedMessageId))
            {
                return true; // Pierwsza uruchomienie - przetwórz wszystko
            }
            return messageId != _lastProcessedMessageId;
        }

        public void UpdateLastProcessed(string messageId)
        {
            _lastProcessedMessageId = messageId;
            SaveLastProcessedId(messageId);
        }

        public void Reset()
        {
            _lastProcessedMessageId = null;
            File.WriteAllText(_CounterFilePath, "");
            Console.WriteLine("Licznik został zresetowany.");
        }
    }
}