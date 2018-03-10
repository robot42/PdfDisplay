// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.IO;
    using Caliburn.Micro;
    using PdfDisplay.Communication;
    using Telerik.Windows.Documents.Fixed;
    using Telerik.Windows.Documents.Fixed.FormatProviders;

    internal class DocumentViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;
        private readonly DocumentWatch watcher = new DocumentWatch();
        private bool isLoadingDocument;

        public DocumentViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.watcher.ShouldReload += (sender, args) =>
            {
                this.Reload();
            };
        }

        public FileModel Model { get; private set; }

        public PdfDocumentSource DocumentSource
        {
            get
            {
                if (string.IsNullOrEmpty(this.Model.FullName))
                {
                    return null;
                }

                var stream = new MemoryStream();

                using (Stream input = File.OpenRead(this.Model.FullName))
                {
                    input.CopyTo(stream);
                }

                this.isLoadingDocument = true;
                return new PdfDocumentSource(stream, new FormatProviderSettings(ReadingMode.OnDemand));
            }
        }

        public int CurrentPage
        {
            get => this.isLoadingDocument ? 1 : this.Model.CurrentPage;

            set
            {
                if (this.isLoadingDocument || value == this.Model.CurrentPage)
                {
                    return;
                }

                this.Model.CurrentPage = value;
                this.NotifyOfPropertyChange();
                this.eventAggregator.PublishOnUIThread(new ScrollInDocumentMessage());
            }
        }

        public double ScaleFactor
        {
            get => this.isLoadingDocument ? 1 : this.Model.ScaleFactor;

            set
            {
                if (this.isLoadingDocument || value.Equals(this.Model.ScaleFactor))
                {
                    return;
                }

                this.Model.ScaleFactor = value;
                this.NotifyOfPropertyChange();
            }
        }

        public void DocumentChanged()
        {
            if (this.isLoadingDocument == false)
            {
                return;
            }

            this.isLoadingDocument = false;
            this.NotifyOfPropertyChange(nameof(this.CurrentPage));
            this.NotifyOfPropertyChange(nameof(this.ScaleFactor));
        }

        public void SetFileModel(FileModel newModel)
        {
            this.Model = newModel ?? new FileModel();
            this.watcher.MonitorFile(this.Model);
            this.NotifyOfPropertyChange(nameof(this.DocumentSource));
        }

        public void Reload()
        {
            this.NotifyOfPropertyChange(nameof(this.DocumentSource));
        }

        public void PageUp()
        {
            if (this.CurrentPage <= 1)
            {
                return;
            }

            this.CurrentPage--;
            this.eventAggregator.PublishOnUIThread(new ScrollInDocumentMessage());
        }

        public void PageDown()
        {
            this.CurrentPage++;
            this.eventAggregator.PublishOnUIThread(new ScrollInDocumentMessage());
        }

        public void ZoomIn()
        {
            if (this.ScaleFactor < 5)
            {
                this.ScaleFactor += 0.2;
            }
        }

        public void ZoomOut()
        {
            if (this.ScaleFactor > 0.3)
            {
                this.ScaleFactor -= 0.2;
            }
        }

        public void CloseDocument()
        {
            this.isLoadingDocument = false;
            this.watcher.StopMonitoring();
            this.eventAggregator.PublishOnBackgroundThread(new CloseDocumentMessage());
        }
    }
}