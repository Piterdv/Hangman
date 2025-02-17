using Caliburn.Micro;
using CommunityToolkit.Mvvm.Input;
using Hangman.Enums;
using Hangman.Helpers;
using Hangman.Models;
using Hangman.SynchronizeSource;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hangman.ViewModels;

public class DictionaryViewModel : Screen, INotifyPropertyChanged
{
    private static readonly string _dictionaryDirPath = AppSettings.DICTIONARY_DIR_FULL_PATH;
    private static string _dictionaryFullPath = "";

    private readonly MainViewModel _mainVM;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly ISynchronizeResource _synchronize;

    private List<WordEntity> _wordEntities = [];
    private List<DictionaryEntity> _dictionaries = [];

    private WordEntity _selectedWordEntity = new();
    private DictionaryEntity _selectedDictionaryEntity = new();

    private Brush _choosenDictionaryColor = Brushes.LightGreen;

    private string _dictionary = string.Empty;
    private string _hidden = "Hidden";
    private string _word = string.Empty;
    private string _explanation = string.Empty;
    private string _speechPart = string.Empty;
    private string _dictionaryName = "";
    private string _hiddenDictionary = "Hidden";
    private bool _isEnabledButton = false;

    public DictionaryViewModel(WordEntity? wordEntity, MainViewModel mainVM, IDialogCoordinator instance, ISynchronizeResource synchronize)
    {
        _dialogCoordinator = instance;
        _synchronize = synchronize;
        _mainVM = mainVM;

        InitializeCommands();
        GetWordsStartValues(wordEntity);
        EnabledButton = false;
    }

    //----Properties

    public BindableCollection<WordEntity> Words { get; set; } = [];
    public BindableCollection<DictionaryEntity> Dictionaries { get; set; } = [];
    public ICommand? AddNewWordToDictionaryCommand { get; private set; } //= null!;
    public ICommand? ChooseDictionaryCommand { get; private set; } //= null!;
    public ICommand? ShowAllDictonariesCommand { get; private set; } //= null!;
    public ICommand? SaveDictionaryCommand { get; private set; } //= null!;
    public ICommand? FindWordCommand { get; private set; } //= null!;
    public ICommand? SynchronizeCommand { get; private set; } //= null!;
    public ICommand? CloseCommand { get; private set; } //= null!;

    public string Word
    {
        get => _word;
        set
        {
            _word = value;
            OnPropertyChanged();
            ChoosenDictionaryColor = Brushes.Red;
        }
    }

    public string DictionaryName
    {
        get => _dictionaryName;
        set
        {
            _dictionaryName = value;
            OnPropertyChanged();
            EnabledButton = false;
        }
    }

    public string Explanation
    {
        get => _explanation;
        set
        {
            _explanation = value.Replace("|", "-");
            OnPropertyChanged();
            ChoosenDictionaryColor = Brushes.Red;
        }
    }

    public string SpeechPart
    {
        get => _speechPart;
        set
        {
            _speechPart = value.Replace("|", "-");
            OnPropertyChanged();
            ChoosenDictionaryColor = Brushes.Red;
        }
    }

    public bool EnabledButton
    {
        get => _isEnabledButton;
        set
        {
            _isEnabledButton = value;
            OnPropertyChanged();
        }
    }

    public string Hidden
    {
        get => _hidden;
        set
        {
            _hidden = value;
            OnPropertyChanged();
        }
    }

    public string HiddenDictionary
    {
        get => _hiddenDictionary;
        set
        {
            _hiddenDictionary = value;
            OnPropertyChanged();
        }
    }

    public Brush ChoosenDictionaryColor
    {
        get => _choosenDictionaryColor;
        set
        {
            _choosenDictionaryColor = value;
            OnPropertyChanged();
        }
    }

