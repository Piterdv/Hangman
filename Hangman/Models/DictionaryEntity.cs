namespace Hangman.Models;

public class DictionaryEntity
{
    public string DictionaryName { get; set; }= "";
    public string DateCreated { get; set; } = "";
    public string DateModified { get; set; } = "";
    public int NumberOfWords { get; set; } = 0;
    public string Upload { get; set; } = "Hidden";
    public string Download { get; set; } = "Hidden";
    public string Ok { get; set; } = "Hidden";
}
