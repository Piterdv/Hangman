using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CurrencyConverter.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isChanged;

        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                if (_isChanged != value)
                {
                    _isChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName != nameof(IsChanged))
            {
                IsChanged = false;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
