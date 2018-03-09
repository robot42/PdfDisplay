// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using Caliburn.Micro;
    using PdfDisplay.Communication;

    public class WelcomeViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        public WelcomeViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public void OpenDocument()
        {
            if (DocumentHelper.TrySelectDocument(out string fileName))
            {
                this.eventAggregator.PublishOnBackgroundThread(new OpenDocumentMessage(fileName));
            }
        }
    }
}