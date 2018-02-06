using System.Collections.Generic;
using System.Linq;

namespace GeoNames.Transcriptors
{
    public class PolandTranscriptor : ITranscriptor
    {
        #region Традиционные названия

        private static readonly Dictionary<string, string> traditionNaming = new Dictionary<string, string>
        {
            {"augustów", "августов"},
            {"augustowska puszcza", "августовкая пуща"},
            {"augustowski Kanał", "августовский канал"},
            {"białystok", "белосток"},
            {"białystockie województwo", "белостокское воеводство"},
            {"białowieska puszcza", "беловежская пуща"},
            {"białowieźa", "беловеж"},
            {"dolnośląskie bory", "Нижнесилезские боры"},
            {"elbląg", "эльблонг"},
            {"elbląski kanał", "эльблонгский канал"},
            {"kaszuby", "кащубия"},
            {"kilecko-sandomierska wyżyna", "келецко-сандомиркая возвышенность"},
            {"koszalińskie województwo", "кошалинское воеводство"},
            {"kraków", "краков"},
            {"kujawy", "куявия"},
            {"lubelskie wyżyna", "люблинская возвышенность"},
            {"lubelskie województwo", "люблинское воеводство"},
            {"łódź", "лодзь"},
            {"łódźkie województwo", "лодзинское воеводство"},
            {"małopolska", "малая польша"},
            {"mazowsze", "мазовия"},
            {"olsztyńskie województwo", "ольштынское воеводство"},
            {"podhale", "подгале"},
            {"pomorze", "поморье"},
            {"poznańskie województwo", "познанское воеводство"},
            {"sandomierska kotlina", "сандомирская низменность"},
            {"sandomierz", "сандомир"},
            {"szczeciński zalew", "щецинский залив"},
            {"szczecińskie województwo", "щецинское воеводство"},
            {"śląsk", "силезия"},
            {"śląsk dolny", "нижняя силезия"},
            {"śląsk górny", "верхняя силезия"},
            {"śląsk wyżyna", "силезская возвышинность"},
            {"warmia", "вармия"},
            {"wielkopolska", "великая польша"},
            {"wielkopolska nizina", "великопольскя низменность"},
            {"wiślany zalew", "вислинский залив"}
        };

        #endregion

        private static readonly List<char> consonants = new List<char>
        {
            'b',
            'c',
            'ć',
            'd',
            'f',
            'g',
            'h',
            'j',
            'k',
            'l',
            'ł',
            'm',
            'n',
            'ń',
            'p',
            'r',
            's',
            'ś',
            't',
            'w',
            'z',
            'ź',
            'ż'
        };

        private static readonly List<char> vowels = new List<char>
        {
            'a',
            'ą',
            'e',
            'ę',
            'i',
            'o',
            'ó',
            'u'
        };


        private static readonly Dictionary<string, string> letterReplaceRules = new Dictionary<string, string>
        {
            {"a", "а"},
            {"ą", "он"}, //перед b, p ом, в конце слова	оу
            {"b", "б"},
            {"c", "ц"},
            {"ć", "ць"},
            {"d", "д"},
            {"e", "e"}, //в начале слова	 э
            {"ę", "ен"}, //перед b, p	 ем, в конце слова и перед ł	е
            {"f", "ф"},
            {"g", "г"},
            {"h", "х"},
            {"i", "и"},
            {"j", "й"},
            {"k", "к"},
            {"l", "л"}, //ль в конце слова и перед соласной
            {"ł", "л"},
            {"m", "м"},
            {"n", "н"},
            {"ń", "нь"},
            {"o", "о"},
            {"ó", "у"},
            {"p", "п"},
            {"r", "р"},
            {"s", "с"},
            {"ś", "сь"}, //перед согласными (кроме c) с последующим i, а также перед ć, l, ń, ś, ź	 с
            {"t", "т"},
            {"u", "у"},
            {"w", "в"},
            {"y", "ы"}, //после cz, rz, sz, ż, а также в личных именах в середине слова	 и, 
            {"z", "з"},
            {"ź", "зь"}, //перед согласными с последующим i, а также перед ć, l, ń, ś, dź	 з
            {"ż", "ж"}
        };

