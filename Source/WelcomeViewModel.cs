// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using Caliburn.Micro;
    using Microsoft.Win32;

    public class WelcomeViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        public WelcomeViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public void OpenDocument()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".pdf",
                Filter = "PDF documents |*.pdf",
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                this.eventAggregator.PublishOnBackgroundThread(new OpenDocumentMessage(dialog.FileName));
            }
        }
    }
}