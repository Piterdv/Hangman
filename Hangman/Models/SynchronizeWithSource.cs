using System.Collections.Generic;

namespace Hangman.Models;

public class StateOfSynchronization
{
    public List<string>? NewestFiles { get; set; }
    public List<string>? FilesToUpload { get; set; }
    public bool IsSomeIssue { get; set; }
    public string? WhatsUp { get; set; }
    public string? Info { get; set; }

}

