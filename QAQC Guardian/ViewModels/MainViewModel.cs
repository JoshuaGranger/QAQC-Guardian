using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QAQC_Guardian.Misc;
using QAQC_Guardian.Models;
using Newtonsoft.Json;

namespace QAQC_Guardian.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Properties
        // Craft
        public ObservableCollection<Craft> Crafts { get; set; }
        private Craft _selectedCraft;
        public Craft SelectedCraft
        {
            get { return _selectedCraft; }
            set
            {
                _selectedCraft = value;
                RaisePropertyChanged(nameof(SelectedCraft));
                CraftEdit.RaiseCanExecuteChanged();
                CraftDelete.RaiseCanExecuteChanged();
                JobAdd.RaiseCanExecuteChanged();
                CraftChanged(); }
        }

        // Job
        private ObservableCollection<Job> _jobs;
        public ObservableCollection<Job> Jobs
        {
            get { return _jobs; }
            set { _jobs = value; RaisePropertyChanged(nameof(Jobs)); }
        }
        private ObservableCollection<Job> _filteredJobs;
        public ObservableCollection<Job> FilteredJobs
        {
            get { return _filteredJobs; }
            set { _filteredJobs = value; RaisePropertyChanged(nameof(FilteredJobs)); }
        }
        private Job _selectedJob;
        public Job SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                _selectedJob = value;
                RaisePropertyChanged(nameof(SelectedJob));
                JobEdit.RaiseCanExecuteChanged();
                JobDelete.RaiseCanExecuteChanged();
                JobChanged();
                OnJobDocChanged();
            }
        }
        private string _jobFilterText;
        public string JobFilterText
        {
            get { return _jobFilterText; }
            set { _jobFilterText = value; RaisePropertyChanged(nameof(JobFilterText)); JobFilterChanged(); }
        }

        // Document
        private ObservableCollection<Document> _documents;
        public ObservableCollection<Document> Documents
        {
            get { return _documents; }
            set { _documents = value; RaisePropertyChanged(nameof(Documents)); }
        }
        private ObservableCollection<Document> _filteredDocuments;
        public ObservableCollection<Document> FilteredDocuments
        {
            get { return _filteredDocuments; }
            set { _filteredDocuments = value; RaisePropertyChanged(nameof(FilteredDocuments)); }
        }

        private Document _selectedDocument;
        public Document SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                RaisePropertyChanged(nameof(SelectedDocument));
                PreviewDocument.RaiseCanExecuteChanged();
            }
        }
        private string _docFilterText;
        public string DocFilterText
        {
            get { return _docFilterText; }
            set { _docFilterText = value; RaisePropertyChanged(nameof(DocFilterText)); DocFilterChanged(); }
        }

        // ICommand
        public MyICommand ClearJobFilter { get; set; }
        public MyICommand ClearDocFilter { get; set; }
        public MyICommand JobDocChanged { get; set; }
        public MyICommand PreviewDocument { get; set; }
        public MyICommand GenerateDocument { get; set; }
        public MyICommand CraftAdd { get; set; }
        public MyICommand CraftEdit { get; set; }
        public MyICommand CraftDelete { get; set; }
        public MyICommand JobAdd { get; set; }
        public MyICommand JobEdit { get; set; }
        public MyICommand JobDelete { get; set; }
        public MyICommand SaveAll { get; set; }
        //public MyICommand Closing { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            // ICommands
            ClearJobFilter = new MyICommand(OnClearJobFilter, CanClearJobFilter);
            ClearDocFilter = new MyICommand(OnClearDocFilter, CanClearDocFilter);
            JobDocChanged = new MyICommand(OnJobDocChanged, CanJobDocChanged);
            PreviewDocument = new MyICommand(OnPreviewDocument, CanPreviewDocument);
            GenerateDocument = new MyICommand(OnGenerateDocument, CanGenerateDocument);
            CraftAdd = new MyICommand(OnCraftAdd, CanCraftAdd);
            CraftEdit = new MyICommand(OnCraftEdit, CanCraftEdit);
            CraftDelete = new MyICommand(OnCraftDelete, CanCraftDelete);
            JobAdd = new MyICommand(OnJobAdd, CanJobAdd);
            JobEdit = new MyICommand(OnJobEdit, CanJobEdit);
            JobDelete = new MyICommand(OnJobDelete, CanJobDelete);
            SaveAll = new MyICommand(OnSaveAll, CanSaveAll);

            // Initializations
            Crafts = new ObservableCollection<Craft>();
            Jobs = new ObservableCollection<Job>();
            FilteredJobs = new ObservableCollection<Job>();
            Documents = new ObservableCollection<Document>();
            FilteredDocuments = new ObservableCollection<Document>();
            JobFilterText = "";
            DocFilterText = "";

            // Check to make sure the user did not copy the executable to the desktop...
            // If they did, tell them and close the application
            Globals.CheckPaths();

            // Attempt setup. If it fails, exit the application.
            if (!AccessGold())
            {
                System.Windows.Application.Current.Shutdown();
            }

            // Import/deserialize information from gold.json
            InitialSetup();
        }
        #endregion

        #region Button Methods
        public void OnClearJobFilter()
        {
            JobFilterText = "";
        }
        public bool CanClearJobFilter()
        {
            return !string.IsNullOrEmpty(JobFilterText);
        }

        public void OnClearDocFilter()
        {
            DocFilterText = "";
        }
        public bool CanClearDocFilter()
        {
            return !string.IsNullOrEmpty(DocFilterText);
        }

        public void OnJobDocChanged()
        {
            if (SelectedJob != null)
            {
                SelectedJob.DocumentsIncluded = new List<string>();
                foreach (Document d in Documents)
                {
                    if (d.Selected)
                        SelectedJob.DocumentsIncluded.Add(d.FullPath);
                }
            }
            Globals.ChangesMade = true;
        }
        public bool CanJobDocChanged()
        {
            return true;
        }

        public void OnPreviewDocument()
        {
            PDFMerge.MergePDFs(new List<Document>() { SelectedDocument });
        }
        public bool CanPreviewDocument()
        {
            if (SelectedDocument != null)
                return System.IO.File.Exists(SelectedDocument.FullPath);

            return false;
        }

        public void OnGenerateDocument()
        {
            PDFMerge.MergePDFs(FilteredDocuments.Where(f => f.Selected == true));
        }
        public bool CanGenerateDocument()
        {
            return true;
        }

        public void OnCraftAdd()
        {
            Windows.DialogWindow dlg = new Windows.DialogWindow();
            dlg.DataContext = new CraftViewModel(this, new Craft(), false, dlg);
            dlg.ShowDialog();
            Globals.ChangesMade = true;

            // Load documents from new folder
            ImportDocuments();
        }
        public bool CanCraftAdd()
        {
            return true;
        }

        public void OnCraftEdit()
        {
            // Store the prior craft folder to change the job filepaths
            string folderBeforeEdit = SelectedCraft.Folder;

            // Process craft editing
            Windows.DialogWindow dlg = new Windows.DialogWindow();
            dlg.DataContext = new CraftViewModel(this, SelectedCraft, true, dlg);
            dlg.ShowDialog();
            Globals.ChangesMade = true;

            // Update folders if the folder was changed
            if (SelectedCraft.Folder != folderBeforeEdit)
            {
                foreach (Craft c in Crafts)
                {
                    foreach (Job j in c.Jobs)
                    {
                        for (int i = 0; i < j.DocumentsIncluded.Count; i++)
                        {
                            j.DocumentsIncluded[i] = j.DocumentsIncluded[i].Replace(folderBeforeEdit, SelectedCraft.Folder);
                        }
                    }
                }
                ImportDocuments();

                // Clear filter to copy Documents into FilteredDocuments
                DocFilterText = "";
            }
        }
        public bool CanCraftEdit()
        {
            return SelectedCraft != null;
        }

        public void OnCraftDelete()
        {
            var result = MessageBox.Show("Are you sure you want to delete this craft?\n\nALL JOBS TIED TO THIS CRAFT WILL BE DELETED AND THEIR CONFIGURATION LOST!",
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                SelectedCraft.Jobs.Clear();
                Crafts.Remove(SelectedCraft);
            }

            Globals.ChangesMade = true;

            // Jobs will have been deleted, so make sure joblist is updated
            LoadJobs();

            // Reload documents without deleted craft
            ImportDocuments();
        }
        public bool CanCraftDelete()
        {
            return SelectedCraft != null;
        }

        public void OnJobAdd()
        {
            Windows.DialogWindow dlg = new Windows.DialogWindow();
            dlg.DataContext = new JobViewModel(this, new Job(), SelectedCraft, false, dlg);
            dlg.ShowDialog();
            JobFilterChanged();
            Globals.ChangesMade = true;
        }
        public bool CanJobAdd()
        {
            return SelectedCraft != null;
        }

        public void OnJobEdit()
        {
            Windows.DialogWindow dlg = new Windows.DialogWindow();
            dlg.DataContext = new JobViewModel(this, SelectedJob, SelectedCraft, true, dlg);
            dlg.ShowDialog();
            JobFilterChanged();
            Globals.ChangesMade = true;
        }
        public bool CanJobEdit()
        {
            return SelectedJob != null;
        }

        public void OnJobDelete()
        {
            var result = MessageBox.Show("Are you sure you want to delete this job?\n\nALL ASSIGNED DOCUMENTS WILL BE UNLINKED FROM THIS JOB!",
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Remove the job from the Craft
                SelectedCraft.Jobs.Remove(SelectedJob);

                // Also remove the job from the local list because each Craft is not imported again
                Jobs.Remove(SelectedJob);
            }

            JobFilterChanged();
            Globals.ChangesMade = true;
        }
        public bool CanJobDelete()
        {
            return SelectedJob != null;
        }

        public void OnSaveAll()
        {
            Serialize(Crafts);
            Globals.ShowMsg("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Globals.ChangesMade = false;
        }
        public bool CanSaveAll()
        {
            return Globals.Locked;
        }
        #endregion

        #region Other Methods
        public bool AccessGold()
        {
            try
            {
                // Filepaths
                string tFile = Globals.PathConfig + Globals.SettingsFile;
                string tFileLock = Globals.PathConfig + Globals.LockFile;
                string user = "";

                // Check for a lock file
                if (File.Exists(tFileLock))
                {
                    user = File.GetAccessControl(tFileLock).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
                    Globals.ShowMsg($"Someone else appears to be editing the configuration. There can only be one user editing at a time. Please try again later.\n\n" +
                            $"User:\n{user}\n\n" +
                            $"Lock file:\n{tFileLock}\n\nIf you are 100% certain no one is editing the file, delete the lock file and try again.\n\n" +
                            $"WARNING: this will cause changes to be overridden if two people are working at the same time!",
                            "File Locked", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }

                if (File.Exists(tFile))
                {
                    if (PDFMerge.IsFileinUse(new FileInfo(tFile)))
                    {
                        Globals.ShowMsg("Oops! You do not have Read/Write access to the setup file (gold.json).\n\n" +
                            $"This may be because someone already has the file open. Try again in 30 seconds.\n\nFile location:\n{tFile}",
                            "Oops!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                    File.Create(tFileLock);
                    Globals.Locked = true;
                }
                else
                {
                    if (!Directory.Exists(Globals.PathConfig))
                        Directory.CreateDirectory(Globals.PathConfig);

                    Globals.ShowMsg($"The configuration file (gold.json) was not found, so a new file will be created.\n\nPath:\n{tFile}",
                            "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    File.Create(tFile);
                }
                return true;
            }
            catch (Exception e)
            {
                Globals.ShowMsg($"Something unexpected happened.\n\nMessage Details:\n{e.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void JobFilterChanged()
        {
            // Determine whether the button should be active
            ClearJobFilter.RaiseCanExecuteChanged();

            // Clear the collection
            FilteredJobs.Clear();

            // Add all jobs that match the filter
            if (SelectedCraft != null)
                if (SelectedCraft.Jobs.Any())
                    foreach (Job j in SelectedCraft.Jobs)
                        if (j.Name.ToLower().Contains(JobFilterText.ToLower()))
                            FilteredJobs.Add(j);

            // If a "Custom" entry exists, put it first
            if (FilteredJobs.Any(x => x.Name == "Custom"))
            {
                int index = FilteredJobs.IndexOf(FilteredJobs.First(x => x.Name == "Custom"));
                if (index != 0)
                    FilteredJobs.Move(index, 0);
            }
        }

        public void DocFilterChanged()
        {
            // Determine whether the button should be active
            ClearDocFilter.RaiseCanExecuteChanged();

            // Clear the collection
            FilteredDocuments.Clear();

            // Add all documents that match the filter
            foreach (Document d in Documents)
                if (d.ShortFileName.ToLower().Contains(DocFilterText.ToLower()))
                    FilteredDocuments.Add(d);
        }

        public void CraftChanged()
        {
            // Clear the document filter and show all documents
            DocFilterText = "";

            // Deselect job and document
            SelectedJob = null;
            SelectedDocument = null;

            // Don't clear the job filter, but reload everything as if the filter was changed
            JobFilterChanged();
        }

        public void JobChanged()
        {
            // Deselect document in listbox
            SelectedDocument = null;

            // Deselect all documents
            foreach (Document doc in Documents)
                doc.Selected = false;

            // Check boxes that are included
            if (SelectedJob != null)
            {
                foreach (string jdoc in SelectedJob.DocumentsIncluded)
                {
                    if (!Documents.Any(d => d.FullPath == jdoc))
                        Misc.Globals.ShowMsg($"This job's configuration includes a document that could not be found.\n\nJob:\n{SelectedJob.Name}\n\nDocument:\n{jdoc}",
                            "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        Documents.First(d => d.FullPath == jdoc).Selected = true;
                }
            }
        }

        public void InitialSetup()
        {
            // Deserialize gold.json into Crafts
            Deserialize();

            // Load jobs from each craft
            LoadJobs();

            // Load the documents from the Crafts folders
            ImportDocuments();
        }

        public void Serialize(object obj)
        {
            var fileName = Globals.SettingsFile;
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            string json = JsonConvert.SerializeObject(obj, settings);

            try
            {
                // Attempt to write results
                File.WriteAllText(Globals.PathConfig + fileName, json);

                // After file is saved, create a backup also
                string sFileCurrent = Globals.PathConfig + Globals.SettingsFile;
                string sFileBackupName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}_{Globals.SettingsFileName}.json";
                string sFileBackup = Globals.PathConfigBackup + sFileBackupName;
                File.Copy(sFileCurrent, sFileBackup);
            }
            catch (Exception e)
            {
                Globals.ShowMsg($"Something went wrong while saving the gold.json file.\n\nMessage:\n{e.Message}\n\nPlease try again.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Deserialize()
        {
            // JSON Parsing - if the user gets this far, the file exists and is not locked
            string tFile = Globals.PathConfig + Globals.SettingsFile;
            string json = File.ReadAllText(tFile);
            var settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            Crafts = JsonConvert.DeserializeObject<ObservableCollection<Craft>>(json, settings);

            // If deserialization resulted in a null object, create the object
            if (Crafts == null)
                Crafts = new ObservableCollection<Craft>();
        }

        public void ImportDocuments()
        {
            // Clear Documents
            Documents.Clear();
            FilteredDocuments.Clear();

            foreach (Craft c in Crafts)
            {
                // return a List of Documents
                var tmpDocs = Document.Import(c);

                foreach (Document d in tmpDocs)
                    Documents.Add(d);
            }
        }

        public void LoadJobs()
        {
            Jobs.Clear();
            FilteredJobs.Clear();

            // Pull the Job list from each Craft into Jobs list
            foreach (Craft c in Crafts)
            {
                // Alphabetize
                c.Jobs.Sort((x, y) => String.Compare(x.Name, y.Name));

                //Add to Jobs
                foreach (Job j in c.Jobs)
                    Jobs.Add(j);
            }
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
