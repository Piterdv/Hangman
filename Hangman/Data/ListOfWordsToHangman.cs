using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hangman.Data
{
    internal class ListOfWordsToHangman
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            var apiKey = "YOUR_WORDNIK_API_KEY";
            var wordList = new List<string>();

            for (int i = 0; i < 500; i++)
            {
                var response = await client.GetStringAsync($"http://api.wordnik.com/v4/words.json/randomWord?api_key={apiKey}");
                var word = ParseWord(response); // You need to implement ParseWord method
                wordList.Add(word);
            }

            // Now wordList contains 500 random words
        }

        private static string ParseWord(string response)
        {
            throw new NotImplementedException();
        }
    }
}
