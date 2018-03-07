
namespace PdfDisplay
{
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaktionslogik für DocumentHistoryView.xaml
    /// </summary>
    public partial class DocumentHistoryView
    {
        public DocumentHistoryView()
        {
            InitializeComponent();
        }

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;

            if (vm != null)
            {
                vm.Browse();
            }
        }

        private void OnSelectRescentFile(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;

            if (vm == null || vm.SelectedRescentFile == null)
            {
                return;
            }

            vm.OpenFile(Path.Combine(vm.SelectedRescentFile.Path, vm.SelectedRescentFile.Name));
        }
    }
}
