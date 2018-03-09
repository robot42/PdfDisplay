// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Caliburn.Micro;
    using PdfDisplay.Communication;

    class DocumentHistoryViewModel : Screen
    {
        private readonly IRecentFilesQuery recentFiles;
        private readonly IEventAggregator eventAggregator;
        private FileModel selectedRecentFile;

        public DocumentHistoryViewModel(IEventAggregator eventAggregator, IRecentFilesQuery recentFiles)
        {
            this.recentFiles = recentFiles ?? throw new ArgumentNullException(nameof(recentFiles));
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.RecentFiles = new ObservableCollection<FileModel>();
        }

        public ObservableCollection<FileModel> RecentFiles { get; private set; }

        public FileModel SelectedRecentFile
        {
            get => this.selectedRecentFile;

            set
            {
                if (Equals(value, this.selectedRecentFile))
                {
                    return;
                }

                this.selectedRecentFile = value;
                this.NotifyOfPropertyChange();

                if (this.selectedRecentFile != null)
                {
                    this.eventAggregator.PublishOnBackgroundThread(new OpenDocumentMessage(this.selectedRecentFile.FullName));
                }
            }
        }

        public void OpenDocument()
        {
            if (DocumentHelper.TrySelectDocument(out string fileName))
            {
                this.eventAggregator.PublishOnBackgroundThread(new OpenDocumentMessage(fileName));
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.RecentFiles = new ObservableCollection<FileModel>(this.recentFiles.Files.OrderBy(file => file.LastOpened));
        }
    }
}