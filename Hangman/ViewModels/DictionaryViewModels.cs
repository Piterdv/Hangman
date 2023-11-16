using Caliburn.Micro;
using Hangman.Commands;
using Hangman.Helpers;
using Hangman.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string _dictionaryName = "";
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
            SynchronizeCommand = new RelayCommand(Synchronize);
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

        private void Synchronize(object obj)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            //może odpuścić sobie konstruktor?
            FtpHelper ftpHelper = new FtpHelper(AppSettings.FtpServer, AppSettings.FtpUser, AppSettings.FtpPassword, AppSettings.FtpPath);
            List<MyFile> ftpl = ftpHelper.GetListOfFiles();
            List<string> files = new List<string>();
            List<string> newestFile = new List<string>();
            int filesGetCount = 0;

            //select all file from ftp server which are not on local directory or there are newest and add then to list
            foreach (var f in ftpl)
            {
                if (!File.Exists(_dictionaryDirPath + f.Name))
                {
                    files.Add(f.Path);
                }
                else
                {
                    if (DateTime.Parse(f.LastModified) > File.GetLastWriteTime(_dictionaryDirPath + f.Name))
                    {
                        files.Add(f.Path);
                    }
                }
            }

            foreach (var f in files)
            {
                if (ftpHelper.GetFileFromFtp(f, _dictionaryDirPath + Path.GetFileName(f)))
                {
                    newestFile.Add(Path.GetFileName(f));
                }
            }

            ShowAllDictonaries(null);
            CheckFtpSynchro(newestFile, 2);

            filesGetCount = files.Count;
            files.Clear();

            //select all files from local directory which are not on ftp file or there are older on ftp server and add them to list
            foreach (var f in Directory.GetFiles(_dictionaryDirPath))
            {
                //jeżeli pobrane z ftp pliki są nowsze niż te na dysku to nie wysyłać ich
                if (newestFile.Contains(Path.GetFileName(f)))
                {
                    continue;
                }

                if (!ftpl.Exists(x => x.Name == Path.GetFileName(f)))
                {
                    files.Add(f);
                }
                else
                {
                    //fuck - z ftpa nie ma sekund?
                    if (File.GetLastWriteTime(f) > DateTime.Parse(ftpl.Find(x => x.Name == Path.GetFileName(f)).LastModified))
                    {
                        files.Add(f);
                    }
                }
            }

            List<string> list = GetOnlyFileNameFromList(files);
            CheckFtpSynchro(list, 1);

            if (files.Count == 0 && filesGetCount == 0)
            {
                MessageBox.Show("All files are up to date!");
                Mouse.OverrideCursor = Cursors.Arrow;
                return;
            }

            if (ftpHelper.SaveAllChoosenFileToFtp(files))
            {
                MessageBox.Show("All files have been synchronized!");
            }
            else
            {
                MessageBox.Show("Something went wrong!");
            }

            //ShowAllDictonaries(null);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static List<string> GetOnlyFileNameFromList(List<string> files)
        {
            List<string> fi = new List<string>();

            foreach (var f in files)
            {
                fi.Add(Path.GetFileName(f));
            }

            return fi;
        }

        public BindableCollection<WordEntity> Words { get; set; } = new BindableCollection<WordEntity>();
        public BindableCollection<DictionaryEntity> Dictionaries { get; set; } = new BindableCollection<DictionaryEntity>();
        public ICommand AddNewWordToDictionaryCommand { get; set; }
        public ICommand ChooseDictionaryCommand { get; set; }
        public ICommand ShowAllDictonariesCommand { get; set; }
        public ICommand SaveDictionaryCommand { get; set; }
        public ICommand FindWordCommand { get; set; }
        public ICommand SynchronizeCommand { get; set; }
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
            FileHelper.SaveDictionaryToJsonFile(_dictionaryFullPath, _wordEntities);
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
                MessageBox.Show("There's no choosen dictionary name. So write it in text box or choose from list of dictionaries after you'll click on \"≡Show all dictionaries\" button :)");
                return;
            }

            _dictionary = ((TextBox)obj).Text;

            CreateOrChooseDictionary(_dictionary);

            EnabledButton = true;
            Hidden = "Visible";

            Words = new BindableCollection<WordEntity>(_wordEntities);
            OnPropertyChanged(nameof(Words));
        }

        private void ShowAllDictonaries(object? obj)
        {
            _dictionaries = FileHelper.GetDictionaryFileNameToList(_dictionaryDirPath);

            if (_dictionaries.Count == 0)
            {
                MessageBox.Show("There's no dictionaries!\nYou have to write name of dictionary (like Animals or Verbs or what you want) in field \"Dictionary name:\" and then click on button \"‹Choose this dictionary\" to create it.");
                return;
            }

            Dictionaries = new BindableCollection<DictionaryEntity>(_dictionaries);
            OnPropertyChanged(nameof(Dictionaries));

            HiddenDictionary = "Visible";
        }

        /// <summary>
        /// Odświeżanie listy słowników po synchronizacji
        /// </summary>
        /// <param name="dictName"></param>
        /// <param name="updwnok">ok 0, upload 1, download 2</param>
        private void CheckFtpSynchro(List<string> dictName, int updwnok = 0)
        {
            foreach (var d in _dictionaries)
            {
                if (dictName.Contains(d.DictionaryName + ".json"))
                {
                    if (updwnok == 1) d.Upload = "Visible";
                    if (updwnok == 2) d.Download = "Visible";
                    if (updwnok == 0) d.Ok = "Visible";
                }
            }

            Dictionaries = new BindableCollection<DictionaryEntity>(_dictionaries);
            OnPropertyChanged(nameof(Dictionaries));

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
            if (_wordEntities != null && _wordEntities.Count > 0) _wordEntities.RemoveAt(0);
            if (_mvm != null) LocalDictionary.Dictionary = _wordEntities;
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
            _wordEntities = FileHelper.CreateOrChooseDictionary(_dictionaryFullPath);
        }


        //------------------implementacja interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
