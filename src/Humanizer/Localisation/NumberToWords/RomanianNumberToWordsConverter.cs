using System;
using System.Collections.Generic;

namespace Humanizer.Localisation.NumberToWords
{
    internal class RomanianNumberToWordsConverter : GenderlessNumberToWordsConverter
    {
        private static readonly string[] UnitsMap = { "zero", "unu", "doi", "trei", "patru", "cinci", "sase", "sapte", "opt", "noua", "zece", "unsprezece", "doisprezece", "treisprezece", "patrusprezece", "cincisprezece", "saisprezece", "saptesprezece", "optsprezece", "nouasprezece" };
        private static readonly string[] TensMap = { "zero", "zece", "douazeci", "trezizeci", "patruzeci", "cincizeci", "saizeci", "saptezeci", "optzeci", "nouazeci" };

        private static readonly Dictionary<int, string> OrdinalExceptions = new Dictionary<int, string>
        {
            {5, "cincelea"},
            {8, "optulea"},
        };

        public override string Convert(int number)
        {
            return Convert(number, false);
        }

        public override string ConvertToOrdinal(int number)
        {
            return Convert(number, true);
        }

        private string Convert(int number, bool isOrdinal)
        {
            if (number == 0)
                return GetUnitValue(0, isOrdinal);

            if (number < 0)
                return string.Format("minus {0}", Convert(-number));

            var parts = new List<string>();

            if ((number / 1000000000) > 0)
            {
                var firstPart = number / 1000000000;
                if (firstPart == 1)
                    parts.Add("un miliard");
                else if (firstPart == 2)
                    parts.Add("doua miliarde");
                else if (firstPart > 20)
                    parts.Add(string.Format("{0} de miliarde", Convert(number/1000000000)));
                else parts.Add(string.Format("{0} miliarde", Convert(number / 1000000000)));
                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                var firstPart = number / 1000000;
                if (firstPart == 1)
                    parts.Add("un milion");
                else if (firstPart == 2)
                    parts.Add("doua milioane");
                else if (firstPart > 20)
                    parts.Add(string.Format("{0} de milioane", Convert(number/1000000)));
                else parts.Add(string.Format("{0} milioane", Convert(number / 1000000)));
                
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                var firstPart = number/1000;
                if (firstPart == 1)
                    parts.Add("o mie");
                else if (firstPart == 2)
                    parts.Add("doua mii");
                else if (firstPart > 20)
                    parts.Add(string.Format("{0} de mii", Convert(number/1000)));
                else parts.Add(string.Format("{0} mii", Convert(number / 1000)));

                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                var firstPart = number/100;
                switch (firstPart)
                {
                    case 1:
                        parts.Add("o suta");
                        break;
                    case 2:
                        parts.Add("doua sute");
                        break;
                    default:
                        parts.Add(string.Format("{0} sute", Convert(firstPart)));
                        break;
                }
                number %= 100;
            }

            if (number > 0)
            {
                if (number == 1 && parts.Count == 0 && isOrdinal)
                    parts.Add("primul");
                else
                {
                    if (number < 20)
                        parts.Add(GetUnitValue(number, isOrdinal));
                    else
                    {
                        var lastPart = TensMap[number/10];
                        if ((number%10) > 0)
                            lastPart += string.Format(" si {0}", GetUnitValue(number%10, isOrdinal));

                        parts.Add(lastPart);
                    }
                }
            }
            else if (isOrdinal)
                parts[parts.Count - 1] += "lea";

            string toWords = string.Join(" ", parts.ToArray());

            if (isOrdinal && parts[0] != "primul")
                toWords = AddPrefix(toWords);

            return toWords;
        }

        private static string GetUnitValue(int number, bool isOrdinal)
        {
            if (isOrdinal)
            {
                string exceptionString;
                if (ExceptionNumbersToWords(number, out exceptionString))
                    return exceptionString;
                else
                    return UnitsMap[number] + "lea";
            }
            else
                return UnitsMap[number];
        }

        private static string AddPrefix(string toWords)
        {
            return "al " + toWords;
        }

        private static bool ExceptionNumbersToWords(int number, out string words)
        {
            return OrdinalExceptions.TryGetValue(number, out words);
        }
    }
}