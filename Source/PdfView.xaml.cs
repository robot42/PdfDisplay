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
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private MainViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
            {
                return;
            }

            this.ViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
            this.pdfViewer.DocumentSource = this.ViewModel.CurrentPdfFile.GetDocumentSource();
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPdfFile")
            {
                this.pdfViewer.DocumentSource = this.ViewModel.CurrentPdfFile.GetDocumentSource();
            }
        }

        private void OnFileSelection(object sender, RoutedEventArgs e)
        {
            this.ViewModel.CurrentPdfFile = FileViewModel.Default;
            this.ViewModel.SelectedRescentFile = null;
            this.pdfViewer.DocumentSource = this.ViewModel.CurrentPdfFile.GetDocumentSource();
        }

        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            if (this.pdfViewer.ScaleFactor > 0.3)
            {
                this.pdfViewer.ScaleFactor -= 0.2;
            }
        }

        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            if (this.pdfViewer.ScaleFactor < 5)
            {
                this.pdfViewer.ScaleFactor += 0.2;
            }
        }

        private void OnPageDown(object sender, RoutedEventArgs e)
        {
            try
            {
                this.pdfViewer.PageDown();
            }
            finally
            {
            }
        }

        private void OnPageUp(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.CurrentPdfFile.CurrentPage <= 1)
            {
                return;
            }

            this.pdfViewer.PageUp();
        }

        private void OnDocumentChanged(object sender, System.EventArgs e)
        {
            try
            {
                // pdfViewer.ScaleFactor = ViewModel.CurrentPdfFile.ScaleFactor;
                this.pdfViewer.CurrentPageNumber = this.ViewModel.CurrentPdfFile.CurrentPage;
            }
            catch
            {
            }
            finally
            {
                this.ViewModel.CurrentPdfFile.IsLoading = false;
            }
        }

        private void OnCurrentDocumentPageChanged(object sender, CurrentPageChangedEventArgs e)
        {
            if (this.ViewModel.CurrentPdfFile.IsLoading)
            {
                return;
            }

            this.ViewModel.CurrentPdfFile.CurrentPage = this.pdfViewer.CurrentPageNumber;
        }

        private void OnCurrentDocumentScaleFactorChanged(object sender, System.EventArgs e)
        {
            if (this.ViewModel.CurrentPdfFile.IsLoading)
            {
                return;
            }

            this.ViewModel.CurrentPdfFile.ScaleFactor = this.pdfViewer.ScaleFactor;
        }
    }
}