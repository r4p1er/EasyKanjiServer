using EasyKanji.Shared;

namespace EasyKanji.Client
{
    static public class GlobalVariables
    {
        static public List<Kanji> LearnKanjiList { get; set; }

        static GlobalVariables()
        {
            LearnKanjiList = new List<Kanji>();
        }
    }
}
