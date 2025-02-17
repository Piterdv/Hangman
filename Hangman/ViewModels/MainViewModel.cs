using Caliburn.Micro;
using CommunityToolkit.Mvvm.Input;
using CurrencyConverter.ViewModels;
using GetWordsAndExplanationFromWordnik;
using Hangman.Enums;
using Hangman.Helpers;
using Hangman.Models;
using Hangman.Views;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Hangman.ViewModels;

public class MainViewModel : ViewModelBase, INotifyPropertyChanged
{
    private const int MaxAttempessToGuessWord = 10;

    private readonly Random random = new();
    private static string _tmpWindowTitle = string.Empty;

    private BindableCollection<char> _guessingLetters = [];
    //private readonly List<string> _availaibleWord = [];
    private readonly List<int> _choosenIndexes = [];

    private string _gameStatus = Resource.ClickNewGame;
    private string _guessingWord = string.Empty;
    private string _wordExplanation = string.Empty;
    private string _partOfSpeach = string.Empty;
    private string _helpMeValue = Resource.HelpMe;
    private string _windowTitle = AppSettings.APP_NAME + Resource.WordsFromInternetInfo.ToUpper();

    private Brush _backgroundColor = Brushes.Transparent;
    private BitmapSource? _hangmanPicture;

    private int _fontSizeTB = 20;
    private int _wrongAttempts;
    private int _helpCounter = 0;
    private int _howManyWordsInHelp;
    private int _increment = 0;

    private bool _isGameOver = false;
    private bool _alphabetBtnEnable = true;
    private bool _newGameIsEnabled = true;
    private bool _internetOrLocalSource = true;

    private readonly IDialogCoordinator _dialogCoordinator;

    public MainViewModel(IDialogCoordinator instance)
    {
        NewGameCommand = new RelayCommand<object>(NewGame!);
        KeyClickedCommand = new RelayCommand<object>(KeyClicked!);
        HelpMeCommand = new RelayCommand<object>(HelpMeSuggestions!);
        ToggleAlphaQwertyCommand = new RelayCommand<object>(ToggleKeyboardToAlphaOrQwerty!);
        ChooseDictionaryCommand = new RelayCommand<object>(ChooseDictionary!);
        EditDictionaryCommand = new RelayCommand<object>(EditDictionary!);
        AddWordToDictionariesCommand = new RelayCommand<object>(AddWordToDictionaries!);
        InternetOrLocalDictionaryCommand = new RelayCommand<object>(GetWordsFromInternetOrLocalFile!);
        _dialogCoordinator = instance;
    }

    //--------------------------properties

    public ICommand NewGameCommand { get; set; }
    public ICommand KeyClickedCommand { get; set; }
    public ICommand HelpMeCommand { get; set; }
    public ICommand ToggleAlphaQwertyCommand { get; set; }
    public ICommand ChooseDictionaryCommand { get; set; }
    public ICommand EditDictionaryCommand { get; set; }
    public ICommand InternetOrLocalDictionaryCommand { get; set; }
    public ICommand AddWordToDictionariesCommand { get; set; }


    public string WindowTitle
    {
        get { return _windowTitle; }
        set
        {
            _windowTitle = value;
            OnPropertyChanged();
        }
    }

    public string HelpMeValue
    {
        get { return _helpMeValue; }
        set
        {
            _helpMeValue = value;
            OnPropertyChanged();
        }
    }

    public BitmapSource? HangmanPicture
    {
        get { return _hangmanPicture; }
        set
        {
            _hangmanPicture = value;
            OnPropertyChanged();
        }
    }

    public Brush BackgroundColor
    {
        get { return _backgroundColor; }
        set
        {
            _backgroundColor = value;
            OnPropertyChanged();
        }
    }

    public string GameStatus
    {
        get { return _gameStatus; }
        set
        {
            _gameStatus = value;
            OnPropertyChanged();
        }
    }

    public bool AlphabetBtnEnable
    {
        get { return _alphabetBtnEnable; }
        set
        {
            _alphabetBtnEnable = value;
            OnPropertyChanged();
        }
    }

    public int FontSizeTB
    {
        get { return _fontSizeTB; }
        set
        {
            _fontSizeTB = value;
            OnPropertyChanged();
        }
    }

