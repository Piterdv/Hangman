using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class DictionaryEntity
    {
        public string DictionaryName { get; set; }= "";
        public string DateCreated { get; set; } = "";
        public string DateModified { get; set; } = "";
        public int NumberOfWords { get; set; } = 0;
    }
}
