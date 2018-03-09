// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using PdfDisplay.Communication;

    public class MainViewModel : Conductor<Screen>.Collection.OneActive,
        IHandleWithTask<OpenDocumentMessage>,
        IHandleWithTask<CloseDocumentMessage>
    {
        private readonly IEventAggregator eventAggregator;

        private readonly FileSystemWatcher watcher = new FileSystemWatcher();

        private readonly DispatcherTimer reloadTimer = new DispatcherTimer();

        private readonly RecentFilesRepository filesRepository = new RecentFilesRepository();

        private readonly WelcomeViewModel welcomeViewModel;

        private readonly DocumentViewModel documentViewModel;

        private readonly DocumentHistoryViewModel historyViewModel;

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
                //if (CurrentPdfFile == FileModel.Default)
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

            this.welcomeViewModel = new WelcomeViewModel(this.eventAggregator);
            this.documentViewModel = new DocumentViewModel(this.eventAggregator);
            this.historyViewModel = new DocumentHistoryViewModel(this.eventAggregator, this.filesRepository);

            this.Items.Add(this.welcomeViewModel);
            this.Items.Add(this.documentViewModel);
            this.Items.Add(this.historyViewModel);

            this.eventAggregator.Subscribe(this);
        }

        public string ApplicationTitle
        {
            get
            {
                if (this.documentViewModel == null || this.documentViewModel.Model != FileModel.Default)
                {
                    return "PDF Display";
                }

                return
                    $"PDF Display - {this.documentViewModel.Model.Name} - Page {this.documentViewModel.Model.CurrentPage}";
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
                return;
            }

            var model = new FileModel { FullName = filePath };
            this.filesRepository.Add(model);
            // watcher.EnableRaisingEvents = false;
            // watcher.Path = CurrentPdfFile.Path;
            // watcher.Filter = CurrentPdfFile.Name;
            // watcher.EnableRaisingEvents = true;
            this.documentViewModel.SetDocumentModel(model);
            this.ActivateItem(this.documentViewModel);
            this.NotifyOfPropertyChange(nameof(this.ApplicationTitle));
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
                    this.ActivateItem(this.GetDocumentSelectionScreen());
                    this.documentViewModel.SetDocumentModel(FileModel.Default);
                }
            );
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.filesRepository.Load();
            this.ActivateItem(this.GetDocumentSelectionScreen());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (close)
            {
                this.filesRepository.Save();
            }
        }

        private Screen GetDocumentSelectionScreen()
        {
            return this.filesRepository.Files.Any() ? (Screen)this.historyViewModel : (Screen)this.welcomeViewModel;
        }
    }
}