using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using QAQC_Guardian.Misc;
using QAQC_Guardian.Models;
using Newtonsoft.Json;

namespace QAQC_Guardian.ViewModels
{
    class JobViewModel : INotifyPropertyChanged
    {
        #region Properties
        // Sender
        public MainViewModel MyMainViewModel { get; set; }
        private Job _myJob;
        public Job MyJob
        {
            get { return _myJob; }
            set { _myJob = value; RaisePropertyChanged(nameof(MyJob)); }
        }
        public Craft MyCraft { get; set; }
        public Window MyWindow { get; set; }
        public object ViewModelType { get; set; }
        public bool Editing { get; set; }
        private string _jobName;
        public string JobName
        {
            get { return _jobName; }
            set { _jobName = value; RaisePropertyChanged(nameof(JobName)); JobSave.RaiseCanExecuteChanged(); }
        }

        // ICommands
        public MyICommand JobSave { get; set; }
        public MyICommand JobCancel { get; set; }
        #endregion

        #region Constructor
        public JobViewModel(MainViewModel mvm, Job job, Craft craft, bool editMode, Window myWindow)
        {
            // ICommands
            JobSave = new MyICommand(OnJobSave, CanJobSave);
            JobCancel = new MyICommand(OnJobCancel, CanJobCancel);
            
            // Misc
            MyMainViewModel = mvm;
            MyJob = job;
            MyCraft = craft;
            Editing = editMode;
            JobName = MyJob.Name;
            ViewModelType = this;
            MyWindow = myWindow;
        }
        #endregion

        #region Button Methods
        public void OnJobSave()
        {
            if (!Editing)
            {
                MyJob = new Job();
                MyJob.Name = JobName;
                MyJob.DocumentsIncluded = new List<string>();
                MyCraft.Jobs.Add(MyJob);
                MyMainViewModel.Jobs.Add(MyJob);
            }
            else
            {
                MyJob.Name = JobName;
            }

            MyWindow.Close();
        }
        public bool CanJobSave()
        {
            return (!MyCraft.Jobs.Any(x => x.Name == JobName) && !String.IsNullOrEmpty(JobName));
        }

        public void OnJobCancel()
        {
            MyWindow.Close();
        }
        public bool CanJobCancel()
        {
            return true;
        }
        #endregion

        #region PropertyChanged Management
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
