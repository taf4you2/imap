namespace imap_samemu
{
    public enum DataType
    {
        HRV,
        Temperatura,
        Sen,
        Oddech,
        Tetno,
        Unknown
    }
    
    public class EmailPattern
    {
        public DataType DataType { get; set; }
        public DateTime DateTime { get; set; }
        public string OrginalSubject { get; set; }
        public bool IsValid { get; set; }
    }

    public static class EmailPatternRecognizer
    {
        private static readonly string[] MonthNames = {
            "sty", "lut", "mar", "kwi", "maj", "cze",
            "lip", "sie", "wrz", "paz", "lis", "gru"
        };
        private static readonly string DateTimePattern = @"pattern do zrobienia";
    }


}