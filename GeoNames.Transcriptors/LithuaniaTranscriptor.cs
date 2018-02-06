using System.Collections.Generic;
using System.Linq;

namespace GeoNames.Transcriptors
{
    public class LithuaniaTranscriptor : ITranscriptor
    {
        private static readonly List<char> consonants = new List<char>
        {
            'b',
            'c',
            'č',
            'd',
            'f',
            'g',
            'h',
            'k',
            'l',
            'm',
            'n',
            'p',
            'r',
            's',
            'š',
            't',
            'v',
            'z',
            'ž'
        };


        private static readonly Dictionary<string, string> letterReplaceRules = new Dictionary<string, string>
        {
            {"a", "а"},
            {"ą", "а"},
            {"b", "б"},
            {"c", "ц"},
            {"č", "ч"},
            {"d", "д"},
            {"e", "я"}, //спорно, по идее нужно е
            {"ė", "э"},
            {"ę", "э"},
            {"f", "ф"},
            {"g", "г"}, //х в некоторых случаях
            {"h", "х"},
            {"i", "и"},
            {"į", "а"},
            {"y", "и"},
            {"k", "к"},
            {"l", "л"},
            {"m", "м"},
            {"n", "н"},
            {"o", "о"},
            {"p", "п"},
            {"r", "р"},
            {"s", "с"},
            {"š", "ш"},
            {"t", "т"},
            {"u", "у"},
            {"ū", "у"},
            {"ų", "у"},
            {"v", "в"},
            {"z", "з"},
            {"ž", "ж"}
        };

        private static readonly Dictionary<string, string> twoLettersReplaceRules = new Dictionary<string, string>
        {
            {"ai", "ай"},
            {"au", "ау"},
            {"ei", "эй"},
            {"ie", "е"},
            {"ia", "я"},
            {"ią", "я"},
            {"io", "ё"},
            {"iu", "ю"},
            {"iū", "ю"},
            {"ių", "ю"},
            {"ui", "уй"},
            {"uo", "уо"},
            {"ja", "я"},
            {"ją", "я"},
            {"je", "е"},
            {"ję", "е"},
            {"ji", "йи"},
            {"iį", "йи"},
            {"jy", "йи"},
            {"jo", "йо"},
            {"ju", "ю"},
            {"jų", "ю"},
            {"jū", "ю"},
            {"ch", "х"},
            {"šč", "щ"}
        };

        private static readonly Dictionary<string, string> threeLettersReplaceRules = new Dictionary<string, string>
        {
            {"iui", "юй"},
            {"iuo", "юо"},
            {"iei", "ей"},
            {"iai", "яй"},
            {"iau", "яу"},
            {"jai", "яй"},
            {"jau", "яу"},
            {"jei", "ей"},
            {"jie", "е"},
            {"juo", "юо"}
        };