    public WordEntity SelectedWordEntity
    {
        get => _selectedWordEntity;
        set
        {
            NotifyOfPropertyChange(() => SelectedWordEntity);
            _selectedWordEntity = value;
            if (_selectedWordEntity != null) FindWord(new TextBox { Text = _selectedWordEntity.Word });
        }
    }

    public DictionaryEntity SelectedDictionaryEntity
    {
        get => _selectedDictionaryEntity;
        set
        {
            NotifyOfPropertyChange(() => SelectedDictionaryEntity);
            _selectedDictionaryEntity = value;
            if (_selectedDictionaryEntity != null) DictionaryName = _selectedDictionaryEntity.DictionaryName;
            HiddenDictionary = "Hidden";
            if (value != null && _selectedDictionaryEntity != null)
            {
                ChooseDictionary(new TextBox { Text = _selectedDictionaryEntity.DictionaryName });
                if (_mainVM != null)
                    _mainVM.WindowTitle = AppSettings.APP_NAME
                        + string.Format(Resource.HowManyWOrdsInDictionary, _selectedDictionaryEntity.DictionaryName, _selectedDictionaryEntity.NumberOfWords);
            }
        }
    }

    //-------Methods

    private void InitializeCommands()
    {
        AddNewWordToDictionaryCommand = new RelayCommand<object>(AddNewWordToDictionary!);
        ChooseDictionaryCommand = new RelayCommand<object>(ChooseDictionary!);
        ShowAllDictonariesCommand = new RelayCommand<object>(ShowAllDictonaries);
        SaveDictionaryCommand = new RelayCommand<object>(SaveDictionary!);
        FindWordCommand = new RelayCommand<object>(FindWord!);
        SynchronizeCommand = new RelayCommand<object>(Synchronize!);
        CloseCommand = new RelayCommand<object>(Close!);
    }

