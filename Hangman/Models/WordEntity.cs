namespace Hangman.Models;

public class WordEntity
{
    public string Word { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string SpeechPart { get; set; } = string.Empty;
}
