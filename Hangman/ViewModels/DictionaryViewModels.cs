using Hangman.Commands;
using Hangman.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hangman.ViewModels
{
    public class DictionaryViewModels : INotifyPropertyChanged
    {
        private string _dictionaryFullPath = string.Empty;
        private string _dirName = "Dictionaries";
        private string _dictionary = string.Empty;
        private bool _enabledButton= false;

        public DictionaryViewModels()
        {
            AddNewWordToDictionaryCommand = new RelayCommand(AddNewWordToDictionary);
            ChooseDictionaryCommand = new RelayCommand(ChooseDictionary);
            CloseCommand = new RelayCommand(Close);
        }

        private void ChooseDictionary(object obj)
        {
            if(((TextBox)obj).Text == string.Empty)
            {
                MessageBox.Show("Wpisz nazwę słownika!");
                return;
            }

            _dictionary = ((TextBox)obj).Text;

            CreateNewDictionary(_dictionary);

            _enabledButton = true;
        }

        private void AddNewWordToDictionary(object obj)
        {

        }

        private void Close(object obj)
        {
            (obj as Window)?.Close();
        }

        private void CreateNewDictionary(string dictionary)
        {
            System.Media.SystemSounds.Beep.Play();

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                dictionary = dictionary.Replace(c, '_');
            }

            _dictionaryFullPath = Directory.GetCurrentDirectory() + "/" + _dirName + "/" + dictionary + ".txt";
            FileHelpers.CreateNewFile(_dictionaryFullPath);
        }

        public ICommand AddNewWordToDictionaryCommand { get; set; }
        public ICommand ChooseDictionaryCommand { get; set; }
        public ICommand CloseCommand { get; set; }


        //------------------implementacja interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