    private async void FindWord(object obj)
    {
        var word = ((TextBox)obj).Text;

        if (string.IsNullOrEmpty(word))
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resource.Attention, Resource.InsertWord);
            return;
        }

        var foundWord = _wordEntities.Find(w => w.Word.Equals(word, StringComparison.OrdinalIgnoreCase));

        if (foundWord != null)
        {
            _word = foundWord.Word;
            _explanation = foundWord.Explanation;
            _speechPart = foundWord.SpeechPart;
            OnPropertyChanged(nameof(Word));
            OnPropertyChanged(nameof(Explanation));
            OnPropertyChanged(nameof(SpeechPart));
        }
        else
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resource.Attention, Resource.WordNotFound);
        }
    }

    private void SaveDictionary(object obj)
    {
        UpdateListOfWords();
        FileHelper.AddWordsToDictionaryFile(_dictionaryFullPath, _wordEntities);
        ChooseDictionary(new TextBox { Text = _dictionary });
        ChoosenDictionaryColor = Brushes.LightGreen;
    }

    private void UpdateListOfWords()
    {
        var actwe = new WordEntity { Word = _word, Explanation = _explanation, SpeechPart = _speechPart };
        var existingWord = _wordEntities.Find(word => word.Word.Equals(_word));

        if (existingWord != null)
        {
            _wordEntities.Remove(existingWord);
        }

        _wordEntities.Add(actwe);
    }

    private void GetWordsStartValues(WordEntity? wordEntity)
    {
        if (wordEntity != null)
        {
            _word = wordEntity.Word;
            _explanation = wordEntity.Explanation;
            _speechPart = wordEntity.SpeechPart;
        }
    }

    private async void Synchronize(object obj)
    {
        Mouse.OverrideCursor = Cursors.Wait;

        var result = await _synchronize.CheckChangesInSource(_dictionaryDirPath);

        ShowIfSomethingNewInData(result);

        await _dialogCoordinator.ShowMessageAsync(this, result.WhatsUp, result.Info);

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    private void ShowIfSomethingNewInData(StateOfSynchronization state)
    {
        if (state.IsSomeIssue)
        {
            ShowAllDictonaries(null);
            CheckFtpSynchro(state.NewestFiles!, FtpAction.Download);
            CheckFtpSynchro(state.FilesToUpload!, FtpAction.Upload);
        }
    }


    private void CheckFtpSynchro(List<string> dictNames, FtpAction ftpAction = FtpAction.Ok)
    {
        var changedDictionaryNames = dictNames.Select(name => Path.GetFileName(name).ToLower()).ToList();

        if (changedDictionaryNames.Count == 0) return;

        foreach (var dictionary in _dictionaries)
        {
            if (changedDictionaryNames.Contains((dictionary.DictionaryName + ".json").ToLower()))
            {
                switch (ftpAction)
                {
                    case FtpAction.Upload:
                        dictionary.Upload = "Visible";
                        break;
                    case FtpAction.Download:
                        dictionary.Download = "Visible";
                        break;
                    case FtpAction.Ok:
                        dictionary.Ok = "Visible";
                        break;
                }
            }
        }

        Dictionaries = [.. _dictionaries];
        OnPropertyChanged(nameof(Dictionaries));
    }

    public async void ChooseDictionary(object obj)
    {
        var dictionaryName = ((TextBox)obj).Text;

        if (string.IsNullOrEmpty(dictionaryName))
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resource.Attention, Resource.NoChosenDictionary);
            return;
        }

        _dictionary = dictionaryName;
        CheckDictionary(_dictionary);
        EnabledButton = true;
        Hidden = "Visible";
        Words = [.. _wordEntities];
        OnPropertyChanged(nameof(Words));
    }

    private async void ShowAllDictonaries(object? obj)
    {
        _dictionaries = FileHelper.GetDictionaryFileNameToList(_dictionaryDirPath);

        if (_dictionaries.Count == 0)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resource.Attention, Resource.NoDirectoriesWriteName);
            return;
        }

        Dictionaries = [.. _dictionaries];
        OnPropertyChanged(nameof(Dictionaries));
        HiddenDictionary = "Visible";
    }

    private async void AddNewWordToDictionary(object obj)
    {
        if (string.IsNullOrEmpty(_word) || string.IsNullOrEmpty(_explanation) || string.IsNullOrEmpty(_speechPart))
        {
            await _dialogCoordinator.ShowMessageAsync(this,
                Resource.Attention, Resource.InsertAllNeededValue);
            return;
        }

        if (_wordEntities.Exists(word => word.Word.Equals(_word, StringComparison.OrdinalIgnoreCase)))
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resource.Attention, string.Format(Resource.WordAllreadyExists, _word.ToUpper()));
            return;
        }

        _wordEntities.Add(new WordEntity { Word = _word, Explanation = _explanation, SpeechPart = _speechPart });
        SaveDictionary(new TextBox { Text = _dictionary });
        ChoosenDictionaryColor = Brushes.Red;
    }

    private void Close(object obj)
    {
        if (_wordEntities != null && _wordEntities.Count > 0) _wordEntities.RemoveAt(0);
        (obj as Window)?.Close();
        if (_mainVM != null && _wordEntities != null) LocalDictionary.Dictionary = _wordEntities;
    }

    private async void CheckDictionary(string dictionary)
    {
        System.Media.SystemSounds.Beep.Play();

        dictionary = ReplaceBadCharInDictionaryName(dictionary);

        _dictionaryFullPath = _dictionaryDirPath + dictionary + ".json";
        _wordEntities = FileHelper.GetWordsFromDictionary(_dictionaryFullPath, out string message);
        if (message != string.Empty)
            await _dialogCoordinator.ShowMessageAsync(this, "Info", message);
    }

    private static string ReplaceBadCharInDictionaryName(string dictionary)
    {
        foreach (var chr in Path.GetInvalidFileNameChars())
        {
            dictionary = dictionary.Replace(chr, '_');
        }

        return dictionary;
    }

    //----INotifyPropertyChanged

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
