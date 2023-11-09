using Hangman.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Hangman.ViewModels
{
    public class DictionaryViewModels : INotifyPropertyChanged
    {

        public DictionaryViewModels()
        {
            CreateNewDictionaryCommand= new RelayCommand(CreateNewDictionary);
            CloseCommand = new RelayCommand(Close);
        }

        private void Close(object obj)
        {
            (obj as Window)?.Close();
        }

        private void CreateNewDictionary(object obj)
        {
            System.Media.SystemSounds.Beep.Play();
        }

        public ICommand CreateNewDictionaryCommand { get; set; }
        public ICommand CloseCommand { get; set; }


        //------------------implementacja interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
