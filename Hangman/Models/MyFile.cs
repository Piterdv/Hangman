using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Models;

public class MyFile
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string LastModified { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; }
    public string Permissions { get; set; } = string.Empty;
    public string MyFileOrDir { get; set; } = string.Empty;
}
