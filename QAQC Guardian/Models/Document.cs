using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using QAQC_Guardian.Misc;

namespace QAQC_Guardian.Models
{
    class Document : INotifyPropertyChanged
    {
        // Properties
        public string FolderPath { get; set; }
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string ShortFileName { get; set; }
        public string ListDisplayName { get; set; }
        public string LastModified { get; set; }
        public Craft Craft { get; set; }
        public string PrettyCraftName { get; set; }
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; RaisePropertyChanged(nameof(Selected)); }
        }

        // Constructor
        public Document(Craft craft, string fullPath)
        {
            FolderPath = craft.Folder;
            FullPath = fullPath;
            FileName = FullPath.Replace(craft.Folder, "").TrimStart('\\');
            ShortFileName = FileName.Replace(".pdf", "");
            ListDisplayName = $"{craft.Name}/{ShortFileName}";
            Craft = craft;
            PrettyCraftName = ("(" + Craft.Name + ")").PadRight(16, ' ');
            Selected = false;
            LastModified = System.IO.File.GetLastWriteTime(FullPath).ToString();

            // LastModified is from StackOverflow -- does it always work?
            // https://stackoverflow.com/questions/3360324/check-last-modified-date-of-file-in-c-sharp
        }

        // Import documents from a Craft's folder
        public static List<Document> Import(Craft craft)
        {
            var tmpDocs = new List<Document>();
            string tmpPath = craft.Folder;

            if (System.IO.Directory.Exists(tmpPath))
                foreach (string f in System.IO.Directory.GetFiles(tmpPath))
                    tmpDocs.Add(new Document(craft, f));
            else
                Misc.Globals.ShowMsg($"Craft:\n{craft.Name}\n\nFolder:\n{craft.Folder}\n\nMessage:\nFolder not found. No documents to import.",
                    "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            
            return tmpDocs;
        }

        // Handle PropertyChanged
        // This is to notify the bindings in the View that a property has changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
