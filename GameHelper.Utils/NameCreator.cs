using System;
using System.Collections.Generic;
using System.Linq;

namespace GameHelper.Utils
{
    public class NameCreator
    {
        public static string CreateWord(string russian, IReadOnlyCollection<NameCreator.Letter> letters)
        {
            var list = new List<char>(russian.Length);

            foreach (var ruCh in russian)
            {
                var letter = letters.FirstOrDefault(l => l.Ru == ruCh);
                if (letter == null)
                    return null;

                list.AddRange(letter.En);
            }

            return new string(list.ToArray());
        }

        public class Letter
        {
            public char Ru { get; set; }

            public string En { get; set; }

            public static Letter Parse(string line)
            {
                line = line.Trim();
                var ru = line[0];

                var i = line.Length - 1;
                while (char.IsLetterOrDigit(line[i]))
                    i--;
                var en = line.Substring(i + 1);

                if (!IsRu(ru))
                    throw new Exception($"{ru} - не русская буква");
                if (!IsEn(en))
                    throw new Exception($"{en} - not english letter");

                return new Letter
                {
                    Ru = ru,
                    En = en
                };
            }

            public static bool IsRu(char value)
            {
                return value is >= 'а' and <= 'я' or >= 'А' and <= 'Я';
            }

            private static bool IsEn(string value)
            {
                return value.All(IsEn);
            }

            private static bool IsEn(char value)
            {
                return value is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '3' or '0';
            }
        }
    }
}
