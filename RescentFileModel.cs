namespace PdfDisplay
{
    using System;
    using System.IO;
    using Caliburn.Micro;
    using Telerik.Windows.Documents.Fixed;
    using Telerik.Windows.Documents.Fixed.FormatProviders;

    public class RescentFileModel
    {
        public string FullName { get; set; }
    }

    public class RescentFileViewModel
    {
        private readonly RescentFileModel model;

        public RescentFileViewModel(RescentFileModel model)
        {
            this.model = model;
        }

        public string FullName { get { return model.FullName; } }

        public string Name { get { return System.IO.Path.GetFileName(model.FullName); } }

        public string Path { get { return System.IO.Path.GetDirectoryName(model.FullName); } }
    }

    public class FileModel
    {
        public FileModel()
        {
            CurrentPage = 1;
            ScaleFactor = 1;
            LastOpened = DateTime.Now;
        }

        public string FullName { get; set; }

        public int CurrentPage { get; set; }

        public double ScaleFactor { get; set; }

        public DateTime LastOpened { get; set; }
    }

    public class FileViewModel : PropertyChangedBase 
    {
        private static readonly FileViewModel DefaultModel = new FileViewModel(new FileModel());

        private readonly FileModel model;

        public FileViewModel(FileModel model)
        {
            this.model = model;
            IsLoading = false;
        }

        public string FullName { get { return model.FullName; } }

        public string Name { get { return System.IO.Path.GetFileName(model.FullName); } }

        public string Path { get { return System.IO.Path.GetDirectoryName(model.FullName); } }

        public int CurrentPage
        {
            get
            {
                return model.CurrentPage;
            }

            set
            {
                model.CurrentPage = value;
                NotifyOfPropertyChange();
            }
        }

        public double ScaleFactor 
        {
            get
            {
                return model.ScaleFactor;
            }

            set
            {
                model.ScaleFactor = value;
            }
        }

        public bool IsLoading { get; set; }

        public PdfDocumentSource GetDocumentSource()
        {
            if (string.IsNullOrEmpty(model.FullName))
            {
                // var str = App.GetResourceStream(new Uri("PdfDisplay;component/EmptyDocument.pdf", UriKind.Relative)).Stream;
                // return new PdfDocumentSource(str);
                return null;
            }

            var stream = new MemoryStream();

            using (Stream input = File.OpenRead(model.FullName))
            {
                input.CopyTo(stream);
            }

            return new PdfDocumentSource(stream, new FormatProviderSettings(ReadingMode.OnDemand));
        }

        public static FileViewModel Default
        {
            get
            {
                return DefaultModel;
            }
        }
    }
}
