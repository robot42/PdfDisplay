// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using Caliburn.Micro;
    using PdfDisplay.Communication;

    internal class WelcomeViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        public WelcomeViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
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