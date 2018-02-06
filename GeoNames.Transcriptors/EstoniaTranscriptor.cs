using System.Collections.Generic;
using System.Linq;

namespace GeoNames.Transcriptors
{
    public class EstoniaTranscriptor : ITranscriptor
    {
        private static readonly List<char> vowels = new List<char>
        {
            'a',
            'i',
            'o',
            'õ',
            'ä',
            'ö',
            'ü',
            'u'
        };

        private static readonly List<char> consonants = new List<char>
        {
            'b',
            'd',
            'f',
            'g',
            'h',
            'j',
            'k',
            'l',
            'm',
            'o',
            'n',
            'p',
            'r',
            's',
            't',
            'v',
            'c',
            'w',
            'x',
            'z'
        };

        private static readonly Dictionary<string, string> letterReplaceRules = new Dictionary<string, string>
        {
            {"a", "а"},
            {"b", "б"},
            {"d", "д"},
            {"e", "э"}, //после согласной и i	 е
            {"f", "ф"},
            {"g", "г"},
            {"h", "х"},
            {"i", "и"}, //в дифтонгах после гласных    й
            {"j", "й"},
            {"k", "к"},
            {"l", "л"},
            {"m", "м"},
            {"o", "о"},
            {"n", "н"},
            {"p", "п"},
            {"r", "р"},
            {"s", "с"}, //между гласными	 з
            {"t", "т"},
            {"u", "у"},
            {"v", "в"},
            {"õ", "ы"},
            {"ä", "я"}, //в начале слова	 э
            {"ö", "ё"}, // в начале слова	 э
            {"ü", "ю"},
            //Следующие буквы и буквосочетания встречаются только в заимствованных словах и передаются соответственно:
            {"c", "к"}, //перед e, i, y	- с 
            {"w", "в"},
            {"x", "кс"},
            {"z", "ц"} //в именах славянского происхождения	 з
        };

        private static readonly Dictionary<string, string> twoLettersReplaceRules = new Dictionary<string, string>
        {
            {"ee", "ee"}, //ee    в начале слова  ээ,  после согласной	 еэ
            {"ii", "ий"},
            {"ja", "я"}, //после согласной	 ья
            {"je", "е"}, //после согласной	 ье
            {"ji", "йи"}, // после согласной	 ьи
            {"jo", "йо"}, // после согласной	 ьо
            {"ju", "ю"}, //после согласной	 ью
            {"jä", "я"}, //после согласной	 ья
            {"jö", "ё"}, //после согласной	 ьё
            {"jõ", "йы"},
            {"jü", "йю"}, // после согласной	 ью
            {"õõ", "ыы"}, //
            {"ää", "яэ"}, //в начале слова или компонента сложного слова	 ээ   в конце слова	 я
            {"öö", "ёэ"}, // в начале слова	 ээ
            {"üü", "юй"},
            //Следующие буквы и буквосочетания встречаются только в заимствованных словах и передаются соответственно:
            {"ph", "ф"},
            {"qu", "кв"},
            {"ch", "к"} //перед e, i, y    -  ч
        };


