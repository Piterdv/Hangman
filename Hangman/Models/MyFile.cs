using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class MyFile
    {
        public string Name { get; set; }= "";
        public string Permissions { get; set; } = "";
        public string MyFileOrDir { get; set; } = "";
        public string Size { get; set; } = "";
        public string Date { get; set; } = "";
    }
}