        public string ToRussian(string text)
        {
            //получаем токены
            var tokens = TokenizeString(text);

            #region применяем правила для изменения русского языка в токене

            foreach (var token in tokens)
            {
                //e	           –     	э (в начале слова и после гласного, за исключением i в дифтонге ie), е (после согласных)
                //ę	           –     	э (в начале слова и после гласного, за исключением i в дифтонге ie), е (после согласных)
                if (token.ForangeText == "ę" || token.ForangeText == "ė")
                    token.RuText = TranslateE(token);

                //ei– эй (в начале слова), ей (в остальных позициях)
                if (token.ForangeText == "ei" && token.StartPosition != 0)
                    token.RuText = "ей";

                //l	   –   л (перед твёрдым согласным), ль (перед мягких согласным)
                if (token.ForangeText == "l")
                    token.RuText = TranslateL(token);

                //если пробел - заменяем на -
                if (token.ForangeText == " ")
                    token.RuText = "-";

                //На стыке согласного с j (Pjūkla) в литовских словах, за исключением jo, при транскрибировании употребляется разделительный мягкий знак ь (Пьюкла).
                if (token.PrevToken != null && token.ForangeText.First() == 'j' && token.ForangeText != "jo" &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.PrevToken.RuText += "ь";


                //Все шипящие в литовском языке произносятся мягко, поэтому, вопреки русской орфографической традиции, во всех случаях после ж, ш, ч, щ пишутся ё, я, ю.

                // в некоторых случаях первый компонент буквенного сочетания io обозначает слогообразующий звук [i] и по-русски передаётся буквой и, т. е. io передаётся как ио.
            }

            #endregion

            var result = tokens.Aggregate(string.Empty, (current, token) => current + token.RuText);

            return result;
        }

        public string Language => "Литовский";

        public int Id => 2;

        public string TableInBase { get; set; }

        private static string TranslateE(LetterToken token)
        {
            //e	           –     	э (в начале слова и после гласного, за исключением i в дифтонге ie), е (после согласных)
            //ę	           –     	э (в начале слова и после гласного, за исключением i в дифтонге ie), е (после согласных)

            if (token.PrevToken != null && consonants.Contains(token.PrevToken.ForangeText.Last()))
                return "е";
            return "э";
        }

        private static string TranslateL(LetterToken token)
        {
            //l	   –   л (перед твёрдым согласным), ль (перед мягких согласным)

            if (token.NextToken != null && IsSoftConsonant(token.NextToken))
                return "ль";
            return "л";
        }

        private static bool IsSoftConsonant(LetterToken token)
        {
            var result = token.RuText.First() == 'ч' || token.RuText.First() == 'щ' || token.RuText.First() == 'й';

            //ч, щ, й

            // И, Е, Ё, Ю, Я
            if (token.NextToken != null)
                if (token.NextToken.RuText.First() == 'и' ||
                    token.NextToken.RuText.First() == 'е' ||
                    token.NextToken.RuText.First() == 'ё' ||
                    token.NextToken.RuText.First() == 'ю' ||
                    token.NextToken.RuText.First() == 'я')
                    result = true;

            return result;
        }


        private static List<LetterToken> TokenizeString(string text)
        {
            var tokens = new List<LetterToken>();

            var ignoreCaseText = text.ToLowerInvariant();
            var textLength = ignoreCaseText.Length;

            for (var index = 0; index < textLength; index++)
            {
                var threeLetters = string.Empty;
                var twoLetters = string.Empty;

                if (index + 3 <= ignoreCaseText.Length)
                    threeLetters = ignoreCaseText.Substring(index, 3);

                if (index + 2 <= ignoreCaseText.Length)
                    twoLetters = ignoreCaseText.Substring(index, 2);

                var oneLetter = ignoreCaseText.Substring(index, 1);

                if (threeLettersReplaceRules.ContainsKey(threeLetters))
                {
                    var token = new LetterToken
                    {
                        StartPosition = index,
                        EndPosition = index + 3,
                        ForangeText = threeLetters,
                        RuText = threeLettersReplaceRules[threeLetters]
                    };
                    tokens.Add(token);
                    index += 2;
                }
                else if (twoLettersReplaceRules.ContainsKey(twoLetters))
                {
                    var token = new LetterToken
                    {
                        StartPosition = index,
                        EndPosition = index + 2,
                        ForangeText = twoLetters,
                        RuText = twoLettersReplaceRules[twoLetters]
                    };
                    tokens.Add(token);
                    index += 1;
                }
                else if (letterReplaceRules.ContainsKey(oneLetter))
                {
                    var token = new LetterToken
                    {
                        StartPosition = index,
                        EndPosition = index,
                        ForangeText = oneLetter,
                        RuText = letterReplaceRules[oneLetter]
                    };
                    tokens.Add(token);
                }
                else
                {
                    var token = new LetterToken
                    {
                        StartPosition = index,
                        EndPosition = index,
                        ForangeText = oneLetter,
                        RuText = oneLetter
                    };
                    tokens.Add(token);
                }
            }

            var result = tokens.OrderBy(t => t.StartPosition).ToList();

            for (var index = 0; index < tokens.Count; index++)
            {
                if (index != 0)
                    result[index].PrevToken = result[index - 1];
                if (index != tokens.Count - 1)
                    result[index].NextToken = result[index + 1];
            }
            return result;
        }
    }
}