        private static readonly Dictionary<string, string> twoLettersReplaceRules = new Dictionary<string, string>
        {
            {"ch", "х"},
            {"cі", "ци"}, //перед e в конце слова, а также перед ó, u	 ч
            {"cz", "ч"},
            {"dz", "дз"},
            {"dź", "дзь"}, //перед мягкими согласными (то есть согласными с последующим i, а также ć, l, ń, ś, ź) дз
            {"dż", "дж"},
            {"ia", "я"}, //в отдельных случаях	 иа, правда непонятно в каких
            {"ią", "ён"}, //перед b, p	 ём, то же, после c	 иом, перед другими согласными	 ён, то же, после c	 ион
            {"ie", "е"},
            {"ię", "ен"}, // перед b, p	 ем
            {"io", "ё"}, //после c	 ио

            {"ió", "ю"}, //после c (кроме сочетания śc)	 у
            {"iu", "ю"}, //после c (кроме сочетания śc)	 у

            {"ja", "я"}, // в начале слова и после гласной	 я, после согласной	 ья
            {
                "ją", "ён"
            }, //в начале слова перед b, p   йом, в начале слова в остальных случаях	 йон, после гласной перед b, p	 ём, после гласной в остальных случаях	 ён, после согласной перед b, p	 ьом, после согласной в остальных случаях	 ьон
            {"je", "е"}, //в начале слова и после гласной	 е, после согласной	 ье
            {
                "ję", "ен"
            }, //в начале слова и после гласной перед b, p	 ем, в начале слова и после гласной в остальных случаях	 ен, после согласной перед b, p	 ьем, после согласной в остальных случаях	 ьен
            {"jo", "ё"}, //в начале слова	 йо, после гласной	 ё,после согласной	 ьо

            {"jó", "ю"}, //в начале слова и после гласной	 ю, после согласной	 ью
            {"ju", "ю"}, //в начале слова и после гласной	 ю, после согласной	 ью

            {"la", "ля"},
            {"lą", "лён"}, //перед b, p	 лём, в остальных случаях	 лён
            {"lo", "лё"},
            {"ló", "лю"},
            {"lu", "лю"},
            {"rz", "ж"}, //перед и после k, p, t, ch	 ш, 
            {"sz", "ш"},
            {"ść", "сць"},
            {"śc", "сьц"}
        };


        private static readonly Dictionary<string, string> fourLettersReplaceRules = new Dictionary<string, string>
        {
            {"szcz", "щ"}
        };


