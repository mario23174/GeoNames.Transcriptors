using System.Collections.Generic;
using System.Linq;

namespace GeoNames.Transcriptors
{
    public class LatviaTranscriptor : ITranscriptor
    {
        private static readonly List<char> softConsonants = new List<char> {'ģ', 'ķ', 'ļ', 'ņ'};

        private static readonly List<char> hardConsonants = new List<char> {'g', 'k', 'l', 'n'};

        private static readonly List<char> vowels = new List<char>
        {
            'a',
            'ā',
            'e',
            'ē',
            'i',
            'o',
            'u',
            'ū'
        };

        private static readonly List<char> consonants = new List<char>
        {
            'b',
            'c',
            'č',
            'd',
            'f',
            'g',
            'ģ',
            'h',
            'j',
            'k',
            'ķ',
            'l',
            'ļ',
            'm',
            'n',
            'ņ',
            'p',
            's',
            'š',
            't',
            'v',
            'z',
            'ž'
        };

        private static readonly Dictionary<string, string> letterReplaceRules = new Dictionary<string, string>
        {
            {"a", "а"}, //я после ģ ķ ļ ņ
            {"ā", "а"},
            {"b", "б"},
            {"c", "ц"},
            {"č", "ч"},
            {"d", "д"},
            {"e", "е"}, //э (в начале слова), е (в остальных позициях)
            {"ē", "е"}, //э (в начале слова), е (в остальных позициях)
            {"f", "ф"},
            {"g", "г"},
            {"ģ", "г"},
            {"h", "х"},
            {"i", "и"}, // ы в сочетаниях li, ni в конце слова
            {"ī", "и"}, // ы в сочетаниях li, ni в конце слова
            {"j", "й"},
            {"k", "к"},
            {"ķ", "к"}, //перед согласными кь, перед гласными k
            {"l", "л"},
            {"ļ", "л"}, //перед согласными ль, перед гласными л
            {"m", "м"},
            {"n", "н"},
            {"ņ", "н"}, //перед согласными нь, перед гласными н
            {"o", "о"}, //после ģ ķ ļ ņ  - ё
            {"p", "п"},
            {"r", "р"},
            {"s", "с"},
            {"š", "ш"},
            {"t", "т"},
            {"u", "у"}, //после ģ ķ ļ ņ  - ю
            {"ū", "у"}, //после ģ ķ ļ ņ  - ю
            {"v", "в"},
            {"z", "з"},
            {"ž", "ж"}
        };

        private static readonly Dictionary<string, string> twoLettersReplaceRules = new Dictionary<string, string>
        {
            {"ie", "ие"},
            {"ai", "ай"},
            {"ei", "ей"}, //эй (в начале слова), ей (в остальных позициях)
            {"ui", "уй"},

            {"ja", "я"}, // в начале слова и после гласных
            {"jā", "я"}, // в начале слова и после гласных

            {"jo", "йо"},

            {"je", "е"}, // в начале слова и после гласных
            {"jē", "е"}, // в начале слова и после гласных

            {"ji", "йи"}, // в начале слова и после гласных
            {"jī", "йи"}, // в начале слова и после гласных

            {"ju", "ю"}, // в начале слова и после гласных
            {"jū", "ю"} // в начале слова и после гласных
        };

        //вот как это сделать я не знаю
        //Примечание: если географический термин состоит из двух основ(Gūtmaņala = Gūtmaņa + ala), и вторая основа начинается с гласного, то после конечного согласного первой основы пишется разделительный мягкий знак ь(Гутманьала).


