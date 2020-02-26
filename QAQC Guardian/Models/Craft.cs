using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAQC_Guardian.Models
{
    class Craft : INotifyPropertyChanged
    {
        // Properties
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(nameof(Name)); }
        }
        private string _folder;
        public string Folder
        {
            get { return _folder; }
            set { _folder = value; RaisePropertyChanged(nameof(Folder)); }
        }
        private List<Job> _jobs;
        public List<Job> Jobs
        {
            get { return _jobs; }
            set { _jobs = value; RaisePropertyChanged(nameof(Jobs)); }
        }

        #region PropertyChanged Management
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
