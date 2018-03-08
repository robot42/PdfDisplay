// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using System.Xml.Serialization;
    using Caliburn.Micro;
    using Microsoft.Win32;
    using PdfDisplay.Communication;

    public class MainViewModel : Conductor<Screen>.Collection.OneActive, IHandleWithTask<OpenDocumentMessage>, IHandleWithTask<CloseDocumentMessage>
    {
        private readonly IEventAggregator eventAggregator;

        private readonly FileSystemWatcher watcher = new FileSystemWatcher();

        private readonly List<RescentFileModel> rescentFiles = new List<RescentFileModel>();

        private readonly List<DocumentModel> fileModels = new List<DocumentModel>();

        private readonly DispatcherTimer reloadTimer = new DispatcherTimer();

        private RescentFileViewModel selectedRescentFile;

        private DocumentModel currentPdfFile = DocumentModel.Default;

        private WelcomeViewModel welcomeViewModel;
        private DocumentViewModel documentViewModel;

        public MainViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            this.watcher.NotifyFilter = NotifyFilters.LastWrite;
            this.watcher.Changed += (s, e) =>
            {
                this.reloadTimer.Stop();
                this.reloadTimer.Start();
            };

            reloadTimer.Interval = TimeSpan.FromSeconds(1);
            reloadTimer.Tick += (s, e) =>
            {
                reloadTimer.Stop();
                //if (CurrentPdfFile == DocumentModel.Default)
                //{
                //    return;
                //}

                //FileStream stream = null;

                //try
                //{
                //    stream = (new FileInfo(CurrentPdfFile.FullName)).Open(FileMode.Open, FileAccess.Read, FileShare.None);
                //}
                //catch (IOException)
                //{
                //    //the file is unavailable because it is:
                //    //still being written to
                //    //or being processed by another thread
                //    //or does not exist (has already been processed)
                //    return;
                //}
                //finally
                //{
                //    stream?.Close();
                //}

                //System.Action a = () =>
                //{
                //    CurrentPdfFile.IsLoading = true;
                //    NotifyOfPropertyChange(() => CurrentPdfFile);
                //};

                // Application.Current.Dispatcher.BeginInvoke(a);
            };

            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                using (var file = new StreamReader(Path.Combine(homePath, "PdfDisplayRescentFiles.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<RescentFileModel>));
                    this.rescentFiles = (List<RescentFileModel>)serializer.Deserialize(file);
                }

                using (var file = new StreamReader(Path.Combine(homePath, "PdfDisplayRescentFileProperties.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<DocumentModel>));
                    this.fileModels = (List<DocumentModel>)serializer.Deserialize(file);
                }
            }
            catch
            {
            }

            this.welcomeViewModel = new WelcomeViewModel(this.eventAggregator);
            this.documentViewModel = new DocumentViewModel(this.eventAggregator);

            this.Items.Add(this.welcomeViewModel);
            this.Items.Add(this.documentViewModel);

            this.eventAggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ActivateItem(this.welcomeViewModel);
        }

        public ObservableCollection<RescentFileViewModel> RescentFiles
        {
            get
            {
                var result = new ObservableCollection<RescentFileViewModel>();

                foreach (var item in this.rescentFiles)
                {
                    result.Add(new RescentFileViewModel(item));
                }

                return result;
            }
        }

        public RescentFileViewModel SelectedRescentFile
        {
            get => selectedRescentFile;

            set
            {
                selectedRescentFile = value;
                NotifyOfPropertyChange();
            }
        }

        public string ApplicationTitle
        {
            get
            {
                if (this.documentViewModel == null || this.documentViewModel.Model != DocumentModel.Default)
                {
                    return "PDF Display";
                }

                return
                    $"PDF Display - {this.documentViewModel.Model.Name} - Page {this.documentViewModel.Model.CurrentPage}";
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
                    $"The file {filePath} is not valid or does not exist.",
                    "Sorry...",
                    MessageBoxButton.OK);

                // remove the file from the recent file list if necessary
                if (TryGetRescentFile(filePath, out RescentFileModel invalidRescentFile) == false)
                {
                    return;
                }

                rescentFiles.Remove(invalidRescentFile);
                NotifyOfPropertyChange(nameof(RescentFiles));
                return;
            }

            RescentFileModel rescentFile;

            if (TryGetRescentFile(filePath, out rescentFile) == false)
            {
                rescentFile = new RescentFileModel { FullName = filePath };
                rescentFiles.Insert(0, rescentFile);
                while (rescentFiles.Count > 8)
                {
                    rescentFiles.RemoveAt(rescentFiles.Count - 1);
                }

                NotifyOfPropertyChange(nameof(RescentFiles));
            }
            else
            {
                MoveRescentFileToTop(rescentFile);
            }

            var fileModel = fileModels.FirstOrDefault(x => x.FullName == filePath);

            if (fileModel == null)
            {
                fileModel = new DocumentModel { FullName = filePath };
                fileModels.Add(fileModel);
            }

            // CurrentPdfFile = new DocumentModel(fileModel) { IsLoading = true };

            // watcher.EnableRaisingEvents = false;
            // watcher.Path = CurrentPdfFile.Path;
            // watcher.Filter = CurrentPdfFile.Name;
            // watcher.EnableRaisingEvents = true;
            this.ActivateItem(this.documentViewModel);
            this.documentViewModel.SetDocumentModel(fileModel);
            this.NotifyOfPropertyChange(nameof(this.ApplicationTitle));
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
                    var serializer = new XmlSerializer(typeof(List<DocumentModel>));
                    serializer.Serialize(file, fileModels);
                }
            }
            catch
            {
            }
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
            NotifyOfPropertyChange(nameof(RescentFiles));
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

        private void OnPdfFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentPage")
            {
                return;
            }

            NotifyOfPropertyChange(nameof(ApplicationTitle));
        }

        public Task Handle(OpenDocumentMessage message)
        {
            return Task.Run(() => this.OpenFile(message.FileName));
        }

        public Task Handle(CloseDocumentMessage message)
        {
            return Task.Run(
                () =>
                {
                    this.ActivateItem(this.welcomeViewModel);
                    this.documentViewModel.SetDocumentModel(DocumentModel.Default);
                }
            );
        }
    }
}