    public bool NewGameIsEnabled
    {
        get { return _newGameIsEnabled; }
        set
        {
            _newGameIsEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool InternetOrLocalSource
    {
        get { return _internetOrLocalSource; }
        set
        {
            _internetOrLocalSource = value;
            OnPropertyChanged();
        }
    }

    public BindableCollection<char> GuessingLetters
    {
        get { return _guessingLetters; }
        set
        {
            _guessingLetters = value;
            OnPropertyChanged();
        }
    }


    //--------------------------methods

    private void NewGame(object keyboardGrid)
    {
        EnableKeyboard(keyboardGrid);
        NewRandomWord();
        if (_wordExplanation.Contains("Wait a moment"))
        {
            CounterNeededForWodnikFreeAccount();
            return;
        }
        InitializeGame();
    }

    private static void EnableKeyboard(object keyboardGrid)
    {
        if (keyboardGrid is Grid grid)
        {
            EnableKeyboard(grid);
        }
    }

    private void InitializeGame()
    {
        InitBoard();
        _isGameOver = false;
        _wrongAttempts = 0;
        UpdateImage();
        GameStatus = Resource.ClickLetterToGuess;
        BackgroundColor = Brushes.Transparent;
        _howManyWordsInHelp = 0;
        HelpMeValue = "HELP ME, please...";
        _helpCounter = 0;
        FontSizeTB = 20;
        AlphabetBtnEnable = true;
    }

    private void EditDictionary(object obj)
    {
        DictionaryWindow dictionaryWindow = new(null, null);
        dictionaryWindow.ShowDialog();
    }

    private void ChooseDictionary(object? obj)
    {
        _choosenIndexes.Clear();
        DictionaryWindow dictionaryWindow = new(null, this);
        dictionaryWindow.ShowDialog();
    }

    private void AddWordToDictionaries(object obj)
    {
        WordEntity wordEntity = new()
        {
            Word = _guessingWord,
            Explanation = _wordExplanation,
            SpeechPart = _partOfSpeach
        };

        DictionaryWindow dictionaryWindow = new(wordEntity, null);
        dictionaryWindow.ShowDialog();
    }

    private void ToggleKeyboardToAlphaOrQwerty(object obj)
    {
        if (obj == null) return;

        for (int i = 0; i < 26; i++)
        {
            Button? button = ((Grid)obj).Children.Cast<StackPanel>()
                            .SelectMany(x => x.Children.Cast<Button>())
                            .FirstOrDefault(x => x.Name == $"_{i}");
            if (button != null)
            {
                button.Content = button.Content.ToString() == ((Alphabet)i).ToString() ? ((Qwerty)i).ToString() : ((Alphabet)i).ToString();
            }
        }
    }

    private void GetWordsFromInternetOrLocalFile(object obj)
    {
        if (_windowTitle.Contains('(')) _tmpWindowTitle = _windowTitle;

        if (!_internetOrLocalSource)
        {
            if (_tmpWindowTitle != string.Empty)
                WindowTitle = _tmpWindowTitle;
            else
                WindowTitle = AppSettings.APP_NAME + Resource.ChooseDictionary;
        }
        else
        {
            WindowTitle = AppSettings.APP_NAME + Resource.WordsFromInternetInfo;
        }
    }

    private void HelpMeSuggestions(object obj)
    {
        if (string.IsNullOrEmpty(_guessingWord)) return;

        _helpCounter++;
        switch (_helpCounter)
        {
            case 1:
                SetProperSizeOfFontInHelp(_wordExplanation);
                GameStatus = _wordExplanation;
                break;
            case 2:
                GameStatus = _wordExplanation + $" ({_partOfSpeach})";
                break;
            default:
                HelpMeValue = GetAPartOfGuessingWord();
                break;
        }
    }

    private void SetProperSizeOfFontInHelp(string input, int max = 80)
    {
        FontSizeTB = input.Length < max ? 20 : 20 * max / input.Length >= 15 ? 20 * max / input.Length : 15;
    }

    private string GetAPartOfGuessingWord()
    {
        if (_howManyWordsInHelp < _guessingWord.Length) _howManyWordsInHelp++;

        return _guessingWord[.._howManyWordsInHelp];
    }

    private void KeyClicked(object clickedButton)
    {
        if (_isGameOver)
        {
            EnableAlphabetButtons();
            return;
        }

        DisableClickedButton(clickedButton);
        char chosenKey = GetChosenKey(clickedButton);

        if (!IsKeyInGuessingWord(chosenKey))
        {
            HandleWrongAttempt();
            return;
        }

        UpdateGuessingLetters(chosenKey);

        if (IsWordGuessed())
        {
            HandleWin();
        }
    }

    private void EnableAlphabetButtons()
    {
        AlphabetBtnEnable = true;
    }

    private void DisableClickedButton(object clickedButton)
    {
        ((Button)clickedButton).IsEnabled = false;
        if (_alphabetBtnEnable) AlphabetBtnEnable = false;
    }

    private static char GetChosenKey(object clickedButton)
    {
        return Convert.ToChar(((Button)clickedButton).Content);
    }

    private bool IsKeyInGuessingWord(char chosenKey)
    {
        return _guessingWord.Contains(chosenKey);
    }

    private void HandleWrongAttempt()
    {
        _wrongAttempts++;
        UpdateImage();
        if (_wrongAttempts == MaxAttempessToGuessWord)
        {
            _isGameOver = true;
            GameStatus = string.Format(Resource.LostGame, _guessingWord);
            BackgroundColor = Brushes.Red;
        }
    }

    private void UpdateGuessingLetters(char chosenKey)
    {
        for (int i = 0; i < _guessingWord.Length; i++)
        {
            if (_guessingWord[i] == chosenKey)
                GuessingLetters[i] = chosenKey;
        }
    }

    private bool IsWordGuessed()
    {
        return !GuessingLetters.Contains('\0');
    }

    private void HandleWin()
    {
        _isGameOver = true;
        GameStatus = Resource.WinGame;
        BackgroundColor = Brushes.DarkGoldenrod;
    }

    private void UpdateImage()
    {
        HangmanPicture =
            new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, "Images", $"{_wrongAttempts}_mistake.png")));

    }

    private void InitBoard()
    {
        _guessingLetters.Clear();

        foreach (var item in _guessingWord)
        {
            if (item == ' ')
            {
                _guessingLetters.Add(' ');
                continue;
            }

            //dodajemy obramowanie dla niepustych
            _guessingLetters.Add('\0');
        }
    }

    private void NewRandomWord()
    {
        if (!_internetOrLocalSource)
        {
            GetWordAndExplFromLocalDictionaries();
        }
        else
        {
            GetWordAndExplFromWordnik();
        }
    }

    private async void GetWordAndExplFromLocalDictionaries()
    {
        int maxIx = LocalDictionary.Dictionary.Count;
        if (maxIx == 0)
        {
            await this._dialogCoordinator.ShowMessageAsync(this, Resource.Attention, Resource.NoDictionarySelected);
            ChooseDictionary(null);
            return;
        }

        int randomIx;
        bool repeat;
        do
        {
            if (maxIx == _choosenIndexes.Count)
            {
                if (await this._dialogCoordinator.ShowMessageAsync(this, Resource.Attention, Resource.AllWordsDrawn, MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                    _choosenIndexes.Clear();
                else
                    return;
            }
            randomIx = random.Next(maxIx);
            if (!_choosenIndexes.Contains(randomIx))
            {
                _choosenIndexes.Add(randomIx);
                repeat = false;
            }
            else
            {
                repeat = true;
            }

        } while (repeat);

        _guessingWord = LocalDictionary.Dictionary[randomIx].Word.ToUpper();
        _wordExplanation = LocalDictionary.Dictionary[randomIx].Explanation;
        _partOfSpeach = LocalDictionary.Dictionary[randomIx].SpeechPart;

    }

    private void GetWordAndExplFromWordnik()
    {
        (string Word, string Text, string PartOfS) wordnikEntity = (string.Empty, string.Empty, string.Empty);

        Mouse.OverrideCursor = Cursors.Wait;

        _wordExplanation = Resource.FetchingWord;
        GameStatus = _wordExplanation;

        Task task = Task.Run(() =>
        {
            WordnikRetriver getWordAndExpl = new();
            var wordAndExplanation = getWordAndExpl.GetExplanation();

            wordnikEntity = (wordAndExplanation.Word, wordAndExplanation.Text[0], wordAndExplanation.PartOfSpeech);

        });
        task.Wait();

        if (wordnikEntity.Word.Contains("ErrorTMR"))
        {
            _guessingWord = "?";
            _wordExplanation = Resource.WaitForWord;
            _partOfSpeach = "?";
            GameStatus = _wordExplanation;
        }
        else
        {
            _guessingWord = wordnikEntity.Word.ToUpper();
            _wordExplanation = wordnikEntity.Text;
            _partOfSpeach = wordnikEntity.PartOfS;
        }

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    void CounterNeededForWodnikFreeAccount()
    {
        Mouse.OverrideCursor = Cursors.No;
        NewGameIsEnabled = false;
        DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        string incrementToString = (60 - _increment++).ToString();
        incrementToString = incrementToString.Length == 2 ? incrementToString : "0" + incrementToString;
        _guessingWord = incrementToString;
        if (_increment == 1) InitBoard();
        MakeCounter(incrementToString);
        MakeCounter(incrementToString, 1);
        if (_increment >= 60)
        {
            _increment = 0;
            ((DispatcherTimer)sender!).Stop();
            _guessingWord = "OK";
            _wordExplanation = Resource.WaitOver;
            GameStatus = _wordExplanation;
            Mouse.OverrideCursor = Cursors.Arrow;
            NewGameIsEnabled = true;
        }
    }

    private void MakeCounter(string its, int frst = 0)
    {
        char choosenKey = Convert.ToChar(its.Substring(frst, 1));
        for (int i = 0; i < _guessingWord.Length; i++)
        {
            if (_guessingWord[i] == choosenKey)
                GuessingLetters[i] = choosenKey;
        }
    }

    private static void EnableKeyboard(Grid grid)
    {
        var stackPannelsKeyboard = grid.Children;

        foreach (StackPanel stackPanel in stackPannelsKeyboard)
            foreach (Button button in stackPanel.Children)
                button.IsEnabled = true;
    }
}
