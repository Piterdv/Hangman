using Hangman.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Hangman.Helpers;

public static class FileHelper
{
    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public static List<WordEntity> GetWordsFromDictionary(string fullPath, out string message)
    {
        message = EnsureDictionaryFileExists(fullPath);
        return GetWordsFromDictionaryFile(fullPath);
    }

    private static string EnsureDictionaryFileExists(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            return CreateDictionary(fullPath);
        }
        return string.Empty;
    }

    public static List<WordEntity> GetWordsFromDictionaryFile(string fullPath)
    {
        var json = File.ReadAllText(fullPath);
        List<WordEntity> words = DeserializeJson(json);

        return words.Where(x => !string.IsNullOrEmpty(x.Word)).OrderBy(x => x.Word).ToList();
    }

    private static List<WordEntity> DeserializeJson(string json)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<WordEntity>>(json) ?? new List<WordEntity>();
    }

    private static string CreateDictionary(string fullPath)
    {
        string fileName = Path.GetFileName(fullPath);
        string dirPath = Path.GetDirectoryName(fullPath)!;
        string dictionaryName = Path.GetFileNameWithoutExtension(fileName);

        EnsureDirectoryExists(dirPath);

        WordEntity wordEntity = CreateNewWordEntity(dictionaryName);
        AddWordsToDictionaryFile(fullPath, new List<WordEntity> { wordEntity });

        return $"Dictionary {dictionaryName} was created.";
    }

    private static WordEntity CreateNewWordEntity(string dictionaryName)
    {
        return new WordEntity
        {
            Word = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            Explanation = string.Format(Resource.DictionaryHasBeenCreated, dictionaryName),
            SpeechPart = "Noun"
        };
    }

    private static void EnsureDirectoryExists(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    public static void AddWordsToDictionaryFile(string fullPath, List<WordEntity> words)
    {
        string json = SerializeJson(words);
        File.WriteAllText(fullPath, json);
    }

    private static string SerializeJson(List<WordEntity> words)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(words);
    }

    public static void AddWordToDictionary(string fullPath, string word, string explanation, string speechPart)
    {
        var wordEntity = new WordEntity
        {
            Word = word,
            Explanation = explanation,
            SpeechPart = speechPart
        };

        var words = GetWordsFromDictionaryFile(fullPath);
        words.Add(wordEntity);
        AddWordsToDictionaryFile(fullPath, words);
    }

    public static List<DictionaryEntity> GetDictionaryFileNameToList(string dirPath)
    {
        EnsureDirectoryExists(dirPath);

        var files = Directory.GetFiles(dirPath, "*.json");
        var list = new List<DictionaryEntity>();

        foreach (var file in files)
        {
            list.Add(new DictionaryEntity
            {
                DictionaryName = Path.GetFileNameWithoutExtension(file),
                DateCreated = File.GetCreationTime(file).ToString("yyyy-MM-dd HH:mm"),
                NumberOfWords = GetWordsFromDictionaryFile(file).Count,
                DateModified = File.GetLastWriteTime(file).ToString("yyyy-MM-dd HH:mm"),
                Upload = "Hidden",
                Download = "Hidden",
                Ok = "Hidden"
            });
        }

        return list;
    }
}
