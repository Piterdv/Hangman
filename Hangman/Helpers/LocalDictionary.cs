using Hangman.Models;
using System.Collections.Generic;

namespace Hangman.Helpers;

/// <summary>
/// This class is used to store the dictionary in memory. Not the best solution, but it works:).
/// </summary>
internal static class LocalDictionary
{
    public static List<WordEntity> Dictionary { get; set; } = new List<WordEntity>();
}
