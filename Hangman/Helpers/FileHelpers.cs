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
        private const char _separator = '|';

        public static bool FileExists(string dirName, string fileName)
        {
            return File.Exists(_filePathDictionary);
        }

        /// <summary>
        /// Create file with name and path, write first line with date and time and return path to file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="fileName"></param>
        public static void CreateNewFile(string fullPath)
        {
            string fileName = Path.GetFileName(fullPath);
            string dirPath = Path.GetDirectoryName(fullPath);

            if (!File.Exists(fullPath))
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                string newFileText = DateTime.Now.ToString("yyyy-MM-dd HH:mm") + $" | {fileName} | Dictionary have been creaded.";
                File.AppendAllText(fullPath, newFileText);
                MessageBox.Show($"Dictionary {fileName[..^4]} was created.");
            }
            else
            {
                MessageBox.Show($"Dictionary {fileName[..^4]} was choosen.");
            }
        }

        public static void AddWordToDictionary(string fullPath, string word, string explanation, string speechPart)
        {
            File.AppendAllText(fullPath,Environment.NewLine + $"{word}|{explanation}|{speechPart}");
        }

        static public Dictionary<string, string> GetAwailableWordsWithExplanation()
        {
            var dictionary = new Dictionary<string, string>();
            var words = DeserializeSlownikToList();

            if (words.Count == 0)
                throw new Exception("Nie mam żadnych słów do losowania, uzupełnij plik!");

            foreach (var line in words)
            {
                try
                {
                    var wordAndExplanation = line.Split(_separator);
                    dictionary.Add(wordAndExplanation[0].ToUpper().Trim(), wordAndExplanation[1].Trim());
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }

            return dictionary;
        }

        static public List<string> DeserializeSlownikToList()
        {
            string slownik = ReadDictionaryFromFile();
            List<string> slownikLst = slownik.Trim().Split(' ').Select(x => x.Trim()).ToList();
            List<string> slownikLstOut = new List<string>();
            StringBuilder sb = new StringBuilder();
            int x = 0;

            for (int i = 0; i < slownikLst.Count; i++)
            {
                try
                {
                    sb.Append(slownikLst[i] + " ");

                    if (i > 1
                        && slownikLst[i] != _separator.ToString()
                        && !int.TryParse(slownikLst[i].Substring(0, 1), out x)
                        && slownikLst[i] != "...,"
                        && !slownikLst[i].Contains("-")
                        && slownikLst[i].Trim().ToUpper() == slownikLst[i].Trim())
                    {
                        if (i < slownikLst.Count)
                            slownikLstOut.Add(sb.ToString().Trim().Replace(slownikLst[i], ""));
                        sb.Clear();
                        sb.Append(slownikLst[i] + " ");
                    }

                    if (i == slownikLst.Count - 1) slownikLstOut.Add(sb.ToString().Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            return slownikLstOut;
        }

        private static string ReadDictionaryFromFile()
        {
            return File.ReadAllText(_filePathDictionary);
        }
    }
}
