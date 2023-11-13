using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hangman.Helpers
{
    //I know:)
    internal static class AppSettings
    {
        //get actual version of application from AssemblyInfo.cs
        internal static string AppVersion { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        internal static string AppName { get; set; } =  $"Hangman 2023 {AppVersion} ";
    }
}
