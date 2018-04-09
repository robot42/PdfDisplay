// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using Caliburn.Micro;
    using PdfDisplay.Communication;

    internal class DocumentNotFoundViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        public DocumentNotFoundViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public FileModel MissingFile { get; set; }

        public void Close()
        {
            this.eventAggregator.PublishOnBackgroundThread(new CloseDocumentMessage());
        }
    }
}