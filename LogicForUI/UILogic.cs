using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BusinessLogic;
using DBManager;
using EFDataManager;
using Entities;
using Newtonsoft.Json;
using Server;

namespace LogicForUI
{
    public class UILogic
    {
        private ITimePredictor TimePredictor;
        public UIDataSupplier UiDataSupplier { get; }
        public UIUserSupplier UiUserSupplier { get; }
        public UIScheduleSupplier UiScheduleSupplier { get; }

        public UILogic()
        {
            UiDataSupplier = new UIDataSupplier();
            TimePredictor = new TimePredictorImpl(new TaskControllerEF());
            UiUserSupplier = new UIUserSupplier();
            UiScheduleSupplier = new UIScheduleSupplier();
        }

        public TimeSpan GetTimePrediction(string userName, Entities.CTask task)
        {
            return TimePredictor.PredictTimeForTask(userName, task);
        }

        public bool CheckInputIsCorrectTime(string hours, string minutes, string seconds)
        {
            return Regex.IsMatch(hours, @"[0-9]+") &&
               Regex.IsMatch(minutes, @"[0-5]{1}[0-9]{1}") &&
               Regex.IsMatch(seconds, @"[0-5]{1}[0-9]{1}");
        }

        public string RefactorTagName(string tagName)
        {
            string result = tagName.Replace(' ', '_');
            return result;
        }

        public TimeSpan FormatTime(string hours, string minutes, string seconds)
        {
            return new TimeSpan(int.Parse(hours), int.Parse(minutes), int.Parse(seconds));
        }
    }
}
