namespace PdfDisplay
{
    using System.Windows;
    using Telerik.Windows.Documents.Fixed;

    /// <summary>
    ///     Interaktionslogik für PdfView.xaml
    /// </summary>
    public partial class PdfView
    {
        public PdfView()
        {
            InitializeComponent();
            Loaded += this.OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
            this.pdfViewer.DocumentSource = ViewModel.CurrentPdfFile.GetDocumentSource();
        }

        void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPdfFile")
            {
                this.pdfViewer.DocumentSource = ViewModel.CurrentPdfFile.GetDocumentSource(); 
            }
        }

        private MainViewModel ViewModel
        {
            get
            {
                return DataContext as MainViewModel;
            }
        }

        private void OnFileSelection(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentPdfFile = FileViewModel.Default;
            ViewModel.SelectedRescentFile = null;
            this.pdfViewer.DocumentSource = ViewModel.CurrentPdfFile.GetDocumentSource();
        }

        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            if (pdfViewer.ScaleFactor > 0.3)
            {
                pdfViewer.ScaleFactor -= 0.2;
            }
        }

        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            if (pdfViewer.ScaleFactor < 5)
            {
                pdfViewer.ScaleFactor += 0.2;
            }
        }

        private void OnPageDown(object sender, RoutedEventArgs e)
        {
            pdfViewer.PageDown();
        }

        private void OnPageUp(object sender, RoutedEventArgs e)
        {
            pdfViewer.PageUp();
        }

        private void OnDocumentChanged(object sender, System.EventArgs e)
        {
            try
            {
                // pdfViewer.ScaleFactor = ViewModel.CurrentPdfFile.ScaleFactor;
                pdfViewer.CurrentPageNumber = ViewModel.CurrentPdfFile.CurrentPage;
            }
            catch
            {
            }
            finally
            {
                ViewModel.CurrentPdfFile.IsLoading = false;
            }
        }

        private void OnCurrentDocumentPageChanged(object sender, CurrentPageChangedEventArgs e)
        {
            if (ViewModel.CurrentPdfFile.IsLoading)
            {
                return;
            }

            ViewModel.CurrentPdfFile.CurrentPage = pdfViewer.CurrentPageNumber;
        }

        private void OnCurrentDocumentScaleFactorChanged(object sender, System.EventArgs e)
        {
            if (ViewModel.CurrentPdfFile.IsLoading)
            {
                return;
            }

            ViewModel.CurrentPdfFile.ScaleFactor = pdfViewer.ScaleFactor;
        }
    }
}
