using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hangman.Helpers
{
    internal static class AppSettings
    {
        internal static string AppVersion { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        internal static string AppName { get; set; } = $"Piterdv Hangman {DateTime.Now.Year} {AppVersion} ";

        //OK - I know that this is not a good practice to store passwords in the code, but here is OK:)
        internal static string FtpServer { get; set; } = "193.227.116.201";//"ftp://193.227.116.201";
        internal static string FtpUser { get; set; } = "u93380_hangman";
        internal static string FtpPassword { get; set; } = "pxSNw3zrYJfk7E8";
        internal static string FtpPath { get; set; } = "Dictionaries";
    }
}
