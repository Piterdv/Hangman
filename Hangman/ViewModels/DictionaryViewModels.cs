using GetWordsAndExplanationFromWordnik.Models;
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
        private bool _enabledButton = false;
        private string _hidden = "Hidden";
        private string _word = string.Empty;
        private string _explanation=string.Empty;
        private string _speechPart=string.Empty;

        public DictionaryViewModels()
        {
            AddNewWordToDictionaryCommand = new RelayCommand(AddNewWordToDictionary);
            ChooseDictionaryCommand = new RelayCommand(ChooseDictionary);
            CloseCommand = new RelayCommand(Close);
            EnabledButton = false;
        }

        public ICommand AddNewWordToDictionaryCommand { get; set; }
        public ICommand ChooseDictionaryCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public string Word
        {
            get{return _word;}
            set
            {
                _word = value;
                OnPropertyChanged();
            }
        }

        public string Explanation
        {
            get{return _explanation;}
            set
            {
                _explanation = value;
                OnPropertyChanged();
            }
        }

        public string SpeechPart
        {
            get{return _speechPart;}
            set
            {
                _speechPart = value;
                OnPropertyChanged();
            }
        }

        public bool EnabledButton
        {
            get{return _enabledButton;}
            set
            {
                _enabledButton = value;
                OnPropertyChanged();
            }
        }

        public string Hidden
        {
            get { return _hidden; }
            set
            {
                _hidden= value;
                OnPropertyChanged();
            }
        }

        private void ChooseDictionary(object obj)
        {
            if (((TextBox)obj).Text == string.Empty)
            {
                MessageBox.Show("Wpisz nazwę słownika!");
                return;
            }

            _dictionary = ((TextBox)obj).Text;

            CreateNewDictionary(_dictionary);

            EnabledButton = true;
            Hidden = "Visible";
        }

        private void AddNewWordToDictionary(object obj)
        {
            if(_word == string.Empty || _explanation == string.Empty || _speechPart == string.Empty)
            {
                MessageBox.Show("Insert all needed value!");
                return;
            }   

            FileHelpers.AddWordToDictionary(_dictionaryFullPath, _word, _explanation, _speechPart);
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

        //------------------implementacja interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
