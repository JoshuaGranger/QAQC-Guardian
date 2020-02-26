using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAQC_Guardian.Models
{
    class Job : INotifyPropertyChanged
    {
        // Properties
        public string Name { get; set; }
        public List<string> DocumentsIncluded { get; set; }

        #region PropertyChanged Management
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
