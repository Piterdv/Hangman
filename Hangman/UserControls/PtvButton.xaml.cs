using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hangman.UserControls
{
    /// <summary>
    /// Interaction logic for PtvButton.xaml
    /// </summary>
    public partial class PtvButton : UserControl
    {
        public PtvButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("Text",
            typeof(string),
            typeof(PtvButton),
            new PropertyMetadata(default(string))); //było "" zamiast default(string)

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

        public static readonly DependencyProperty ButtonClickCommandProperty = DependencyProperty.Register(
        "ButtonClickCommand", typeof(ICommand), typeof(PtvButton), new PropertyMetadata(default(ICommand)));

        public ICommand ButtonClickCommand
        {
            get { return (ICommand)GetValue(ButtonClickCommandProperty); }
            set { SetValue(ButtonClickCommandProperty, value); }
        }


        //public static readonly DependencyProperty MyCommandProperty = DependencyProperty.Register("MyCommand",
        //    typeof(ICommand),
        //    typeof(PtvButton),
        //    new PropertyMetadata(default(ICommand)));

        //public ICommand MyCommand
        //{
        //    get { return (ICommand)GetValue(MyCommandProperty); }
        //    set { SetValue(MyCommandProperty, value); }
        //}

        ////add method to handle MyCommandParameter
        //public static readonly DependencyProperty MyCommandParameterProperty = DependencyProperty.Register("MyCommandParameter",
        //               typeof(object),
        //                          typeof(PtvButton),
        //                                     new PropertyMetadata(default(object)));
        //public object MyCommandParameter
        //{
        //    get { return GetValue(MyCommandParameterProperty); }
        //    set { SetValue(MyCommandParameterProperty, value); }
        //}

    }
}
