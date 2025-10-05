namespace imap_samemu
{
    internal class MessageCounter
    {
        private readonly string _CounterFilePath;
        private HashSet<string> _checkpointIds;

        public MessageCounter(string CounterFilePath = "message_counter.txt")
        {
            _CounterFilePath = CounterFilePath;
            LoadCheckpoints();
        }

        private void LoadCheckpoints()
        {
            try
            {
                if (File.Exists(_CounterFilePath))
                {
                    var lines = File.ReadAllLines(_CounterFilePath);
                    _checkpointIds = new HashSet<string>(lines);
                    Console.WriteLine($"Wczytano {_checkpointIds.Count} punktów kontrolnych");
                }
                else
                {
                    _checkpointIds = new HashSet<string>();
                    Console.WriteLine("Plik licznika nie istnieje. Pierwszy start.");
                }
            }
            catch (Exception ex)
            {
                _checkpointIds = new HashSet<string>();
                Console.WriteLine($"Błąd podczas wczytywania licznika: {ex.Message}");
            }
        }

        private void SaveCheckpoints()
        {
            try
            {
                File.WriteAllLines(_CounterFilePath, _checkpointIds);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisywania licznika: {ex.Message}");
            }
        }

        public bool IsCheckpoint(string messageId)
        {
            return _checkpointIds.Contains(messageId);
        }

        public void AddCheckpoint(string newestMessageId)
        {
            if (!string.IsNullOrEmpty(newestMessageId))
            {
                _checkpointIds.Add(newestMessageId);
                SaveCheckpoints();
                Console.WriteLine($"Dodano nowy punkt kontrolny: {newestMessageId}");
            }
        }

        public void Reset()
        {
            _checkpointIds.Clear();
            SaveCheckpoints();
            Console.WriteLine("Licznik został zresetowany.");
        }
    }
}