using Caliburn.Micro;
using Hangman.Commands;
using Hangman.Helpers;
using Hangman.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hangman.ViewModels
{
    public class DictionaryViewModels : Screen, INotifyPropertyChanged
    {
        private static string _dirName = "Dictionaries";
        private static string _dictionaryDirPath = Directory.GetCurrentDirectory() + "\\" + _dirName + "\\";
        private static string _dictionaryFullPath = "";
        private string _dictionary = string.Empty;
        private bool _enabledButton = false;
        private string _hidden = "Hidden";
        //TODO: zrobić zmienną _wordEntities zamiast _word, _explanation, _speechPart
        private string _word = string.Empty;
        private string _explanation = string.Empty;
        private string _speechPart = string.Empty;
        private List<WordEntity> _wordEntities = new List<WordEntity>();
        private List<DictionaryEntity> _dictionaries = new List<DictionaryEntity>();
        private WordEntity _selectedWordEntity = new WordEntity();
        private string _dictionaryName = ""; //"DefaultDictionary";
        private string _hiddenDictionary = "Hidden";
        private DictionaryEntity _selectedDictionaryEntity = new DictionaryEntity();
        private Brush _choosenDictionaryColor = Brushes.LightGreen;
        private readonly MainViewModels _mvm;

        public DictionaryViewModels(WordEntity? de, MainViewModels mvm)
        {
            AddNewWordToDictionaryCommand = new RelayCommand(AddNewWordToDictionary);
            ChooseDictionaryCommand = new RelayCommand(ChooseDictionary);
            ShowAllDictonariesCommand = new RelayCommand(ShowAllDictonaries);
            SaveDictionaryCommand = new RelayCommand(SaveDictionary);
            FindWordCommand = new RelayCommand(FindWord);
            CloseCommand = new RelayCommand(Close);
            EnabledButton = false;

            if (de != null)
            {
                _word = de.Word;
                _explanation = de.Explanation;
                _speechPart = de.SpeechPart;
            }
            _mvm = mvm;
        }


        public BindableCollection<WordEntity> Words { get; set; } = new BindableCollection<WordEntity>();
        public BindableCollection<DictionaryEntity> Dictionaries { get; set; } = new BindableCollection<DictionaryEntity>();
        public ICommand AddNewWordToDictionaryCommand { get; set; }
        public ICommand ChooseDictionaryCommand { get; set; }
        public ICommand ShowAllDictonariesCommand { get; set; }
        public ICommand SaveDictionaryCommand { get; set; }
        public ICommand FindWordCommand { get; set; }
        public ICommand CloseCommand { get; set; }


        private void FindWord(object obj)
        {
            string word = ((TextBox)obj).Text;

            if (word == string.Empty)
            {
                MessageBox.Show("Insert word!");
                return;
            }

            foreach (var w in _wordEntities)
            {
                if (w.Word.ToLower() == word.ToLower())
                {
                    _word = w.Word;
                    _explanation = w.Explanation;
                    _speechPart = w.SpeechPart;
                    OnPropertyChanged(nameof(Word));
                    OnPropertyChanged(nameof(Explanation));
                    OnPropertyChanged(nameof(SpeechPart));
                    return;
                }
            }

            MessageBox.Show("Word not found!");
        }

        private void SaveDictionary(object obj)
        {
            UpdateListOfWords();
            FileHelpers.SaveDictionaryToJsonFile(_dictionaryFullPath, _wordEntities);
            //MessageBox.Show("Dictionary saved!");
            ChooseDictionary(new TextBox { Text = _dictionary });
            ChoosenDictionaryColor = Brushes.LightGreen;
        }

        private void UpdateListOfWords()
        {
            var actwe = new WordEntity { Word = _word, Explanation = _explanation, SpeechPart = _speechPart };
            var we = _wordEntities.Find(w => w.Word.Equals(_word));

            if (we != null && actwe.Word == we.Word)
            {
                _wordEntities.Remove(we);
            }

            _wordEntities.Add(actwe);
        }

        public string Word
        {
            get { return _word; }
            set
            {
                _word = value;
                OnPropertyChanged();
                ChoosenDictionaryColor = Brushes.Red;
            }
        }

        public string DictionaryName
        {
            get { return _dictionaryName; }
            set
            {
                _dictionaryName = value;
                OnPropertyChanged();
                EnabledButton = false;
            }
        }

        public string Explanation
        {
            get { return _explanation; }
            set
            {
                _explanation = value.Replace("|", "-");
                OnPropertyChanged();
                ChoosenDictionaryColor = Brushes.Red;
            }
        }

        public string SpeechPart
        {
            get { return _speechPart; }
            set
            {
                _speechPart = value.Replace("|", "-");
                OnPropertyChanged();
                ChoosenDictionaryColor = Brushes.Red;
            }
        }
        public bool EnabledButton
        {
            get { return _enabledButton; }
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
                _hidden = value;
                OnPropertyChanged();
            }
        }

        public string HiddenDictionary
        {
            get { return _hiddenDictionary; }
            set
            {
                _hiddenDictionary = value;
                OnPropertyChanged();
            }
        }

        public Brush ChoosenDictionaryColor
        {
            get { return _choosenDictionaryColor; }
            set
            {
                _choosenDictionaryColor = value;
                OnPropertyChanged();
            }
        }

        public WordEntity SelectedWordEntity
        {
            get { return _selectedWordEntity; }
            set
            {
                NotifyOfPropertyChange(() => SelectedWordEntity);
                _selectedWordEntity = value;
                if (_selectedWordEntity != null) FindWord(new TextBox { Text = _selectedWordEntity.Word });
            }
        }

        public DictionaryEntity SelectedDictionaryEntity
        {
            get
            {
                return _selectedDictionaryEntity;
            }
            set
            {
                NotifyOfPropertyChange(() => SelectedDictionaryEntity);
                _selectedDictionaryEntity = value;
                if (_selectedDictionaryEntity != null) DictionaryName = _selectedDictionaryEntity.DictionaryName;
                HiddenDictionary = "Hidden";
                if (value != null)
                {
                    ChooseDictionary(new TextBox { Text = _selectedDictionaryEntity.DictionaryName });
                    if (_mvm != null)
                        _mvm.WindowTitle = AppSettings.AppName + " (dictionary: " + _selectedDictionaryEntity.DictionaryName + "| words#: " + _selectedDictionaryEntity.NumberOfWords + ")";
                }

            }
        }

        public void ChooseDictionary(object obj)
        {
            if (((TextBox)obj).Text == string.Empty)
            {
                MessageBox.Show("There's no dictionary name, write it:)");
                return;
            }

            _dictionary = ((TextBox)obj).Text;

            CreateOrChooseDictionary(_dictionary);

            EnabledButton = true;
            Hidden = "Visible";

            Words = new BindableCollection<WordEntity>(_wordEntities);
            OnPropertyChanged(nameof(Words));
        }

        private void ShowAllDictonaries(object obj)
        {
            _dictionaries = FileHelpers.GetDictionaryFileNameToList(_dictionaryDirPath);

            Dictionaries = new BindableCollection<DictionaryEntity>(_dictionaries);
            OnPropertyChanged(nameof(Dictionaries));

            HiddenDictionary = "Visible";
        }

        private void AddNewWordToDictionary(object obj)
        {
            if (_word == string.Empty || _explanation == string.Empty || _speechPart == string.Empty)
            {
                MessageBox.Show("Insert all needed value!");
                return;
            }

            foreach (var word in _wordEntities)
            {
                if (word.Word.ToLower() == _word.ToLower())
                {
                    MessageBox.Show($"{word.Word.ToUpper()} already exists in dictionary!\n\nIf you only want to change explanation or part of speech - click on \"Save changes\" button after your modifications.");
                    return;
                }
            }

            _wordEntities.Add(new WordEntity { Word = _word, Explanation = _explanation, SpeechPart = _speechPart });
            SaveDictionary(new TextBox { Text = _dictionary });

            ChoosenDictionaryColor = Brushes.Red;
        }

        private void Close(object obj)
        {
            LocalDictionary.Dictionary = _wordEntities;
            (obj as Window)?.Close();
        }

        private void CreateOrChooseDictionary(string dictionary)
        {
            System.Media.SystemSounds.Beep.Play();

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                dictionary = dictionary.Replace(c, '_');
            }

            _dictionaryFullPath = _dictionaryDirPath + dictionary + ".json";
            _wordEntities = FileHelpers.CreateOrChooseDictionary(_dictionaryFullPath);
        }


        //------------------implementacja interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
