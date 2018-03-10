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
        private readonly IRecentFilesQuery recentFilesQuery;
        private readonly IEventAggregator eventAggregator;
        private FileModel selectedRecentFile;
        private ObservableCollection<FileModel> recentFiles;

        public DocumentHistoryViewModel(IEventAggregator eventAggregator, IRecentFilesQuery recentFiles)
        {
            this.recentFilesQuery = recentFiles ?? throw new ArgumentNullException(nameof(recentFiles));
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.RecentFiles = new ObservableCollection<FileModel>();
        }

        public ObservableCollection<FileModel> RecentFiles
        {
            get => this.recentFiles;

            private set
            {
                this.recentFiles = value;
                this.NotifyOfPropertyChange();
            }
        }

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
            this.SelectedRecentFile = null;
            this.RecentFiles = new ObservableCollection<FileModel>(this.recentFilesQuery.Files.OrderByDescending(file => file.LastOpened));
        }
    }
}