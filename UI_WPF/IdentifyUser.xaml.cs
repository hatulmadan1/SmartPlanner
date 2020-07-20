using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LogicForUI;


namespace UI_WPF
{
    /// <summary>
    /// Логика взаимодействия для IdentifyUser.xaml
    /// </summary>
    public partial class IdentifyUser : Window
    {
        private UILogic logic;
        public IdentifyUser()
        {
            InitializeComponent();
            logic = new UILogic();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (userName.Text.Length > 0)
            {
                logic.UiUserSupplier.CreateUserIfNotExists(userName.Text);
                this.DialogResult = true;
                return;
            }
            MessageBox.Show("Please, enter a login");
        }

        public string UserName
        {
            get { return userName.Text; }
        }
    }
}