        public string ToRussian(string text)
        {
            //если есть в традиционном словаре, то возвращаем традиционную форму
            var invariant = text.Trim().ToLowerInvariant();
            if (traditionNaming.ContainsKey(invariant))
                return traditionNaming[invariant];

            //получаем токены
            var tokens = TokenizeString(text);

            #region применяем правила для изменения русского языка в токене

            foreach (var token in tokens)
            {
                // {"ą", "он"},//перед b, p ом, в конце слова	оу
                if (token.ForangeText == "ą")
                    token.RuText = TranslateA(token);

                //{ "e", "e"}, //в начале слова	 э
                if (token.ForangeText == "e" && token.StartPosition == 0)
                    token.RuText = "э";

                // {"ę", "ен"},//перед b, p	 ем, в конце слова и перед ł	е
                if (token.ForangeText == "ę")
                    token.RuText = TranslateE(token);

                //{ "l", "л"},//ль в конце слова и перед соласной
                if (token.ForangeText == "l")
                    token.RuText = TranslateL(token);

                //{"ś", "сь"},//перед согласными (кроме c) с последующим i, а также перед ć, l, ń, ś, ź	 с
                if (token.ForangeText == "ś")
                    token.RuText = TranslateS(token);

                //{"y", "ы"},//после cz, rz, sz, ż, а также в личных именах в середине слова	 и, 
                if (token.ForangeText == "y")
                    token.RuText = TranslateY(token);

                //{ "ź", "зь"},//перед согласными с последующим i, а также перед ć, l, ń, ś, dź	 з
                if (token.ForangeText == "ź")
                    token.RuText = TranslateZ(token);

                //{ "сі", "ци"},//перед e в конце слова, а также перед ó, u	   ч
                if (token.ForangeText == "сі")
                    token.RuText = TranslateCi(token);

                //{"dź", "дзь"},//перед мягкими согласными (то есть согласными с последующим i, а также ć, l, ń, ś, ź) дз
                if (token.ForangeText == "dź")
                    token.RuText = TranslateDz(token);

                //{ "ią", "ён"}, //перед b, p	 ём, то же, после c	 иом, перед другими согласными	 ён, то же, после c	 ион
                if (token.ForangeText == "ią")
                    token.RuText = TranslateIa(token);

                //{ "ię", "ен"},// перед b, p	 ем
                if (token.ForangeText == "ię" && token.NextToken != null &&
                    (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                    token.RuText = "ем";

                // {"io", "ё"},//после c	 ио
                if (token.ForangeText == "io" && token.PrevToken != null && token.PrevToken.ForangeText == "c")
                    token.RuText = "ио";

                // { "ió", "ю"},//после c (кроме сочетания śc)	 у
                //{ "iu", "ю"},//после c (кроме сочетания śc)	 у
                if ((token.ForangeText == "ió" || token.ForangeText == "iu") && token.PrevToken != null &&
                    token.PrevToken.ForangeText == "c")
                    token.RuText = "у";

                // {"ja", "я"},// в начале слова и после гласной	 я, после согласной	 ья
                if (token.ForangeText == "ja" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ья";


                //{ "je", "е"},//в начале слова и после гласной	 е, после согласной	 ье
                if (token.ForangeText == "je" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ье";

                //{ "jó", "ю"},//в начале слова и после гласной	 ю, после согласной	 ью
                //{ "ju", "ю"},//в начале слова и после гласной	 ю, после согласной	 ью
                if ((token.ForangeText == "jó" || token.ForangeText == "ju") && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ью";

                //{ "ję", "ен"},//в начале слова и после гласной перед b, p	 ем, в начале слова и после гласной в остальных случаях	 ен, после согласной перед b, p	 ьем, после согласной в остальных случаях	 ьен
                if (token.ForangeText == "ję")
                    token.RuText = TranslateJe(token);

                //{ "ją", "ён"},//в начале слова перед b, p   йом, в начале слова в остальных случаях	 йон, после гласной перед b, p	 ём, после гласной в остальных случаях	 ён, после согласной перед b, p	 ьом, после согласной в остальных случаях	 ьон
                if (token.ForangeText == "ją")
                    token.RuText = TranslateJa(token);

                //{ "jo", "ё"},//в начале слова	 йо, после гласной	 ё,после согласной	 ьо
                if (token.ForangeText == "jo")
                    token.RuText = TranslateJo(token);

                //{ "lą", "лён"},//перед b, p	 лём, в остальных случаях	 лён
                if (token.ForangeText == "lą")
                    token.RuText = TranslateLa(token);

                //{ "rz", "ж"},//перед и после k, p, t, ch	 ш, 
                if (token.ForangeText == "rz")
                    token.RuText = TranslateRz(token);
            }

            #endregion

            var result = tokens.Aggregate(string.Empty, (current, token) => current + token.RuText);

            return result;
        }

        public string Language => "Польский";

        public int Id => 3;

        public string TableInBase { get; set; }


        private static bool IsSoftConsonant(LetterToken token)
        {
            //согласными с последующим i, а также ć, l, ń, ś, ź
            var result = token.ForangeText.First() == 'ć' ||
                         token.ForangeText.First() == 'l' ||
                         token.ForangeText.First() == 'ń' ||
                         token.ForangeText.First() == 'ź' ||
                         token.ForangeText.First() == 'ś' || token.NextToken != null &&
                         token.NextToken.ForangeText.First() == 'i';

            return result;
        }

        private string TranslateL(LetterToken token)
        {
            //{ "l", "л"},//ль в конце слова и перед соласной
            var result = "л";
            if (token.NextToken == null || token.NextToken != null &&
                consonants.Contains(token.NextToken.ForangeText.First()))
                result = "ль";

            return result;
        }

        private string TranslateRz(LetterToken token)
        {
            //{ "rz", "ж"},//перед и после k, p, t, ch	 ш, 
            var result = "ж";
            if (token.NextToken != null
                && (
                    token.NextToken.ForangeText == "k" ||
                    token.NextToken.ForangeText == "p" ||
                    token.NextToken.ForangeText == "t" ||
                    token.NextToken.ForangeText == "ch"))
                result = "ш";

            if (token.PrevToken != null && (
                    token.PrevToken.ForangeText == "k" ||
                    token.PrevToken.ForangeText == "p" ||
                    token.PrevToken.ForangeText == "t" ||
                    token.PrevToken.ForangeText == "ch"))
                result = "ш";

            return result;
        }

        private string TranslateLa(LetterToken token)
        {
            //{ "lą", "лён"},//перед b, p	 лём, в остальных случаях	 лён
            var result = "лён";
            if (token.NextToken != null && (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                result = "лём";

            return result;
        }

        private string TranslateJa(LetterToken token)
        {
            //{ "ją", "ён"},//в начале слова перед b, p   йом, в начале слова в остальных случаях	 йон, 
            //после гласной перед b, p	 ём, после гласной в остальных случаях	 ён, 
            //после согласной перед b, p	 ьом, после согласной в остальных случаях	 ьон
            var result = "ён";
            if (token.PrevToken == null || token.PrevToken != null &&
                vowels.Contains(token.PrevToken.ForangeText.Last()))
                if (token.NextToken != null &&
                    (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                    result = "йом";
                else
                    result = "йон";

            if (token.PrevToken != null && vowels.Contains(token.PrevToken.ForangeText.Last()))
                if (token.NextToken != null &&
                    (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                    result = "ём";
                else
                    result = "ён";

            if (token.PrevToken != null && consonants.Contains(token.PrevToken.ForangeText.Last()))
                if (token.NextToken != null &&
                    (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                    result = "ьом";
                else
                    result = "ьон";

            return result;
        }

        private string TranslateJo(LetterToken token)
        {
            //{ "jo", "ё"},//в начале слова	 йо, после гласной	 ё, после согласной	 ьо
            var result = "ё";
            if (token.PrevToken == null)
                result = "йо";
            if (token.PrevToken != null && vowels.Contains(token.PrevToken.ForangeText.Last()))
                result = "ё";

            if (token.PrevToken != null && consonants.Contains(token.PrevToken.ForangeText.Last()))
                result = "ьо";

            return result;
        }

        private string TranslateJe(LetterToken token)
        {
            //{ "ję", "ен"},//в начале слова и после гласной перед b, p	 ем, в начале слова и после гласной в остальных случаях	 ен, 
            //после согласной перед b, p	 ьем, после согласной в остальных случаях	 ьен
            var result = "ен";
            if (token.PrevToken == null || token.PrevToken != null &&
                vowels.Contains(token.PrevToken.ForangeText.Last()))
                if (token.NextToken != null &&
                    (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                    result = "ем";
                else
                    result = "ен";

            if (token.PrevToken != null && consonants.Contains(token.PrevToken.ForangeText.Last()))
                if (token.NextToken != null &&
                    (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                    result = "ьем";
                else
                    result = "ьен";

            return result;
        }

        private string TranslateIa(LetterToken token)
        {
            //{ "ią", "ён"}, //перед b, p	 ём, то же, после c	 иом, перед другими согласными	 ён, то же, после c	 ион
            var result = "ён";
            if (token.NextToken != null && (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                if (token.PrevToken != null && token.PrevToken.ForangeText == "c")
                    result = "иом";
                else
                    result = "ём";
            if (token.NextToken != null && token.NextToken.ForangeText != "b" && token.NextToken.ForangeText != "p")
                if (token.PrevToken != null && token.PrevToken.ForangeText == "c")
                    result = "ион";
                else
                    result = "ён";

            return result;
        }

        private string TranslateDz(LetterToken token)
        {
            //{"dź", "дзь"},//перед мягкими согласными (то есть согласными с последующим i, а также ć, l, ń, ś, ź) дз
            var result = "дзь";
            if (token.NextToken != null && IsSoftConsonant(token.NextToken))
                result = "дз";

            return result;
        }

        private string TranslateCi(LetterToken token)
        {
            //{ "сі", "ци"},//перед e в конце слова, а также перед ó, u	   ч
            var result = "ци";
            if (token.NextToken != null && token.NextToken.ForangeText == "e" && token.NextToken.NextToken == null)
                result = "ч";

            if (token.NextToken != null && (token.NextToken.ForangeText == "ó" || token.NextToken.ForangeText == "u"))
                result = "ч";

            return result;
        }

        private string TranslateZ(LetterToken token)
        {
            //{ "ź", "зь"},//перед согласными с последующим i, а также перед ć, l, ń, ś, dź	 з
            var result = "зь";
            if (token.NextToken != null && IsSoftConsonant(token.NextToken) && token.NextToken.ForangeText != "dź")
                result = "з";

            return result;
        }

        private string TranslateY(LetterToken token)
        {
            //{"y", "ы"},//после cz, rz, sz, ż	 и, 
            var result = "ы";
            if (token.PrevToken != null && (
                    token.PrevToken.ForangeText == "cz"
                    || token.PrevToken.ForangeText == "rz"
                    || token.PrevToken.ForangeText == "sz"
                    || token.PrevToken.ForangeText == "ż"))
                result = "и";

            return result;
        }

        private string TranslateS(LetterToken token)
        {
            //{"ś", "сь"},//перед согласными (кроме c) с последующим i, а также перед ć, l, ń, ś, ź	   - с
            var result = "сь";
            if (token.NextToken != null && IsSoftConsonant(token.NextToken) && token.NextToken.ForangeText != "c")
                result = "с";

            return result;
        }

        private string TranslateE(LetterToken token)
        {
            // {"ę", "ен"},//перед b, p	 ем, в конце слова и перед ł	е
            var result = "ен";
            if (token.NextToken != null && (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                result = "ем";

            if (token.NextToken == null || token.NextToken != null && token.NextToken.ForangeText == "ł")
                result = "е";
            return result;
        }

        private string TranslateA(LetterToken token)
        {
            // {"ą", "он"},//перед b, p ом, в конце слова	оу
            var result = "он";
            if (token.NextToken != null && (token.NextToken.ForangeText == "b" || token.NextToken.ForangeText == "p"))
                result = "ом";

            if (token.NextToken == null)
                result = "оу";
            return result;
        }


        private static List<LetterToken> TokenizeString(string text)
        {
            var tokens = new List<LetterToken>();

            var ignoreCaseText = text.ToLowerInvariant();
            var textLength = ignoreCaseText.Length;

            for (var index = 0; index < textLength; index++)
            {
                var fourLetters = string.Empty;
                var twoLetters = string.Empty;

                if (index + 4 <= ignoreCaseText.Length)
                    fourLetters = ignoreCaseText.Substring(index, 4);

                if (index + 2 <= ignoreCaseText.Length)
                    twoLetters = ignoreCaseText.Substring(index, 2);

                var oneLetter = ignoreCaseText.Substring(index, 1);

                if (fourLettersReplaceRules.ContainsKey(fourLetters))
                {
                    var token = new LetterToken
                    {
                        StartPosition = index,
                        EndPosition = index + 4,
                        ForangeText = fourLetters,
                        RuText = fourLettersReplaceRules[fourLetters]
                    };
                    tokens.Add(token);
                    index += 3;
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