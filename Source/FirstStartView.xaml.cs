namespace PdfDisplay
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaktionslogik für FirstStartView.xaml
    /// </summary>
    public partial class FirstStartView : UserControl
    {
        public FirstStartView()
        {
            this.InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            var dc = this.DataContext as MainViewModel;

            if (dc != null)
            {
                dc.Browse();
            }
        }
    }
}