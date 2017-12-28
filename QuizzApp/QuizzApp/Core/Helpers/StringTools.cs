using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuizzApp.Core.Helpers
{
    public class StringTools
    {
        static char[] QUIZZ_APP_ALPHABET = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        static char[] englishReplace = { 'a', 'a', 'a', 'a', 'c', 'e', 'e', 'e', 'e', 'i', 'i', 'o', 'o', 'u', 'u', 'u', 'o', 'A', 'A', 'A', 'A', 'A', 'A', 'I', 'I', 'I', 'I', 'O', 'O', 'O', 'O', 'O', 'O', 'U', 'U', 'U', 'U', 'E', 'E', 'E', 'E', 'C', 'Y', 'N' };
        static char[] englishAccents = { 'à', 'â', 'ä', 'æ', 'ç', 'é', 'è', 'ê', 'ë', 'î', 'ï', 'ô', 'œ', 'ù', 'û', 'ü', 'ö', 'À', 'Â', 'Ä', 'Á', 'Ã', 'Å', 'Î', 'Ï', 'Ì', 'Í', 'Ô', 'Ö', 'Ò', 'Ó', 'Õ', 'Ø', 'Ù', 'Û', 'Ü', 'Ú', 'É', 'È', 'Ê', 'Ë', 'Ç', 'Ÿ', 'Ñ' };

        static char[] frenchReplace = { 'a', 'a', 'a', 'a', 'c', 'e', 'e', 'e', 'e', 'i', 'i', 'o', 'o', 'u', 'u', 'u', 'o', 'A', 'A', 'A', 'A', 'A', 'A', 'I', 'I', 'I', 'I', 'O', 'O', 'O', 'O', 'O', 'O', 'U', 'U', 'U', 'U', 'E', 'E', 'E', 'E', 'C', 'Y', 'N' };
        static char[] frenchAccents = { 'à', 'â', 'ä', 'æ', 'ç', 'é', 'è', 'ê', 'ë', 'î', 'ï', 'ô', 'œ', 'ù', 'û', 'ü', 'ö', 'À', 'Â', 'Ä', 'Á', 'Ã', 'Å', 'Î', 'Ï', 'Ì', 'Í', 'Ô', 'Ö', 'Ò', 'Ó', 'Õ', 'Ø', 'Ù', 'Û', 'Ü', 'Ú', 'É', 'È', 'Ê', 'Ë', 'Ç', 'Ÿ', 'Ñ' };

        static char[] germanReplace = { 'a', 'o', 'u', 's' };
        static char[] germanAccents = { 'ä', 'ö', 'ü', 'ß' };

        static char[] spanishReplace = { 'a', 'e', 'i', 'o', 'u' };
        static char[] spanishAccents = { 'á', 'é', 'í', 'ó', 'ú' };

        static char[] catalanReplace = { 'a', 'e', 'e', 'i', 'i', 'o', 'o', 'u', 'u' };
        static char[] catalanAccents = { 'à', 'è', 'é', 'í', 'ï', 'ò', 'ó', 'ú', 'ü' };

        static char[] italianReplace = { 'a', 'e', 'e', 'i', 'o', 'o', 'u' };
        static char[] italianAccents = { 'à', 'è', 'é', 'ì', 'ò', 'ó', 'ù' };

        static char[] polishReplace = { 'a', 'c', 'e', 'l', 'n', 'o', 's', 'z', 'z' };
        static char[] polishAccents = { 'ą', 'ć', 'ę', 'ł', 'ń', 'ó', 'ś', 'ż', 'ź' };

        static char[] hungarianReplace = { 'a', 'e', 'i', 'o', 'o', 'o', 'u', 'u', 'u' };
        static char[] hungarianAccents = { 'á', 'é', 'í', 'ö', 'ó', 'ő', 'ü', 'ú', 'ű' };

        static char[] portugueseReplace = { 'a', 'a', 'a', 'a', 'e', 'e', 'i', 'o', 'o', 'o', 'u', 'u' };
        static char[] portugueseAccents = { 'ã', 'á', 'â', 'à', 'é', 'ê', 'í', 'õ', 'ó', 'ô', 'ú', 'ü' };

        static char[] czechReplace = { 'a', 'a', 'a', 'c', 'd', 'e', 'e', 'i', 'n', 'o', 'r', 's', 't', 'u', 'u', 'y', 'z' };
        static char[] czechAccents = { 'ã', 'á', 'á', 'č', 'ď', 'é', 'ě', 'í', 'ň', 'ó', 'ř', 'š', 'ť', 'ú', 'ů', 'ý', 'ž' };

        static char[] dutchReplace = { 'e', 'e', 'i', 'o', 'o', 'u' };
        static char[] dutchAccents = { 'é', 'ë', 'ï', 'ó', 'ö', 'ü' };

        static char[] turkishReplace = { 'c', 'e', 'e', 'g', 'i', 'i', 'o', 'o', 'u' };
        static char[] turkishAccents = { 'ç', 'é', 'ë', 'ğ', 'İ', 'ï', 'ó', 'ö', 'ü' };

        static char[] romanianReplace = { 'a', 'a', 'i', 's', 's', 't', 't' };
        static char[] romanianAccents = { 'ă', 'â', 'î', 'ş', 'ș', 'ţ', 'ț' };

        static char[] filipinoReplace = { 'a', 'a', 'a', 'e', 'e', 'e', 'i', 'i', 'i', 'o', 'o', 'o', 'u', 'u', 'u' };
        static char[] filipinoAccents = { 'á', 'à', 'â', 'é', 'è', 'ê', 'í', 'ì', 'î', 'ó', 'ò', 'ô', 'ú', 'ù', 'û' };

        static char[] ukarainianReplace = { 'i', 'r' };
        static char[] ukarainianAccents = { 'ї', 'ґ' };

        static char[] russianReplace = { 'b' };
        static char[] russianAccents = { 'ъ' };

        static char[] greekReplace = { 'α', 'ε', 'η', 'ι', 'ι', 'ι', 'ο', 'υ', 'υ', 'υ', 'ω' };
        static char[] greekAccents = { 'ά', 'έ', 'ή', 'ί', 'ϊ', 'ΐ', 'ό', 'ύ', 'ϋ', 'ΰ', 'ώ' };

        static char[] arabicAccents = { 'أ', 'إ', 'آ', 'ء', 'پ', 'ض', 'ذ', 'ـ', 'خ', 'خ', 'غ', 'ش', 'ة', 'ث', 'ً', 'ٰ', 'ؤ', 'ظ', 'ى', 'ئ' };
        static char[] arabicReplace = { 'ا', 'ا', 'ا', 'ا', 'ب', 'ص', 'د', 'ّ', 'ح', 'ك', 'ع', 'س', 'ت', 'ت', 'َ', 'َ', 'و', 'ط', 'ي', 'ي' };

        static char[] bulgarianReplace = { 'ь', 'и' };
        static char[] bulgarianAccents = { 'ъ', 'ѝ' };

        static char[] croatianReplace = { 'c', 'c', 'd', 's', 'z' };
        static char[] croatianAccents = { 'č', 'ć', 'đ', 'š', 'ž' };

        static char[] estonianReplace = { 'a', 'o', 'o', 'u' };
        static char[] estonianAccents = { 'ä', 'ö', 'õ', 'ü' };

        static char[] icelandicReplace = { 'o' };
        static char[] icelandicAccents = { 'ö' };

        static char[] latvianReplace = { 'e' };
        static char[] latvianAccents = { 'ē' };

        static char[] slovakianReplace = { 'a', 'a', 'c', 'd', 'e', 'i', 'l', 'l', 'n', 'o', 'o', 'r', 's', 't', 'u', 'y', 'z' };
        static char[] slovakianAccents = { 'á', 'ä', 'č', 'ď', 'é', 'í', 'ĺ', 'ľ', 'ň', 'ó', 'ô', 'ŕ', 'š', 'ť', 'ú', 'ý', 'ž' };

        private enum DictionaryDef
        {
            NotSet,
            Arabic,
            Bulgarian,
            Catalan,
            ChineseSimplified,
            ChineseTraditional,
            Croatian,
            Czech,
            CzechAlt,
            Danish,
            Dutch,
            DutchBelgium,
            English,
            Estonian,
            Finnish,
            French,
            CanadianFrench,
            SwissFrench,
            German,
            Greek,
            Hebrew,
            Hungarian,
            Icelandic,
            Italian,
            Latvian,
            //Macedonian,
            Norwegian,
            Polish,
            Portuguese,
            BrazilianPortuguese,
            Romanian,
            Russian,
            Spanish,
            Slovak,
            SlovakAlt,
            Slovenian,
            Swedish,
            Turkish,
            Ukrainian
        }

        static StringBuilder sbStripAccents = new StringBuilder();

        private static string RemoveDiacritics(string accentedStr, DictionaryDef eDictionary)
        {
            char[] replacement = null;
            char[] accents = null;
            switch (eDictionary)
            {
                case DictionaryDef.Arabic:
                    replacement = arabicReplace;
                    accents = arabicAccents;
                    break;

                case DictionaryDef.Slovak:
                case DictionaryDef.SlovakAlt:
                    replacement = slovakianReplace;
                    accents = slovakianAccents;
                    break;

                case DictionaryDef.Latvian:
                    replacement = latvianReplace;
                    accents = latvianAccents;
                    break;

                case DictionaryDef.Icelandic:
                    replacement = icelandicReplace;
                    accents = icelandicAccents;
                    break;

                case DictionaryDef.Estonian:
                    replacement = estonianReplace;
                    accents = estonianAccents;
                    break;

                case DictionaryDef.Bulgarian:
                    replacement = bulgarianReplace;
                    accents = bulgarianAccents;
                    break;

                case DictionaryDef.Romanian:
                    replacement = romanianReplace;
                    accents = romanianAccents;
                    break;

                case DictionaryDef.Croatian:
                case DictionaryDef.Slovenian:
                    replacement = croatianReplace;
                    accents = croatianAccents;
                    break;

                case DictionaryDef.English:
                    replacement = englishReplace;
                    accents = englishAccents;
                    break;

                case DictionaryDef.French:
                case DictionaryDef.CanadianFrench:
                case DictionaryDef.SwissFrench:
                    replacement = frenchReplace;
                    accents = frenchAccents;
                    break;

                case DictionaryDef.German:
                    replacement = germanReplace;
                    accents = germanAccents;
                    break;

                case DictionaryDef.Spanish:
                    replacement = spanishReplace;
                    accents = spanishAccents;
                    break;

                case DictionaryDef.Catalan:
                    replacement = catalanReplace;
                    accents = catalanAccents;
                    break;

                case DictionaryDef.Italian:
                    replacement = italianReplace;
                    accents = italianAccents;
                    break;

                case DictionaryDef.Polish:
                    replacement = polishReplace;
                    accents = polishAccents;
                    break;

                case DictionaryDef.Hungarian:
                    replacement = hungarianReplace;
                    accents = hungarianAccents;
                    break;

                case DictionaryDef.Portuguese:
                case DictionaryDef.BrazilianPortuguese:
                    replacement = portugueseReplace;
                    accents = portugueseAccents;
                    break;

                case DictionaryDef.Czech:
                case DictionaryDef.CzechAlt:
                    replacement = czechReplace;
                    accents = czechAccents;
                    break;

                case DictionaryDef.Dutch:
                    replacement = dutchReplace;
                    accents = dutchAccents;
                    break;

                case DictionaryDef.Turkish:
                    replacement = turkishReplace;
                    accents = turkishAccents;
                    break;

                case DictionaryDef.Russian:
                    replacement = russianReplace;
                    accents = russianAccents;
                    break;

                case DictionaryDef.Ukrainian:
                    replacement = ukarainianReplace;
                    accents = ukarainianAccents;
                    break;

                case DictionaryDef.Greek:
                    replacement = greekReplace;
                    accents = greekAccents;
                    break;

                default:
                    return accentedStr;

            }

            if (accents != null && replacement != null && accentedStr.IndexOfAny(accents) > -1)
            {
                sbStripAccents.Length = 0;
                sbStripAccents.Append(accentedStr);
                for (int i = 0; i < accents.Length; i++)
                {
                    sbStripAccents.Replace(accents[i], replacement[i]);
                }

                return sbStripAccents.ToString();
            }
            else
                return accentedStr;
        }

        private static DictionaryDef GetCurrentDictionnary()
        {
            CultureInfo cultureFr = new CultureInfo("fr");
            CultureInfo cultureEn = new CultureInfo("fr");
            if (CultureInfo.CurrentUICulture.Equals(cultureFr) || CultureInfo.CurrentUICulture.Parent.Equals(cultureFr))
                return DictionaryDef.French;
            else if (CultureInfo.CurrentUICulture.Equals(cultureEn) || CultureInfo.CurrentUICulture.Parent.Equals(cultureEn))
                return DictionaryDef.English;
            else
                return DictionaryDef.NotSet;
        }

        public static string RemoveAccents(string myString)
        {
            return RemoveDiacritics(myString, GetCurrentDictionnary());
        }

        
        public static string KeepLettersAndNumbers(string myString)
        {
            Regex pattern = new Regex("[^a-zA-Z0-9]");
            return pattern.Replace(myString, "");            
        }


        private static Random randomer = new Random();
        public static char GetRandomLetterUpperCase()
        {
            // This method returns a random lowercase letter.
            // ... Between 'a' and 'z' inclusize.
            int num = randomer.Next(0, 26); // Zero to 25
            char let = (char)('A' + num);
            return let;
        }

        public static bool IsOnQuizzAppAlphabet(char aChar)
        {
            return QUIZZ_APP_ALPHABET.Contains(aChar);
        }



        
    }
}
