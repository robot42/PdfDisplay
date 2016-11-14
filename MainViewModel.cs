using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace PdfDisplay
{
    using System.ComponentModel;
    using System.Windows;

    class MainViewModel : PropertyChangedBase
    {
        private readonly FileSystemWatcher watcher = new FileSystemWatcher();

        private readonly List<RescentFileModel> rescentFiles = new List<RescentFileModel>();

        private readonly List<FileModel> fileModels = new List<FileModel>(); 

        private RescentFileViewModel selectedRescentFile;

        private FileViewModel currentPdfFile = FileViewModel.Default;

        private readonly DispatcherTimer reloadTimer = new DispatcherTimer();

        public MainViewModel()
        {

            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (s, e) => { reloadTimer.Stop(); reloadTimer.Start(); };

            reloadTimer.Interval = TimeSpan.FromSeconds(1);
            reloadTimer.Tick += (s, e) =>
            {
                reloadTimer.Stop();
                if (CurrentPdfFile != FileViewModel.Default) 
                {
                    FileStream stream = null;

                    try
                    {
                        stream = (new FileInfo(CurrentPdfFile.FullName)).Open(FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch (IOException)
                    {
                        //the file is unavailable because it is:
                        //still being written to
                        //or being processed by another thread
                        //or does not exist (has already been processed)
                        return;
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                    }

                    System.Action a = () =>
                    {
                        CurrentPdfFile.IsLoading = true;
                        NotifyOfPropertyChange(() => CurrentPdfFile);
                    };

                    Application.Current.Dispatcher.BeginInvoke(a);
                }
            };
            
            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                using (var file = new StreamReader(Path.Combine(homePath, "PdfDisplayRescentFiles.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<RescentFileModel>));
                    rescentFiles = (List<RescentFileModel>)serializer.Deserialize(file);
                }

                using (var file = new StreamReader(Path.Combine(homePath, "PdfDisplayRescentFileProperties.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<FileModel>));
                    fileModels = (List<FileModel>)serializer.Deserialize(file);
                }
            }
            catch
            {
            }
        }

        public void Browse()
        {
            var dialog = new OpenFileDialog
                             {
                                 DefaultExt = ".pdf",
                                 Filter = "PDF documents |*.pdf",
                                 RestoreDirectory = true,
                                 CheckFileExists = true,
                                 CheckPathExists = true
                             };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                OpenFile(dialog.FileName);
            }
        }

        public void OpenFile(string filePath)
        {
            if (!filePath.ToLower().EndsWith(".pdf") || !File.Exists(filePath))
            {
                MessageBox.Show(
                    string.Format("The file {0} is not valid or does not exist.", filePath),
                    "Sorry ...",
                    MessageBoxButton.OK);

                // remove the file from the rescent file list if necessary
                RescentFileModel invalidRescentFile;

                if (TryGetRescentFile(filePath, out invalidRescentFile))
                {
                    rescentFiles.Remove(invalidRescentFile);
                    NotifyOfPropertyChange(() => RescentFiles);
                }

                return;
            }

            RescentFileModel rescentFile;

            if (!TryGetRescentFile(filePath, out rescentFile))
            {
                rescentFile = new RescentFileModel { FullName = filePath };
                rescentFiles.Insert(0, rescentFile);
                while (rescentFiles.Count > 8)
                {
                    rescentFiles.RemoveAt(rescentFiles.Count - 1);
                }

                NotifyOfPropertyChange(() => RescentFiles);
            }
            else
            {
                MoveRescentFileToTop(rescentFile);
            }
            

            var fileModel = fileModels.FirstOrDefault(x => x.FullName == filePath);

            if (fileModel == null)
            {
                fileModel = new FileModel { FullName = filePath };
                fileModels.Add(fileModel);
            }

            CurrentPdfFile = new FileViewModel(fileModel) { IsLoading = true };

            watcher.EnableRaisingEvents = false;
            watcher.Path = CurrentPdfFile.Path;
            watcher.Filter = CurrentPdfFile.Name;
            watcher.EnableRaisingEvents = true;
        }

        private void MoveRescentFileToTop(RescentFileModel rescentFile)
        {
            if (rescentFile == null)
            {
                return;
            }

            if (this.rescentFiles.IndexOf(rescentFile) <= 0)
            {
                return;
            }

            this.rescentFiles.Remove(rescentFile);
            this.rescentFiles.Insert(0, rescentFile);
            NotifyOfPropertyChange(() => RescentFiles);
        }

        private bool TryGetRescentFile(string filePath, out RescentFileModel rescentFile)
        {
            foreach (var file in rescentFiles)
            {
                if (file.FullName == filePath)
                {
                    rescentFile = file;
                    return true;
                }
            }

            rescentFile = null;
            return false;
        }

        public ObservableCollection<RescentFileViewModel> RescentFiles
        {
            get
            {
                var result = new ObservableCollection<RescentFileViewModel>();

                foreach (var item in rescentFiles)
                {
                    result.Add(new RescentFileViewModel(item));
                }

                return result;
            }
        }

        public RescentFileViewModel SelectedRescentFile
        {
            get
            {
                return selectedRescentFile;
            }

            set
            {
                selectedRescentFile = value;
                NotifyOfPropertyChange();
            }
        }

        public FileViewModel CurrentPdfFile
        {
            get
            {
                return currentPdfFile;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                if (currentPdfFile != null)
                {
                    currentPdfFile.PropertyChanged -= this.OnPdfFilePropertyChanged;
                }
                currentPdfFile = value;
                if (currentPdfFile != null)
                {
                    currentPdfFile.PropertyChanged += this.OnPdfFilePropertyChanged;
                }
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => ApplicationTitle);
            }
        }

        private void OnPdfFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentPage")
            {
                return;
            }

            NotifyOfPropertyChange(() => ApplicationTitle);
        }

        public string ApplicationTitle
        {
            get
            {
                return
                    this.CurrentPdfFile != FileViewModel.Default
                    ? string.Format("PDF Display - {0} - Page {1}", this.CurrentPdfFile.Name, this.CurrentPdfFile.CurrentPage)
                    : "PDF Display";
            }
        }

        internal void Save()
        {
            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                using (var file = new StreamWriter(Path.Combine(homePath, "PdfDisplayRescentFiles.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<RescentFileModel>));
                    serializer.Serialize(file, rescentFiles);
                }

                fileModels.RemoveAll(x => (DateTime.Now - x.LastOpened) > TimeSpan.FromDays(180));

                using (var file = new StreamWriter(Path.Combine(homePath, "PdfDisplayRescentFileProperties.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<FileModel>));
                    serializer.Serialize(file, fileModels);
                }
            }
            catch
            {
            }
        }
    }
}
