using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Entities;
using LogicForUI;

namespace UI_WPF
{
    /// <summary>
    /// Логика взаимодействия для EditUsers.xaml
    /// </summary>
    public partial class EditUsers : Window
    {
        HashSet<string> allPossibleUsers;
        private UILogic logic;
        public List<string> SettedUsersOfTask
        {
            get
            {
                SettedUsers.Remove("");
                return new List<string>(SettedUsers);
            }
        }
        private ObservableCollection<string> SettedUsers;
        public ObservableCollection<string> couldBeAdded;
        public EditUsers(List<string> current)
        {
            InitializeComponent();

            logic = new UILogic();
            allPossibleUsers = new HashSet<string>(logic.UiUserSupplier.GetAllUsers());
            SettedUsers = new ObservableCollection<string>(current);

            couldBeAdded = new ObservableCollection<string>(allPossibleUsers);
            foreach (string user in current)
            {
                couldBeAdded.Remove(user);
            }
            UsersCouldBeAdded.ItemsSource = couldBeAdded;

            currentUsers.Text = GetCurrentUserListAsString();

            UsersCouldBeDeleted.ItemsSource = SettedUsers;

            couldBeAdded.Insert(0, "");
            SettedUsers.Insert(0, "");
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private string GetCurrentUserListAsString()
        {
            StringBuilder sb = new StringBuilder("Users: ");
            int i = 1;
            foreach (string user in SettedUsers)
            {
                if (user == "")
                {
                    continue;
                }
                sb.Append(user + "; ");
                i++;
                if (i == 4)
                {
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }


        private void AddUser(object sender, RoutedEventArgs e)
        {
            if (UsersCouldBeAdded.SelectedItem == null || UsersCouldBeAdded.SelectedIndex == 0)
            {
                return;
            }
            string selectedUser = UsersCouldBeAdded.SelectedItem.ToString();
            SettedUsers.Add(selectedUser);
            couldBeAdded.Remove(selectedUser);


            currentUsers.Text = GetCurrentUserListAsString();
            UsersCouldBeAdded.SelectedIndex = 0;
        }

        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            if (UsersCouldBeDeleted.SelectedItem == null || UsersCouldBeDeleted.SelectedIndex == 0)
            {
                return;
            }
            string selectedUser = UsersCouldBeDeleted.SelectedItem.ToString();
            foreach (string user in SettedUsers)
            {
                if (user.Equals(selectedUser))
                {
                    SettedUsers.Remove(selectedUser);
                    break;
                }
            }
            couldBeAdded.Add(selectedUser);

            currentUsers.Text = GetCurrentUserListAsString();
            UsersCouldBeDeleted.SelectedIndex = 0;
        }
    }
}
