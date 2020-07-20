using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Entities;
using System.IO;
using LogicForUI;

namespace UI_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<CTask> Tasks;
        static private UILogic logic = new UILogic();
        public static User UserName;
        public MainWindow()
        {
            InitializeComponent();
            IdentifyUser identifyUserWindow = new IdentifyUser();
            if (identifyUserWindow.ShowDialog() == true)
            {
                UserName = logic.UiUserSupplier.GetUserByName(identifyUserWindow.UserName);
            }

            if (UserName == null)
            {
                System.Windows.Application.Current.Shutdown();
            }

            //Tasks = new ObservableCollection<Task>(UserName.TaskList);
            Tasks = new ObservableCollection<CTask>(logic.UiDataSupplier.GetUserData(UserName.Name));
            TaskList.ItemsSource = Tasks;

            SetTotalTime();
        }
        private void AddNewTaskButton(object sender, RoutedEventArgs e)
        {
            Entities.CTask newTask = new Entities.CTask();
            EnterNameWindow newTaskWindow = new EnterNameWindow("");

            if (newTaskWindow.ShowDialog() == true)
            {
                if (newTaskWindow.taskName.Text.Length > 0)
                {
                    newTask.TaskName = newTaskWindow.taskName.Text;
                    newTask.ConnectedUsers.Add(UserName.Name);
                    //newTask.SetId();
                    newTask.Id = logic.UiDataSupplier.CreateTask(newTask.TaskName, UserName.Name);
                    newTask.PredictedDuration = logic.GetTimePrediction(UserName.Name, newTask);
                    Tasks.Add(newTask);
                }
                else
                {
                    MessageBox.Show("Incorrect name");
                }
            }
        }

        private void SaveData(object sender, EventArgs e)
        {
            //logic.UiDataSupplier.SaveUserData(UserName.Name, new List<Task>(Tasks));
        }

        private void DeleteSelectedTask(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem == null || TaskList.SelectedItems.Count != 1)
            {
                TaskList.UnselectAll();
                return;
            }

            logic.UiScheduleSupplier.DeleteTaskFromSchedule(MainWindow.UserName, (CTask)TaskList.SelectedItem);
            logic.UiDataSupplier.DeleteTask(UserName, (CTask)TaskList.SelectedItem);
            Tasks.Remove((CTask)TaskList.SelectedItem);
            

            SetTotalTime();
        }

        private void EditSelectedTaskName(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem == null || TaskList.SelectedItems.Count != 1)
            {
                TaskList.UnselectAll();
                return;
            }

            CTask onEdit = (CTask)TaskList.SelectedItem;
            CTask old = new CTask((CTask)TaskList.SelectedItem);
            int index = TaskList.SelectedIndex;
            EnterNameWindow enterNameWindow = new EnterNameWindow(onEdit.TaskName);

            if (enterNameWindow.ShowDialog() == true)
            {
                if (enterNameWindow.taskName.Text.Length > 0)
                {
                    onEdit.TaskName = enterNameWindow.taskName.Text;

                    Tasks.Remove((CTask)TaskList.SelectedItem);
                    Tasks.Insert(index, onEdit);
                    logic.UiDataSupplier.UpdateTask(UserName, onEdit);
                    //logic.UiScheduleSupplier.RefreshTaskInSchedule(MainWindow.UserName, old, onEdit);
                }
                else
                {
                    MessageBox.Show("Incorrect name");
                }
            }
        }

        private void PredictSelectedTask(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem == null || TaskList.SelectedItems.Count != 1)
            {
                TaskList.UnselectAll();
                return;
            }

            CTask onEdit = (CTask)TaskList.SelectedItem;

            CTask old = new CTask((CTask)TaskList.SelectedItem);
            int index = TaskList.SelectedIndex;

            onEdit.PredictedDuration = logic.GetTimePrediction(UserName.Name, onEdit);

            Tasks.Remove((CTask)TaskList.SelectedItem);
            Tasks.Insert(index, onEdit);

            //logic.UiScheduleSupplier.RefreshTaskInSchedule(MainWindow.UserName, old, onEdit);
            SetTotalTime();
        }

        private void ActualDurationTask(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem == null || TaskList.SelectedItems.Count != 1)
            {
                TaskList.UnselectAll();
                return;
            }
            
            CTask onEdit = (CTask)TaskList.SelectedItem;
            CTask old = new CTask((CTask)TaskList.SelectedItem);
            int index = TaskList.SelectedIndex;
            EnterActualDuration durationWindow = new EnterActualDuration();

            if (durationWindow.ShowDialog() == true)
            { 
                onEdit.ActualDuration = durationWindow.ActualDuration;
                Tasks.Remove((CTask)TaskList.SelectedItem);
                //logic.UiScheduleSupplier.RefreshTaskInSchedule(MainWindow.UserName, old, onEdit);
                logic.UiDataSupplier.UpdateTask(UserName, onEdit);
                logic.UiDataSupplier.CompleteForAllUsers(onEdit);
            }
            SetTotalTime();
        }

        private void EditTagList(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem == null || TaskList.SelectedItems.Count != 1)
            {
                TaskList.UnselectAll();
                return;
            }

            CTask onEdit = (CTask)TaskList.SelectedItem;
            CTask old = new CTask((CTask)TaskList.SelectedItem);
            int index = TaskList.SelectedIndex;
            EditTags editTagWindow = new EditTags(onEdit.Tags);

            if (editTagWindow.ShowDialog() == true)
            {
                onEdit.Tags = editTagWindow.SettedTags;
                onEdit.PredictedDuration = logic.GetTimePrediction(UserName.Name, onEdit);
                logic.UiDataSupplier.UpdateTask(UserName, onEdit);

                Tasks.Remove((CTask)TaskList.SelectedItem);
                Tasks.Insert(index, onEdit);
                //logic.UiScheduleSupplier.RefreshTaskInSchedule(MainWindow.UserName, old, onEdit);
                SetTotalTime();
            }
        }

        private void SetTotalTime()
        {
            TotalTime.Text = "Total lead time: " + TotalPredictedTime();
        }

        private TimeSpan TotalPredictedTime()
        {
            TimeSpan time = new TimeSpan();
            foreach (CTask task in Tasks)
            {
                time += task.PredictedDuration;
            }
            return time;
        }

        private void Deselect(object sender, MouseButtonEventArgs e)
        {
            TaskList.UnselectAll();
        }

        private void CreateScheduleButton(object sender, RoutedEventArgs e)
        {
            Schedule scheduleWindow = new Schedule(new List<CTask>(Tasks));

            if (scheduleWindow.ShowDialog() == true)
            {

            }
        }

        private void EditUserList(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem == null || TaskList.SelectedItems.Count != 1)
            {
                TaskList.UnselectAll();
                return;
            }

            CTask onEdit = (CTask)TaskList.SelectedItem;
            CTask old = new CTask((CTask)TaskList.SelectedItem);
            int index = TaskList.SelectedIndex;
            EditUsers editUserWindow = new EditUsers(onEdit.ConnectedUsers);

            if (editUserWindow.ShowDialog() == true)
            {
                onEdit.ConnectedUsers = editUserWindow.SettedUsersOfTask;

                Tasks.Remove((CTask)TaskList.SelectedItem);
                Tasks.Insert(index, onEdit);

                //logic.UiScheduleSupplier.RefreshTaskInSchedule(MainWindow.UserName, old, onEdit);
                logic.UiDataSupplier.AddTaskToAllConnectedUsers(UserName, onEdit);
            }
        }
    }
}
