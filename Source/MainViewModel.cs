// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using PdfDisplay.Communication;

    public class MainViewModel : Conductor<Screen>.Collection.OneActive,
        IHandleWithTask<OpenDocumentMessage>,
        IHandleWithTask<CloseDocumentMessage>,
        IHandleWithTask<ScrollInDocumentMessage>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly RecentFilesRepository filesRepository = new RecentFilesRepository();
        private readonly WelcomeViewModel welcomeViewModel;
        private readonly DocumentViewModel documentViewModel;
        private readonly DocumentHistoryViewModel historyViewModel;
        private readonly DocumentNotFoundViewModel notFoundViewModel;

        public MainViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.welcomeViewModel = new WelcomeViewModel(this.eventAggregator);
            this.documentViewModel = new DocumentViewModel(this.eventAggregator);
            this.historyViewModel = new DocumentHistoryViewModel(this.eventAggregator, this.filesRepository);
            this.notFoundViewModel = new DocumentNotFoundViewModel(this.eventAggregator);

            this.Items.Add(this.welcomeViewModel);
            this.Items.Add(this.documentViewModel);
            this.Items.Add(this.historyViewModel);

            this.eventAggregator.Subscribe(this);
        }

        public string ApplicationTitle
        {
            get
            {
                if (this.documentViewModel?.Model == null)
                {
                    return "PDF Display";
                }

                return
                    $"PDF Display - {this.documentViewModel.Model.Name} - Page {this.documentViewModel.Model.CurrentPage}";
            }
        }

        public void OpenFile(string filePath)
        {
            if (filePath.ToLower().EndsWith(".pdf") == false || File.Exists(filePath) == false)
            {
                this.notFoundViewModel.MissingFile = new FileModel { FullName = filePath };
                this.ActivateItem(this.notFoundViewModel);
                this.filesRepository.Remove(filePath);
                return;
            }

            var model = this.filesRepository.GetOrAdd(filePath);

            this.documentViewModel.SetFileModel(model);
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
                    this.documentViewModel.SetFileModel(null);
                }
            );
        }

        public Task Handle(ScrollInDocumentMessage message)
        {
            return Task.Run(() => this.NotifyOfPropertyChange(nameof(this.ApplicationTitle)));
        }

        public void FilePreviewDragEnter(DragEventArgs e)
        {
            e.Effects = AllPdfFilesToBeDroped(e.Data).Any() ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        public void FileDropped(DragEventArgs e)
        {
            var fileName = AllPdfFilesToBeDroped(e.Data).FirstOrDefault();

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            this.OpenFile(fileName);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.filesRepository.Load();

            if (TryGetFileFromCommandLine(out string fileName))
            {
                this.OpenFile(fileName);
                return;
            }

            this.ActivateItem(this.GetDocumentSelectionScreen());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
            {
                this.filesRepository.Save();
                Properties.Settings.Default.Save();
                this.eventAggregator.Unsubscribe(this);
            }
        }

        private static bool TryGetFileFromCommandLine(out string fileName)
        {
            var args = Environment.GetCommandLineArgs();

            fileName = string.Empty;

            if (args.Length <= 1)
            {
                return false;
            }

            // try to load the first command line argument
            var result = args[1];

            if (result.ToLower().EndsWith(".pdf") == false || File.Exists(result) == false)
            {
                return false;
            }

            fileName = result;
            return true;
        }

        private static IEnumerable<string> AllPdfFilesToBeDroped(IDataObject data)
        {
            var allFileNames = (string[])data.GetData(DataFormats.FileDrop);
            var allPdfs = allFileNames?.Where(x => x.ToLower().EndsWith(".pdf"));

            return allPdfs ?? new List<string>();
        }

        private Screen GetDocumentSelectionScreen()
        {
            return this.filesRepository.Files.Any() ? (Screen)this.historyViewModel : this.welcomeViewModel;
        }
    }
}