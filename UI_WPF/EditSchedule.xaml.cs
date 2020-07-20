using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace UI_WPF
{
    /// <summary>
    /// Логика взаимодействия для EditSchedule.xaml
    /// </summary>
    public partial class EditSchedule : Window
    {
        private ObservableCollection<CTask> avaliable;
        UILogic logic;
        public EditSchedule(List<CTask> avaliableTasks)
        {
            InitializeComponent();
            avaliable = new ObservableCollection<CTask>(avaliableTasks);
            tasks.ItemsSource = avaliable;
            logic = new UILogic();
            SelectedTasks = new List<CTask>();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            foreach(object o in tasks.SelectedItems)
            {
                SelectedTasks.Add((CTask)o);
            }
            
            this.DialogResult = true;
        }

        public List<CTask> SelectedTasks
        {
            get;
            private set;
        }
    }
}
