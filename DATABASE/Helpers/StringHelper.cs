using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE
{
    public static class StringHelper
    {
        // Split text
        // Returns array of words (strings)
        public static string[] SplitText(string inputText)
        {
            char[] delimiterChars = { ' ', ',', ';', '.', ':', '\t', '\n' }; // Znaky, ktore urcuju rozdelenie textu

            //System.Console.WriteLine($"Original text: '{inputText}'");

            string[] words = inputText.Split(delimiterChars);

            /*
            System.Console.WriteLine($"{words.Length} words in text:");

                foreach (var word in words)
                {
                    System.Console.WriteLine($"<{word}>");
            }
            */

            return words;
        }

        // Convert string to array of int IDs

        public static int[] ConvertStringArrayOfIDs(string inputText)
        {
            string[] words = SplitText(inputText);

            int[] arrayIDs = new int[words.Length];

            for (int i = 0; i < words.Length; i++)
                arrayIDs[i] = Int32.Parse(words[i]);

            return arrayIDs;
        }

    }
}
