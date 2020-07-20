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
using Entities;
using LogicForUI;
using System.Collections.ObjectModel;

namespace UI_WPF
{
    /// <summary>
    /// Логика взаимодействия для EditTags.xaml
    /// </summary>
    public partial class EditTags : Window
    {
        HashSet<string> allPossibleTags;
        private UILogic logic;
        public List<Tag> SettedTags { get; private set; }
        private ObservableCollection<string> SettedTagNames;
        public ObservableCollection<string> couldBeAdded;
        public EditTags(List<Tag> current)
        {
            InitializeComponent();

            logic = new UILogic();
            allPossibleTags = new HashSet<string>(logic.UiDataSupplier.GetAllTags(MainWindow.UserName.Name));
            SettedTags = current;
            couldBeAdded = new ObservableCollection<string>(allPossibleTags);

            foreach(Tag t in current)
            {
                couldBeAdded.Remove(t.Name);
            }

            TagsCouldBeAdded.ItemsSource = couldBeAdded;
            currentTags.Text = GetCurrentTagListAsString();
            SettedTagNames = new ObservableCollection<string>();

            foreach(Tag t in SettedTags)
            {
                SettedTagNames.Add(t.Name);
            }

            TagsCouldBeDeleted.ItemsSource = SettedTagNames;

            couldBeAdded.Insert(0, "");
            SettedTagNames.Insert(0, "");
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private string GetCurrentTagListAsString()
        {
            StringBuilder sb = new StringBuilder("Tags: ");
            int i = 1;
            foreach(Tag tag in SettedTags)
            {
                sb.Append("#" + tag.Name + " ");
                i++;
                if (i == 4)
                {
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }


        private void AddTag(object sender, RoutedEventArgs e)
        {
            if (TagsCouldBeAdded.SelectedItem == null || TagsCouldBeAdded.SelectedIndex == 0)
            {
                return;
            }
            string selectedTag = TagsCouldBeAdded.SelectedItem.ToString();
            SettedTagNames.Add(selectedTag);
            SettedTags.Add(new Tag { Name = selectedTag });
            couldBeAdded.Remove(selectedTag);

            currentTags.Text = GetCurrentTagListAsString();
            TagsCouldBeAdded.SelectedIndex = 0;
        }

        private void DeleteTag(object sender, RoutedEventArgs e) //не обновляется выпадайка делитов
        {
            if (TagsCouldBeDeleted.SelectedItem == null || TagsCouldBeDeleted.SelectedIndex == 0)
            {
                return;
            }
            string selectedTag = TagsCouldBeDeleted.SelectedItem.ToString();
            foreach (Tag t in SettedTags)
            {
                if (t.Name == selectedTag)
                {
                    SettedTags.Remove(t);
                    break;
                }
            }
            SettedTagNames.Remove(selectedTag);
            couldBeAdded.Add(selectedTag);

            currentTags.Text = GetCurrentTagListAsString();
            TagsCouldBeDeleted.SelectedIndex = 0;
            
        }

        private void CreateTag(object sender, RoutedEventArgs e)
        {
            String newTag = logic.RefactorTagName(NewTagName.Text);
            
            foreach (string t in allPossibleTags)
            {
                if(t.Equals(newTag))
                {
                    MessageBox.Show("Tag already exists");
                    return;
                }
            }
            allPossibleTags.Add(newTag);
            couldBeAdded.Add(newTag);
            NewTagName.Text = "#You_can";
            MessageBox.Show("Tag created!");
        }
    }
}
