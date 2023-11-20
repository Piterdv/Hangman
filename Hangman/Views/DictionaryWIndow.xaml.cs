using Hangman.Models;
using Hangman.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace Hangman.Views
{
    public partial class DictionaryWindow
    {
        public DictionaryWindow(WordEntity? we, MainViewModels? mvm)
        {
            InitializeComponent();
            DataContext = new DictionaryViewModels(we, mvm, DialogCoordinator.Instance);
        }

    }
}
