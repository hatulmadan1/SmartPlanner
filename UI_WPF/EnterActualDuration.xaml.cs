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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using LogicForUI;

namespace UI_WPF
{
    /// <summary>
    /// Логика взаимодействия для EnterActualDuration.xaml
    /// </summary>
    public partial class EnterActualDuration : Window
    {
        private UILogic logic;
        public EnterActualDuration()
        {
            InitializeComponent();
            logic = new UILogic();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (logic.CheckInputIsCorrectTime(hours.Text, minutes.Text, seconds.Text))
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Time is not correct, please, reenter it.");
            }
        }

        public TimeSpan ActualDuration
        {
            get { return logic.FormatTime(hours.Text, minutes.Text, seconds.Text); }
        }
    }
}
