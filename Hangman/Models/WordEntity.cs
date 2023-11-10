using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class WordEntity
    {
        public string Word { get; set; }=string.Empty;
        public string Explanation { get; set; }=string.Empty;
        public string SpeechPart { get; set; } = string.Empty;
    }
}
