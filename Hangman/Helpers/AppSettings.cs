using System;
using System.IO;

namespace Hangman.Helpers;

internal static class AppSettings
{
    internal static string APP_VERSION { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();
    internal static string APP_NAME { get; } = $"Piterdv Hangman {DateTime.Now.Year} {APP_VERSION} ";
    internal static string FTP_SERVER { get; } = "193.227.116.201";//"ftp://193.227.116.201";
    internal static string FTP_USER { get; } = "u93380_hangman";
    internal static string FTP_PASSWORD { get; } = "pxSNw3zrYJfk7E8";
    internal static string FTP_PATH { get; } = "Dictionaries";

    internal static string DICTIONARIES_DIR { get; } = "Dictionaries";
    internal static string DICTIONARY_DIR_FULL_PATH { get; } = Directory.GetCurrentDirectory() + "\\" + DICTIONARIES_DIR + "\\";

    //TODO - wrzuć to i resztę do resources
    //internal static string GAME_STATUS_START { get; } = "Click on button \"NEW GAME\"...";
    //internal static string GAME_STATUS_LETTER_INFO { get; } = "Click the choosen letter to guess the entry...";
    //internal static string HELP_ME_MESSAGE { get; } = "\"HELP ME, please...\"";

}
