using System.Windows;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

namespace PdfDisplay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainView
    {
        private readonly MainViewModel vm;

        private string lastFileName;

        public MainView()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            this.InitializeComponent();
            this.DataContext = this.vm = new MainViewModel();

            this.vm.PropertyChanged += this.OnPropertyChanged;
            this.Loaded += this.OnLoaded;
            this.Closing += this.OnClosing;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.vm.Save();
            Properties.Settings.Default.Save();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var storyboardName = this.vm.RescentFiles.Count == 0 ? "ToFirstStart" : "ToHistoryStart";
            var sb = (Storyboard)this.TryFindResource(storyboardName);

            sb.Begin();

            var args = Environment.GetCommandLineArgs();

            if (args.Length <= 1)
            {
                return;
            }

            // try to load the first command line argument
            this.vm.OpenFile(args[1]);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentPdfFile":
                    if (this.lastFileName == this.vm.CurrentPdfFile.FullName)
                    {
                        return;
                    }

                    this.lastFileName = this.vm.CurrentPdfFile.FullName;
                    
                    string storyboardName;

                    if (!string.IsNullOrEmpty(this.vm.CurrentPdfFile.FullName))
                    {
                        storyboardName = this.vm.RescentFiles.Count <= 1 ? "FirstStartToPdf" : "HistoryStartToPdf";
                    }
                    else
                    {
                        storyboardName = this.vm.RescentFiles.Count == 0 ? "PdfToFirstStart" : "PdfToHistoryStart";
                    }

                    if (!string.IsNullOrEmpty(storyboardName))
                    {
                        var sb = (Storyboard)this.TryFindResource(storyboardName);

                        sb.Begin();
                    }
                    break;
            }
        }

        private void OnDrag(object sender, DragEventArgs e)
        {
            e.Effects = AllPdfFilesToBeDroped(e.Data).Any() ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            var pdf = AllPdfFilesToBeDroped(e.Data).FirstOrDefault();

            this.vm.OpenFile(pdf);  
        }

        private static IEnumerable<string> AllPdfFilesToBeDroped(IDataObject data)
        {
            var filenames = (string[])data.GetData(DataFormats.FileDrop);
            var allPdfs = filenames.Where(x => x.ToLower().EndsWith(".pdf"));
            return allPdfs;
        }
    }
}