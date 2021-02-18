using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Interfaces;

namespace GameHelper.Translators
{
    public class ChangeKeyboardLayoutService: IChangeKeyboardLayoutService
    {
        /// <summary>
        /// для неправильной раскладки клавиатуры
        /// </summary>
        public static readonly IReadOnlyDictionary<char, char> KeyboardPairs = new Dictionary<char, char>
        {
            { 'q', 'й' },
            { 'w', 'ц' },
            { 'e', 'у' },
            { 'r', 'к' },
            { 't', 'е' },
            { 'y', 'н' },
            { 'u', 'г' },
            { 'i', 'ш' },
            { 'o', 'щ' },
            { 'p', 'з' },
            { '[', 'х' },
            { ']', 'ъ' },
            { 'a', 'ф' },
            { 's', 'ы' },
            { 'd', 'в' },
            { 'f', 'а' },
            { 'g', 'п' },
            { 'h', 'р' },
            { 'j', 'о' },
            { 'k', 'л' },
            { 'l', 'д' },
            { ';', 'ж' },
            {'\'', 'э' },
            { 'z', 'я' },
            { 'x', 'ч' },
            { 'c', 'с' },
            { 'v', 'м' },
            { 'b', 'и' },
            { 'n', 'т' },
            { 'm', 'ь' },
            { ',', 'б' },
            { '.', 'ю' },

            { '{', 'Х' },
            { '}', 'Ъ' },
            { '"', 'Э' },
            { ':', 'Ж' },
            { '<', 'Б' },
            { '>', 'Ю' },

            { '#', '№' },
        };

        /// <inheritdoc />
        public string ToAnotherKeyboardLayout(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var sb = new StringBuilder(value.Length);
            foreach (var ch in value)
                sb.Append(ToAnotherKeyboardLayout(ch));
            return sb.ToString();
        }

        private static char ToAnotherKeyboardLayout(char ch)
        {
            var c = char.ToLowerInvariant(ch);

            if (KeyboardPairs.Keys.Contains(c))
            {
                c = KeyboardPairs[c];
                if (char.IsUpper(ch))
                    c = char.ToUpper(c);
                return c;
            }

            if (KeyboardPairs.Values.Contains(c))
            {
                c = KeyboardPairs.First(p => p.Value == c).Key;
                if (char.IsUpper(ch))
                    c = char.ToUpper(c);
                return c;
            }

            return ch;
        }
    }
}
