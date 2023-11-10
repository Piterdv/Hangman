using Hangman.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Hangman.Helpers
{
    public static class FileHelpers
    {

        private static string _filePathDictionary = "";

        public static bool FileExists(string dirName, string fileName)
        {
            return File.Exists(_filePathDictionary);
        }

        public static List<WordEntity> CreateOrChooseDictionary(string fullPath)
        {
            string fileName = Path.GetFileName(fullPath);
            string dirPath = Path.GetDirectoryName(fullPath);

            if (!File.Exists(fullPath))
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                WordEntity wordEntity = new WordEntity
                {
                    Word = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Explanation = $"Dictionary {fileName[..^5]} have been creaded.",
                    SpeechPart = "Noun"
                };
                var words = Newtonsoft.Json.JsonConvert.SerializeObject(new List<WordEntity> { wordEntity });

                File.AppendAllText(fullPath, words);
                MessageBox.Show($"Dictionary {fileName[..^5]} was created.");
            }

            return GetWordsFromJsonFile(fullPath);
        }

        public static List<WordEntity> GetWordsFromJsonFile(string fullPath)
        {
            var words = new List<WordEntity>();
            var json = File.ReadAllText(fullPath);

            words = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WordEntity>>(json);

            return words.OrderBy(x => x.Word).ToList<WordEntity>();
        }

        public static void SaveDictionaryToJsonFile(string fullPath, List<WordEntity> words)
        {
            File.WriteAllText(fullPath, string.Empty);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(words);

            File.AppendAllText(fullPath, json);
            
        }

        public static void AddWordToDictionary(string fullPath, string word, string explanation, string speechPart)
        {
            File.AppendAllText(fullPath, Environment.NewLine + $"{word}|{explanation}|{speechPart}");
        }

        private static string ReadDictionaryFromFile()
        {
            return File.ReadAllText(_filePathDictionary);
        }
    }
}
