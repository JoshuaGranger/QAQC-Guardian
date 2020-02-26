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
    class CraftViewModel : INotifyPropertyChanged
    {
        #region Properties
        public MainViewModel MyMainViewModel { get; set; }
        private Craft _myCraft;
        public Craft MyCraft
        {
            get { return _myCraft; }
            set { _myCraft = value; RaisePropertyChanged(nameof(MyCraft)); }
        }
        public Window MyWindow { get; set; }
        public object ViewModelType { get; set; }
        public bool Editing { get; set; }
        private string _craftName;
        public string CraftName
        {
            get { return _craftName; }
            set { _craftName = value; RaisePropertyChanged(nameof(CraftName)); CraftSave.RaiseCanExecuteChanged(); }
        }
        private string _craftFolder;
        public string CraftFolder
        {
            get { return _craftFolder; }
            set { _craftFolder = value; RaisePropertyChanged(nameof(CraftFolder)); CraftSave.RaiseCanExecuteChanged(); }
        }

        // ICommands
        public MyICommand CraftSave { get; set; }
        public MyICommand CraftCancel { get; set; }
        #endregion

        #region Constructor
        public CraftViewModel(MainViewModel mvm, Craft craft, bool editMode, Window myWindow)
        {
            // ICommands
            CraftSave = new MyICommand(OnCraftSave, CanCraftSave);
            CraftCancel = new MyICommand(OnCraftCancel, CanCraftCancel);

            // Misc
            MyMainViewModel = mvm;
            MyCraft = craft;
            Editing = editMode;
            CraftName = MyCraft.Name;
            CraftFolder = MyCraft.Folder;
            ViewModelType = this;
            MyWindow = myWindow;
        }
        #endregion

        #region Button Methods
        public void OnCraftSave()
        {
            if (!Editing)
            {
                MyCraft = new Craft();
                MyCraft.Name = CraftName;
                MyCraft.Folder = CraftFolder;
                MyCraft.Jobs = new List<Job>();
                MyMainViewModel.Crafts.Add(MyCraft);
            }
            else
            {
                MyCraft.Name = CraftName;
                MyCraft.Folder = CraftFolder;
            }

            MyWindow.Close();
        }
        public bool CanCraftSave()
        {
            if (String.IsNullOrEmpty(CraftName))
                return false;

            if (String.IsNullOrEmpty(CraftFolder))
                return false;

            if (CraftName != MyCraft.Name)
            {
                if (MyMainViewModel.Crafts.Any(x => x.Name == CraftName))
                    return false;
            }

            if (CraftFolder != MyCraft.Folder)
            {
                if (MyMainViewModel.Crafts.Any(x => x.Folder == CraftFolder))
                    return false;
            }

            if (CraftName == MyCraft.Name && CraftFolder == MyCraft.Folder)
                return false;

            return true;
        }

        public void OnCraftCancel()
        {
            MyWindow.Close();
        }
        public bool CanCraftCancel()
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