        public string ToRussian(string text)
        {
            //получаем токены
            var tokens = TokenizeString(text);

            #region применяем правила для изменения русского языка в токене

            foreach (var token in tokens)
            {
                // {"e", "э"},//после согласной и i	 е
                if (token.ForangeText == "e")
                    token.RuText = TranslateE(token);

                // {"i", "и"}, //в дифтонгах после гласных    й
                if (token.ForangeText == "i")
                    token.RuText = TranslateI(token);

                //{"s", "с"},//между гласными	 з
                if (token.ForangeText == "s")
                    token.RuText = TranslateS(token);

                //{"ä", "я"},//в начале слова	 э
                if (token.ForangeText == "ä" && token.StartPosition == 0)
                    token.RuText = "э";

                //{"ö", "ё"},// в начале слова	 э
                if (token.ForangeText == "ö" && token.StartPosition == 0)
                    token.RuText = "э";

                //{"c","к"}, //перед e, i, y	- с, 
                if (token.ForangeText == "c")
                    token.RuText = TranslateC(token);

                //   {"ch", "к"} //перед e, i, y    -  ч
                if (token.ForangeText == "ch")
                    token.RuText = TranslateCh(token);

                //{"ee", "ee"},  //ee    в начале слова  ээ,  после согласной	 еэ
                if (token.ForangeText == "ee")
                    token.RuText = TranslateEe(token);

                //{"ja", "я"},//после согласной	 ья
                if (token.ForangeText == "ja" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ья";

                //{"je", "е"},//после согласной	 ье
                if (token.ForangeText == "je" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ье";

                //{"ji", "йи"},// после согласной	 ьи
                if (token.ForangeText == "ji" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ьи";

                //{"jo", "йо"},// после согласной	 ьо
                if (token.ForangeText == "jo" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ьо";

                //{"ju", "ю"},//после согласной	 ью
                if (token.ForangeText == "ju" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ью";

                //{"jä", "я"},//после согласной	 ья
                if (token.ForangeText == "jä" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ья";

                //{"jö", "ё"},//после согласной	 ьё
                if (token.ForangeText == "jö" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ьё";

                //{"jü", "йю"},// после согласной	 ью
                if (token.ForangeText == "jü" && token.PrevToken != null &&
                    consonants.Contains(token.PrevToken.ForangeText.Last()))
                    token.RuText = "ью";

                //{"ää", "яэ"},//в начале слова или компонента сложного слова	 ээ   в конце слова	 я
                if (token.ForangeText == "ää")
                    token.RuText = TranslateAa(token);

                //{"öö", "ёэ"},// в начале слова	 ээ
                if (token.ForangeText == "öö" && token.StartPosition == 0)
                    token.RuText = "ээ";
            }

            #endregion

            var result = tokens.Aggregate(string.Empty, (current, token) => current + token.RuText);

            return result;
        }

        public string Language => "Эстонский";

        public int Id => 4;

        public string TableInBase { get; set; }

        private string TranslateE(LetterToken token)
        {
            // {"e", "э"},//после согласной и i	 е
            var result = "э";
            if (token.PrevToken != null && (consonants.Contains(token.PrevToken.ForangeText.Last()) ||
                                            token.PrevToken.ForangeText.Last() == 'i'))
                result = "е";
            return result;
        }

        private string TranslateAa(LetterToken token)
        {
            //{"ää", "яэ"},//в начале слова или компонента сложного слова	 ээ   в конце слова	 я
            var result = "яэ";
            if (token.StartPosition == 0 || token.PrevToken != null && token.PrevToken.ForangeText.Last() == '-')
                result = "ээ";

            if (token.NextToken == null)
                result = "я";
            return result;
        }

        private string TranslateEe(LetterToken token)
        {
            //{"ee", "ee"},  //ee    в начале слова  ээ,  после согласной	 еэ
            var result = "ee";
            if (token.StartPosition == 0)
                result = "ээ";

            if (token.PrevToken != null && consonants.Contains(token.PrevToken.ForangeText.Last()))
                result = "еэ";

            return result;
        }

        private string TranslateCh(LetterToken token)
        {
            //   {"ch", "к"} //перед e, i, y    -  ч
            var result = "к";
            if (token.NextToken != null && (token.NextToken.ForangeText.First() == 'e' ||
                                            token.NextToken.ForangeText.First() == 'i' ||
                                            token.NextToken.ForangeText.First() == 'y'))
                result = "ч";
            return result;
        }

        private string TranslateC(LetterToken token)
        {
            //{"c","к"}, //перед e, i, y - с 
            var result = "к";
            if (token.NextToken != null && (token.NextToken.ForangeText.First() == 'e' ||
                                            token.NextToken.ForangeText.First() == 'i' ||
                                            token.NextToken.ForangeText.First() == 'y'))
                result = "с";
            return result;
        }

        private string TranslateS(LetterToken token)
        {
            //{"s", "с"},//между гласными	 з
            var result = "с";
            if (token.PrevToken != null && token.NextToken != null &&
                vowels.Contains(token.PrevToken.ForangeText.Last()) &&
                vowels.Contains(token.NextToken.ForangeText.First()))
                result = "з";
            return result;
        }

        private string TranslateI(LetterToken token)
        {
            // {"i", "и"}, //в дифтонгах после гласных    й
            var result = "и";
            if (token.PrevToken != null && vowels.Contains(token.PrevToken.ForangeText.Last()))
                result = "й";
            return result;
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