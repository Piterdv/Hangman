using Hangman.Commands;
using Hangman.Helpers;
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
    public class DictionaryViewModels : INotifyPropertyChanged
    {

        public DictionaryViewModels()
        {
            CreateNewDictionaryCommand= new RelayCommand(CreateNewDictionary);
        }

        private void CreateNewDictionary(object obj)
        {
            System.Media.SystemSounds.Beep.Play();
        }

        public ICommand CreateNewDictionaryCommand { get; set; }

        //------------------implementacja interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
