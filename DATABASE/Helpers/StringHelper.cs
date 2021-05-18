using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace DATABASE
{
    public static class StringHelper
    {
        // Split text
        // Returns array of words (strings)
        public static string[] SplitText(string inputText)
        {
            char[] delimiterChars = {/*' ', ',', , '.', ':',*/ ';', '\t', '\n' }; // Znaky, ktore urcuju rozdelenie textu

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

        public static string[] SplitText(string inputText, char delimiterChar)
        {
            //System.Console.WriteLine($"Original text: '{inputText}'");

            string[] words = inputText.Split(delimiterChar);

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
        public static int[] ConvertStringArrayOfIDs(string inputText, char delimiterChar)
        {
            if (inputText == "") return null;

            string[] words = SplitText(inputText, delimiterChar);

            int[] arrayIDs = new int[words.Length];

            for (int i = 0; i < words.Length; i++)
                arrayIDs[i] = Int32.Parse(words[i]);

            return arrayIDs;
        }

        // Convert string to of numbers (double)
        public static double[] ConvertStringArray(string inputText, char delimiterChar)
        {
            if (inputText == "") return null;

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            string[] words = SplitText(inputText, delimiterChar);

            double[] array = new double[words.Length];

            for (int i = 0; i < words.Length; i++)
                array[i] = double.Parse(words[i], nfi);

            return array;
        }
    }
}