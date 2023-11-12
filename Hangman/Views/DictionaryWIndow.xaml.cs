using Hangman.Models;
using Hangman.ViewModels;

namespace Hangman.Views
{
    public partial class DictionaryWindow
    {
        public DictionaryWindow(WordEntity? we)
        {
            InitializeComponent();
            DataContext = new DictionaryViewModels(we);
        }

    }
}
