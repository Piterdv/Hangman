using Hangman.ViewModels;

namespace Hangman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow //nie musi być : MetroWindow:)
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModels();
        }

    }
}
