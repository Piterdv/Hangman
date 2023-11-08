using Hangman.Commands;
using Hangman.Helpers;
using Hangman.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Hangman.ViewModels
{
    public class MainViewModels : INotifyPropertyChanged
    {
        private const int MaxAttempessToGuessWord = 10;
        private ObservableCollection<char> _guessingLetters = new();
        private string _gameStatus = "Click on button \"NEW GAME\"...";
        private Brush _backgroundColor = Brushes.Transparent;
        private BitmapSource? _hangmanPicture;
        private List<String> _availaibleWord = new List<string>();
        private Dictionary<string, string> _availaibleWordWithExplanation = new Dictionary<string, string>();
        private readonly Random random = new Random();
        private string _guessingWord = string.Empty;
        private string _wordExplanation = string.Empty;
        private string _partOfSpeach = string.Empty;
        private int _fontSizeTB = 20;
        private int _wrongAttempts;
        private bool _isGameOver = false;
        private string _helpMeValue = "HELP ME, please...";
        private int _helpCounter = 0;
        private int _howManyWordsInHelp;
        private bool _alphabetBtnEnable = true;
        private int _increment = 0;

        public MainViewModels()
        {
            NewGameCommand = new RelayCommand(NewGame);
            KeyClickedCommand = new RelayCommand(KeyClicked);
            HelpMeCommand = new RelayCommand(HelpMe);
            ToggleAlphaQwertyCommand = new RelayCommand(ToggleAlphaQwerty);
            ChooseDictionaryCommand = new RelayCommand(ChooseDictionary);

            //GetAvailableWordsFromFile();
        }

        private void ChooseDictionary(object obj)
        {

            //System.Media.SystemSounds.Beep.Play();
            DictionaryWindow dictionaryWindow = new DictionaryWindow();
            dictionaryWindow.ShowDialog();
        }

        private void ToggleAlphaQwerty(object obj)
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

        private void HelpMe(object obj)
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

        private void SetProperSizeOfFontInHelp(string str, int max = 80)
        {
            FontSizeTB = str.Length < max ? 20 : 20 * max / str.Length >= 15 ? 20 * max / str.Length : 15;
        }

        private string GetAPartOfGuessingWord()
        {
            if (_howManyWordsInHelp < _guessingWord.Length) _howManyWordsInHelp++;

            return _guessingWord.Substring(0, _howManyWordsInHelp);
        }

        private void GetAvailableWordsFromFile()
        {
            if (!FileHelpers.FileExists())
                FileHelpers.CreateNewFile();

            _availaibleWordWithExplanation = FileHelpers.GetAwailableWordsWithExplanation();
            _availaibleWord = _availaibleWordWithExplanation.Keys.Select(x => x).ToList<string>();
        }

        public ICommand NewGameCommand { get; set; }
        public ICommand KeyClickedCommand { get; set; }
        public ICommand HelpMeCommand { get; set; }
        public ICommand ToggleAlphaQwertyCommand { get; set; }
        public ICommand ChooseDictionaryCommand { get; set; }


        private void KeyClicked(object clickedButton)
        {
            if (_isGameOver)
            {
                AlphabetBtnEnable = true;
                return;
            }

            ((Button)clickedButton).IsEnabled = false;
            if (_alphabetBtnEnable) AlphabetBtnEnable = false;

            char choosenKey = Convert.ToChar(((Button)clickedButton).Content);

            var keyExistsInGuessingWord = _guessingWord.Contains(choosenKey);

            if (!keyExistsInGuessingWord)
            {
                _wrongAttempts++;
                UpdateImage();
                if (_wrongAttempts == MaxAttempessToGuessWord)
                {
                    _isGameOver = true;
                    GameStatus = $"Przegrałeś. Prawidłowe hasło to: {_guessingWord}";
                    BackgroundColor = Brushes.Red;
                }
                return;
            }

            for (int i = 0; i < _guessingWord.Length; i++)
            {
                if (_guessingWord[i] == choosenKey)
                    GuessingLetters[i] = choosenKey;
            }

            if (!GuessingLetters.Contains('\0'))
            {
                _isGameOver = true;
                GameStatus = "You are the winner, congratulations :)";
                BackgroundColor = Brushes.DarkGoldenrod;
            }
        }

        private void NewGame(object keyboardGrid)
        {
            EnableKeyboard(keyboardGrid as Grid);
            NewRandomWord();
            if (_wordExplanation.Contains("Wait a moment"))
            {
                Counter();
                return;
            }
            InitBoard();
            _isGameOver = false;
            _wrongAttempts = 0;
            UpdateImage();
            GameStatus = "Click the choosen letter to guess the entry";
            BackgroundColor = Brushes.Transparent;
            _howManyWordsInHelp = 0;
            HelpMeValue = "HELP ME, please...";
            _helpCounter = 0;
            FontSizeTB = 20;
            AlphabetBtnEnable = true;
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
            //var randomIx = random.Next(_availaibleWord.Count);
            //_guessingWord = _availaibleWord[randomIx];
            //_wordExplanation = _availaibleWordWithExplanation[_guessingWord];

            GetWordAndExplFromWordnik();
        }

        private void GetWordAndExplFromWordnik()
        {
            (string Word, string Text, string PartOfS) wt = (string.Empty, string.Empty, string.Empty);

            Mouse.OverrideCursor = Cursors.Wait;

            _wordExplanation = "Uno momento, I need to take some word from server...";
            GameStatus = _wordExplanation;

            Task task = Task.Run(() =>
            {
                GetWordAndExpl getWordAndExpl = new GetWordAndExpl();
                var wae = getWordAndExpl.GetWAndE();

                wt = (wae.Word, wae.Text[0], wae.PartOfSpeech);

            });
            task.Wait();

            if (wt.Word.Contains("ErrorTMR"))
            {
                _guessingWord = "?";
                _wordExplanation = "Wait a moment, because you try to get too many words is short period of time. Wait 60s ;)";
                _partOfSpeach = "?";
                GameStatus = _wordExplanation;
            }
            else
            {
                _guessingWord = wt.Word.ToUpper();
                _wordExplanation = wt.Text;
                _partOfSpeach = wt.PartOfS;
            }

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        void Counter()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string its = (60 - _increment++).ToString();
            its = its.Length == 2 ? its : "0" + its;
            _guessingWord = its;
            if (_increment == 1) InitBoard();
            MakeCounter(its);
            MakeCounter(its, 1);
            if (_increment >= 60)
            {
                _increment = 0;
                ((DispatcherTimer)sender).Stop();
                _guessingWord = "OK";
                _wordExplanation = "Oh yeah, now it's OK:) Click on button \"NEW GAME\"!";
                GameStatus = _wordExplanation;
                Mouse.OverrideCursor = Cursors.Arrow;
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

        private void EnableKeyboard(Grid? grid)
        {
            var stackPannelsKeyboard = grid.Children;

            foreach (StackPanel stackPanel in stackPannelsKeyboard)
                foreach (Button button in stackPanel.Children)
                    button.IsEnabled = true;
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

        public BitmapSource HangmanPicture
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

        public ObservableCollection<char> GuessingLetters
        {
            get { return _guessingLetters; }
            set
            {
                _guessingLetters = value;
                OnPropertyChanged();
            }
        }

        //------------------implementacja interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
