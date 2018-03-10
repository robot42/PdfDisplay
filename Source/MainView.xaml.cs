namespace PdfDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Animation;

    using Telerik.Windows.Controls;

    public partial class MainView
    {
        private MainViewModel vm;

        public MainView()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            this.InitializeComponent();

            
            this.Loaded += this.OnLoaded;
            this.Closing += this.OnClosing;
            this.DataContextChanged += (sender, args) =>
            {
                this.vm = this.DataContext as MainViewModel;
            };
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //var storyboardName = this.vm.RescentFiles.Count == 0 ? "ToFirstStart" : "ToHistoryStart";
            //var sb = (Storyboard)this.TryFindResource(storyboardName);

            //sb.Begin();

            var args = Environment.GetCommandLineArgs();

            if (args.Length <= 1)
            {
                return;
            }

            // try to load the first command line argument
            this.vm.OpenFile(args[1]);
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
            var allPdfs = filenames?.Where(x => x.ToLower().EndsWith(".pdf"));

            return allPdfs ?? new List<string>();
        }
    }
}