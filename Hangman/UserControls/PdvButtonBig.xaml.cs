using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hangman.UserControls;

public partial class PdvButtonBig : UserControl
{

    public static readonly DependencyProperty ButtonBigTextProperty = DependencyProperty.Register(
        "TextBig",
        typeof(string),
        typeof(PdvButtonBig),
        new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ButtonBigClickCommandProperty = DependencyProperty.Register(
        "ButtonBigClickCommand",
        typeof(ICommand),
        typeof(PdvButtonBig),
        new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty ButtonBigClickCommandParameterProperty = DependencyProperty.Register(
        "ButtonBigClickCommandParameter",
        typeof(object),
        typeof(PdvButtonBig),
        new PropertyMetadata(default(object)));

    public PdvButtonBig()
    {
        InitializeComponent();
    }

    public string ButtonBigText
    {
        get { return (string)GetValue(ButtonBigTextProperty); }
        set
        {
            SetValue(ButtonBigTextProperty, value);

                BigLetter.Text = value[0].ToString();
                BtnText.Text = value.ToString().Substring(1);
        }
    }

    public ICommand ButtonBigClickCommand
    {
        get { return (ICommand)GetValue(ButtonBigClickCommandProperty); }
        set { SetValue(ButtonBigClickCommandProperty, value); }
    }

    public object ButtonBigClickCommandParameter
    {
        get { return GetValue(ButtonBigClickCommandParameterProperty); }
        set { SetValue(ButtonBigClickCommandParameterProperty, value); }
    }
}
