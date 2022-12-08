namespace EasyKanji.Server.Models
{
    public class Kanji
    {
        public int Id { get; set; }
        public string Writing { get; set; }
        public string Meanings { get; set; }
        public string OnReadings { get; set; }
        public string KunReadings { get; set; }

        public Kanji(string writing, string meanings, string onReadings, string kunReadings)
        {
            Writing = writing;
            Meanings = meanings;
            OnReadings = onReadings;
            KunReadings = kunReadings;
        }
    }
}
