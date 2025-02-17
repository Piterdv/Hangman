using Hangman.Models;
using Hangman.SynchronizeSource;
using Hangman.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace Hangman.Views;

public partial class DictionaryWindow
{
    public DictionaryWindow(WordEntity? wordEntity, MainViewModel? mainVM)
    {
        InitializeComponent();

        ISynchronizeResource synchronizeSource = new SynchronizeFtp();
        DataContext = new DictionaryViewModel(wordEntity, mainVM!, DialogCoordinator.Instance, synchronizeSource);
    }
}
