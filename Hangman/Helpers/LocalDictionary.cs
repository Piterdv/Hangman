using Hangman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Helpers
{
    internal static class LocalDictionary
    {
        public static List<WordEntity> Dictionary { get; set; } = new List<WordEntity>();
    }
}
