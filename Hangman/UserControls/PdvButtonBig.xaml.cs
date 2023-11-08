using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hangman.UserControls
{
    public partial class PdvButtonBig : UserControl
    {

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(PdvButton),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ButtonClickCommandProperty = DependencyProperty.Register(
            "ButtonClickCommand",
            typeof(ICommand),
            typeof(PdvButton),
            new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty ButtonClickCommandParameterProperty = DependencyProperty.Register(
            "ButtonClickCommandParameter",
            typeof(object),
            typeof(PdvButton),
            new PropertyMetadata(default(object)));

        public PdvButtonBig()
        {
            InitializeComponent();
        }

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set
            {
                SetValue(ButtonTextProperty, value);

                    BigLetter.Text = value[0].ToString();
                    BtnText.Text = value.ToString().Substring(1);
            }
        }

        public ICommand ButtonClickCommand
        {
            get { return (ICommand)GetValue(ButtonClickCommandProperty); }
            set { SetValue(ButtonClickCommandProperty, value); }
        }

        public object ButtonClickCommandParameter
        {
            get { return GetValue(ButtonClickCommandParameterProperty); }
            set { SetValue(ButtonClickCommandParameterProperty, value); }
        }
    }
}