        public string ToRussian(string text)
        {
            //получаем токены
            var tokens = TokenizeString(text);

            #region применяем правила для изменения русского языка в токене

            foreach (var token in tokens)
            {
                if (token.ForangeText == "a" || token.ForangeText == "ā")
                    token.RuText = TranslateА(token);

                if (token.ForangeText == "e" || token.ForangeText == "ē")
                    token.RuText = TranslateE(token);


                if (token.ForangeText == "i" || token.ForangeText == "ī")
                    token.RuText = TranslateI(token);

                if ((token.ForangeText == "ķ" || token.ForangeText == "ļ" || token.ForangeText == "ņ") &&
                    token.NextToken != null && consonants.Contains(token.NextToken.ForangeText.First()))
                    token.RuText = token.RuText + "ь";

                if (token.ForangeText == "o" && token.PrevToken != null &&
                    softConsonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ё";

                if ((token.ForangeText == "u" || token.ForangeText == "ū") && token.PrevToken != null &&
                    softConsonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ю";

                //ei– эй (в начале слова), ей (в остальных позициях)
                if (token.ForangeText == "ei" && token.StartPosition != 0)
                    token.RuText = "ей";

                if (token.ForangeText == "ja" || token.ForangeText == "jā")
                    token.RuText = TranslateJa(token);

                if (token.ForangeText == "je" || token.ForangeText == "jē")
                    token.RuText = TranslateJe(token);

                if (token.ForangeText == "ji" || token.ForangeText == "jī")
                    token.RuText = TranslateJi(token);

                if (token.ForangeText == "ju" || token.ForangeText == "jū")
                    token.RuText = TranslateJu(token);
            }

            #endregion

            var result = tokens.Aggregate(string.Empty, (current, token) => current + token.RuText);

            return result;
        }

        public string Language => "Латвийский";

        public int Id => 1;

        public string TableInBase { get; set; }

        private static string TranslateJu(LetterToken token)
        {
            //{ "jū", "ю"}, // в начале слова и после гласных
            //{ "jū", "ъю"}, // после g k l n
            //{ "jū", "ью"}, // после остальных согласных
            if (token.StartPosition == 0 ||
                token.PrevToken != null && vowels.Contains(token.PrevToken.ForangeText.Last()))
                return "ю";

            if (token.PrevToken != null && hardConsonants.Contains(token.PrevToken.ForangeText.Last()))
                return "ъю";

            return "ью";
        }

        private static string TranslateJi(LetterToken token)
        {
            //{ "jī", "йи"}, // в начале слова и после гласных
            //{ "jī", "ъи"}, // после g k l n
            //{ "jī", "ьи"}, // после остальных согласных
            if (token.StartPosition == 0 ||
                token.PrevToken != null && vowels.Contains(token.PrevToken.ForangeText.Last()))
                return "йи";

            if (token.PrevToken != null && hardConsonants.Contains(token.PrevToken.ForangeText.Last()))
                return "ъи";

            return "ьи";
        }

        private static string TranslateJe(LetterToken token)
        {
            //{ "je", "е"}, // в начале слова и после гласных
            //{ "je", "ъе"}, // после g k l n
            //{ "je", "ье"}, // после остальных согласных
            if (token.StartPosition == 0 ||
                token.PrevToken != null && vowels.Contains(token.PrevToken.ForangeText.Last()))
                return "е";

            if (token.PrevToken != null && hardConsonants.Contains(token.PrevToken.ForangeText.Last()))
                return "ъе";

            return "ье";
        }

        private static string TranslateJa(LetterToken token)
        {
            //{ "ja", "я"}, // в начале слова и после гласных
            //{ "ja", "ъя"}, // после g k l n
            //{ "ja", "ья"}, // после остальных согласных
            if (token.StartPosition == 0 ||
                token.PrevToken != null && vowels.Contains(token.PrevToken.ForangeText.Last()))
                return "я";

            if (token.PrevToken != null && hardConsonants.Contains(token.PrevToken.ForangeText.Last()))
                return "ъя";

            return "ья";
        }


        private static string TranslateI(LetterToken token)
        {
            //{ "i", "и"},// ы в сочетаниях li, ni в конце слова
            //{ "ī", "и"},// ы в сочетаниях li, ni в конце слова
            if (token.PrevToken != null && (token.PrevToken.ForangeText == "l" || token.PrevToken.ForangeText == "n") &&
                token.NextToken == null)
                return "ы";
            return "и";
        }

        private static string TranslateE(LetterToken token)
        {
            // { "e", "е"},//э (в начале слова), е (в остальных позициях)
            //{ "ē", "е"},//э (в начале слова), е (в остальных позициях)
            return token.StartPosition == 0 ? "э" : "e";
        }

        private static string TranslateА(LetterToken token)
        {
            //{ "a", "а"}, //я после ģ ķ ļ ņ

            if (token.PrevToken != null && softConsonants.Contains(token.PrevToken.ForangeText.First()))
                return "я";
            return "а";
        }


        private static List<LetterToken> TokenizeString(string text)
        {
            var tokens = new List<LetterToken>();

            var ignoreCaseText = text.ToLowerInvariant();
            var textLength = ignoreCaseText.Length;

            for (var index = 0; index < textLength; index++)
            {
                var twoLetters = string.Empty;


                if (index + 2 <= ignoreCaseText.Length)
                    twoLetters = ignoreCaseText.Substring(index, 2);

                var oneLetter = ignoreCaseText.Substring(index, 1);

                if (twoLettersReplaceRules.ContainsKey(twoLetters))
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