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
    /// Логика взаимодействия для Schedule.xaml
    /// </summary>
    public partial class Schedule : Window
    {
        private ObservableCollection<CTask> scheduledTasks;
        UILogic logic;
        private List<CTask> allTasksOfUser;
        private List<CTask> notInSchedule;
        public Schedule(List<CTask> allTasks)
        {
            InitializeComponent();
            logic = new UILogic();
            scheduledTasks = new ObservableCollection<CTask>(logic.UiScheduleSupplier.GetSchedule(MainWindow.UserName.Name));
            allTasksOfUser = allTasks;

            schedule.ItemsSource = scheduledTasks;
            notInSchedule = new List<CTask>();

            SetTotalTime();
        }

        private void FillNotInSchedule()
        {
            notInSchedule = new List<CTask>(allTasksOfUser);
            foreach (CTask t in allTasksOfUser)
            {
                foreach(CTask tt in scheduledTasks)
                {
                    if (t.Id == tt.Id)
                    {
                        notInSchedule.Remove(t);
                        break;
                    }
                }
            }
        }
        private TimeSpan TotalScheduledTime()
        {
            TimeSpan time = new TimeSpan();
            foreach (CTask task in scheduledTasks)
            {
                time += task.PredictedDuration;
            }
            return time;
        }
        private void SetTotalTime()
        {
            TotalTime.Text = "Total lead time: " + TotalScheduledTime();
        }

        private void CreateScheduleButton(object sender, RoutedEventArgs e)
        {
            ScheduleDuration durationWindow = new ScheduleDuration();
            scheduledTasks.Clear();            

            if (durationWindow.ShowDialog() == true)
            {
                var newSchedule = logic.UiScheduleSupplier.GetPredictedSchedule(MainWindow.UserName,
                    durationWindow._ScheduleDuration, allTasksOfUser);
                foreach(CTask t in newSchedule)
                {
                    scheduledTasks.Add(t);
                }
            }
            SetTotalTime();
        }

        private void AddTasksToScheduleButton(object sender, RoutedEventArgs e)
        {
            FillNotInSchedule();

            EditSchedule editSchedule = new EditSchedule(notInSchedule);

            if (editSchedule.ShowDialog() == true)
            {
                foreach(CTask t in editSchedule.SelectedTasks)
                {
                    scheduledTasks.Add(t);
                    logic.UiScheduleSupplier.AddTaskToSchedule(MainWindow.UserName, t);
                }
            }

            SetTotalTime();
        }

        private void DeleteTaskFromScheduleButton(object sender, RoutedEventArgs e)
        {
            if (schedule.SelectedItem == null || schedule.SelectedItems.Count != 1)
            {
                schedule.UnselectAll();
                return;
            }

            logic.UiScheduleSupplier.DeleteTaskFromSchedule(MainWindow.UserName, (CTask)schedule.SelectedItem);
            logic.UiScheduleSupplier.ScheduleToSomeoneOther(MainWindow.UserName.Name, (CTask)schedule.SelectedItem);
            scheduledTasks.Remove((CTask)schedule.SelectedItem);
            SetTotalTime();
        }

        private void CompleteTaskButton(object sender, RoutedEventArgs e)
        {
            if (schedule.SelectedItem == null || schedule.SelectedItems.Count != 1)
            {
                schedule.UnselectAll();
                return;
            }

            CTask onEdit = (CTask)schedule.SelectedItem;
            CTask old = new CTask((CTask)schedule.SelectedItem);
            int index = schedule.SelectedIndex;
            EnterActualDuration durationWindow = new EnterActualDuration();

            if (durationWindow.ShowDialog() == true)
            {
                MainWindow.Tasks.Remove((CTask)schedule.SelectedItem);
                onEdit.ActualDuration = durationWindow.ActualDuration;
                scheduledTasks.Remove((CTask)schedule.SelectedItem);
                logic.UiScheduleSupplier.DeleteTaskFromSchedule(MainWindow.UserName, onEdit);
                logic.UiDataSupplier.UpdateTask(MainWindow.UserName, onEdit);
                logic.UiDataSupplier.CompleteForAllUsers(onEdit);
            }
            SetTotalTime();
        }
    }
}
