namespace EasyKanji.Server.Models
{
    public class Kanji
    {
        public int Id { get; set; }
        public string Writing { get; set; }
        public string Meaning { get; set; }
        public string OnReadings { get; set; }
        public string KunReadings { get; set; }
        public string Words { get; set; }

        public Kanji(string writing, string meaning, string onReadings, string kunReadings, string words)
        {
            Writing = writing;
            Meaning = meaning;
            OnReadings = onReadings;
            KunReadings = kunReadings;
            Words = words;
        }
    }
}
