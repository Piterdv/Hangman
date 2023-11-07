using Hangman.ViewModels;

namespace Hangman.Views
{
    public partial class DictionaryWindow
    {
        public DictionaryWindow()
        {
            InitializeComponent();
            DataContext = new DictionaryViewModels();
        }
    }
}
