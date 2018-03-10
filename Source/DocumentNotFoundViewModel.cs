// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using Caliburn.Micro;
    using PdfDisplay.Communication;

    class DocumentNotFoundViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        public DocumentNotFoundViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public FileModel MissingFile { get; set; }

        public void Close()
        {
            this.eventAggregator.PublishOnBackgroundThread(new CloseDocumentMessage());
        }
    }
}