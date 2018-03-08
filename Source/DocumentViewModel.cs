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
        private readonly IEventAggregator eventAggreagator;

        public DocumentViewModel(IEventAggregator eventAggreagator)
        {
            this.eventAggreagator = eventAggreagator ?? throw new ArgumentNullException(nameof(eventAggreagator));
        }

        public DocumentModel Model { get; private set; }

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

                return new PdfDocumentSource(stream, new FormatProviderSettings(ReadingMode.OnDemand));
            }
        }

        public int CurrentPage
        {
            get => this.Model.CurrentPage;

            set
            {
                if (value == this.Model.CurrentPage)
                {
                    return;
                }

                this.Model.CurrentPage = value;
                this.NotifyOfPropertyChange();
            }
        }

        public double ScaleFactor
        {
            get => this.Model.ScaleFactor;

            set
            {
                if (value.Equals(this.Model.ScaleFactor))
                {
                    return;
                }

                this.Model.ScaleFactor = value;
                this.NotifyOfPropertyChange();
            }
        }

        public void SetDocumentModel(DocumentModel newModel)
        {
            this.Model = newModel;
            this.NotifyOfPropertyChange(nameof(this.DocumentSource));
        }

        public void PageUp()
        {
            if (this.CurrentPage <= 1)
            {
                return;
            }

            this.CurrentPage--;
        }

        public void PageDown()
        {
            this.CurrentPage += 1;
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
            this.eventAggreagator.PublishOnBackgroundThread(new CloseDocumentMessage());
        }
    }
}