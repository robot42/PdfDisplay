using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfDisplay
{
    using Caliburn.Micro;
    class DocumentViewModel : Screen
    {
        private readonly IEventAggregator eventAggreagator;
        private int currentPage;
        private FileViewModel currentPdfFile;

        public DocumentViewModel(IEventAggregator eventAggreagator)
        {
            this.eventAggreagator = eventAggreagator ?? throw new ArgumentNullException(nameof(eventAggreagator));
        }

        public FileViewModel CurrentPdfFile
        {
            get => currentPdfFile;

            set
            {
                if (value == null)
                {
                    return;
                }

                if (currentPdfFile != null)
                {
                    // currentPdfFile.PropertyChanged -= this.OnPdfFilePropertyChanged;
                }
                currentPdfFile = value;
                if (currentPdfFile != null)
                {
                    // currentPdfFile.PropertyChanged += this.OnPdfFilePropertyChanged;
                }
                NotifyOfPropertyChange();
                // NotifyOfPropertyChange(() => ApplicationTitle);
            }
        }

        public int CurrentPage
        {
            get => this.currentPage;

            set
            {
                if (value == this.currentPage)
                {
                    return;
                }

                this.currentPage = value;
                this.NotifyOfPropertyChange();
            }
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

        public void CloseDocument()
        {
            this.eventAggreagator.PublishOnBackgroundThread(new CloseDocumentMessage());
        }
    }
}
