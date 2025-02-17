using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hangman.UserControls;

public partial class PdvButton : UserControl
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

    public PdvButton()
    {
        InitializeComponent();
    }

    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set
        {
            SetValue(ButtonTextProperty, value);

            SmL1.Text = "";
            SmL2.Text = "";
            SmL3.Text = "";

            if (value.Length > 1)
            {
                BigLetter.Text = value[0].ToString();
                SmL1.Text = value[1].ToString();
                if (value.Length > 2) SmL2.Text = value[2].ToString();
                if (value.Length > 3) SmL3.Text = value[3].ToString();
            }
            else
            {
                BigLetter.Text = value;
            }
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
