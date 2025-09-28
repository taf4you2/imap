
using Org.BouncyCastle.Bcpg;
using System.Diagnostics;

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
                    string content = File.ReadAllText(_CounterFilePath).Trim();

                    if (int.TryParse(content, out int count))
                    {
                        LastCount = count;
                        Console.WriteLine($"Wczytano ostatnią liczbę wiadomości: {_lastProcessedCount}");
                    }
                    else
                    {

                    }
                }
            }
        }


    }
}
